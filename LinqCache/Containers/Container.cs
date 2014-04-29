using System;

namespace LinqCache.Containers
{
	/// <summary>
	/// Abstract base class for cache containers.
	/// </summary>
	public abstract class Container
	{
		/// <summary>
		/// If the cache container supports duration invalidation, set this to true.
		/// </summary>
		public bool SupportsDurationInvalidation { get; protected set; }

		/// <summary>
		/// Called internally by LinqCache to get an item from the cache.
		/// </summary>
		/// <param name="key">Cache key.</param>
		/// <param name="value">Cached value if present in the cache.</param>
		/// <returns>True if the cached value for the key is present in the cache.</returns>
		public abstract bool Get(string key, out object value);

		/// <summary>
		/// Called internally by LinqCache to set a new item in the cache.
		/// </summary>
		/// <param name="key">Cache key.</param>
		/// <param name="value">Value to cache.</param>
		public abstract void Set(string key, object value);

		/// <summary>
		/// Called internally by LinqCache to set a new item int the cache with a specific duration, optional to implement.
		/// </summary>
		/// <param name="key">Cache key.</param>
		/// <param name="value">Value to cache.</param>
		/// <param name="duration">Duration to keep the value in the cache.</param>
		public virtual void Set(string key, object value, TimeSpan duration)
		{
			throw new NotSupportedException("Time based invalidation rules are not supported with this cache container.");
		}

		/// <summary>
		/// Called internally by LinqCache to remove an item from the cache.
		/// </summary>
		/// <param name="key">Cache key.</param>
		public abstract void Remove(string key);

		/// <summary>
		/// Called internally by LinqCache to remove all items from the cache.
		/// </summary>
		public abstract void RemoveAll();
	}
}
