using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.Core.DTOs.UserPanel;
using Toplearn.Core.Generator;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Toplearn.Web.Areas.UserPanel.Controllers
{
	[Area("UserPanel")]
	[Authorize]
	public class HomeController(IMapperUserPanel mapperUserPanel, IUserPanelService _userPanelService) : Controller
	{

		public IActionResult Index()
		{
			var userModel = mapperUserPanel.MapTheUserPanelViewModelFromClaims(User.Claims.ToList());
			return View(userModel);
		}


		#region Edit Profile


		[Route("UserPanel/EditProfile")]
		public IActionResult EditProfile()
		{
			var userModel = mapperUserPanel.MapTheEditPanelViewModelFromClaims(User.Claims);
			return View(userModel);
		}

		[HttpPost]
		[Route("UserPanel/EditProfile")]
		public async Task<IActionResult> EditProfile(EditPanelViewModel editPanelViewModel)
		{
			if (ModelState.IsValid == false)
			{
				return View();
			}

			// Check that User Change Image Or Not !
			var user = await _userPanelService.GetUserByUserId(
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
				var IsUserNameExist = await _userPanelService.IsUserNameExist(editPanelViewModel.UserName);
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

			var res = await _userPanelService.ImageTaskInEditUser(User.Claims.Single(x => x.Type == "ImageUrl").Value,
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
			if (await _userPanelService.UpdateUser(user))
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
		[Route("UserPanel/ResetImageOfUser")]
		public async Task<IActionResult> ResetImageOfUser()
		{
			var userId = GetUserIdFromClaims();
			var  user = await _userPanelService.GetUserByUserId(userId);
			var removedImageUrl = $"{Directory.GetCurrentDirectory()}\\wwwroot{user.ImageUrl}" ;
			user.ImageUrl = @"\images\pic\Default.png";

			// Update User 
			if (await _userPanelService.UpdateUser(user))
			{
				await UpdateUserClaims(user);
				
				if (System.IO.File.Exists(removedImageUrl))
				{
					System.IO.File.Delete(removedImageUrl);
				}

				CreateMassageAlert("success", "عکس شما با موفقیت تغییر یافت ." ,
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

		#region Methods

		public void CreateMassageAlert(string TypeOfAlert, string DescriptionOfAlert, string TitleOfAlert)
		{
			TempData["Massage_TypeOfAlert"] = TypeOfAlert;
			TempData["Massage_DescriptionOfAlert"] = DescriptionOfAlert;
			TempData["Massage_TitleOfAlert"] = TitleOfAlert;
		}

		private async Task UpdateUserClaims(User user)
		{
			await HttpContext.SignOutAsync();
			var claims = new List<Claim>()
			{
				new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
				new(ClaimTypes.Name, user.FullName),
				new("ImageUrl", user.ImageUrl),
				new("UserName", user.UserName),
				new(ClaimTypes.Email, user.Email),
				new("DateTimeOfRegister", user.DateTime.ToString())
			};
			var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var principal = new ClaimsPrincipal(identity);
			var properties = new AuthenticationProperties
			{
				IsPersistent = false
			};
			await HttpContext.SignInAsync(principal, properties);
		}

		[Route("UserPanel/CheckUserInEdit", Name = "CheckUserInEdit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CheckUserInEdit(string username)
		{
			if (await _userPanelService.IsUserNameExist(username) && User.Claims.Single(x => x.Type == "UserName").Value != username)
			{
				return Json("این نام کاربری از قبل موجود است .");
			}

			return Json(true);
		}

		private int GetUserIdFromClaims()
		{
			return int.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
		}

		#endregion
	}
}
