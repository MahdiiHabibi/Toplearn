using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.DataLayer.Entities.Permission
{
	public class Permission
	{
		public Permission()
		{
			
		}

		[Key]
		public int PermissionId { get; set; }

		public string PermissionDetail { get; set; }

		public string PermissionName { get; set; }

		public int? ParentId { get; set; } = -1;



		#region Navigation Props

		public virtual List<RolesPermissions>? RolesPermissionsList { get; set; }


		[ForeignKey(nameof(ParentId))]
		public virtual List<Permission> ParentPermission { get; set; }

		#endregion


	}
}
