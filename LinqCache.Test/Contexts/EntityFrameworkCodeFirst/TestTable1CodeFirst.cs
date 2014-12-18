using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LinqCache.Test.Contexts.EntityFrameworkCodeFirst
{
    [Table("TestTable1")]
	public class TestTable1CodeFirst
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int Id { get; set; }
		public string Column { get; set; }
	}
}
