using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test
{
	[TestClass]
	public class LinqCacheConfigurationTest
	{
		[TestMethod]
		public void DefaultReturnsSameInstance()
		{
			Assert.AreEqual(LinqCacheConfiguration.Default, LinqCacheConfiguration.Default);			
		}
	}
}
