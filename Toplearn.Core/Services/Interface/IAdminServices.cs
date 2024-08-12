using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface
{
	public interface IAdminServices
	{
		public ShowUsersViewModel GetUsersForShow(int pageId = 1, int take = 2, string filterEmail = "", string filterUserName = "", string filterFullname = "");

		public Task<bool> UpdateUser(User user);
	}
}
