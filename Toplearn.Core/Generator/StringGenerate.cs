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
	}
}
