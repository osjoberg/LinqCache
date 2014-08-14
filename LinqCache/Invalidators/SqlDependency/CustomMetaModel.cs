using System;
using System.Collections.Generic;
using System.Data.Linq.Mapping;

namespace LinqCache.Invalidators.SqlDependency
{
	internal class CustomMetaModel : MetaModel
	{
		private readonly MetaModel _model;

		public CustomMetaModel(MetaModel model)
		{
			_model = model;
		}

		public override Type ContextType
		{
			get { return _model.ContextType; }
		}

		public override MappingSource MappingSource
		{
			get { return _model.MappingSource; }
		}

		public override string DatabaseName
		{
			get { return _model.DatabaseName; }
		}

		public override Type ProviderType
		{
			get { return _model.ProviderType; }
		}

		public override MetaTable GetTable(Type rowType)
		{
			return new CustomMetaTable(_model.GetTable(rowType), _model);
		}

		public override IEnumerable<MetaTable> GetTables()
		{
			return _model.GetTables();
		}

		public override MetaFunction GetFunction(System.Reflection.MethodInfo method)
		{
			return _model.GetFunction(method);
		}

		public override IEnumerable<MetaFunction> GetFunctions()
		{
			return _model.GetFunctions();
		}

		public override MetaType GetMetaType(Type type)
		{
			return _model.GetMetaType(type);
		}
	}

}
