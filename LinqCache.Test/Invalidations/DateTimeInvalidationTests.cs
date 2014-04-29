using System;
using LinqCache.Invalidations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Invalidations
{
	[TestClass]
	public class DateTimeInvalidationTests
	{
		[TestMethod]
		public void Ctor_SetsDurationCorrectly()
		{
			var invalidate = DateTime.Now.AddHours(1);

			var dateTimeInvalidation = new DateTimeInvalidation(invalidate);

			Assert.AreEqual(1, Math.Round(dateTimeInvalidation.Duration.TotalHours));
		}
	}
}
