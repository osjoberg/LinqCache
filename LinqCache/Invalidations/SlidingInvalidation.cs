using System;

namespace LinqCache.Invalidations
{
	public class SlidingInvalidation : Invalidation
	{
		public TimeSpan Invalidate { get; private set; }

		public SlidingInvalidation(TimeSpan invalidate)
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
