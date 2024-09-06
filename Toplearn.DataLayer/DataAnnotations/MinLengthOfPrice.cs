using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.DataAnnotations
{
	public class MinLengthOfPrice : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			int price = (int) value;

			return price >= 500000;
		}
	}
}
