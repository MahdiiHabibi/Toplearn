using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.ISendEmail;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Security.Identity.CheckIVGAuthotization
{
	public class CheckIVGPolicyHandler(
		IHttpContextAccessor contextAccessor,
		IUtilities utilities,
		IMemoryCache memoryCache,
		IDataProtectionProvider dataProtector,
		IUserPanelService userPanelService)
		: AuthorizationHandler<CheckIVGPolicyRequirment>
	{
		private readonly IDataProtector _dataProtector = dataProtector.CreateProtector("IdentityValidationGuid");
		private readonly HttpContext _httpContext = contextAccessor.HttpContext;

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckIVGPolicyRequirment requirement)
		{
			
			if (_httpContext.User.Identity is { IsAuthenticated: false })
			{
				context.Fail();
				return;
			}

			requirement.UserId = int.Parse(_httpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var sysIVG = await utilities.IdentityValidationGuid();

			var protectedIvg = GetIVGOfUserCookie();
			if (protectedIvg.IsNullOrEmpty())
			{
				await ReLoginOfUser(requirement.UserId, bool.Parse(context.User.FindFirst(TopLearnClaimTypes.IsPersistent).Value));
				await utilities.SendIVG(requirement.UserId);
				_httpContext.Request.Path = "/UserPanel";
				context.Succeed(requirement);
				return;
			}

			var unProtectedIvg = GetUnProtected(protectedIvg, requirement.UserId);
			if (unProtectedIvg.IsNullOrEmpty())
			{
				await utilities.Logout();
				context.Fail();
				return;
			}

			SplitIvgCookie(unProtectedIvg, out var userCookieIvg, out var userId);

			if (userId != requirement.UserId)
			{
				await utilities.Logout();
				context.Fail();
				return;
			}
			if (userCookieIvg == sysIVG)
			{
				context.Succeed(requirement);
			}
			else
			{
				await ReLoginOfUser(requirement.UserId,bool.Parse(context.User.FindFirst(TopLearnClaimTypes.IsPersistent).Value));
				await File.AppendAllTextAsync("D:\\m.json", JsonConvert.SerializeObject(_httpContext.Request.Cookies));
				_httpContext.Request.Path = _httpContext.Request.Path;
			}
		}


		private string GetIVGOfUserCookie() =>
			_httpContext.Request.Cookies["IVG"];
		private void SplitIvgCookie(string ivgCookieData, out string userCookieIvg, out int userId)
		{
			var strings = ivgCookieData.Split("|\\|");
			userCookieIvg = strings[0];
			userId = int.Parse(strings[1]);
		}
		private async Task ReLoginOfUser(int userId,bool isPersistent)
		{
			await utilities.Logout();
			var user = await userPanelService.GetUserByUserId(userId);
			await utilities.Login(user,isPersistent);
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