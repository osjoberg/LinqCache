using System;

namespace LinqCache
{
	public class LinqCacheException : Exception
	{
		private LinqCacheException(string message) : base(message)
		{
		}

		private LinqCacheException(string message, Exception innerException) : base(message, innerException)
		{
		}

		internal static LinqCacheException ContextIsNotSupported { get { return new LinqCacheException("Current context is not supported by the SqlDependencyInvalidator."); } }
		internal static LinqCacheException BrokerIsNotEnabled(InvalidOperationException innerException) { return new LinqCacheException("The SQL Server Service Broker for the current database is not enabled.", innerException); }
	}
}
