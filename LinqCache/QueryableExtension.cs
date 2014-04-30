using System.Linq;
using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public static class QueryableExtension
	{
		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query)
		{
			return AsCached(query, LinqCacheConfiguration.Default.Container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, Container container)
		{
			return AsCached(query, container, LinqCacheConfiguration.Default.Invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, Invalidator invalidator)
		{
			return AsCached(query, LinqCacheConfiguration.Default.Container, invalidator);
		}

		public static CachedEnumerable<TType> AsCached<TType>(this IQueryable<TType> query, Container container, Invalidator invalidator)
		{
			return new CachedEnumerable<TType>(query, container, invalidator);
		}
	}
}
