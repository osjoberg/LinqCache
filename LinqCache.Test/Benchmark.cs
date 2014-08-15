using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using LinqCache.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test
{
	[TestClass]
	public class Benchmark
	{
		private static readonly Func<LinqToSqlContext, string, IQueryable<TestTable1>> CompiledQuery = System.Data.Linq.CompiledQuery.Compile((LinqToSqlContext db, string column) => db.TestTable1s.Where(t => t.Column == column));

		private static readonly string DatabaseName = Path.GetFullPath(@"..\..\LinqToSqlDatabase.mdf");
		private static readonly string LinqToSqlConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + DatabaseName + ";Integrated Security=True;Connect Timeout=30";

		[TestMethod]
		public void Uncached()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					var l = context.TestTable1s.Where(t => t.Column == "Test").ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}

		}

		[TestMethod]
		public void CompiledUncached()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					var l = CompiledQuery(context, "Test").ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}


		[TestMethod]
		public void Cached()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					var l = context.TestTable1s.Where(t => t.Column == "Test").AsCached().ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}

		[TestMethod]
		public void CachedWithProvidedKey()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			{
				var watch = Stopwatch.StartNew();
				for (var repeats = 0; repeats <= 10000; repeats++)
				{
					var l = context.TestTable1s.Where(t => t.Column == "Test").AsCached("testKey").ToList();
				}
				Trace.WriteLine("Performed 10000 iterations in " + watch.ElapsedMilliseconds + "ms. Average speed: " + (int)(10000 / watch.Elapsed.TotalSeconds) + " iterations/second.");
			}
		}
	}
}
