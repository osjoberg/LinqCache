using System.Linq.Expressions;

namespace LinqCache.KeyGenerators
{
	public abstract class KeyGenerator
	{
		public abstract string GetKey(Expression expression);
	}
}
