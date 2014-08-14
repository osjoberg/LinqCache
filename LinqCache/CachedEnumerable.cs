using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public class CachedEnumerable<TType> : IEnumerable<TType>
	{
		private readonly IQueryable<TType> _query;
		private readonly string _key;
		private readonly Container _container;
		private readonly Invalidator _invalidator;

		internal CachedEnumerable(IQueryable<TType> query, Container container, Invalidator invalidator)
		{
			ArgumentValidator.IsNotNull(query, "query");
			ArgumentValidator.IsNotNull(container, "container");
			ArgumentValidator.IsNotNull(invalidator, "invalidator");

			if (invalidator.UsesDuration && !container.SupportsDurationInvalidation)
			{
				throw LinqCacheException.ContainerDoesNotSupportDuration;
			}

			_container = container;
			_query = query;
			_invalidator = invalidator;

			// Get key from query.
			_key = ExpressionKeyGenerator.GetKey(_query.Expression);

			if (_invalidator.IsInitialized == false)
			{
				_invalidator.OnInit(container, _query, _key);
				_invalidator.IsInitialized = true;
			}
			
		}

		public IEnumerator<TType> GetEnumerator()
		{		
			// Query cache.
			object cachedValue;
			var isCached = _container.Get(_key, out cachedValue);
			if (isCached)
			{
				// Return item from cache.
				_invalidator.OnCacheHit(_container, _query, _key, cachedValue);
				return ((IEnumerable<TType>)cachedValue).GetEnumerator();
			}

			// If not cached, cache item.
			_invalidator.OnCacheMiss(_container, _query, _key);
			var value = _query.ToArray();
			
			// Cache item.
			if (_container.SupportsDurationInvalidation && _invalidator.UsesDuration)
			{
				_container.Set(_key, value, _invalidator.Duration);
			}
			else
			{
				_container.Set(_key, value);
			}

			_invalidator.OnCacheRefresh(_container, _query, _key, value);

			return ((IEnumerable<TType>)value).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Invalidate()
		{
			_container.Delete(_key);			
		}
	}
}