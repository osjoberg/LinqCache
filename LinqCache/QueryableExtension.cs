using System.Linq;
using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public static class QueryableExtension
	{
		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query)
		{
			return AsCached(query, ExpressionKeyGenerator.GetKey(query.Expression), LinqCacheConfiguration.Default.Container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, Container container)
		{
			return AsCached(query, ExpressionKeyGenerator.GetKey(query.Expression), container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, Invalidator invalidator)
		{
			return AsCached(query, ExpressionKeyGenerator.GetKey(query.Expression), LinqCacheConfiguration.Default.Container, invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey)
		{
			return AsCached(query, cacheKey, LinqCacheConfiguration.Default.Container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey, Container container)
		{
			return AsCached(query, cacheKey, container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey, Invalidator invalidator)
		{
			return AsCached(query, cacheKey, LinqCacheConfiguration.Default.Container, invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, string cacheKey, Container container, Invalidator invalidator)
		{
			return new CachedEnumerable<TType>(query, cacheKey, container, invalidator);
		}
	}
}
