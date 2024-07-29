using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Accounts
{
	public class SendEmailHtmlViewModel
	{
		public string ActiveCode { get; set; }

		public string FullName { get; set; }

		public string BackUrl { get; set; }
		public string HostUrl { get; set; }
	}
}
