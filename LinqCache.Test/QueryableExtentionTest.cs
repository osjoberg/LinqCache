using System.Collections.Generic;
using LinqCache.Containers;
using LinqCache.Invalidations;
using LinqCache.KeyGenerators;
using Moq;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test
{
	[TestClass]
	public class QueryableExtentionTest
	{
		[TestInitialize]
		public void Initialize()
		{
		}

		[TestMethod]
		public void AsCached_CachesData()
		{
			var data = new[] { new { Te2st = "value" } }.ToList();
			var query = data.Where(item => item.Te2st == "value").AsQueryable().AsCached();

			query.ToList();

			data.Clear();

			Assert.AreEqual(1, query.Count());
		}

		[TestMethod]
		public void AsCached_NotifysInvalidatonAfterGet()
		{
			var invalidationMock = new Mock<Invalidation>();
			var configuration = new LinqCacheConfiguration(new MemoryCacheContainer(), invalidationMock.Object, new ExpressionGeneratedKey());

			var data = new[] { new { Test = "value" } }.ToList();
			var query = data.Where(item => item.Test == "value").AsQueryable();

			query.AsCached().ToArray();
			query.AsCached(configuration).ToArray();

			invalidationMock.Verify(mock => mock.AfterGet(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
		}


		[TestMethod]
		public void Invalidate_InvalidatesCache()
		{
			var data = new[] { new { Test = "value" } }.ToList();
			var query = data.Where(item => item.Test == "value").AsQueryable();

			query.AsCached();

			data.Clear();

			query.Invalidate();

			Assert.AreEqual(0, query.AsCached().Count());
		}

		[TestMethod]
		public void AsCached_DoesNotReevaluateQueryAfterItIsCached()
		{
			var invalidationMock = new Mock<IInterface>();
			invalidationMock.Setup(mock => mock.Items).Returns(new List<string>());

			var query = invalidationMock.Object.Items.Where(item => item == "value").AsQueryable();

			query.AsCached();
			query.AsCached();

			invalidationMock.Verify(mock => mock.Items, Times.Once);
		}

		public interface IInterface
		{
			 List<string> Items { get; set; }
		}
	}
}
