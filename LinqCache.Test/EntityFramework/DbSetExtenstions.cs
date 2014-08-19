using System.Collections.Generic;
using System.Data.Entity;

namespace LinqCache.Test.EntityFramework
{
	public static class DbSetExtenstions
	{
		public static void InsertOnSubmit<T>(this DbSet<T> dbSet, T instance) where T : class
		{
			dbSet.Add(instance);
		}

		public static void DeleteAllOnSubmit<T>(this DbSet<T> dbSet, IEnumerable<T> instances) where T : class
		{
			dbSet.RemoveRange(instances);
		}
	}
}
