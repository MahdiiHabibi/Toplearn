using Toplearn.Core.DTOs.Accounts;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface.Mapper
{

    public interface IMapperAccount
	{
		public User MapTheUserFromRegisterViewModel(RegisterViewModel registerViewModel);
		public SendEmailHtmlViewModel MapTheSendEmailHtmlViewModelFromUser(User user);
	}
}