using System.Data.SqlClient;
using System.Linq;

namespace LinqCache.Test.Contexts.LinqToSql
{
	public partial class LinqToSqlContext : IContext
	{
        public SqlConnection SqlConnection { get { return (SqlConnection)Connection; } }

        public void RemoveAllTable1()
        {
            TestTable1s.DeleteAllOnSubmit(TestTable1s);
        }

        public void AddIntoTable1(int id, string column)
        {
            TestTable1s.InsertOnSubmit(new TestTable1 { Column = column, Id = id });
        }

        public IQueryable<string> GetTable1()
        {
            return TestTable1s.Select(t => t.Column);
        }

        public int SaveChanges()
        {
            SubmitChanges();
            return 0;
        }
	}
}
