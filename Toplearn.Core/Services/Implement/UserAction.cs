using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Toplearn.Core.Convertors;
using Toplearn.Core.Generator;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.Core.DTOs.Accounts;
using TopLearn.Core.Security;

namespace Toplearn.Core.Services.Implement
{
	// I Initialize the Fields with Primary ctor
	public class UserAction(IContextActions<User> contextActionsForUser, IMapperAccount mapperAccount, IContextActions<User_Role> contextActionsForUserRole, IViewRenderService viewRender) : IUserAction
	{
		private readonly IContextActions<User> _contextActionsForUser = contextActionsForUser;
		private readonly IContextActions<User_Role> _contextActionsForUserRole = contextActionsForUserRole;
		private readonly IViewRenderService _viewRender = viewRender;
		public async Task<bool> IsEmailExist(string email) =>
			await _contextActionsForUser.Exists(x => x.Email == email.FixedEmail());

		public async Task<bool> IsUserNameExist(string username) =>
			await _contextActionsForUser.Exists(x => x.UserName == username.FixedUsername());

		public async Task<bool> IsActiveCodeExist(string activecode) =>
			await _contextActionsForUser.Exists(x => x.ActiveCode == activecode);

		public async Task<bool> ActiveAccount(string activecode)
		{
			try
			{
				var user = await _contextActionsForUser.GetOne(x => x.ActiveCode == activecode);
				user.IsActive = true;
				user.ActiveCode = StringGenerate.GuidGenerate();
				var res = await _contextActionsForUser.UpdateTblOfContext(user);

				if (!res) return res;

				var RoleToUser = new User_Role()
				{
					RoleId = 1,
					UserId = user.UserId
				};
				res = await _contextActionsForUserRole.AddToContext(RoleToUser);
				return res;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> IsEmailActived(string email)
		=> ((await _contextActionsForUser.GetOne(x => x.Email == email.FixedEmail()))!).IsActive;

		public async Task<User?> CheckTheInfoForLogin(LoginViewModel loginViewModel)
		=> await _contextActionsForUser
				.GetOne(x =>
					x.Email == loginViewModel.Email.FixedEmail() &&
					x.Password == loginViewModel.Password.EncodePasswordMd5());

		public async Task<bool> SendTheVerificationCodeWithEmail(User user, string View, string subject, string HostUrl, string BackUrl = "2%home2%index")
		{
			try
			{
				var MassageModel = mapperAccount.MapTheSendEmailHtmlViewModelFromUser(user);
				MassageModel.BackUrl = BackUrl;
				MassageModel.HostUrl = HostUrl;
				var MassageBody = _viewRender.RenderToStringAsync(View, MassageModel);
				return SendEmail.Send(user.Email, subject, MassageBody);
			}
			catch
			{
				return false;
			}
		}

		public async Task<User> GetUserByEmail(string email)
		{
			return await _contextActionsForUser.GetOne(x => x.Email == email.FixedEmail());
		}

		public async Task<User?> SignUpUser(RegisterViewModel registerViewModel)
		{

			try
			{
				// Removing the first and last spaces of the email and UPPER it
				registerViewModel.UserName = registerViewModel.UserName.FixedUsername();
				// Removing the first and last spaces of the Username
				registerViewModel.Email = registerViewModel.Email.FixedEmail();
				// Convert RegisterViewModel To User Entity with AutoMapper Library 
				// AutoMapper was injected in program of WEB and its inheritance is in AutoMapper.cs <Toplearn.Core.Convertors.AutoMapper>
				// The Method Of Map is in MapperAccount.cs <Toplearn.Core.Services.Interface.Mapper> 
				User user = mapperAccount.MapTheUserFromRegisterViewModel(registerViewModel);
				// IContextActions is a Dynamic interface for Add & Update & Remove & GetInfo & ... From Database For All type of Entities
				// AddToContext is in IContextActions and it is the dynamic method too, it will Add to Context and Save in Database
				if (await _contextActionsForUser.AddToContext(user))
				{
					return user;
				}
				return null;
			}
			catch
			{
				return null;
			}

		}

		public async Task<bool> ChangePassword(ResetPasswordViewModel resetPasswordViewModel)
		{
			var user = await _contextActionsForUser.GetOne(x => x.ActiveCode == resetPasswordViewModel.ActiveCode);
			user.Password = resetPasswordViewModel.Password.EncodePasswordMd5();
			user.ActiveCode = StringGenerate.GuidGenerate();
			var res = await _contextActionsForUser.UpdateTblOfContext(user);
			return res;
		}
	}
}
