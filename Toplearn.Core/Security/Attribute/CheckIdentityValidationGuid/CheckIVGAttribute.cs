using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Implement.Setting;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CheckIVGAttribute : System.Attribute, IAuthorizationFilter
	{
		private IDataProtector _dataProtector;
		private HttpContext _httpContext;
		private IUtilities _utilities;
		private IUserPanelService _userPanelService;
		private IMemoryCache _memoryCache;

		public async void OnAuthorization(AuthorizationFilterContext context)
		{
			#region Dependecy Injection 

			_httpContext = ((IHttpContextAccessor)context.HttpContext.RequestServices.GetService(typeof(IHttpContextAccessor))!).HttpContext;
			_dataProtector = ((IDataProtectionProvider)context.HttpContext.RequestServices.GetService(typeof(IDataProtectionProvider))!).CreateProtector("IdentityValidationGuid");
			_utilities = ((IUtilities)context.HttpContext.RequestServices.GetService(typeof(IUtilities))!);
			_userPanelService = (IUserPanelService)context.HttpContext.RequestServices.GetService(typeof(IUserPanelService))!;
			_memoryCache = (IMemoryCache)context.HttpContext.RequestServices.GetService(typeof(IMemoryCache))!;

			#endregion

			if (_httpContext.User.Identity is { IsAuthenticated: false })
			{
				context.Result = new RedirectResult($"/Login?BackUrl={context.HttpContext.Request.Path.ToString().Replace("/", "%2F")}");
				return;
			}
			var UserId = int.Parse(_httpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var sysIVG = await _utilities.IdentityValidationGuid();

			var protectedIvg = GetIVGOfUserCookie();

			if (protectedIvg.IsNullOrEmpty())
			{
				await _utilities.Logout();
				context.Result = new RedirectResult("/Login");
				return;
			}

			var unProtectedIvg = GetUnProtected(protectedIvg, UserId);
			if (unProtectedIvg.IsNullOrEmpty())
			{
				await _utilities.Logout();
				context.Result = new RedirectResult("/Login");
				return;
			}

			SplitIvgCookie(unProtectedIvg, out var userCookieIvg, out var userId);

			if (userId != UserId)
			{
				await _utilities.Logout();
				context.Result = new RedirectResult("/Login");
				return;
			}

			if (userCookieIvg == sysIVG)
			{
				return;
			}

			await ReLoginOfUser(UserId, bool.Parse(context.HttpContext.User.FindFirst(TopLearnClaimTypes.IsPersistent)!.Value));
		}


		private string GetIVGOfUserCookie() =>
			_httpContext.Request.Cookies["IVG"];
		private void SplitIvgCookie(string ivgCookieData, out string userCookieIvg, out int userId)
		{
			var strings = ivgCookieData.Split("|\\|");
			userCookieIvg = strings[0];
			userId = int.Parse(strings[1]);
		}
		private async Task ReLoginOfUser(int userId, bool isPersistent)
		{
			await _utilities.Logout();
			var user = await _userPanelService.GetUserByUserId(userId);
			await _utilities.Login(user, isPersistent);
		}

		private string GetUnProtected(string protectedIvg, int userId)
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
