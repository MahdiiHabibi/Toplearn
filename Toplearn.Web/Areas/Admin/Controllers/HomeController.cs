using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize("CheckIdentityValodationGuid")]
	[Authorize("GeneralAdminPolicy")]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
