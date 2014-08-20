using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public class LinqCacheConfiguration
	{
		private static readonly LinqCacheConfiguration InternalDefault = new LinqCacheConfiguration(new MemoryCacheContainer(), new ManualInvalidator());

		public static LinqCacheConfiguration Default
		{
			get { return InternalDefault;   }			
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
