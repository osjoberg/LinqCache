using System;
using System.Collections.Generic;
using System.Linq;

namespace LinqCache
{
	public static class QueryableExtension
	{
		public static IEnumerable<TType> AsCached<TType>(this IQueryable<TType> query)
		{
			return AsCached(query, LinqCacheConfiguration.Default);
		}

		public static IEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, LinqCacheConfiguration configuration)
		{
			return new DeferredEnumerable<TType>(() => AsChachedDeferred(query, configuration));
		}

		private static IEnumerable<TType> AsChachedDeferred<TType>(IQueryable<TType> query, LinqCacheConfiguration configuration)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}

			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			var container = configuration.Container;
			var invalidation = configuration.Invalidation;
			var keyGenerator = configuration.KeyGenerator;

			// Get key from query.
			var key = keyGenerator.GetKey(query.Expression);

			// Query cache.
			object cachedValue;
			var isCached = container.Get(key, out cachedValue);
			if (isCached)
			{
				// Return item from cache.
				invalidation.AfterGet(key, cachedValue);
				return (IQueryable<TType>) cachedValue;
			}

			// If not cached, cache item.
			var value = query.ToArray().AsQueryable();

			// Cache item.
			if (container.SupportsDurationInvalidation && invalidation.SupportsDuration)
			{
				container.Set(key, value, invalidation.Duration);
			}
			else
			{
				container.Set(key, value);
			}

			return value.AsQueryable();
		}

		public static void Invalidate<TType>(this IQueryable<TType> query)
		{
			Invalidate(query, LinqCacheConfiguration.Default);
		}

		public static void Invalidate<TType>(this IQueryable<TType> query, LinqCacheConfiguration configuration)
		{
			if (query == null)
			{
				throw new ArgumentNullException("query");
			}

			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}

			var container = configuration.Container;
			var keyGenerator = configuration.KeyGenerator;

			// Get key from query.
			var key = keyGenerator.GetKey(query.Expression);

			container.Remove(key);
		}
	}
}
