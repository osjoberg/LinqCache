using System;
using System.Runtime.Caching;
using System.Threading;
using LinqCache.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Containers
{
	[TestClass]
	public class MemoryCacheContainerTests
	{
		[TestMethod]
		public void Add_StoresCachedItem()
		{
			var memoryCache = new MemoryCacheContainer();

			memoryCache.Set("test", "value");
			object value;
			var isCached = memoryCache.Get("test", out value);
			Assert.AreEqual("value", value);
			Assert.IsTrue(isCached);
		}

		[TestMethod]
		public void Add_StoresCachedItemForOneHundredMilliseconds()
		{
			var memoryCache = new MemoryCacheContainer();

			memoryCache.Set("test", "value", TimeSpan.FromMilliseconds(100));
			object value;
			var isCached = memoryCache.Get("test", out value);
			Assert.IsTrue(isCached);
			Assert.AreEqual("value", value);

			Thread.Sleep(100);

			Assert.IsFalse(memoryCache.Get("test", out value));
		}

		[TestMethod]
		public void Ctor_ByDefaultCreatesSameUnderlyingCacheContainer()
		{
			var memoryCache1 = new MemoryCacheContainer();
			memoryCache1.Set("test", "value");

			var memoryCache2 = new MemoryCacheContainer();

			object value;
			Assert.IsTrue(memoryCache2.Get("test", out value));
		}

		[TestMethod]
		public void Ctor_ByCanCreatesSeparateUnderlyingCacheContainers()
		{
			var memoryCache1 = new MemoryCacheContainer(new MemoryCache("Cache"));
			memoryCache1.Set("test", "value");

			var memoryCache2 = new MemoryCacheContainer(new MemoryCache("Cache"));

			object value;
			Assert.IsFalse(memoryCache2.Get("test", out value));
		}

	}
}
