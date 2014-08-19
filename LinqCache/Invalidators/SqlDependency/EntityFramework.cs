using System.Data;
using System.Linq;
using System.Reflection;

namespace LinqCache.Invalidators.SqlDependency
{
	static class EntityFramework
	{
		private const string Provider = "System.Linq.IQueryable.Provider";

		internal static string GetConnectionString(IQueryable query)
		{
			ArgumentValidator.IsNotNull(query, "query");

			var queryType = query.GetType();
			var providerProperty = queryType.GetProperty(Provider, BindingFlags.Instance | BindingFlags.NonPublic);
			if (providerProperty == null)
			{
				var queryBaseType = queryType.BaseType;
				if (queryBaseType == null)
				{
					throw LinqCacheException.ContextIsNotSupported;
				}

				providerProperty = queryBaseType.GetProperty(Provider, BindingFlags.Instance | BindingFlags.NonPublic);
			}

			if (providerProperty == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var provider = providerProperty.GetValue(query);
			if (provider == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var contextField = provider.GetType().GetField("_internalContext", BindingFlags.Instance | BindingFlags.NonPublic);
			if (contextField == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var context = contextField.GetValue(provider);
			if (context == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var connectionProperty = context.GetType().GetProperty("Connection");
			if (connectionProperty == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var connection = connectionProperty.GetValue(context) as IDbConnection;
			if (connection == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			return connection.ConnectionString;			
		}
	}
}
