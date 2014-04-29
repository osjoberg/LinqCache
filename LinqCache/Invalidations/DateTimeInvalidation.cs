using System;

namespace LinqCache.Invalidations
{
	public class DateTimeInvalidation : Invalidation
	{
		public DateTime Invalidate { get; private set; }

		public DateTimeInvalidation(DateTime invalidate)
		{
			Invalidate = invalidate;
			Duration = invalidate - DateTime.Now;
			SupportsDuration = true;
		}
	}
}
