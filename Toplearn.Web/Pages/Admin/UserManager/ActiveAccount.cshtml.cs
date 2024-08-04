using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Toplearn.Core.Services.Implement;
using Toplearn.Core.Services.Interface;
using Toplearn.Web.Controllers;

namespace Toplearn.Web.Pages.Admin.UserManager
{
    public class ActiveAccountModel(IUserAction userAction) : PagesModel
	{
		public async Task<IActionResult> OnGet(string email)
		{
			if ((await userAction.IsEmailExist(email)) == false || (await userAction.IsEmailActived(email)) == true)
			{
				CreateMassageAlert("info",
					 "ایمیلی که وارد کرده اید یا نا معتبر است یا از قبل در سامانه فعال شده است",
					 "بروز خطا !",true);
				
			}
			var res = await userAction.SendTheVerificationCodeWithEmail(await userAction.GetUserByEmail(email),
				"_ActiveEmail", "فعالسازی حساب کاربری شما در تاپ لرن", Request.Scheme + "://" + Request.Host, "");
			if (res)
			{
				CreateMassageAlert("success",
					"لینک فعال سازی به ایمیل کاربر ارسال شد ."
					, "موفق  ",true
				);
			}
			else
			{
				CreateMassageAlert("danger", " در ارسال لینک فعال سازی اکانت کاربر مشکلی به وجود آمده است .", "نا موفق",true);
			}

			return Redirect($"/admin/usermanager/user?email={email}");
		}
	}
}
