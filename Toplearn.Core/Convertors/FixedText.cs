using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Convertors
{
	public static class FixedText
	{
		public static string FixedEmail(this string email) =>
			email.Trim().ToUpper();

		public static string FixedUsername(this string username) =>
			username.Trim();

		public static string CapitalizeFirstLetter(this string model) =>
			model.First().ToString().ToUpper() + model[1..].ToLower();

	}
}
