using System;
using System.Linq;
using System.Linq.Expressions;
using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	class LinqCacheQueryProvider<TType> : IQueryProvider
	{
		private readonly IQueryable<TType> _query;
		private readonly string _cacheKey;
		private readonly Container _container;
		private readonly Invalidator _invalidator;

		public LinqCacheQueryProvider(IQueryable<TType> query, string cacheKey, Container container, Invalidator invalidator)
		{
			_query = query;
			_cacheKey = cacheKey;
			_container = container;
			_invalidator = invalidator;

			// Get key from query.
			if (_invalidator.IsInitialized == false)
			{
				_invalidator.OnInit(container, _query, _cacheKey);
				_invalidator.IsInitialized = true;
			}
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			return _query.Provider.CreateQuery<TElement>(expression);
		}

		public IQueryable CreateQuery(Expression expression)
		{
			return _query.Provider.CreateQuery(expression);
		}

		public TResult Execute<TResult>(Expression expression)
		{
			return (TResult)InternalExcecute(expression, () => _query.Provider.Execute(expression));
		}

		public object Execute(Expression expression)
		{
			return InternalExcecute(expression, () => _query.Provider.Execute(expression));
		}

		internal object InternalExcecute(Expression expression, Func<object> getValue)
		{
			var cacheKey = _cacheKey ?? ExpressionKeyGenerator.GetKey(expression);

			// Query cache.
			object cachedValue;
			var isCached = _container.Get(cacheKey, out cachedValue);
			if (isCached)
			{
				// Return item from cache.
				_invalidator.OnCacheHit(_container, _query, cacheKey, cachedValue);
				return cachedValue;
			}

			// If not cached, cache item.
			_invalidator.OnCacheMiss(_container, _query, cacheKey);
			var value = getValue();
			
			// Cache item.
			if (_container.SupportsDurationInvalidation && _invalidator.UsesDuration)
			{
				_container.Set(cacheKey, value, _invalidator.Duration);
			}
			else
			{
				_container.Set(cacheKey, value);
			}

			_invalidator.OnCacheRefresh(_container, _query, cacheKey, value);

			return value;
		}

		internal void Invalidate()
		{
			var cacheKey = _cacheKey ?? ExpressionKeyGenerator.GetKey(_query.Expression);
			_container.Delete(cacheKey);
		}
	}
}
