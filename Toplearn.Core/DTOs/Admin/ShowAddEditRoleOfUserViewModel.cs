using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.Convertors;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.DTOs.Admin
{
	public class ShowAddEditRoleOfUserViewModel
	{
		public int RoleId { get; set; }

		public string RoleDetail { get; set; }

		public bool IsChecked { get; set; }
	}

	public class UserForShowAddEditRoleViewModel : User
	{
		public UserForShowAddEditRoleViewModel()
		{
			ShowAddEditRoleViewModels = [];
			ActiveCode = "nullll";
			Password = "nullll";
		}

		public  List<ShowAddEditRoleOfUserViewModel> ShowAddEditRoleViewModels { get; set; }	
	}
}
