using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Security.Attribute.AuthorizeWithPermissionAttribute;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Implement.Setting;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[CheckIVG]
	[CheckUIC]
	public class HomeController(IUtilities utilities) : TopLearnController
	{
		[Permission("Admin_Home_Index")]
		public IActionResult Index()
		{
			return View();
		}


		[Route("Admin/ChangeIvg")]
		[Permission("ChangeIvg")]
		public async Task<IActionResult> ChangeIvg(string BackUrl = "%2FAdmin")
		{
			var appSetting = await utilities.ChangeIVGOfTopLearn();
			HttpContext.Response.Cookies.Delete("IVG");
			bool res = await utilities.SendIVG(GetUserIdFromClaims());
			CreateMassageAlert(res ? "success" : "warning", "تغییر کد احراز هویت سایت با موفقیت انجام شد . " + (res ? "" : "برای کارکرد درست سایت باید دوباره از به حساب خود وارد شوید ."), "موفق  ");
			return Redirect(CheckTheBackUrl(BackUrl));
		}
	}
}
