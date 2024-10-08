﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Accounts;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Interface
{
    public interface IUserAction
	{
		public Task<bool> IsEmailExist(string email);
		public Task<bool> IsUserNameExist(string username);
		public Task<User> SignUpUser(RegisterViewModel registerViewModel);
		public Task<bool> IsActiveCodeExist(string activecode);
		public Task<bool> ActiveAccount(string activecode);
		public Task<bool> IsEmailActived(string email);
		public Task<User?> CheckTheInfoForLogin(LoginViewModel loginViewModel);
		public Task<bool> SendTheVerificationCodeWithEmail(User user, string View, string subject, string HostUrl, string BackUrl = "2%home2%index");
		public Task<User> GetUserByEmail(string email);
		public Task<bool> ChangePassword(ResetPasswordViewModel resetPasswordViewModel);



	}
}
