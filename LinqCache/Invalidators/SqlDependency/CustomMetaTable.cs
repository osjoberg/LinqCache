using System.Data.Linq.Mapping;
using System.Reflection;

namespace LinqCache.Invalidators.SqlDependency
{
	class CustomMetaTable : MetaTable
	{
		private readonly MetaTable _table;
		private readonly MetaModel _model;
		private readonly string _tableName;

		public CustomMetaTable(MetaTable table, MetaModel model)
		{
			_table = table;
			_model = model;

			_tableName = (_table.TableName.Contains(".") ? "" : "dbo.") + _table.TableName;

			var tableNameField = table.GetType().GetField("tableName", BindingFlags.NonPublic | BindingFlags.Instance);
			tableNameField.SetValue(_table, _tableName);
		}

		public override MethodInfo DeleteMethod
		{
			get { return _table.DeleteMethod; }
		}

		public override MethodInfo InsertMethod
		{
			get { return _table.InsertMethod; }
		}

		public override MethodInfo UpdateMethod
		{
			get { return _table.UpdateMethod; }
		}

		public override MetaModel Model
		{
			get { return _model; }
		}

		public override string TableName { get { return _tableName; } }

		public override MetaType RowType
		{
			get { return _table.RowType; }
		}
	}
}