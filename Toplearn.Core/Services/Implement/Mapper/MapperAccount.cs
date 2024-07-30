using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Toplearn.Core.DTOs.Accounts;
using Toplearn.Core.DTOs.UserPanel;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement.Mapper
{
	// I Initialize the Fields with Primary ctor
	public class MapperAccount(IMapper mapper) : IMapperAccount
	{
		public User MapTheUserFromRegisterViewModel(RegisterViewModel registerViewModel) =>
			 mapper.Map<User>(registerViewModel);


		public SendEmailHtmlViewModel MapTheSendEmailHtmlViewModelFromUser(User user) =>
			 mapper.Map<SendEmailHtmlViewModel>(user);


	}
}
