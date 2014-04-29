using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqCache.KeyGenerators
{
	/// <summary>
	/// Enables cache key support for local collection values.
	/// </summary>
	internal class LocalCollectionExpander : ExpressionVisitor
	{
		public static Expression Rewrite(Expression expression)
		{
			return new LocalCollectionExpander().Visit(expression);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			var instanceType = node.Object == null ? null : node.Object.Type;

			var map = new[] { new { Param = instanceType, Arg = node.Object } }.ToList();
			map.AddRange(node.Method.GetParameters()
				.Zip(node.Arguments, (p, a) => new { Param = p.ParameterType, Arg = a }));
			
			// for any local collection parameters in the method, make a
			// replacement argument which will print its elements
			var replacements = (from x in map
								where x.Param != null && x.Param.IsGenericType
								let g = x.Param.GetGenericTypeDefinition()
								where g == typeof(IEnumerable<>) || g == typeof(List<>)
								where x.Arg.NodeType == ExpressionType.Constant
								let elementType = x.Param.GetGenericArguments().Single()
								select new { x.Arg, Replacement = Expression.Constant("{" + string.Join("|", (IEnumerable)((ConstantExpression)x.Arg).Value) + "}") }).ToList();

			if (replacements.Any())
			{
				var args = map.Select(x => (from r in replacements
											where r.Arg == x.Arg
											select r.Replacement).SingleOrDefault() ?? x.Arg).ToList();

				node = node.Update(args.First(), args.Skip(1));
			}

			return base.VisitMethodCall(node);
		}
	}

}
