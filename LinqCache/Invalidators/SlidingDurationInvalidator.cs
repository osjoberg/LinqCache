using System;
using System.Linq;
using LinqCache.Containers;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Invalidates cached items after a specified duration, if data is requested whithin the time of the duration, the cached data is kept alive for another time of the duration.
	/// </summary>
	public class SlidingDurationInvalidator : Invalidator
	{
		/// <summary>
		/// Sliding duration.
		/// </summary>
		public TimeSpan Invalidate { get; private set; }

		/// <summary>
		/// Create a new sliding duration invalidator.
		/// </summary>
		/// <param name="invalidate"></param>
		public SlidingDurationInvalidator(TimeSpan invalidate)
		{
			Duration = invalidate;
			Invalidate = invalidate;
			UsesDuration = true;
		}

		/// <summary>
		/// Whenever the cache is hit.
		/// </summary>
		internal protected override void OnCacheHit(Container container, IQueryable query, string key, object cachedValue)
		{
			container.Set(key, cachedValue, Duration);
		}
	}
}
