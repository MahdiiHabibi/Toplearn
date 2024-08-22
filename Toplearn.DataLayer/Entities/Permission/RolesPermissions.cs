using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.DataLayer.Entities.Permission
{
	public class RolesPermissions
	{
		public RolesPermissions()
		{
			
		}

		[Key]
		public int RolesPermissionsId { get; set; }

		public int RoleId { get; set; }

		public int PermissionId { get; set; }


		#region Navigation Prop

		[ForeignKey(nameof(RoleId))]
		public virtual Role Role { get; set; }

		[ForeignKey(nameof(PermissionId))]
		public virtual Permission Permission { get; set; }

		#endregion


	}
}
