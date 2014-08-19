using System.Data.Common;
using System.Data.Entity;

namespace LinqCache.Test.EntityFramework
{
	public class EntityFrameworkContext : DbContext
	{
		public EntityFrameworkContext() : base(TestDatabase.ConnectionString)
		{			
			Database.SetInitializer<EntityFrameworkContext>(null);
		}

		public DbSet<TestTable1> TestTable1s { get; set; }

		public DbConnection Connection { get { return Database.Connection; }}

		public void SubmitChanges()
		{
			SaveChanges();
		}
	}
}