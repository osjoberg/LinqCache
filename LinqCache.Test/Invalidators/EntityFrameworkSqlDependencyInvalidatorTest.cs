using System.Data.SqlClient;
using System.Linq;
using LinqCache.Invalidators;
using LinqCache.Test.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTable1 = LinqCache.Test.EntityFramework.TestTable1;

namespace LinqCache.Test.Invalidators
{
	[TestClass]
	public class SqlDependencyInvalidatorEntifyFrameworkTest
	{
		[TestInitialize]
		public void Initialize()
		{
			LinqCacheConfiguration.Default.Container.Clear();

			using (var context = new EntityFrameworkContext())
			{
				context.TestTable1s.DeleteAllOnSubmit(context.TestTable1s.ToList());
				context.SubmitChanges();
			}

		}
		[TestMethod]
		public void WhenUnderlyingDataIsChangedCacheIsRefreshed()
		{
			using (var context = new EntityFrameworkContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{
				Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).ToList().Count);

				context.TestTable1s.InsertOnSubmit(new TestTable1 { Id = 1 } );
				context.SubmitChanges();

				invalidator.OnChangeReceived.WaitOne();

				Assert.AreEqual(1, context.TestTable1s.AsCached(invalidator).ToList().Count);
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedMultipleTimesCacheIsRefreshedMultipleTimes()
		{
			using (var context = new EntityFrameworkContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{
				Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).ToList().Count);

				context.TestTable1s.InsertOnSubmit(new TestTable1 { Id = 1, Column = "Column" });
				context.SubmitChanges();
				invalidator.OnChangeReceived.WaitOne();

				Assert.AreEqual(1, context.TestTable1s.AsCached(invalidator).Count());

				context.TestTable1s.InsertOnSubmit(new TestTable1 { Id = 2, Column = "Column2" });
				context.SubmitChanges();

				invalidator.OnChangeReceived.WaitOne();

				Assert.AreEqual(2, context.TestTable1s.AsCached(invalidator).ToList().Count);
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedUsingDifferentContextsCacheIsRefreshed()
		{
			using (var invalidator = new SqlDependencyInvalidator())
			{
				using (var context = new EntityFrameworkContext())
				{
					Assert.AreEqual(0, context.TestTable1s.AsCached(invalidator).ToList().Count);
				}

				using (var context = new EntityFrameworkContext())
				{
					context.TestTable1s.InsertOnSubmit(new TestTable1 { Column = "Column", Id = 1 });
					context.SubmitChanges();

					invalidator.OnChangeReceived.WaitOne();

					Assert.AreEqual(1, context.TestTable1s.AsCached(invalidator).ToList().Count);
				}
			}
		}

		[TestMethod]
		public void WhenUnsupportedQueryIsExecutedUnsupportedQueryEventIsFired()
		{
			using (var context = new EntityFrameworkContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{
				bool unsupportedQueryFired = false;
				invalidator.UnsupportedQuery += (sender, queryable) =>
				{
					unsupportedQueryFired = true;
				};

				context.TestTable1s.GroupBy(row => row.Column).Select(group => group.Key).AsCached(invalidator).ToList();

				invalidator.OnChangeReceived.WaitOne();

				Assert.IsTrue(unsupportedQueryFired);
			}			
		}

		[TestMethod]
		public void WhenBrokerIsNotRunningExceptionIsThrown()
		{
			using (var context = new EntityFrameworkContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{				
				context.Connection.Open();

				var disableCommand = new SqlCommand(@"ALTER DATABASE [" + TestDatabase.Name + "] SET DISABLE_BROKER", (SqlConnection)context.Connection);
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
				
				var enableCommand = new SqlCommand(@"ALTER DATABASE [" + TestDatabase.Name + "] SET ENABLE_BROKER", (SqlConnection)context.Connection);
				enableCommand.ExecuteNonQuery();
			}
		}		
	}
}
