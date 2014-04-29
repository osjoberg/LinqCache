using System;
using System.Linq;
using System.Linq.Expressions;

namespace LinqCache.KeyGenerators
{
	public class ExpressionGeneratedKey : KeyGenerator
	{
		public override string GetKey(Expression expression)
		{
			// locally evaluate as much of the query as possible
			expression = Evaluator.PartialEval(expression, CanBeEvaluatedLocally);

			// support local collections
			expression = LocalCollectionExpander.Rewrite(expression);

			// use the string representation of the expression for the cache key
			string key = expression.ToString();

			return key;
		}

		private static Func<Expression, bool> CanBeEvaluatedLocally
		{
			get
			{
				return expression =>
				{
					// don't evaluate parameters
					if (expression.NodeType == ExpressionType.Parameter)
						return false;

					// can't evaluate queries
					if (typeof(IQueryable).IsAssignableFrom(expression.Type))
						return false;

					return true;
				};
			}
		}
	}
}
