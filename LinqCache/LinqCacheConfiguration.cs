using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public class LinqCacheConfiguration
	{
		private Container _container;
		private Invalidator _invalidator;

		public static LinqCacheConfiguration Default
		{
			get { return new LinqCacheConfiguration(new MemoryCacheContainer(), new ManualInvalidator());   }			
		}

		public LinqCacheConfiguration(Container container, Invalidator invalidator)
		{
			Invalidator = invalidator;
			Container = container;
		}

		public Container Container
		{
			get { return _container; }
			set
			{
				_container = value;
				Invalidator.Container = value;
			}
		}

		public Invalidator Invalidator
		{
			get { return _invalidator; }
			set
			{
				_invalidator = value;
				_invalidator.Container = Container;
			}
		}
	}
}
