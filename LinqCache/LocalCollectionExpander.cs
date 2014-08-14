using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LinqCache
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
			var replacements = (map.Where(x => x.Param != null && x.Param.IsGenericType)
				.Select(x => new {x, g = x.Param.GetGenericTypeDefinition()})
				.Where(@t => @t.g == typeof (IEnumerable<>) || @t.g == typeof (List<>))
				.Where(@t => @t.x.Arg.NodeType == ExpressionType.Constant)
				.Select(@t => new {@t, elementType = @t.x.Param.GetGenericArguments().Single()})
				.Select(
					@t =>
						new
						{
							@t.@t.x.Arg,
							Replacement =
								Expression.Constant("{" + string.Join("|", (IEnumerable) ((ConstantExpression) @t.@t.x.Arg).Value) + "}")
						})).ToList();

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
