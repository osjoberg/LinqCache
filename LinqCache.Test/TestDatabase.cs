using System.IO;

namespace LinqCache.Test
{
	class TestDatabase
	{
        internal static string Name { get { return Path.GetFullPath(@"..\..\TestDatabase.mdf"); } }
        internal static string ConnectionString { get { return @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + Name + ";Integrated Security=True;Connect Timeout=30"; } }

		//internal const string ConnectionString = "Data Source=(local);Initial Catalog=TestDatabase;Integrated Security=True;";
	    internal static readonly string EntityFrameworkDatabaseFirstConnectionString = "metadata=res://*/Contexts.EntityFrameworkDatabaseFirst.EntityFrameworkDatabaseFirst.csdl|res://*/Contexts.EntityFrameworkDatabaseFirst.EntityFrameworkDatabaseFirst.ssdl|res://*/Contexts.EntityFrameworkDatabaseFirst.EntityFrameworkDatabaseFirst.msl;provider=System.Data.SqlClient;provider connection string=\"" + ConnectionString + "\"";
	}
}
