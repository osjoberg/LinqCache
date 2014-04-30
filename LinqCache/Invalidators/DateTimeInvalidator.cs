using System;

namespace LinqCache.Invalidators
{
	/// <summary>
	/// Invalidates the cached data at the specified date and time.
	/// </summary>
	public class DateTimeInvalidator : Invalidator
	{
		/// <summary>
		/// Date and time when the data is invalidated.
		/// </summary>
		public DateTime Invalidate { get; private set; }

		/// <summary>
		/// Create a new instance of the data time invalidator.
		/// </summary>
		/// <param name="invalidate"></param>
		public DateTimeInvalidator(DateTime invalidate)
		{
			Invalidate = invalidate;
			Duration = invalidate - DateTime.Now;
			UsesDuration = true;
		}
	}
}
