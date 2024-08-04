using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Toplearn.Core.DTOs.Wallet;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;
using System.ComponentModel.DataAnnotations;



namespace Toplearn.Web.Pages.Admin.UserManager
{
	public class UserModel(IUserPanelService userPanelService, IWalletManager walletManager) : PagesModel
	{
		public readonly IWalletManager WalletManager = walletManager;


		public User UserOfModel { get; set; } = new User();


		[BindProperty]
		[Required(ErrorMessage = "مبلغ درخواستی را وارد کنید")]
		public int Amount { get; set; }


		public async Task<IActionResult> OnGet(string email)
		{
			UserOfModel = null;

			if (email == null) return Page();
			
			UserOfModel = await userPanelService.GetUserByEmail(email);
			if (UserOfModel!= null)
			{
				TempData["Email"] = UserOfModel.Email;
				TempData["Fullname"] = UserOfModel.FullName;
				TempData["IsActive"] = UserOfModel.IsActive;
				TempData["UserId"] = UserOfModel.UserId;
			}
			return Page();
		}


		public async Task<IActionResult> OnPostAsync()
		{
			UserOfModel.Email = TempData["Email"].ToString();
			UserOfModel.IsActive = bool.Parse(TempData["IsActive"].ToString());
			UserOfModel.UserId = int.Parse(TempData["UserId"].ToString());

			if (!ModelState.IsValid || Amount <= 0) return RedirectToPage("User", new { email = UserOfModel.Email });

			if (UserOfModel.IsActive)
			{
				var setWallet = await WalletManager.SetWalletIncrease(UserOfModel.UserId, Amount, true, User.Claims.Single(x => x.Type == "UserName").Value);

				if (setWallet != null)
				{
					if (await WalletManager.WalletIncrease(setWallet.WalletId, -1, -1))
					{
						CreateMassageAlert("success", "شارژ حساب با موفقیت انجام شد", "موفق");
						return RedirectToPage("User", new { email = UserOfModel.Email });
					}
				}
				CreateMassageAlert("danger", "در ثبت درخواست مبلغ شارژ مشکلی به وجود آمده است .", "خطا !");
				return RedirectToPage("User", new { email = UserOfModel.Email });

			}
			CreateMassageAlert("warning", "کاربر مورد نظر هنوز ایمیل خود را احراز هویت نکرده است .", "غیر قابل انجام دستور ");
			return RedirectToPage("User", new { email = UserOfModel.Email });
		}
	}
}
