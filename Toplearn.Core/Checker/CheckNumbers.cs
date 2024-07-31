using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Checker
{
	public static class CheckNumbers
	{
		private const string Numbers = "0123456789";
		public static bool CheckAmount(this int amount)=>
			!(from amountChar in amount.ToString() from number in Numbers where number != amountChar select amountChar).Any();
		
	}
}
