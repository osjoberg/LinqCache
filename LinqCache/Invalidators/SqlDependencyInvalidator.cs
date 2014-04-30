using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using LinqCache.Containers;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Automatically invalidate cached items when the source data changes in the SQL-server database.
	/// </summary>
	public class SqlDependencyInvalidator : Invalidator
	{
		/// <summary>
		/// On init.
		/// </summary>
		protected internal override void OnInit(Container container, IQueryable query, string key)
		{
			var queryType = query.GetType();

			var contextFieldInfo = queryType.GetField("context", BindingFlags.NonPublic | BindingFlags.Instance);
			if (contextFieldInfo == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var context = contextFieldInfo.GetValue(query);
			if (context == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var dataContext = context as DataContext;
			if (dataContext == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var command = dataContext.GetCommand(query) as SqlCommand;
			if (command == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var dependency = new SqlDependency(command);
			dependency.OnChange += delegate
			{				
				container.Delete(key);
			};
		}
	}
}
