using System;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;

namespace LinqCache.Invalidators.SqlDependency
{
	static class LinqToSql
	{
		private const string Provider = "System.Linq.IQueryable.Provider";

		internal static string GetConnectionString(IQueryable query)
		{
			ArgumentValidator.IsNotNull(query, "query");

			var queryType = query.GetType();
			var providerProperty = queryType.GetProperty(Provider, BindingFlags.Instance | BindingFlags.NonPublic);

			if (providerProperty == null)
			{
				return null;
			}

			var provider = providerProperty.GetValue(query);
			if (provider == null)
			{
				return null;
			}

			var contextField = provider.GetType().GetField("context", BindingFlags.Instance | BindingFlags.NonPublic);
			if (contextField == null)
			{
				return null;
			}

			var context = contextField.GetValue(provider);
			if (context == null)
			{
				return null;
			}

			var connectionProperty = context.GetType().GetProperty("Connection");
			if (connectionProperty == null)
			{
				return null;
			}

			var connection = connectionProperty.GetValue(context) as IDbConnection;
			if (connection == null)
			{
				return null;
			}

			return connection.ConnectionString;
		}

		static object GetFieldValue(Type type, object instance, string fieldName, BindingFlags bindingFlags)
		{
			var fieldInfo = type.GetField(fieldName, bindingFlags);
			if (fieldInfo == null)
			{
				return null;
			}

			return fieldInfo.GetValue(instance);
		}

		internal static void AddSchemaToModel(IQueryable query)
		{
			var dataContext = GetFieldValue(query.GetType(), query, "context", BindingFlags.NonPublic | BindingFlags.Instance) as DataContext;
			if (dataContext == null)
			{
				return;
			}

			var services = GetFieldValue(typeof(DataContext), dataContext, "services", BindingFlags.Instance | BindingFlags.NonPublic);
			if (services == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var metaModelFieldInfo = services.GetType().GetField("metaModel", BindingFlags.Instance | BindingFlags.NonPublic);
			if (metaModelFieldInfo == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			var metaModel = metaModelFieldInfo.GetValue(services);
			if (metaModel is CustomMetaModel == false)
			{
				metaModelFieldInfo.SetValue(services, new CustomMetaModel((MetaModel)metaModel));
			}
		}
	}
}
