
using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.Setting;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;

namespace Toplearn.Web.Controllers
{
    public class HomeController : TopLearnController
	{
		public IActionResult Index()
		{
			return View();
		}
		[Route("/p")]
		public async Task<string> p()
		{
			return "ClaimTypes.Email" ;
		}

		#region Access Denied


		[Route("AccessDenied")]
		public IActionResult AccessDenied() => View();


		#endregion


	}

}
