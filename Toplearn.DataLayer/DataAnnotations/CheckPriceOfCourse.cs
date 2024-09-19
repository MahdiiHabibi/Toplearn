using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.DataAnnotations
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CheckPriceOfCourse : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			var val = (int)value;

			if (val == 0)
			{
				return true;
			}

			return val >= 500000;
		}
	}
}
