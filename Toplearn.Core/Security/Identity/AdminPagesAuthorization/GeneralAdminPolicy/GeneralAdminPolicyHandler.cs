using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Core.Security.Identity.AdminPagesAuthorization.GeneralAdminPolicy
{
	public class GeneralAdminPolicyHandler(IRoleManager roleManager) : AuthorizationHandler<GeneralAdminPolicyRequirement>
	{
		private readonly IRoleManager _roleManager = roleManager;
		protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, GeneralAdminPolicyRequirement requirement)
		{
			
			if (context.User.Identity is { IsAuthenticated: false })
			{
				context.Fail();
				return;
			}
			requirement.UserId = int.Parse(context.User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
			var roles = await _roleManager.GetRolesOfUser(x => x.UserId == requirement.UserId);
			if (roles.Length == 0)
			{
				context.Fail();
				return;
			}
			if (roles.Any(x => x == "ادمین"))
			{
				context.Succeed(requirement);
			}
			else
			{
				context.Fail();
				return;
			}
			await Task.CompletedTask;
		}
	}
}
