using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public class LinqCacheConfiguration
	{
		public static LinqCacheConfiguration Default
		{
			get { return new LinqCacheConfiguration(new MemoryCacheContainer(), new ManualInvalidator());   }			
		}

		public LinqCacheConfiguration(Container container, Invalidator invalidator)
		{
			Invalidator = invalidator;
			Container = container;
		}

		public Container Container { get; set; }

		public Invalidator Invalidator { get; set; }
	}
}
