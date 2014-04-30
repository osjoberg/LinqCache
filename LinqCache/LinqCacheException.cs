using System;

namespace LinqCache
{
	public class LinqCacheException : Exception
	{
		private LinqCacheException(string message) : base(message)
		{
		}

		internal static readonly LinqCacheException ContextIsNotSupported = new LinqCacheException("Current context is not supported by the SqlDependencyInvalidator.");
		internal static readonly LinqCacheException ContainerDoesNotSupportDuration = new LinqCacheException("Container does not support duration invalidation.");
	}
}
