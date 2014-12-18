using System;
using System.Linq;

namespace LinqCache.Invalidators
{
    public class SqlDependencyEventArgs : EventArgs
    {
        public SqlDependencyEventArgs(IQueryable queryable)
        {
            Queryable = queryable;
        }

        public IQueryable Queryable { get; private set; }
    }
}
