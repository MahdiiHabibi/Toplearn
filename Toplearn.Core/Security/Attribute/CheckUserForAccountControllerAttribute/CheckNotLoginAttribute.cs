using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Toplearn.Core.Security.Attribute.CheckUserForAccountControllerAttribute
{
	 
	public class CheckNotLoginAttribute : System.Attribute,IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			if (context.HttpContext.User.Identity is { IsAuthenticated: false }) return;
			if (!context.HttpContext.Request.Path.Value.Equals("/logout", StringComparison.CurrentCultureIgnoreCase))
			{
				context.Result = new RedirectResult("/");
			}
		}
	}
}
