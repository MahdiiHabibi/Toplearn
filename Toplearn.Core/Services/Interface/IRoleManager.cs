using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface
{
	public interface IRoleManager
	{
		public Task<string[]?> GetRolesOfUser(Func<User, bool> func, bool enableIgnoreQueryFilters = false);

		public Task<List<Role>> GetRolesOfTopLearn();

		public Task<List<ShowAddEditRoleOfUserViewModel>> GetRolesForShows(int userId);

		public Task<bool> UpdateOfUserRoles(int userId, int[] roles);

		public Task<bool> AddRole(string roleDetails);

		public Task<Role?> GetRoleById(int id, bool enableIgnoreQueryFilters = false);

		public Task<bool> UpdateRole(Role role);

		public Task<AddEditRoleViewModel> GetRoleForEdit(int roleId);

	}
}