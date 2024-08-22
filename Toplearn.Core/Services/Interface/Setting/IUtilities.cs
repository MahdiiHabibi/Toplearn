using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Setting;
using Toplearn.DataLayer.Entities.Setting;
using Toplearn.DataLayer.Entities.User;

namespace IdentitySample.Repositories
{
	public interface IUtilities
	{
		public Task<string?> IdentityValidationGuid();
		public Task<bool> SendIVG(int userId);
		public Task<AppSetting?> ChangeIVGOfTopLearn();
		public Task<bool> SetCookie(string key, string value);
		public Task<bool> Login(User user,bool isPersistent);
		public Task Logout();
	}
}
