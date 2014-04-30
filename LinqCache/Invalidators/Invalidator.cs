using System;
using System.Linq;
using LinqCache.Containers;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Abstract base class for cache invalidators.
	/// </summary>
	public abstract class Invalidator
	{
		/// <summary>
		/// On initialization of the invalidator.
		/// </summary>
		internal protected virtual void OnInit(Container container, IQueryable query, string key)
		{			
		}

		/// <summary>
		/// After a cache hit has occured.
		/// </summary>
		internal protected virtual void OnCacheHit(Container container, IQueryable query, string key, object cachedValue)
		{
		}

		/// <summary>
		/// After a cache miss has occured.
		/// </summary>
		internal protected virtual void OnCacheMiss(Container container, IQueryable query, string key, object value)
		{			
		}

		/// <summary>
		/// If the invalidator uses duration.
		/// </summary>
		internal protected bool UsesDuration { get; protected set; }

		/// <summary>
		/// Duration until invalidation occurs if invalidator uses duration.
		/// </summary>
		internal protected TimeSpan Duration { get; protected set; }		
	}
}
