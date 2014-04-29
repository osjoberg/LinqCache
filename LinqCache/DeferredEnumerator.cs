using System;
using System.Collections;
using System.Collections.Generic;

namespace LinqCache
{
	public class DeferredEnumerable<TType> : IEnumerable<TType>
	{
		private readonly Func<IEnumerable<TType>> _delegate;

		public DeferredEnumerable(Func<IEnumerable<TType>> @delegate)
		{
			_delegate = @delegate;
		}

		public IEnumerator<TType> GetEnumerator()
		{
			return _delegate.Invoke().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}