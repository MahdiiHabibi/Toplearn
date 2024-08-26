using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Permission;

namespace Toplearn.Core.Services.Interface
{
	public interface IPermissionServices
	{

		public ImmutableHashSet<Permission> Permissions { get; }

		public Task<List<Permission>> GetListOfPermissionsOfTopLearn();

		public Task<List<Permission>> GetListOfPermissionsOfUser(int userId);

		public Task<int[]> GetIdOfUserPermissionsFromCookie();

		public Task<HashSet<Permission>?> GetUserPermissionsFromCookie();
	}
}
