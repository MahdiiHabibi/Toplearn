
using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.Setting;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;

namespace Toplearn.Web.Controllers
{
	public class HomeController(TopLearnContext context, IUtilities utilities) : TopLearnController
	{
		public IActionResult Index()
		{
			return View();
		}

		[Route("/p")]
		public async Task<IActionResult> P()
		{
			var p = context.Permissions.Select(x => x.PermissionId).ToList();
			await context.RolesPermissions.AddRangeAsync(p.Select(x => new RolesPermissions()
			{
				RoleId = 4,
				PermissionId = x
			}));
			await context.SaveChangesAsync();

			return RedirectToAction("ChangeIvg");
		}

		#region Access Denied


		[Route("AccessDenied")]
		public IActionResult AccessDenied() => View();


		#endregion


	}

}
