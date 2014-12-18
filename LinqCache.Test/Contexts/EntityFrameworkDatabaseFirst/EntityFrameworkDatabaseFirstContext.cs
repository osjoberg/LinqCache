using System.Data.SqlClient;
using System.Linq;

namespace LinqCache.Test.Contexts.EntityFrameworkDatabaseFirst
{
    public partial class EntityFrameworkDatabaseFirstContext : IContext
    {
        public EntityFrameworkDatabaseFirstContext(string connectionString) : base(connectionString)
        {            
        }

        public SqlConnection SqlConnection
        {
            get { return (SqlConnection)Database.Connection; }
        }

        public void RemoveAllTable1()
        {
            TestTable1DatabaseFirst.RemoveRange(TestTable1DatabaseFirst);
        }

        public void AddIntoTable1(int id, string column)
        {
            TestTable1DatabaseFirst.Add(new TestTable1DatabaseFirst { Id = id, Column = column });
        }

        public IQueryable<string> GetTable1()
        {
            return TestTable1DatabaseFirst.Select(testTable => testTable.Column);
        }
    }
}
