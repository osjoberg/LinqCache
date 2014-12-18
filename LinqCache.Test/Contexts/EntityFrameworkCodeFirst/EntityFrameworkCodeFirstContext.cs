using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace LinqCache.Test.Contexts.EntityFrameworkCodeFirst
{
	public class EntityFrameworkCodeFirstContext : DbContext, IContext
	{
		public EntityFrameworkCodeFirstContext(string connectionString) : base(connectionString)
		{			
			Database.SetInitializer<EntityFrameworkCodeFirstContext>(null);
		}

		public DbSet<TestTable1CodeFirst> TestTable1s { get; set; }

        public SqlConnection SqlConnection
        {
            get { return (SqlConnection)Database.Connection; }
        }

        public void RemoveAllTable1()
        {
            TestTable1s.RemoveRange(TestTable1s);
        }

        public void AddIntoTable1(int id, string column)
        {
            TestTable1s.Add(new TestTable1CodeFirst { Id = id, Column = column });
        }

        public IQueryable<string> GetTable1()
        {
            return TestTable1s.Select(testTable => testTable.Column);
        }
    }
}