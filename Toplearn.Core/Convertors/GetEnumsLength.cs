using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Convertors
{
	public static class GetEnumsLength
	{
		public static int GetLength(this Type model)
		{
			return Enum.GetNames(model).Length;
		}
	}
}
