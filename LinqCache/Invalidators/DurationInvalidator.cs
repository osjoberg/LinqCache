using System;

namespace LinqCache.Invalidators
{
	public class DurationInvalidator : Invalidator
	{
		public TimeSpan Invalidate { get; private set; }

		public DurationInvalidator(TimeSpan invalidate)
		{
			Duration = invalidate;
			Invalidate = invalidate;
			SupportsDuration = true;
		}
	}
}
