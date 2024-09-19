using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Generator
{
	public class StringGenerate
	{
		public static string GuidGenerate()
		{
			return Guid.NewGuid()
				.ToString()
				.Replace("-", "");
		}
		public static string GuidGenerateWithOutNum()
		{
			return Guid.NewGuid()
				.ToString()
				.Replace("-", "")
				.Replace("1", "a")
				.Replace("2", "b")
				.Replace("3", "c")
				.Replace("4", "d")
				.Replace("5", "e")
				.Replace("6", "r")
				.Replace("7", "t")
				.Replace("8", "y")
				.Replace("9", "u")
				.Replace("0", "i");
		}
	}
}
