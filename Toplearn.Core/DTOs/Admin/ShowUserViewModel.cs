using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.DTOs.Admin
{
	public class ShowUserViewModel : User
	{
		public ShowUserViewModel()
		{
			ShowWalletsViewModel = [];
			ActiveCode = "nullll";
			Password = "nullll";
		}

		public List<ShowWalletsViewModel> ShowWalletsViewModel { get; set; }
	}
}
