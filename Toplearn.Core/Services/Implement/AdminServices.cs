using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement
{
	public class AdminServices(TopLearnContext context) : IAdminServices
	{
		public ShowUserViewModel GetUsersForShow(int pageId = 1, int take = 2, string filterEmail = "", string filterUserName = "",string filterFullname ="")
		{
			IQueryable<User> result = context.Users;

			if (!string.IsNullOrEmpty(filterEmail))
			{
				result = result.Where(u => u.Email.Contains(filterEmail));
			}

			if (!string.IsNullOrEmpty(filterUserName))
			{
				result = result.Where(u => u.UserName.Contains(filterUserName));
			}
			if (!string.IsNullOrEmpty(filterFullname))
			{
				result = result.Where(u => u.FullName.Contains(filterFullname));
			}

			// Show Item In Page

			int skip = (pageId - 1) * take;

			List<User> users = [];
			
			foreach (var user in result.OrderBy(u => u.DateTime).Skip(skip).Take(take)) users.Add(user);

			var list = new ShowUserViewModel
			{
				CurrentPage = pageId,
				PageCount = result.Count() / take,
				Users = users
			};

			return list;
		}
	}

}
