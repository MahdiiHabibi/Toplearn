using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement
{
	public class RoleManager(TopLearnContext context,IContextActions<Role> contextActionsForRole) : IRoleManager
	{
		private readonly TopLearnContext _context = context;
		private readonly IContextActions<Role> _contextActionsForRole = contextActionsForRole;

		public async Task<string[]> GetRolesOfUser(Func<User, bool> func)
			=> _context.Users.
				Include(x => x.UserRoles)
				.ThenInclude(x => x.Role)
				.Single(func)
				.UserRoles.Select(x =>
					new string(x.Role.RoleDetail)).ToArray();

		public async Task<List<Role>> GetRolesOfTopLearn() =>
			_context.Roles.ToList();

		public async Task<List<ShowAddEditRoleViewModel>> GetRolesForShows(int userId)
		{
			var rolesOfTopLearn = await GetRolesOfTopLearn();
			var rolesOfUser = await GetRolesOfUser(x => x.UserId == userId);

			var roles =
				rolesOfTopLearn
					.Select(item =>
						new ShowAddEditRoleViewModel()
						{
							RoleId = item.RoleId,
							RoleDetail = item.RoleDetail,
							IsChecked = rolesOfUser.Any(x => x == item.RoleDetail)
						}).ToList();

			return roles;
		}

		public async Task<bool> UpdateOfUserRoles(int userId, int[] roles)
		{
			try
			{
				// Remove All Role Of User
				_context.UserRoles.RemoveRange(_context.UserRoles.Where(x => x.UserId == userId));

				IEnumerable<User_Role> userRoles = roles.Select(x => new User_Role()
				{
					UserId = userId,
					RoleId = x
				});

				// Add Checked Roles
				await _context.UserRoles.AddRangeAsync(userRoles);

				// If SaveChangesAsync does its job correctly it will send the number one .
				await _context.SaveChangesAsync();
				
				return true;

			}
			catch
			{
				return false;
			}


		}

		public async Task<bool> AddRole(string roleDetails)
		{
			var role = new Role()
			{
				RoleDetail = roleDetails
			};
			return await _contextActionsForRole.AddToContext(role);
		}
	}
}