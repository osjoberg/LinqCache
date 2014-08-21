using System.Collections.Generic;
using LinqCache.Containers;
using LinqCache.Invalidators;
using Moq;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test
{
	[TestClass]
	public class QueryableExtentionTest
	{
		[TestMethod]
		public void AsCached_CachesData()
		{
			var data = new[] { new { Te2st = "value" } }.ToList();
			var query = data.Where(item => item.Te2st == "value").AsQueryable().AsCached();

			query.ToList();

			data.Clear();

			Assert.AreEqual(1, query.ToList().Count);
		}

		[TestMethod]
		public void AsCached_CachesScalarData()
		{
			var data = new[] { new { Te2st = "value" } }.ToList();
			var query = data.Where(item => item.Te2st == "value").AsQueryable().AsCached();

			query.Count();

			data.Clear();

			Assert.AreEqual(1, query.Count());
		}


		[TestMethod]
		public void AsCached_NotifysInvalidatonAfterGet()
		{
			var invalidatorMock = new Mock<Invalidator>();

			var data = new[] { new { Test = "value" } }.ToList();
			var query = data.Where(item => item.Test == "value").AsQueryable();

			query.AsCached().ToArray();
			query.AsCached(invalidatorMock.Object).ToArray();

			invalidatorMock.Verify(mock => mock.OnCacheHit(It.IsAny<Container>(), It.IsAny<IQueryable>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once);
		}


		[TestMethod]
		public void Invalidate_InvalidatesCache()
		{
			var data = new[] { new { Test = "value" } }.ToList();
			var query = data.Where(item => item.Test == "value").AsQueryable().AsCached();

			query.ToList();

			data.Clear();

			query.Invalidate();

			Assert.AreEqual(0, query.Count());
		}

		[TestMethod]
		public void AsCached_DoesNotReevaluateQueryAfterItIsCached()
		{
			var invalidatorMock = new Mock<IInterface>();
			invalidatorMock.Setup(mock => mock.Items).Returns(new List<string>());

			var query = invalidatorMock.Object.Items.Where(item => item == "value").AsQueryable();

			query.AsCached();
			query.AsCached();

			invalidatorMock.Verify(mock => mock.Items, Times.Once);
		}

		public interface IInterface
		{
			 List<string> Items { get; set; }
		}
	}
}
