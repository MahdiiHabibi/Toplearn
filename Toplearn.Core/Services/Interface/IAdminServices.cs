using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.DataLayer.Entities.Order;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface
{
	public interface IAdminServices
	{
		public ShowUsersViewModel GetUsersForShow(int pageId = 1, int take = 2, string filterEmail = "", string filterUserName = "", string filterFullname = "");

		public Task<bool> UpdateUser(User user);

		public ShowDiscountsInAdminViewModel GetDiscountsForShow(int pageId = 1, int take = 2, string filter = "");

		public bool IsDiscountCodeExist(string discountCode);

		public bool AddDiscount(OrderDiscount orderDiscount);

		public ShowOffsViewModel GetOffsForShow(int teacherId = 0, int pageId = 1, int take = 5, string courseFilter = "");

		public bool AddOffToCourses(List<int> courses, int offPercent, DateTime endTime,int adminId);

	}
}
