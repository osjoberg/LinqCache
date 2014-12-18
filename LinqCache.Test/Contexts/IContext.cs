using System;
using System.Data.SqlClient;
using System.Linq;

namespace LinqCache.Test.Contexts
{
    public interface IContext : IDisposable
    {
        SqlConnection SqlConnection { get; }
        void RemoveAllTable1();
        void AddIntoTable1(int id, string column);
        IQueryable<string> GetTable1();
        int SaveChanges();
    }
}