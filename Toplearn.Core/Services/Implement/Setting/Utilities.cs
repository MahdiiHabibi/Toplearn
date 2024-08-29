using Azure.Core;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Toplearn.Core.DTOs.Setting;
using Toplearn.Core.Generator;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Setting;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Core.Security.Identity;
using Toplearn.DataLayer.Entities.Permission;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Toplearn.Core.Services.Implement.Setting
{
	public class Utilities(
		IMemoryCache memoryCache,
		IHttpContextAccessor contextAccessor,
		IDataProtectionProvider dataProtectionProvider,
		IContextActions<AppSetting> contextActionsForAppSetting,
		IPermissionServices _permissionServices,
		IUserPanelService userPanelService)
		: IUtilities
	{

		private readonly IMemoryCache _memoryCache = memoryCache;
		private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("IdentityValidationGuid");
		private readonly HttpContext _httpContext = contextAccessor.HttpContext;

		public Task<bool> SetCookie(string key, string value)
		{

			try
			{
				_httpContext.Response.Cookies.Append(key, _dataProtector.Protect(value), new CookieOptions
				{
					HttpOnly = true,
					Secure = _httpContext.Request.IsHttps,
					Path = _httpContext.Request.PathBase.HasValue ? _httpContext.Request.PathBase.ToString() : "/",
					Expires = DateTime.Now.AddDays(90),
				});
				return Task.FromResult(true);
			}

			catch
			{
				return Task.FromResult(false);
			}
		}

		public async Task<bool> Login(User user, bool isPersistent)
		{
			#region Get Permission

			var permissions = string.Empty;
			List<Permission> userPermission =
				(await _permissionServices.GetListOfPermissionsOfUser(user.UserId))!
				.DistinctBy(x=>x.PermissionId)
				.ToList();

			permissions = userPermission
				.Aggregate(
					permissions, (current, permission) =>
						current + (permission.PermissionId + "\\" + permission.PermissionDetail + "\\" + permission.PermissionPersianDetail + "|"));
			if (!permissions.IsNullOrEmpty())
				permissions = permissions.Remove(permissions.Length - 1);

			#endregion

			#region Get Roles


			var roles = string.Empty;
			await using (var scope = _httpContext.RequestServices.CreateAsyncScope())
			{
				IRoleManager roleManager =
					(IRoleManager)
							scope.ServiceProvider.GetService(typeof(IRoleManager))!;

				var userRoles = await roleManager.GetRolesOfUser(x => x.UserId == user.UserId);

				if (userRoles != null)
				{
					roles = userRoles.Aggregate(roles, (current, role) => current + (role + "|"));
					if (!roles.IsNullOrEmpty())
						roles = roles.Remove(roles.Length - 1);
				}

				else
				{
					roles = "بدون مقام";
				}
			}


			#endregion

			#region Set Login Cookies

			var claims = new List<Claim>()
			{
				new (TopLearnClaimTypes.NameIdentifier,user.UserId.ToString()),
				new (TopLearnClaimTypes.Name,user.FullName),
				new (TopLearnClaimTypes.ImageUrl,user.ImageUrl),
				new (TopLearnClaimTypes.UserName,user.UserName),
				new (TopLearnClaimTypes.Email,user.Email),
				new (TopLearnClaimTypes.DateTimeOfRegister,user.DateTime.ToString()),
				new (TopLearnClaimTypes.IsPersistent,isPersistent.ToString()),
				new (TopLearnClaimTypes.Permission,permissions),
				new (TopLearnClaimTypes.UIC,_dataProtector.Protect(user.ActiveCode)),
				new (TopLearnClaimTypes.Role,roles)
			};

			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			var properties = new AuthenticationProperties
			{
				IsPersistent = isPersistent
			};
			await _httpContext.SignInAsync(principal, properties);

			#region Set Identity Validation Guid (IVG)

			bool res = await SendIVG(user.UserId);

			#endregion

			#endregion

			return res;
		}

		public async Task Logout()
		{
			await _httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			_httpContext.Response.Cookies.Delete("IVG");
		}

		public async Task ChangeUICOfUser(User user)
		{
			user.ActiveCode = StringGenerate.GuidGenerate();
			await userPanelService.UpdateUser(user);
		}

		#region IVG

		public async Task<string?> IdentityValidationGuid() =>
			await _memoryCache.GetOrCreateAsync("IVG", async op =>
			{
				op.AbsoluteExpiration = DateTimeOffset.MaxValue;
				return await GetIvgFromOwnDb();
			});
		private async Task<string> GetIvgFromOwnDb()
		{
			try
			{
				var IVG = (await contextActionsForAppSetting.GetOne(x => x.Key == "IVG"))?.Value;

				if (IVG != null)
				{
					return IVG;
				}
			}
			catch
			{
				// ignored
			}

			var model = await ChangeIVGOfTopLearn();
			return model.Value;
		}
		public async Task<bool> SendIVG(int userId)
		{

			var ivg = await IdentityValidationGuid();
			if (ivg == null)
			{
				return false;
			}

			return await SetCookie("IVG", $"{ivg}|\\|{userId}");
		}
		public async Task<AppSetting?> ChangeIVGOfTopLearn()
		{
			var model = new AppSetting()
			{
				Key = "IVG",
				Value = StringGenerate.GuidGenerate()
			};

			if (await contextActionsForAppSetting.Exists(x => x.Key == "IVG"))
			{
				var res = true;
				while (res)
				{
					res = !(await contextActionsForAppSetting.UpdateTblOfContext(model));
				}
			}
			else
			{
				var res = true;
				while (res)
				{
					res = !(await contextActionsForAppSetting.AddToContext(model));
				}
			}
			_memoryCache.Remove("IVG");
			await IdentityValidationGuid();
			return model;
		}

		#endregion


	}
}