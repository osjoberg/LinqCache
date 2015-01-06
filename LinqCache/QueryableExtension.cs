using System.Linq;
using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public static class QueryableExtension
	{
		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query)
		{
			return AsCached(query, null, LinqCacheConfiguration.Default.Container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query, Container container)
		{
			return AsCached(query, null, container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query, Invalidator invalidator)
		{
			return AsCached(query, null, LinqCacheConfiguration.Default.Container, invalidator);
		}

		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey)
		{
			return AsCached(query, cacheKey, LinqCacheConfiguration.Default.Container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey, Container container)
		{
			return AsCached(query, cacheKey, container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey, Invalidator invalidator)
		{
			return AsCached(query, cacheKey, LinqCacheConfiguration.Default.Container, invalidator);
		}

		public static LinqCacheQueryable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey, Container container, Invalidator invalidator)
		{
			return new LinqCacheQueryable<TType>(query, cacheKey, container, invalidator);
		}
	}
}
