using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using System.Xml.Schema;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement
{
	public class RoleManager(TopLearnContext context, IContextActions<Role> contextActionsForRole, IUtilities utilities, IPermissionServices permissionServices) : IRoleManager
	{
		private readonly TopLearnContext _context = context;
		private readonly IContextActions<Role> _contextActionsForRole = contextActionsForRole;
		private readonly IPermissionServices _permissionServices = permissionServices;

		public Task<string[]?> GetRolesOfUser(Func<User, bool> func, bool enableIgnoreQueryFilters = false)
		{
			try
			{
				var user = _context.Users.Single(func);

				IQueryable<Role> roles = _context.Roles.Where(x => x.UserRoles.Any(x => x.UserId == user.UserId));

				if (enableIgnoreQueryFilters)
				{
					roles = roles.IgnoreQueryFilters();
				}


				return Task.FromResult(roles
					.Select(x =>
						new string(x.RoleDetail))
					.ToArray());
			}
			catch
			{
				return Task.FromResult<string[]?>(null);
			}
		}

		public Task<List<Role>> GetRolesOfTopLearn() =>
			Task.FromResult(_context.Roles.IgnoreQueryFilters().ToList());

		public async Task<List<ShowAddEditRoleOfUserViewModel>> GetRolesForShows(int userId)
		{
			var rolesOfTopLearn = await GetRolesOfTopLearn();
			foreach (var role in rolesOfTopLearn.Where(x => x.IsActived == false).ToList())
			{
				rolesOfTopLearn.Remove(role);
			}

			var rolesOfUser = await GetRolesOfUser(x => x.UserId == userId);

			var roles =
				rolesOfTopLearn
					.Select(item =>
						new ShowAddEditRoleOfUserViewModel()
						{
							RoleId = item.RoleId,
							RoleDetail = item.RoleDetail,
							IsChecked = rolesOfUser!.Any(x => x == item.RoleDetail)
						}).ToList();

			return roles;
		}

		public async Task<bool> UpdateOfUserRoles(int userId, int[] roles)
		{
			try
			{
				// Remove All Role Of User
				_context.UserRoles.RemoveRange(_context.UserRoles.Where(x => x.UserId == userId));

				List<User_Role> userRoles = roles.Select(x => new User_Role()
				{
					UserId = userId,
					RoleId = x
				})
					.ToList();

				// Remove The owner Of Site Role
				userRoles.Remove(new User_Role()
				{
					UserId = userId,
					RoleId = 4
				});

				// Add Checked Roles
				await _context.UserRoles.AddRangeAsync(userRoles);

				// If SaveChangesAsync does its job correctly it will send the number one .
				await _context.SaveChangesAsync();

				await utilities.ChangeUICOfUser(await _context.Users.SingleAsync(x => x.UserId == userId));

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

		public async Task<Role?> GetRoleById(int id, bool enableIgnoreQueryFilters = false)
		{
			IQueryable<Role> roles = _context.Roles;

			if (enableIgnoreQueryFilters)
			{
				roles = roles.IgnoreQueryFilters();
			}

			return await roles.SingleOrDefaultAsync(x => x.RoleId == id);
		}

		public async Task<bool> UpdateRole(Role role)
		{
			try
			{
				await utilities.ChangeIVGOfTopLearn();
				return await _contextActionsForRole.UpdateTblOfContext(role);
			}
			catch
			{
				return false;
			}
		}
		
        public async Task<AddEditRoleViewModel> GetRoleForEdit(int roleId)
		{
			var role = (await _context.Roles.IgnoreQueryFilters()
				.Where(x => x.RoleId == roleId)
				.Include(x => x.RolesPermissionsList)!
				.ThenInclude(x => x.Permission)
				.SingleOrDefaultAsync());

			if (role == null)
			{
				return null;
			}

			AddEditRoleViewModel model = new()
			{
				RoleId = role.RoleId,
				RoleDetail = role.RoleDetail,
				IsActived = role.IsActived,
				PermissionsOfRole = []
			};

			model.PermissionsOfRole.AddRange(await _permissionServices.GetPermissionForShowInRole(role));
			
			return model;
		}
	}
}