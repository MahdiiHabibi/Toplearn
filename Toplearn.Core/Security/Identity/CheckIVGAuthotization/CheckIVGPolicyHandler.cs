using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
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

		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CheckIVGPolicyRequirment requirement)
		{
			if (context.User.Identity is { IsAuthenticated: false })
			{
				context.Fail();
				return;
			}

			requirement.UserId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
			var sysIVG = await utilities.RoleValidationGuid();
			//var sysIVG = await memoryCache.GetOrCreateAsync("IVG", async p =>
			//{
			//	p.AbsoluteExpiration = DateTimeOffset.MaxValue;
			//	return await utilities.RoleValidationGuid();
			//});

			var protectedIvg = GetIVGOfUserCookie();

			if (protectedIvg == null)
			{
				await ReLoginOfUser(contextAccessor.HttpContext,requirement.UserId);
			}

			SplitIvgCookie(_dataProtector.Unprotect(protectedIvg), out string UserCookieIVG, out int UserId);

			if (UserId != requirement.UserId)
			{
				Logout(contextAccessor.HttpContext);
				context.Fail();
				return;
			}

			if (UserCookieIVG == sysIVG)
			{
				context.Succeed(requirement);
				return;
			}
			else
			{
				await ReLoginOfUser(contextAccessor.HttpContext, requirement.UserId);
				contextAccessor.HttpContext.Request.Path = "/";
			}
		}


		private string GetIVGOfUserCookie() =>
			contextAccessor.HttpContext.Request.Cookies.SingleOrDefault(x => x.Key == "IVG").Value ?? "";


		private void SplitIvgCookie(string ivgCookieData, out string UserCookieIVG, out int UserId)
		{
			var strings = ivgCookieData.Split("|\\|");
			UserCookieIVG = strings[0];
			UserId = int.Parse(strings[1]);
		}


		private async Task ReLoginOfUser(HttpContext _httpContext, int userId)
		{
			Logout(_httpContext);
			var user = await userPanelService.GetUserByUserId(userId);
			await Login(_httpContext, user);
		}



		private void Logout(HttpContext _httpContext)
		{
			_httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			_httpContext.Response.Cookies.Delete("IVG");
		}

		private async Task Login(HttpContext _httpContext, User user)
		{
			#region SetLoginCookies

			var claims = new List<Claim>()
			{
				new(ClaimTypes.NameIdentifier,user.UserId.ToString()),
				new(ClaimTypes.Name,user.FullName),
				new ("ImageUrl",user.ImageUrl),
				new ("UserName",user.UserName),
				new (ClaimTypes.Email,user.Email),
				new ("DateTimeOfRegister",user.DateTime.ToString())
			};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			var properties = new AuthenticationProperties
			{
				IsPersistent = false
			};
			await _httpContext.SignInAsync(principal, properties);

			#region Set Identity Validation Guid (IVG)

			await utilities.SendIVG(user.UserId);

			#endregion

			#endregion
		}
	}
}
