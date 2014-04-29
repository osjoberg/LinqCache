using System.Linq.Expressions;

namespace LinqCache.KeyGenerators
{
	public class CustomKeyGenerator : KeyGenerator
	{
		private readonly string _key;

		public CustomKeyGenerator(string key)
		{
			_key = key;
		}

		public override string GetKey(Expression expression)
		{
			return _key;
		}
	}
}
