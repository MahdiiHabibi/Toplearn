using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize("CheckIdentityValodationGuid")]
	[Authorize(policy: "GeneralAdminPolicy")]
	public class RoleManagerController(IRoleManager roleManager,TopLearnContext _context) : TopLearnController
	{

		private readonly IRoleManager _roleManager = roleManager;

		[Route("/Admin/Roles")]
		public async Task<IActionResult> Index()
		{
			var roles = await _roleManager.GetRolesOfTopLearn();
			roles.Remove(roles.Single(x=>x.RoleId == 4));

			return View(roles);
		}


		#region Update User Role

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


		#endregion

		#region Add Role

		[HttpPost]
		public async Task<IActionResult> AddRole(string role)
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


		#endregion
		
		#region Change Role Status

		// TODO: Authorize
		public async Task<IActionResult> ChangeRoleStatus(int roleId)
		{
			var role = await _roleManager.GetRoleById(roleId,true);
			if (role == null)
			{
				CreateMassageAlert("danger", "لینک دریافتی برای تغییر اطلاعات مقام اشتباه است .", "خطا ");
			}
			else
			{
				role.IsActived = !role.IsActived;

				if (await _roleManager.UpdateRole(role))
				{
					CreateMassageAlert("success", "تغییرات با موفقیت انجام یافت .", "موفق ");
				}
				else
				{
					CreateMassageAlert("danger", "در ارسال اطلاعات مشکلی به وجود آمده است .", "خطا ");
				}
			}
			return RedirectToAction("Index", "RoleManager");
		}

		#endregion

		#region Edit Role

		public async Task<IActionResult> EditRole(int roleId)
		{
			var role = await _roleManager.GetRoleById(id: roleId);
			TempData["RoleDetail"] = role.RoleDetail;
			return View(role);
		}

		[HttpPost]
		public async Task<IActionResult> EditRole(Role role)
		{
			if (!ModelState.IsValid)
			{
				return View("EditRole", role);
			}

			if (await _roleManager.UpdateRole(role))
			{
				CreateMassageAlert("success", "تغییرات با موفقیت انجام یافت .", "موفق ");
			}
			else
			{
				CreateMassageAlert("danger", "در ارسال اطلاعات مشکلی به وجود آمده است .", "خطا ");
			}


			return RedirectToAction("Index", "RoleManager");
		}

		#endregion

	}
}
