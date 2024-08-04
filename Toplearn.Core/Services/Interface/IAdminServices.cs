using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;

namespace Toplearn.Core.Services.Interface
{
	public interface IAdminServices
	{
		public ShowUserViewModel GetUsersForShow(int pageId = 1, int take = 2, string filterEmail = "", string filterUserName = "", string filterFullname = "");
	}
}
