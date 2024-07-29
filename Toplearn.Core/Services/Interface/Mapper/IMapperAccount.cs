using System.Security.Claims;
using Toplearn.Core.DTOs.Accounts;
using Toplearn.Core.DTOs.UserPanel;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface.Mapper
{

    public interface IMapperAccount
	{
		public User MapTheUserFromRegisterViewModel(RegisterViewModel registerViewModel);
		public SendEmailHtmlViewModel MapTheSendEmailHtmlViewModelFromUser(User user);
    }
}