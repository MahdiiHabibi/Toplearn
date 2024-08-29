using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Security.Attribute.AuthorizeWithPermissionAttribute;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[CheckIVG]
	[CheckUIC]
	public class RoleManagerController(IRoleManager roleManager, IContextActions<Role> contextActionsForRole,IPermissionServices permissionServices) : TopLearnController
	{

		private readonly IRoleManager _roleManager = roleManager;

		[Route("/Admin/Roles")]
		[Permission("Admin_Roles_Index")]
		public async Task<IActionResult> Index()
		{
			var roles = await _roleManager.GetRolesOfTopLearn();
			roles.Remove(roles.Single(x => x.RoleId == 4));

			return View(roles);
		}


		#region Update User Role

		[HttpPost]
		[Permission("Admin_Roles_UpdateUserRole")]
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
		[Permission("Admin_Roles_AddRole")]
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

		[Permission("Admin_Roles_ChangeRoleStatus")]
		public async Task<IActionResult> ChangeRoleStatus(int roleId)
		{
			var role = await _roleManager.GetRoleById(roleId, true);
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

		[Permission("Admin_Roles_EditRole")]
		public async Task<IActionResult> EditRole(int roleId)
		{
			var role = await _roleManager.GetRoleForEdit(roleId);
			if (role == null)
				return NotFound();

			TempData["RoleDetail"] = role!.RoleDetail;
			return View(role);
		}

		[HttpPost]
		[Permission("Admin_Roles_EditRole")]
		public async Task<IActionResult> EditRole(AddEditRoleViewModel role)
		{
			if (!ModelState.IsValid)
			{
				return View("EditRole", role);
			}

			if (await _roleManager.UpdateRole(role))
			{
				if (await permissionServices.UpdateRolePermissions(role.PermissionsOfRole,role.RoleId))
				{
					CreateMassageAlert("success", "تغییرات با موفقیت انجام یافت .", "موفق ");
				}

				else
				{
					CreateMassageAlert("warning","اطلاعات مقام تغییر یافت ولی در ثبت دسترسی مقام مشکلی به وجود آمده است .","توجه");
				}
			}
			else
			{
				CreateMassageAlert("danger", "در ارسال اطلاعات مشکلی به وجود آمده است .", "خطا ");
			}


			return RedirectToAction("Index", "RoleManager");
		}

		#endregion

		#region Edit Permissions Of Role

		[Route("/Admin/RoleManager/EditPermissions/{roleId:int}")]
		[Permission("Admin_Roles_EditPermissionsOfRole")]
		public async Task<IActionResult> EditPermissionsOfRole(int roleId)
		{
			return View();
		}

		#endregion

	}
}
