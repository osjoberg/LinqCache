using System;
using System.Diagnostics;
using System.Linq;
using LinqCache.Test.Contexts.LinqToSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test
{
	[TestClass]
	public class BenchmarkTest
	{
		private static readonly Func<LinqToSqlContext, string, IQueryable<TestTable1>> CompiledQuery = System.Data.Linq.CompiledQuery.Compile((LinqToSqlContext db, string column) => db.TestTable1s.Where(t => t.Column == column));

		[TestMethod]
		public void Uncached()
		{
			using (var context = new LinqToSqlContext(TestDatabase.ConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					context.TestTable1s.Where(t => t.Column == "Test").ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}

		[TestMethod]
		public void CompiledUncached()
		{
            using (var context = new LinqToSqlContext(TestDatabase.ConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					CompiledQuery(context, "Test").ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}


		[TestMethod]
		public void Cached()
		{
            using (var context = new LinqToSqlContext(TestDatabase.ConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					context.TestTable1s.Where(t => t.Column == "Test").AsCached().ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}

		[TestMethod]
		public void CachedWithProvidedKey()
		{
            using (var context = new LinqToSqlContext(TestDatabase.ConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					context.TestTable1s.Where(t => t.Column == "Test").AsCached("testKey").ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}
	}
}
