using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using LinqCache.Containers;
using LinqCache.Invalidators.SqlDependency;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Automatically invalidate cached items when the source data changes in the SQL-server database.
	/// </summary>
	public class SqlDependencyInvalidator : Invalidator, IDisposable
	{
		private const string CookieName = "MS.SqlDependencyCookie";
		private const string BrokerNotEnabledMessage = "The SQL Server Service Broker for the current database is not enabled, and as a result query notifications are not supported.  Please enable the Service Broker for this database if you wish to use notifications.";
		
		private SqlCommand _command;

		/// <summary>
		/// Used for testing to notify that we got a message from the broker.
		/// </summary>
		internal readonly AutoResetEvent OnChangeReceived = new AutoResetEvent(false);

		public event EventHandler<IQueryable> UnsupportedQuery;

		/// <summary>
		/// Copy the connection string since the _command.Connection.ConnectionString may be reset when dispose is runned.
		/// </summary>
		private string _connectionString;


		object GetFieldValue(Type type, object instance, string fieldName, BindingFlags bindingFlags)
		{
			var fieldInfo = type.GetField(fieldName, bindingFlags);
			if (fieldInfo == null)
			{
				return null;
			}

			return fieldInfo.GetValue(instance);
		}

		/// <summary>
		/// On init.
		/// </summary>
		protected internal override void OnInit(Container container, IQueryable query, string key)
		{
			var dataContext = GetFieldValue(query.GetType(), query, "context", BindingFlags.NonPublic | BindingFlags.Instance) as DataContext;
			if (dataContext == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			_command = dataContext.GetCommand(query) as SqlCommand;
			if (_command == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}
			_connectionString = _command.Connection.ConnectionString;

			try
			{
				System.Data.SqlClient.SqlDependency.Start(_command.Connection.ConnectionString);	
			}
			catch (InvalidOperationException exception)
			{
				if (exception.Message == BrokerNotEnabledMessage)
				{
					throw LinqCacheException.BrokerIsNotEnabled(exception);
				}
				throw;
			}			
		}

		/// <summary>
		/// On cache miss.
		/// </summary>
		internal protected override void OnCacheMiss(Container container, IQueryable query, string key)
		{
			var dataContext = GetFieldValue(query.GetType(), query, "context", BindingFlags.NonPublic | BindingFlags.Instance) as DataContext; 
			if (dataContext == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}
			
			var services = GetFieldValue(typeof(DataContext), dataContext, "services", BindingFlags.NonPublic | BindingFlags.Instance);
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

			var dependency = new System.Data.SqlClient.SqlDependency();
			dependency.OnChange += (sender, args) =>
			{
				if (args.Info == SqlNotificationInfo.Invalid)
				{
					var unsupportedQueryHandler = UnsupportedQuery;
					if (unsupportedQueryHandler != null)
					{
						UnsupportedQuery(this, query);
					}
				}
				else
				{
					container.Delete(key);	
				}
				
				OnChangeReceived.Set();
			};
			CallContext.SetData(CookieName, dependency.Id);			
		}

		/// <summary>
		/// On cache refresh.
		/// </summary>
		protected internal override void OnCacheRefresh(Container container, IQueryable query, string key, object value)
		{
			CallContext.FreeNamedDataSlot(CookieName);
		}

		public void Dispose()
		{
			try
			{
				if (_connectionString == null)
				{
					return;
				}

				System.Data.SqlClient.SqlDependency.Stop(_connectionString);
			}
			catch (InvalidOperationException exception)
			{
				if (exception.Message == BrokerNotEnabledMessage)
				{
					throw LinqCacheException.BrokerIsNotEnabled(exception);
				}
				throw;
			}			
		}
	}
}
