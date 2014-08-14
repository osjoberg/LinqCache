using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using LinqCache.Invalidators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Invalidators
{
	[TestClass]
	public class SqlDependencyInvalidatorTest
	{
		private static readonly string DatabaseName = Path.GetFullPath(@"..\..\LinqToSqlDatabase.mdf");
		private static readonly TimeSpan TimeOut = new TimeSpan(0, 0, 10);
		private static readonly string LinqToSqlConnectionString = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + DatabaseName + ";Integrated Security=True;Connect Timeout=30";

		[TestInitialize]
		public void Initialize()
		{
			LinqCacheConfiguration.Default.Container.Clear();

			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			{
				context.TestTable1s.DeleteAllOnSubmit(context.TestTable1s);
				context.SubmitChanges();
			}

		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedCacheIsRefreshed()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			using (var invalidator = new SqlDependencyInvalidator())
			{
				Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).Count());

				context.TestTable1s.InsertOnSubmit(new TestTable1 {Column = "Column"});
				context.SubmitChanges();

				invalidator.OnChangeReceived.WaitOne(TimeOut);

				Assert.AreEqual(1, context.TestTable1s.AsCached(invalidator).Count());
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedMultipleTimesCacheIsRefreshedMultipleTimes()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			using (var invalidator = new SqlDependencyInvalidator())
			{
				Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).Count());

				context.TestTable1s.InsertOnSubmit(new TestTable1 { Id = 1, Column = "Column" });
				context.SubmitChanges();
				invalidator.OnChangeReceived.WaitOne(TimeOut);

				Assert.AreEqual(1, context.TestTable1s.AsCached(invalidator).Count());

				context.TestTable1s.InsertOnSubmit(new TestTable1 { Id = 2, Column = "Column2" });
				context.SubmitChanges();

				invalidator.OnChangeReceived.WaitOne(TimeOut);

				Assert.AreEqual(2, context.TestTable1s.AsCached(invalidator).Count());
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedUsingDifferentContextsCacheIsRefreshed()
		{
			using (var invalidator = new SqlDependencyInvalidator())
			{
				using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
				{
					Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).Count());
				}

				using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
				{
					context.TestTable1s.InsertOnSubmit(new TestTable1 {Column = "Column"});
					context.SubmitChanges();

					invalidator.OnChangeReceived.WaitOne(TimeOut);

					Assert.AreEqual(1, context.TestTable1s.AsCached(invalidator).Count());
				}
			}
		}

		[TestMethod]
		public void WhenUnsupportedQueryIsExecutedUnsupportedQueryEventIsFired()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			using (var invalidator = new SqlDependencyInvalidator())
			{
				bool unsupportedQueryFired = false;
				invalidator.UnsupportedQuery += (sender, queryable) =>
				{
					unsupportedQueryFired = true;
				};

				context.TestTable1s.GroupBy(row => row.Column).Select(group => group.Key).AsCached(invalidator).ToList();

				invalidator.OnChangeReceived.WaitOne(TimeOut);

				Assert.IsTrue(unsupportedQueryFired);
			}			
		}

		[TestMethod]
		public void WhenBrokerIsNotRunningExceptionIsThrown()
		{
			using (var context = new LinqToSqlContext(LinqToSqlConnectionString))
			using (var invalidator = new SqlDependencyInvalidator())
			{				
				context.Connection.Open();

				var disableCommand = new SqlCommand(@"ALTER DATABASE [" + DatabaseName + "] SET DISABLE_BROKER", (SqlConnection)context.Connection);
				disableCommand.ExecuteNonQuery();

				var exceptionWasThrown = false;
				try
				{
					Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).Count());
				}
				catch (LinqCacheException exception)
				{
					exceptionWasThrown = true;	
					Assert.AreEqual("The SQL Server Service Broker for the current database is not enabled.", exception.Message);
				}

				Assert.IsTrue(exceptionWasThrown);
				
				var enableCommand = new SqlCommand(@"ALTER DATABASE [" + DatabaseName + "] SET ENABLE_BROKER", (SqlConnection)context.Connection);
				enableCommand.ExecuteNonQuery();
			}
		}		
	}
}
