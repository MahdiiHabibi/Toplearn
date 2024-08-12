using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement.Mapper
{
	public class MapperAdmin(IMapper _mapper) : IMapperAdmin
	{
		public UserForShowAddEditRoleViewModel MapUserForShowAddEditRoleViewModelFromUser(User user)
		{
			return _mapper.Map<UserForShowAddEditRoleViewModel>(user);
		}

		public ShowUserViewModel MapUserForShowUserViewModelFromUser(User user)
		{
			return _mapper.Map<ShowUserViewModel>(user);
		}
	}
}
