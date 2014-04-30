using System;

namespace LinqCache
{
	static class ArgumentValidator
	{
		public static void IsNotNull<TType>(TType argument, string name) where TType: class 
		{
			if (argument == null)
			{
				throw new ArgumentNullException(name);	
			}			
		}
	}
}
