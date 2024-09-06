using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Toplearn.Core.Security
{
	public static class FileValidator
	{
		public static bool IsImage(this IFormFile file)
		{
			try
			{
				var res = Image.FromStream(file.OpenReadStream());
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
