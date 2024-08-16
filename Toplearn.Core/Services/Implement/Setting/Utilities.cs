using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Toplearn.Core.DTOs.Setting;
using Toplearn.Core.Generator;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Setting;

namespace IdentitySample.Repositories
{
	public class Utilities(
		IMemoryCache memoryCache,
		IHttpContextAccessor contextAccessor,
		IDataProtectionProvider dataProtectionProvider)
		: IUtilities
	{
		private const string ValidCodeFileName = "TopLearnValidationIdentityGuid.json";

		//private static readonly string ValidCodeFilePath =
		//	@$"{Directory.GetCurrentDirectory().Replace("Toplearn.Web", "Toplearn.DataLayer")}\Entities\Setting\{ValidCodeFileName}";

		private static readonly string ValidCodeFilePath =
			@$"{Directory.GetCurrentDirectory()}\{ValidCodeFileName}";

		private readonly IMemoryCache _memoryCache = memoryCache;
		private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("IdentityValidationGuid");


		public IList<ActionAndControllerAndAreaViewModel> AreaAndActionAndControllerNamesList()
		{
			var asm = Assembly.GetExecutingAssembly();

			var contradistinction = asm.GetTypes()
				.Where(type => typeof(Controller).IsAssignableFrom(type))
				.SelectMany(type =>
					type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
				.Select(x => new
				{
					Controller = x.DeclaringType?.Name,
					Action = x.Name,
					Area = x.DeclaringType?.CustomAttributes.Where(c => c.AttributeType == typeof(AreaAttribute))
				});

			var list = new List<ActionAndControllerAndAreaViewModel>();

			foreach (var item in contradistinction)
			{
				if (item.Area.Count() != 0)
				{
					list.Add(new ActionAndControllerAndAreaViewModel()
					{
						ControllerName = item.Controller,
						ActionName = item.Action,
						AreaName = item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault()
					});
				}
				else
				{
					list.Add(new ActionAndControllerAndAreaViewModel()
					{
						ControllerName = item.Controller,
						ActionName = item.Action,
						AreaName = null,
					});
				}
			}

			return list.Distinct().ToList();
		}

		public IList<string> GetAllAreasNames()
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			var contradistinction = asm.GetTypes()
				.Where(type => typeof(Controller).IsAssignableFrom(type))
				.SelectMany(type =>
					type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
				.Select(x => new
				{
					Area = x.DeclaringType?.CustomAttributes.Where(c => c.AttributeType == typeof(AreaAttribute))

				});

			var list = contradistinction.Select(item =>
				item.Area.Select(v => v.ConstructorArguments[0].Value.ToString()).FirstOrDefault()).ToList();

			if (list.All(string.IsNullOrEmpty))
			{
				return new List<string>();
			}

			list.RemoveAll(x => x == null);

			return list.Distinct().ToList();
		}

		public Task<string?> RoleValidationGuid() =>
			GetIVGFromOwnDB();

		public async Task<string> CreateAndSaveNewValidationCode()
		{
			IdentityCookie identityCookie = new()
			{
				CreateTime = DateTime.Now,
				IdentityCode = StringGenerate.GuidGenerate()
			};

			await File.WriteAllTextAsync(ValidCodeFilePath, JsonConvert.SerializeObject(identityCookie));

			return identityCookie.IdentityCode;
		}

		private async Task<string> GetIVGFromOwnDB()
		{
			try
			{
				var getData = await File.ReadAllTextAsync(ValidCodeFilePath);

				IdentityCookie? identityCookie = JsonConvert.DeserializeObject<IdentityCookie>(getData);

				if (identityCookie == null)
					throw new NullReferenceException();

				return identityCookie.IdentityCode;
			}
			catch
			{
				return await CreateAndSaveNewValidationCode();
			}
		}

		public async System.Threading.Tasks.Task<bool> SendIVG(int userId)
		{
			var _httpContext = contextAccessor.HttpContext;

			var ivg = await RoleValidationGuid();
			if (ivg == null)
			{
				return false;
			}

			try
			{
				_httpContext.Response.Cookies.Append("IVG", _dataProtector.Protect($"{ivg}|\\|{userId}"),
					new CookieOptions()
					{
						MaxAge = TimeSpan.FromMinutes(43200),
						HttpOnly = true,
						Secure = true,
						SameSite = SameSiteMode.Lax
					});
				return true;
			}

			catch
			{
				return false;

			}
		}
	}
}
