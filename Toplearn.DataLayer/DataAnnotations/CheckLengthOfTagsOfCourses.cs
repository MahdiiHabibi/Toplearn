using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.DataAnnotations
{

	public class MaxLengthTagsOfCourses : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			var propValue = value as string;
			if (propValue != null)
				return true;
			var propValues = propValue?.Split("-");

			if (propValues == null || propValues.Length == 0)
			{
				return true;
			}

			return propValue != null && propValue.Replace("-", "").Length <= 1000;
		}
	}

}
