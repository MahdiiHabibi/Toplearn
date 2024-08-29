using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface
{
	public interface IPermissionServices
	{

		public ImmutableHashSet<Permission> Permissions { get; }

		public Task<List<Permission>> GetListOfPermissionsOfTopLearn();

		public Task<List<Permission>> GetListOfPermissionsOfUser(int userId);

		public Task<int[]?> GetIdOfUserPermissionsFromCookie();

		public Task<string[]> GetDetailOfUserPermissionsFromCookie();

		public Task<ImmutableHashSet<Permission>?> GetUserPermissionsFromCookie();

		public Task<List<PermissionOfRole>> GetPermissionForShowInRole(Role role);

		public Task<bool> UpdateRolePermissions(List<PermissionOfRole> permissions, int roleId);

	}
}
