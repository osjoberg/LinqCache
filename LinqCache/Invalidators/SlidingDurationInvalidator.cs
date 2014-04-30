using System;

namespace LinqCache.Invalidators
{
	public class SlidingDurationInvalidator : Invalidator
	{
		public TimeSpan Invalidate { get; private set; }

		public SlidingDurationInvalidator(TimeSpan invalidate)
		{
			Duration = invalidate;
			Invalidate = invalidate;
			SupportsDuration = true;
		}

		internal protected override void AfterGet(string key, object cachedValue)
		{
			if (Container.SupportsDurationInvalidation)
			{
				Container.Set(key, cachedValue, Duration);	
			}
			else
			{
				Container.Set(key, cachedValue);
			}			
		}
	}
}
