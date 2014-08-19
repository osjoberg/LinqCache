using System.Diagnostics;
using System.Linq;
using LinqCache.Test.LinqToSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Keys
{
	[TestClass]
	public class ExpressionKeyGeneratorTests
	{
		[TestMethod]
		public void GenerateKey_GenratesKeyForExpression()
		{
			var value = "7";
			var expression = new[] { new { Value = "7" } }.AsQueryable().Where(i => i.Value.StartsWith(value));

			var key = ExpressionKeyGenerator.GetKey(expression.Expression);

			Assert.AreEqual(@"<>f__AnonymousType2`1[System.String][].Where(i => i.Value.StartsWith(""7""))", key);
		}

		[TestMethod]
		public void GenerateKey_GenratesUniqueKeysForSameExpressionOnDifferentTablesInLinqToSql()
		{
			var context = new LinqToSqlContext("");

			var key1 = ExpressionKeyGenerator.GetKey(context.TestTable1s.Where(row => row.Column == "test").Expression);
			var key2 = ExpressionKeyGenerator.GetKey(context.TestTable2s.Where(row => row.Column == "test").Expression);

			Assert.AreNotEqual(key1, key2);
		}

		[TestMethod]
		public void GetKeyPerformance()
		{
			var context = new LinqToSqlContext("");

			Stopwatch watch = Stopwatch.StartNew();
			for (int i = 0; i < 100000; i++)
			{
				ExpressionKeyGenerator.GetKey(context.TestTable1s.Where(row => row.Column == "test").Expression);
			}
			watch.Stop();

			Trace.WriteLine((int)(100000 / watch.Elapsed.TotalSeconds) + " key lookups per second." );
		}

	}
}
