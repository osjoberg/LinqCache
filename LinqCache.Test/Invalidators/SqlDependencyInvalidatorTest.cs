using System.Data.SqlClient;
using System.Linq;
using LinqCache.Invalidators;
using LinqCache.Test.Contexts;
using LinqCache.Test.Contexts.EntityFrameworkCodeFirst;
using LinqCache.Test.Contexts.EntityFrameworkDatabaseFirst;
using LinqCache.Test.Contexts.LinqToSql;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Invalidators
{
    [TestClass]
	public abstract class SqlDependencyInvalidatorTest
	{
	    protected abstract IContext CreateContext();

		[TestInitialize]
		public void Initialize()
		{
			LinqCacheConfiguration.Default.Container.Clear();

            using (var context = CreateContext())
			{
                context.RemoveAllTable1();
				context.SaveChanges();
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedCacheIsRefreshed()
		{
			using (var context = CreateContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{
				Assert.AreEqual(0, context.GetTable1().AsCached(invalidator).ToList().Count);

				context.AddIntoTable1(1, "Data");
				context.SaveChanges();

				invalidator.OnChangeReceived.WaitOne();

                Assert.AreEqual(1, context.GetTable1().AsCached(invalidator).ToList().Count);
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedMultipleTimesCacheIsRefreshedMultipleTimes()
		{
            using (var context = CreateContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{
				Assert.AreEqual(0, context.GetTable1().AsCached(invalidator).ToList().Count);

				context.AddIntoTable1(1, "Data");
				context.SaveChanges();
				invalidator.OnChangeReceived.WaitOne();

				Assert.AreEqual(1, context.GetTable1().AsCached(invalidator).ToList().Count);

				context.AddIntoTable1(2, "Data2");
				context.SaveChanges();

				invalidator.OnChangeReceived.WaitOne();

				Assert.AreEqual(2, context.GetTable1().AsCached(invalidator).ToList().Count);
			}
		}

		[TestMethod]
		public void WhenUnderlyingDataIsChangedUsingDifferentContextsCacheIsRefreshed()
		{
			using (var invalidator = new SqlDependencyInvalidator())
			{
				using (var context = CreateContext())
				{
					Assert.AreEqual(0, context.GetTable1().AsCached(invalidator).ToList().Count);
				}

				using (var context = CreateContext())
				{
					context.AddIntoTable1(1, "Data");
					context.SaveChanges();

					invalidator.OnChangeReceived.WaitOne();

					Assert.AreEqual(1, context.GetTable1().AsCached(invalidator).ToList().Count);
				}
			}
		}

		[TestMethod]
		public void WhenUnsupportedQueryIsExecutedUnsupportedQueryEventIsFired()
		{
            using (var context = CreateContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{
				bool unsupportedQueryFired = false;
				invalidator.UnsupportedQuery += (sender, queryable) =>
				{
					unsupportedQueryFired = true;
				};

				context.GetTable1().GroupBy(row => row).Select(group => group).AsCached(invalidator).ToList();

				invalidator.OnChangeReceived.WaitOne();

				Assert.IsTrue(unsupportedQueryFired);
			}			
		}

		[TestMethod]
		public void WhenBrokerIsNotRunningExceptionIsThrown()
		{
            using (var context = CreateContext())
			using (var invalidator = new SqlDependencyInvalidator())
			{				
				context.SqlConnection.Open();

                var disableCommand = new SqlCommand(@"ALTER DATABASE [" + TestDatabase.Name + "] SET DISABLE_BROKER WITH ROLLBACK IMMEDIATE", context.SqlConnection);
				disableCommand.ExecuteNonQuery();

				var exceptionWasThrown = false;
				try
				{
					Assert.AreEqual(0, context.GetTable1().AsCached(invalidator).Count());
				}
				catch (LinqCacheException exception)
				{
					exceptionWasThrown = true;	
					Assert.AreEqual("The SQL Server Service Broker for the current database is not enabled.", exception.Message);
				}

				Assert.IsTrue(exceptionWasThrown);

                var enableCommand = new SqlCommand(@"ALTER DATABASE [" + TestDatabase.Name + "] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE", context.SqlConnection);
				enableCommand.ExecuteNonQuery();
			}
		}

        [TestClass]
        public class EntityFrameworkCodeFirstSqlDependencyInvalidatorTest : SqlDependencyInvalidatorTest
        {
            protected override IContext CreateContext()
            {
                return new EntityFrameworkCodeFirstContext(TestDatabase.ConnectionString);
            }
        }

        [TestClass]
        public class EntityFrameworkDatabaseFirstSqlDependencyInvalidatorTest : SqlDependencyInvalidatorTest
        {
            protected override IContext CreateContext()
            {
                return new EntityFrameworkDatabaseFirstContext(TestDatabase.EntityFrameworkDatabaseFirstConnectionString);
            }
        }

        [TestClass]
        public class LinqToSqlSqlDependencyInvalidatorTest : SqlDependencyInvalidatorTest
        {
            protected override IContext CreateContext()
            {
                return new LinqToSqlContext(TestDatabase.ConnectionString);
            }
        }
	}
}
