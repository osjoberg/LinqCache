using System.Diagnostics;
using System.Linq;
using LinqCache.Test.Contexts.LinqToSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test
{
	[TestClass]
	public class ExpressionKeyGeneratorTests
	{
		[TestMethod]
		public void GenerateKeyGenratesKeyForExpression()
		{
			const string value = "7";
			var expression = new[] { new { Value = "7" } }.AsQueryable().Where(i => i.Value.StartsWith(value));

			var key = ExpressionKeyGenerator.GetKey(expression.Expression);

			Assert.AreEqual(@"<>f__AnonymousType2`1[System.String][].Where(i => i.Value.StartsWith(""7""))", key);
		}

		[TestMethod]
		public void GenerateKeyGenratesUniqueKeysForSameExpressionOnDifferentTablesInLinqToSql()
		{
		    using (var context = new LinqToSqlContext(TestDatabase.ConnectionString))
		    {
                var key1 = ExpressionKeyGenerator.GetKey(context.TestTable1s.Where(row => row.Column == "test").Expression);
                var key2 = ExpressionKeyGenerator.GetKey(context.TestTable2s.Where(row => row.Column == "test").Expression);
            
                Assert.AreNotEqual(key1, key2);
            }
		}

		[TestMethod]
		public void GetKeyPerformance()
		{
		    using (var context = new LinqToSqlContext(TestDatabase.ConnectionString))
		    {		        
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
}
