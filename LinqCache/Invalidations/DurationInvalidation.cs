using System;

namespace LinqCache.Invalidations
{
	public class DurationInvalidation : Invalidation
	{
		public TimeSpan Invalidate { get; private set; }

		public DurationInvalidation(TimeSpan invalidate)
		{
			Duration = invalidate;
			Invalidate = invalidate;
			SupportsDuration = true;
		}
	}
}
