using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using LinqCache.Containers;
using LinqCache.Invalidators;

namespace LinqCache
{
	public class LinqCacheQueryable<TType> : IQueryable<TType>
	{
		private readonly IQueryable<TType> _query;
		private readonly LinqCacheQueryProvider<TType> _provider;

		public LinqCacheQueryable(IQueryable<TType> query, string cacheKey, Container container, Invalidator invalidator)
		{
			_query = query;
			_provider = new LinqCacheQueryProvider<TType>(query, cacheKey, container, invalidator);
		}

		public IEnumerator<TType> GetEnumerator()
		{
			return ((IEnumerable<TType>)_provider.InternalExcecute(_query.Expression, () => _query.ToArray())).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_provider.InternalExcecute(_query.Expression, () => _query.ToArray())).GetEnumerator();
		}

		public Type ElementType
		{
			get { return _query.ElementType; }
		}

		public Expression Expression
		{
			get { return _query.Expression; }
		}

		public IQueryProvider Provider
		{
			get { return _provider; }
		}

		public void Invalidate()
		{
			_provider.Invalidate();
		}
	}
}
