using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.DTOs.Admin
{
	public class AddEditRoleViewModel : Role
	{
		public AddEditRoleViewModel()
		{ }

		public virtual List<PermissionOfRole> PermissionsOfRole { get; set; }
	}

	public class PermissionOfRole : Permission
	{
		public bool IsSelected { get; set; }

		public List<PermissionOfRole>? ParentsPermission { get; set; }
	}
}
