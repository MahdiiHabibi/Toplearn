using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.DTOs.Admin
{
	public class ShowUserViewModel
	{
		public List<User> Users { get; set; }
		public int CurrentPage { get; set; }
		public int PageCount { get; set; }
	}
}
