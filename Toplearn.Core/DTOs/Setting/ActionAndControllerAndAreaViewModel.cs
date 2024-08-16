using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Setting
{
	public class ActionAndControllerAndAreaViewModel
	{
		public string AreaName { get; set; }
		public string ActionName { get; set; }
		public string ControllerName { get; set; }
		public bool IsSelected { get; set; } = false;
	}
}
