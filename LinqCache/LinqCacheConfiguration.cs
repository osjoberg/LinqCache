using LinqCache.Containers;
using LinqCache.Invalidations;
using LinqCache.KeyGenerators;

namespace LinqCache
{
	public class LinqCacheConfiguration
	{
		private Container _container;
		private Invalidation _invalidation;

		public static LinqCacheConfiguration Default
		{
			get { return new LinqCacheConfiguration(new MemoryCacheContainer(), new ManualInvalidation(), new ExpressionGeneratedKey());   }			
		}

		public LinqCacheConfiguration(Container container, Invalidation invalidation, KeyGenerator keyGenerator)
		{
			Invalidation = invalidation;
			Container = container;
			KeyGenerator = keyGenerator;
		}

		public Container Container
		{
			get { return _container; }
			set
			{
				_container = value;
				Invalidation.Container = value;
			}
		}

		public Invalidation Invalidation
		{
			get { return _invalidation; }
			set
			{
				_invalidation = value;
				_invalidation.Container = Container;
			}
		}

		public KeyGenerator KeyGenerator { get; set; }
	}
}
