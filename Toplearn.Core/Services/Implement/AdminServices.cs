using Microsoft.EntityFrameworkCore;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Order;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement
{
	public class AdminServices(TopLearnContext context) : IAdminServices
	{
		public ShowUsersViewModel GetUsersForShow(int pageId = 1, int take = 2, string filterEmail = "", string filterUserName = "", string filterFullname = "")
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

			users.AddRange(
				result.OrderBy(u => u.DateTime)
					.Skip(skip)
					.Take(take)
					.Include(x => x.UserRoles)
					.ThenInclude(r => r.Role));

			var list = new ShowUsersViewModel
			{
				CurrentPage = pageId,
				PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(result.Count()) / take).ToString()),
				Users = users
			};

			return list;
		}

		public async Task<bool> UpdateUser(User user)
		{
			try
			{
				context.Update(user);

				// If SaveChangesAsync does its job correctly it will send the number one .
				int res = await context.SaveChangesAsync();

				// If the result of the above method is True, the number one is returned, and in this case, one is equal to one < (1==1)== true >. But if any other number is returned, the result is false
				return res == 1;

			}
			catch
			{
				return false;
			}
		}

		public ShowDiscountsInAdminViewModel GetDiscountsForShow(int pageId = 1, int take = 2, string filter = "")
		{
			IQueryable<OrderDiscount> result = context.OrderDiscounts;

			if (!string.IsNullOrEmpty(filter))
			{
				result = result.Where(u => u.DiscountCode.Contains(filter));
			}


			// Show Item In Page
			int skip = (pageId - 1) * take;

			List<OrderDiscount> discounts = [];

			discounts.AddRange(
				result.OrderByDescending(d => d.StartDate)
					.Skip(skip)
					.Take(take)
					.Include(x => x.OrderToDiscounts));

			var list = new ShowDiscountsInAdminViewModel
			{
				CurrentPage = pageId,
				PageCount = (int) Math.Ceiling(Convert.ToDouble(result.Count()) / take),
				Discounts = discounts
			};

			return list;

		}

		public bool IsDiscountCodeExist(string discountCode)
		{
			return context.OrderDiscounts.Any(x => x.DiscountCode == discountCode);
		}

		public bool AddDiscount(OrderDiscount orderDiscount)
		{
			try
			{
				context.OrderDiscounts.Add(orderDiscount);
				context.SaveChanges();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}

}
