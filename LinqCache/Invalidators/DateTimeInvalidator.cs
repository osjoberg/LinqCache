using System;

namespace LinqCache.Invalidators
{
	public class DateTimeInvalidator : Invalidator
	{
		public DateTime Invalidate { get; private set; }

		public DateTimeInvalidator(DateTime invalidate)
		{
			Invalidate = invalidate;
			Duration = invalidate - DateTime.Now;
			SupportsDuration = true;
		}
	}
}
