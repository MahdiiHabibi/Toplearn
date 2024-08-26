using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Core.Security.Attribute.AuthorizeWithPermissionAttribute
{
	[AttributeUsage(AttributeTargets.Method)]
	public class PermissionAttribute(string permissionDetail) : System.Attribute, IAuthorizationFilter
	{
		private readonly string _permissionDetail = permissionDetail;

		public async void OnAuthorization(AuthorizationFilterContext context)
		{
			IPermissionServices permissionServices = (IPermissionServices)
															context.HttpContext.RequestServices.GetService(typeof(IPermissionServices))!;
			var permissionsOfUser = await permissionServices.GetUserPermissionsFromCookie();
			
			if (permissionsOfUser == null || permissionsOfUser.All(x => x.PermissionDetail != _permissionDetail))
			{
				context.Result = new RedirectResult("/AccessDenied");
			}
		}
	}
}
