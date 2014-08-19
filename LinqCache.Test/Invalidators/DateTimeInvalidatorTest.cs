using System;
using LinqCache.Invalidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Invalidators
{
	[TestClass]
	public class DateTimeInvalidatorTest
	{
		[TestMethod]
		public void Ctor_SetsDurationCorrectly()
		{
			var invalidate = DateTime.Now.AddHours(1);

			var dateTimeInvalidator = new DateTimeInvalidator(invalidate);

			Assert.AreEqual(1, Math.Round(dateTimeInvalidator.Duration.TotalHours));
		}
	}
}
