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
		public Task<string[]> GetRolesOfUser(Func<User, bool> func);

		public Task<List<Role>> GetRolesOfTopLearn();

		public Task<List<ShowAddEditRoleViewModel>> GetRolesForShows(int userId);

		public Task<bool> UpdateOfUserRoles(int userId, int[] roles);

		public Task<bool> AddRole(string roleDetails);
	}
}