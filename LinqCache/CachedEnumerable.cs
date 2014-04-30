using System;
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
		private string _key;
		private readonly Container _container;
		private readonly Invalidator _invalidator;

		public CachedEnumerable(IQueryable<TType> query, Container container, Invalidator invalidator)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}

			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			if (invalidator == null)
			{
				throw new ArgumentNullException("invalidator");
			}

			_container = container;
			_query = query;
			_invalidator = invalidator;
		}

		public IEnumerator<TType> GetEnumerator()
		{
			// Get key from query.
			var expressionKeyGenerator = new ExpressionKeyGenerator();
			_key = expressionKeyGenerator.GetKey(_query.Expression);

			// Query cache.
			object cachedValue;
			var isCached = _container.Get(_key, out cachedValue);
			if (isCached)
			{
				// Return item from cache.
				_invalidator.AfterGet(_key, cachedValue);
				return ((IEnumerable<TType>)cachedValue).GetEnumerator();
			}

			// If not cached, cache item.
			var value = _query.ToArray();

			// Cache item.
			if (_container.SupportsDurationInvalidation && _invalidator.SupportsDuration)
			{
				_container.Set(_key, value, _invalidator.Duration);
			}
			else
			{
				_container.Set(_key, value);
			}

			return ((IEnumerable<TType>)value).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Invalidate()
		{
			// Data is not retrieved from source then _key is null and we cannot remove an item that is not cached.
			if (_key == null)
			{
				return;
			}

			_container.Remove(_key);			
		}
	}
}