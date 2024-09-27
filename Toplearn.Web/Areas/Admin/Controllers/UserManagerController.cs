using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.Core.Security.Attribute.AuthorizeWithPermissionAttribute;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.Mapper;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[CheckIVG]
    [CheckUIC]
	public class UserManagerController(IAdminServices adminServices, IUserAction userAction, IWalletManager walletManager, IMapperAdmin mapperAdmin) : TopLearnController
	{


		private readonly IAdminServices _adminServices = adminServices;
		private readonly IUserAction _userAction = userAction;
		private readonly IWalletManager _walletManager = walletManager;
		private readonly IMapperAdmin _mapperAdmin = mapperAdmin;

		//[Permission("Admin_User_Index")]
		public IActionResult Index(int pageId = 1, int take = 2, string filterUserName = "", string filterEmail = "", string filterFullname = "")
		{
			ViewData["take"] = take;
			ViewData["filterUserName"] = filterUserName;
			ViewData["filterEmail"] = filterEmail;
			ViewData["filterFullname"] = filterFullname;

			var model = _adminServices.GetUsersForShow(pageId, take, filterEmail, filterUserName, filterFullname);
			return View(model);
		}


		[Permission("Admin_UserManager_ActiveAccount")]
		public async Task<IActionResult> ActiveAccount(string email)
		{
			if ((await _userAction.IsEmailExist(email)) == false || (await _userAction.IsEmailActived(email)) == true)
			{
				CreateMassageAlert("info",
					"ایمیلی که وارد کرده اید یا نا معتبر است یا از قبل در سامانه فعال شده است",
					"بروز خطا !", true);

			}

			var res = await _userAction.SendTheVerificationCodeWithEmail(await _userAction.GetUserByEmail(email),
				"_ActiveEmail", "فعالسازی حساب کاربری شما در تاپ لرن", Request.Scheme + "://" + Request.Host, "");
			if (res)
			{
				CreateMassageAlert("success",
					"لینک فعال سازی به ایمیل کاربر ارسال شد ."
					, "موفق  ", true
				);
			}
			else
			{
				CreateMassageAlert("danger", " در ارسال لینک فعال سازی اکانت کاربر مشکلی به وجود آمده است .", "نا موفق",
					true);
			}

			return RedirectToAction("UserForShow", "UserManager",new{email = email});
		}

		[Permission("Admin_UserManager_RemoveUserImage")]
		public async Task<IActionResult> RemoveUserImage(string? email = null)
		{
			var user = await _userAction.GetUserByEmail(email);

			if (user != null)
			{

				var removedImageUrl = $"{Directory.GetCurrentDirectory()}\\wwwroot{user.ImageUrl}";
				user.ImageUrl = @"\images\pic\Default.png";

				// Update User 
				if (await _adminServices.UpdateUser(user))
				{
					await UpdateUserClaims(user);

					if (System.IO.File.Exists(removedImageUrl))
					{
						System.IO.File.Delete(removedImageUrl);
					}

					CreateMassageAlert("success", "عکس  با موفقیت تغییر یافت .",
						"موفق");
				}
				else
				{
					CreateMassageAlert("danger",
						"در ثبت حذف عکس  مشکلی به وجود آمده است .",
						"ناموفق");
				}

			}
			else
			{
				CreateMassageAlert("danger", "ایمیل ارسالی درست نمیباشد  .", "ناموفق");
			}
			return RedirectToAction("Index", "UserManager");
		}


		[Route("/Admin/UserManager/User/{email}")]
		//[Permission("Admin_UserManager_UserForShow")]
		public async Task<IActionResult> UserForShow(string email)
		{
			if (email == null) return RedirectToPage("index");

			User user = await _userAction.GetUserByEmail(email);
			if (user == null) return RedirectToPage("index");

			ShowUserViewModel model = _mapperAdmin.MapUserForShowUserViewModelFromUser(user);
			model.ShowWalletsViewModel = await _walletManager.GetShowWallets(user.UserId);
			return View(model);
		}

		[HttpPost]
		//[Permission("Admin_UserManager_IncreaseTheWallet")]
		public async Task<IActionResult> IncreaseTheWallet(int amount, string email)
		{
			if (amount <= 0 || email == null)
			{
				CreateMassageAlert("danger", "در ثبت درخواست مبلغ شارژ مشکلی به وجود آمده است .", "خطا !");
			}
			else
			{
				var user = await _userAction.GetUserByEmail(email);

				if (!ModelState.IsValid || amount <= 0) return RedirectToPage("User", new { email = user?.Email });

				if (user is { IsActive: true })
				{
					var setWallet = await _walletManager.SetWalletIncrease(user.UserId, amount, true, User.FindFirstValue(ClaimTypes.NameIdentifier));

					if (setWallet != null)
					{
						if (await _walletManager.WalletIncrease(setWallet.WalletId, -1, -1))
						{
							CreateMassageAlert("success", "شارژ حساب با موفقیت انجام شد", "موفق");
						}
					}
					else
					{
						CreateMassageAlert("danger", "در ثبت درخواست مبلغ شارژ مشکلی به وجود آمده است .", "خطا !");
					}
				}

				else
				{
					CreateMassageAlert("warning", "کاربر مورد نظر هنوز ایمیل خود را احراز هویت نکرده است .", "غیر قابل انجام دستور ");
				}
			}

			return RedirectToAction("UserForShow", "UserManager",new{email = email});
		}

	}
}
