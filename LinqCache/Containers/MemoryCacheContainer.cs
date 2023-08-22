using System;
using System.Runtime.Caching;

namespace LinqCache.Containers
{
	/// <summary>
	/// Cache container wrapper around System.Runtime.Caching.MemoryCache providing a fast in-process memory cache.
	/// </summary>
	public sealed class MemoryCacheContainer : Container, IDisposable
	{
		/// <summary>
		/// Internal instance of MemoryCache.
		/// </summary>
		private readonly MemoryCache _memoryCache;

  		/// <summary>
        	/// MemoryCache does not allow storing null values
        	/// </summary>
  		private readonly Object _nullObject = new object();

		/// <summary>
		/// Initializes a new instance of the MemoryCacheContainer.
		/// </summary>
		public MemoryCacheContainer() : this(MemoryCache.Default)
		{
		}

		/// <summary>
		/// Initializes a new instance of the MemoryCacheContainer.
		/// </summary>
		/// <param name="memoryCache">Custom MemoryCache instance.</param>
		public MemoryCacheContainer(MemoryCache memoryCache)
		{
			SupportsDurationInvalidation = true;
			_memoryCache = memoryCache;
		}

		/// <summary>
		/// Disposes the internal MemoryCache instance.
		/// </summary>
		public void Dispose()
		{
			_memoryCache.Dispose();
		}

		public override void Set(string key, object value)
		{
		  	if (value == null) value = _nullObject;
			_memoryCache.Set(key, value, null);
		}

		public override void Set(string key, object value, TimeSpan duration)
		{
  			if (value == null) value = _nullObject;
			var absoluteExperiation = DateTimeOffset.Now + duration;
			_memoryCache.Set(key, value, absoluteExperiation);
		}

		public override bool Get(string key, out object value)
		{
			var cacheItem = _memoryCache.GetCacheItem(key);

			var isCached = cacheItem != null;

			value = isCached ? cacheItem.Value : null;
   			if (Object.ReferenceEquals(value, _nullObject)) value = null;
      
			return isCached;
		}

		public override void Delete(string key)
		{
			_memoryCache.Remove(key);
		}

		public override void Clear()
		{
			foreach (var item in _memoryCache)
			{
				_memoryCache.Remove(item.Key);
			}
		}
	}
}
