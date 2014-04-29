using System;
using LinqCache.Containers;

namespace LinqCache.Invalidations
{
	/// <summary>
	/// Abstract base class for cache invalidations.
	/// </summary>
	public abstract class Invalidation
	{
		/// <summary>
		/// Acccess to container.
		/// </summary>
		internal protected Container Container { get; set; }

		internal protected virtual void AfterGet(string key, object cachedValue)
		{
		}

		internal protected TimeSpan Duration { get; protected set; }
		internal protected bool SupportsDuration { get; protected set; }
	}
}
