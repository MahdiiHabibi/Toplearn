using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Convertors
{
	public static class PriceRounder
	{
		public static int PriceRound(this int price, bool isRial = true)
		{
			var num = price.ToString();

			var numS = new string[num.Length];

			for (var i = 0; i < num.Length; i++)
			{
				numS[i] = num[i].ToString();
			}

			numS[num.Length - 1] = "0";
			numS[num.Length - 2] = "0";
			numS[num.Length - 3] = "0";
			numS[num.Length - 4] = "0";

			if (isRial)
			{
				numS[num.Length - 5] = "0";
			}

			string model = numS.Aggregate(string.Empty, (current, s) => current + s);

			return int.Parse(model);
		}
	}
}
