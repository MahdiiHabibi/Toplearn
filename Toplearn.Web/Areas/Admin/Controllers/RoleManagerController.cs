using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(policy: "GeneralAdminPolicy")]
	public class RoleManagerController(IRoleManager roleManager) : TopLearnController
	{

		private readonly IRoleManager _roleManager = roleManager;

		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> UpdateUserRole(UserForShowAddEditRoleViewModel model)
		{
			var checkedRoles = model.ShowAddEditRoleViewModels.Where(x => x.IsChecked).Select(r => r.RoleId).ToArray();

			var res = await _roleManager.UpdateOfUserRoles(model.UserId, checkedRoles);
			if (res)
			{
				CreateMassageAlert("success", "بروزرسانی اطلاعات دسترسی کاربر با موفقیت انجام شد", "موفق ");
			}
			else
			{
				CreateMassageAlert("danger", "بروزرسانی اطللاعات درخواستی با مشکلی رو به رو شده است", "نا موفق ");
			}

			return RedirectToAction("UserForShow", "UserManager", new { email = model.Email });
		}


		[HttpGet]
		public async Task<IActionResult> Add()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> Add(string role)
		{
			if (role != null)
			{
				var res = await _roleManager.AddRole(role);
				if (res)
				{
					CreateMassageAlert("success", "افزودن مقام جدید به سایت با موفقیت انجام شد", "موفق  ");
				}
				else
				{
					CreateMassageAlert("danger", "در ثبت مقام جدید مشکلی به وجود آمده است . ", "خطا  ");
				}

			}
			else
			{
				CreateMassageAlert("danger", "تایپ ورودی برای مقام نا معتبر است .  ", "خطا  ");
			}
			return RedirectToAction("index", "RoleManager");
		}
	}
}
