using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.Core.Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Toplearn.Core.DTOs.UserPanel;
using TopLearn.Core.Security;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
	[CheckIVG]
	[Route("UserPanel")]
	public class HomeController(IMapperUserPanel mapperUserPanel, IUserPanelService userPanelService, IWalletManager walletManager) : TopLearnController
	{
		[Route("")]
		public async Task<IActionResult> Index()
		{
			var userModel = mapperUserPanel.MapTheUserPanelViewModelFromClaims(User.Claims.ToList());
			userModel.WalletBalance = await walletManager.GetBalanceOfUser(userModel.UserId);
			return View(userModel);
		}


		#region Edit Profile


		[Route("EditProfile")]
		public IActionResult EditProfile()
		{
			var userModel = mapperUserPanel.MapTheEditPanelViewModelFromClaims(User.Claims);
			return View(userModel);
		}

		[HttpPost]
		[Route("EditProfile")]
		public async Task<IActionResult> EditProfile(EditPanelViewModel editPanelViewModel)
		{
			if (ModelState.IsValid == false)
			{
				return View();
			}

			// Check that User Change Image Or Not !
			var user = await userPanelService.GetUserByUserId(
				int.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value));
			if (user == null)
			{
				return NoContent();
			}

			var claims = new List<Claim>();
			user.FullName = editPanelViewModel.FullName;
			claims.Add(new Claim(ClaimTypes.Name, editPanelViewModel.FullName));

			#region CheckUserName

			if (user.UserName != editPanelViewModel.UserName)
			{
				// UserName is Unique
				var IsUserNameExist = await userPanelService.IsUserNameExist(editPanelViewModel.UserName);
				if (IsUserNameExist)
				{
					ModelState.AddModelError("UserName", "نام کاربری جدیدی که انتخاب کرده اید از قبل موجود است .");
					return View(editPanelViewModel);
				}

				user.UserName = editPanelViewModel.UserName;
				claims.Add(new Claim("UserName", editPanelViewModel.UserName));
			}

			#endregion

			#region CheckUserImage

			var res = await userPanelService.ImageTaskInEditUser(User.Claims.Single(x => x.Type == "ImageUrl").Value,
				editPanelViewModel.ImageFile);

			var imageResDescription = "";
			var imageResAlertType = "";
			if (res != string.Empty)
			{
				imageResAlertType = "success";
				user.ImageUrl = res;
				claims.Add(new Claim("ImageUrl", res));
			}
			else
			{
				imageResAlertType = "info";
				imageResDescription = "ولی در ثبت عکس جدید شما مشکلی به وجود آمد";
			}

			#endregion

			// Update User 
			if (await userPanelService.UpdateUser(user))
			{
				await UpdateUserClaims(user);
				CreateMassageAlert(imageResAlertType, "اطلاعات شما با موفقیت تغییر یافت ." + imageResDescription,
					"موفق");
				return RedirectToAction("Index", "Home");
			}
			else
			{
				if (res != string.Empty && res != @"\images\pic\Default.png")
				{
					System.IO.File.Delete(res);
				}

				CreateMassageAlert("danger",
					"در ثبت اطلاعات جدید شما مشکلی به وجود آمده است. دوباره تلاش کرده و در صورت خطای مجدد دوباره لاگین کنید .",
					"ناموفق");
				return RedirectToAction("Index", "Home");
			}

		}

		#endregion

		#region RessetUserImage

		[Route("ResetImageOfUser")]
		public async Task<IActionResult> ResetImageOfUser()
		{
			var userId = GetUserIdFromClaims();
			var user = await userPanelService.GetUserByUserId(userId);
			var removedImageUrl = $"{Directory.GetCurrentDirectory()}\\wwwroot{user.ImageUrl}";
			user.ImageUrl = @"\images\pic\Default.png";

			// Update User 
			if (await userPanelService.UpdateUser(user))
			{
				await UpdateUserClaims(user);

				if (System.IO.File.Exists(removedImageUrl))
				{
					System.IO.File.Delete(removedImageUrl);
				}

				CreateMassageAlert("success", "عکس شما با موفقیت تغییر یافت .",
					"موفق");
				return RedirectToAction("Index", "Home");
			}
			else
			{
				CreateMassageAlert("danger",
					"در ثبت عکس شما مشکلی به وجود آمده است .",
					"ناموفق");
				return RedirectToAction("Index", "Home");
			}


		}


		#endregion

		#region Methods


		
		[Route("CheckUserInEdit", Name = "CheckUserNameIsExist")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> CheckUserNameIsExist(string username)
		{
			var UserNameInClaims = User.Identity.IsAuthenticated
				? User.Claims.SingleOrDefault(x => x.Type == "UserName")!.Value
				: "";

			if (await userPanelService.IsUserNameExist(username) && UserNameInClaims != username)
			{
				return Json("این نام کاربری از قبل موجود است .");
			}

			return Json(true);
		}

		[Route("CheckEmailIsExist", Name = "CheckEmailIsExist")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public async Task<IActionResult> CheckEmailIsExist(string email)
		{
			var emailInUserClaims = User.Identity.IsAuthenticated
				? User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)!.Value
				: "";
			if (await userPanelService.IsEmailExist(email) && emailInUserClaims != email)
			{
				return Json("این ایمیل از قبل موجود است .");
			}

			return Json(true);
		}


		#endregion

		#region ChangePassword

		[Route("ChangePassword")]
		public IActionResult ChangePassword()
		{
			return View(new ChangePasswordViewModel());
		}

		[Route("ChangePassword")]
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
		{
			var userId = GetUserIdFromClaims();
			var user = await userPanelService.GetUserByUserId(userId);
			if (user == null)
			{
				CreateMassageAlert("danger", "مشکلی به وجود آمده است . لطفا مجددا وارد شوید .", "بروز خطا");
				return RedirectToAction("Logout", "Account");
			}

			if (user.Password != changePasswordViewModel.OldPassword.EncodePasswordMd5())
			{
				ModelState.AddModelError("OldPassword", "رمز عبور فعلی شما نادرست میباشد .");
				return View(changePasswordViewModel);
			}
			user.Password = changePasswordViewModel.NewPassword.EncodePasswordMd5();
			if (await userPanelService.UpdateUser(user))
			{
				await UpdateUserClaims(user);
				CreateMassageAlert("success", "رمز عبور شما با موفقیت تغییر یافت .",
					"موفق");
				return RedirectToAction("Index", "Home");
			}

			CreateMassageAlert("danger",
				"در ثبت اطلاعات جدید شما مشکلی به وجود آمده است. دوباره تلاش کرده و در صورت خطای مجدد به ما اطلاع دهید .",
				"ناموفق");
			return RedirectToAction("Index", "Home");

		}


		#endregion
	}
}
