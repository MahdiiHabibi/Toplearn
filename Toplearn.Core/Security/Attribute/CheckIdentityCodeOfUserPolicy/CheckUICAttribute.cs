using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Implement.Setting;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy
{
	public class CheckUICAttribute : System.Attribute,IAuthorizationFilter
	{
		private IDataProtector _dataProtector;
		public async void OnAuthorization(AuthorizationFilterContext context)
		{
			#region Dependecy Injection

			IUtilities utilities = 
				(IUtilities) context.HttpContext.RequestServices.GetService(typeof(IUtilities))!;
			IUserPanelService userPanelService =
				(IUserPanelService)context.HttpContext.RequestServices.GetService(typeof(IUserPanelService))!;
			_dataProtector =
				((IDataProtectionProvider)context.HttpContext.RequestServices.GetService(
					typeof(IDataProtectionProvider))!).CreateProtector("IdentityValidationGuid");

			#endregion


			var User = await userPanelService.GetUserByUserId(int.Parse(context.HttpContext.User.FindFirst(TopLearnClaimTypes.NameIdentifier)!.Value));

			var protectedUicOfClaims = context.HttpContext.User!.Claims.Single(x => x.Type == TopLearnClaimTypes.UIC).Value;

			var unProtectedUICOfClaims = GetUnProtected(protectedUicOfClaims);

			if (protectedUicOfClaims.IsNullOrEmpty() || unProtectedUICOfClaims.IsNullOrEmpty() || unProtectedUICOfClaims != User.ActiveCode)
			{
				var isPersistent = bool.Parse(context.HttpContext.User.Claims.First(x => x.Type == TopLearnClaimTypes.IsPersistent).Value);
				await utilities.Logout();
				await utilities.Login(User, isPersistent);
			}
		}

		private string GetUnProtected(string protectedIvg)
		{
			try
			{
				return _dataProtector.Unprotect(protectedIvg);
			}
			catch
			{
				return null;
			}
		}
	}
}
