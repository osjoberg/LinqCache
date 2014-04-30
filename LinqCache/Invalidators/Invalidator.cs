using System;
using LinqCache.Containers;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Abstract base class for cache invalidators.
	/// </summary>
	public abstract class Invalidator
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
