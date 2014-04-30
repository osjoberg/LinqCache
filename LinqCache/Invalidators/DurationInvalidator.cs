using System;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Invalidates the cached data after a specified amount of time.
	/// </summary>
	public class DurationInvalidator : Invalidator
	{
		/// <summary>
		/// Duration to when the data is invalidated.
		/// </summary>
		public TimeSpan Invalidate { get; private set; }

		/// <summary>
		/// Creates a new duration invalidator.
		/// </summary>
		/// <param name="invalidate">Duration to when the data is invalidated.</param>
		public DurationInvalidator(TimeSpan invalidate)
		{
			Duration = invalidate;
			Invalidate = invalidate;
			UsesDuration = true;
		}
	}
}
