using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface.Mapper
{
	public interface IMapperAdmin
	{
		public UserForShowAddEditRoleViewModel MapUserForShowAddEditRoleViewModelFromUser(User  user);

		public ShowUserViewModel MapUserForShowUserViewModelFromUser(User user);
	}
}
