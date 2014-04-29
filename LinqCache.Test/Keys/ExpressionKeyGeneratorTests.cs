using System.Linq;
using LinqCache.KeyGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqCache.Test.Keys
{
	[TestClass]
	public class ExpressionKeyGeneratorTests
	{
		[TestMethod]
		public void GenerateKey_GenratesKeyForExpression()
		{
			var value = "7";
			var expression = new[] { new { Value = "7" } }.AsQueryable().Where(i => i.Value.StartsWith(value));

			var expressionKeyGenerator = new ExpressionGeneratedKey();

			var key = expressionKeyGenerator.GetKey(expression.Expression);

			Assert.AreEqual(@"<>f__AnonymousType2`1[System.String][].Where(i => i.Value.StartsWith(""7""))", key);
		}

		[TestMethod]
		public void GenerateKey_GenratesUniqueKeysForSameExpressionOnDifferentTablesInLinqToSql()
		{
			var context = new LinqToSqlDataContext("");

			var expressionKeyGenerator = new ExpressionGeneratedKey();

			var key1 = expressionKeyGenerator.GetKey(context.TestTable1s.Where(row => row.Column == "test").Expression);
			var key2 = expressionKeyGenerator.GetKey(context.TestTable2s.Where(row => row.Column == "test").Expression);

			Assert.AreNotEqual(key1, key2);
		}
	}
}
