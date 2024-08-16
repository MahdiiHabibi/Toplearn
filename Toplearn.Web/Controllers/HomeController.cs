
using Microsoft.AspNetCore.Mvc;
using Toplearn.DataLayer.Entities.Setting;

namespace Toplearn.Web.Controllers
{
	public class HomeController : TopLearnController
	{
		public IActionResult Index()
		{
			return View();
		}
		[Route("/p")]
		public string p()
		{
			return Directory.GetCurrentDirectory();
		}


	}

}
