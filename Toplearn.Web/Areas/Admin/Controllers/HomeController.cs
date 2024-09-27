using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Security.Attribute.AuthorizeWithPermissionAttribute;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Implement.Setting;
using Toplearn.Core.Services.Interface;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	[CheckIVG]
	[CheckUIC]
	public class HomeController(IUtilities utilities, IUserPanelService userPanelService) : TopLearnController
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
			CreateMassageAlert(appSetting != null ? "success" : "warning", appSetting != null ? "تغییر کد احراز هویت سایت با موفقیت انجام شد . " : "مشکلی به وجود آمده است", appSetting != null ? "موفق  " : "نا موفق");
			return Redirect(CheckTheBackUrl(BackUrl));
		}

		
	}
}
