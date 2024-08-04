using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.DTOs.Admin
{
	public class UserInformationViewModel
	{
		public User User { get; set; }

		public int Amount { get; set; }

	}
}
