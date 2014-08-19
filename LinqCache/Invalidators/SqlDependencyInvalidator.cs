using System;
using System.Data.SqlClient;
using System.Linq;
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
		const string CookieName = "MS.SqlDependencyCookie";
		const string BrokerNotEnabledMessage = "The SQL Server Service Broker for the current database is not enabled, and as a result query notifications are not supported.  Please enable the Service Broker for this database if you wish to use notifications.";
		
		/// <summary>
		/// Used for testing to notify that we got a message from the broker.
		/// </summary>
		internal readonly AutoResetEvent OnChangeReceived = new AutoResetEvent(false);

		public event EventHandler<IQueryable> UnsupportedQuery;

		/// <summary>
		/// Copy the connection string since the _command.Connection.ConnectionString may be reset when dispose is runned.
		/// </summary>
		string _connectionString;

		/// <summary>
		/// On init.
		/// </summary>
		protected internal override void OnInit(Container container, IQueryable query, string key)
		{
			_connectionString = LinqToSql.GetConnectionString(query) ?? EntityFramework.GetConnectionString(query);
			if (_connectionString == null)
			{
				throw LinqCacheException.ContextIsNotSupported;
			}

			try
			{
				System.Data.SqlClient.SqlDependency.Start(_connectionString);	
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
			// Patch model to include schema information in LinqToSql, this is a requirement for SqlDependency to work.
			LinqToSql.AddSchemaToModel(query);

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

		/// <summary>
		/// On dispose.
		/// </summary>
		public void Dispose()
		{
			if (_connectionString == null)
			{
				return;
			}

			try
			{
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
