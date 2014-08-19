using System.IO;

namespace LinqCache.Test
{
	class TestDatabase
	{
		internal static string Name { get { return Path.GetFullPath(@"..\..\TestDatabase.mdf"); } }
		internal static string ConnectionString { get { return @"Data Source=(LocalDB)\v11.0;AttachDbFilename=" + Name + ";Integrated Security=True;Connect Timeout=30"; } }
	}
}
