
using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Mvc;
using Toplearn.DataLayer.Entities.Setting;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;

namespace Toplearn.Web.Controllers
{
    public class HomeController(IUtilities utilities) : TopLearnController
	{
		public IActionResult Index()
		{
			return View();
		}
		[Route("/p")]
		public async Task<string> p()
		{
			
			return ClaimTypes.Email;
		}

		#region Access Denied


		[Route("AccessDenied")]
		public IActionResult AccessDenied() => View();


		#endregion


	}

}
