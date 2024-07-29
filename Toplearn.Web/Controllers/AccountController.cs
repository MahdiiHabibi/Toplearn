
using System.Numerics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Toplearn.Core.Convertors;
using Toplearn.Core.DTOs;
using Toplearn.Core.DTOs.Accounts;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Controllers
{

	public class AccountController(IUserAction userAction) : Controller
	{

		#region Register

		[HttpGet]
		[Route("Register/{BackUrl_Url?}")]
		public IActionResult Register(string BackUrl_Url = null)
		{
			var registerViewModel = new RegisterViewModel
			{
				// Check The Url If there is No Problem : it returns the input , else : It returns the Url Of the Login Page
				BackUrl = CheckTheBackUrl(BackUrl_Url)
			};

			return View(registerViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View(registerViewModel);
			}

			if (await userAction.IsEmailExist(registerViewModel.Email))
			{
				ModelState.AddModelError("Email", "ایمیل وارد شده قبلا در سایت ما ثبت نام کرده است. میتوانید وارد سایت شوید .");
				return View(registerViewModel);
			}

			if (await userAction.IsUserNameExist(registerViewModel.UserName))
			{
				ModelState.AddModelError("UserName", "با این نام کاربری قبلا ثابت نام شده است .");
				return View(registerViewModel);
			}

			var user = await userAction.SignUpUser(registerViewModel);

			switch (user)
			{
				case null:
					CreateMassageAlert("danger",
					   "مشکلی در ثبت نام شما در پایگاه داده سایت ما به وجود آمده است .",
					   "بروز خطا !"
					   );
					return View(registerViewModel);
				default:
					var res = await userAction.SendTheVerificationCodeWithEmail(user, "_ActiveEmail", "فعالسازی حساب کاربری شما در تاپ لرن", Request.Scheme + "://" + Request.Host, CheckTheBackUrl(registerViewModel.BackUrl));
					if (res)
					{
						CreateMassageAlert("success",
							"ثبت نام شما با موفقیت انجام شد ." + $"برای فعالسازی حساب کاربری خود لینک فرستاده شده به ایمیل شما را باز کنید ."
							, "موفق  "
						);
						return RedirectToAction("Login", "Account",new {BackUrl=CheckTheBackUrl(registerViewModel.BackUrl)});
					}
					else
					{
						ModelState.AddModelError("Email", "ثبت نام شما با موفقیت انجام شد ولی در ارسال لینک فعال سازی اکانت شما مشکلی به وجود آمده است .");
						return View(registerViewModel);
					}

			}
		}


		#endregion

		#region Login

		[Route("Login/{BackUrl_Url?}")]
		public IActionResult Login(string BackUrl_Url = null)
		{
			var loginViewModel = new LoginViewModel()
			{
				RememberMe = false,
				// Check The Url If there is No Problem : it returns the input , else : It returns the Url Of the Login Page
				BackUrl = CheckTheBackUrl(BackUrl_Url)
			};


			return View(loginViewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel loginViewModel)
		{

			if (ModelState.IsValid == false)
			{
				return View(loginViewModel);
			}

			if (await userAction.IsEmailExist(loginViewModel.Email))
			{
				User? user = await userAction.CheckTheInfoForLogin(loginViewModel);
				if (user != null)
				{
					if (await userAction.IsEmailActived(loginViewModel.Email) == false)
					{
						ModelState.AddModelError("Email", "حساب کاربری شما فعال نمیباشد . اکانت خود را با لینک فرستاده شده به ایمیل خود فعال نمایید ");
						return View(loginViewModel);
					}
					else
					{
						#region SetLoginCookies

						var claims = new List<Claim>()
						{
							new(ClaimTypes.NameIdentifier,user.UserId.ToString()),
							new(ClaimTypes.Name,user.FullName),
							new ("ImageUrl",user.ImageUrl),
							new ("UserName",user.UserName),
							new (ClaimTypes.Email,user.Email),
							new ("DateTimeOfRegister",user.DateTime.ToString())
						};
						var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						var principal = new ClaimsPrincipal(identity);
						var properties = new AuthenticationProperties
						{
							IsPersistent = loginViewModel.RememberMe
						};
						await HttpContext.SignInAsync(principal, properties);

						#endregion

						CreateMassageAlert("primary"
							, $"ورود به اکانت خود با موفقیت انجام شد"
							 , $"سلام  {user.FullName}");
						return Redirect(loginViewModel.BackUrl ?? "/");
					}
				}
			}
			ModelState.AddModelError("Email", "رمز عبور یا ایمیل وارد شده درست نمی باشد و باهم تطابقت ندارند . ");
			return View(loginViewModel);
		}

		#endregion

		#region Methods

		// Check The Url If there is No Problem : it returns the input , else : It returns the Url Of the Login Page
		public string CheckTheBackUrl(string BackUrl)
		{
			if (!BackUrl.IsNullOrEmpty() && Url.IsLocalUrl(BackUrl.Replace("%2", "/")))
			{
				return BackUrl;
			}

			return "";
		}

		public void CreateMassageAlert(string TypeOfAlert, string DescriptionOfAlert, string TitleOfAlert)
		{
			TempData["Massage_TypeOfAlert"] = TypeOfAlert;
			TempData["Massage_DescriptionOfAlert"] = DescriptionOfAlert;
			TempData["Massage_TitleOfAlert"] = TitleOfAlert;
		}

		#endregion

		#region Logout

		[Route("Logout")]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Account");
		}

		#endregion

		#region Activation Account

		[Route("Account/ActiveAccount/{ActiveCode}/{BackUrl?}")]
		public async Task<IActionResult> ActiveAccount(string ActiveCode, string BackUrl = " ")
		{
			string TypeOfAlert = string.Empty;
			string DescriptionOfAlert = string.Empty;
			string TitleOfAlert = string.Empty;

			if (ActiveCode.IsNullOrEmpty() || !(await userAction.IsActiveCodeExist(ActiveCode)))
			{
				TypeOfAlert = "danger";
				DescriptionOfAlert = "ایمیلی که برای فعالسازی آن از این لینک استفاده کرده اید یافت نشد" + " ";
				TitleOfAlert = "ناموفق  ";
			}
			else
			{

				if (!(await userAction.ActiveAccount(ActiveCode)))
				{
					TypeOfAlert = "danger";
					DescriptionOfAlert = "در فعال سازی اکانت شما برای احراز ایمیل شما مشکلی به وجود آمده است ." +
										 "دوباره تلاش کرده و در صورت خطای دوباره اطلاع دهید .";
					TitleOfAlert = "بروز خطا ! ";
				}
				else
				{
					TypeOfAlert = "success";
					DescriptionOfAlert =
						"حساب کاربری شما با موفقیت فعال گردید میتوانید وارد حساب کاربری خود با ایمیل و رمز عبور خود شوید . ";
					TitleOfAlert = "موفق  ";
				}
			}

			CreateMassageAlert(TypeOfAlert, DescriptionOfAlert, TitleOfAlert);

			return Redirect(
				(TypeOfAlert == "success" ?
					Url.Action("Login", "Account", new { BackUrl = CheckTheBackUrl(BackUrl), Request.Scheme })
					: "/Register") ?? "/Register");

		}

		[Route("/ActiveAccount")]
		public IActionResult ActiveAccount()
		{
			return View("ActiveAccount");
		}


		[HttpPost]
		[Route("/ActiveAccount")]
		public async Task<IActionResult> ActiveAccount(string email)
		{
			if ((await userAction.IsEmailExist(email)) == false || (await userAction.IsEmailActived(email)) == true)
			{
				CreateMassageAlert("info",
					"ایمیلی که وارد کرده اید یا نا معتبر است یا از قبل در سامانه فعال شده است",
					"بروز خطا !");
				return RedirectToAction("index", "Home");
			}

			var res = await userAction.SendTheVerificationCodeWithEmail(await userAction.GetUser(email), "_ActiveEmail", "فعالسازی حساب کاربری شما در تاپ لرن", Request.Scheme + "://" + Request.Host, "");
			if (res)
			{
				CreateMassageAlert("success",
					"ثبت نام شما با موفقیت انجام شد ." + $"برای فعالسازی حساب کاربری خود لینک فرستاده شده به ایمیل شما را باز کنید ."
					, "موفق  "
				);
				return RedirectToAction("Login", "Account");
			}
			else
			{
				CreateMassageAlert("danger", " در ارسال لینک فعال سازی اکانت شما مشکلی به وجود آمده است .","نا موفق");
				return RedirectToAction("Login", "Account");
			}


		}

		#endregion

		#region ForgotPassWord

		[Route("/ForgotPassword")]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[Route("/ForgotPassword")]
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			ViewBag.Email = email;
			if (await userAction.IsEmailExist(email))
			{
				TempData["Url"] = Request.Scheme + "://" + Request.Host;
				bool res = await userAction.SendTheVerificationCodeWithEmail(await userAction.GetUser(email), "_ForgotPassword", "لینک بازیابی کلمه ی رمز عبور", Request.Scheme + "://" + Request.Host, "2%home2%index");

				if (res)
				{
					CreateMassageAlert("success",
						"لینک بازیابی رمز عبور اکانت شما برای اکانت شما ارسال شد ."
						, "موفق  "
					);
					ViewBag.Email = null;
					return RedirectToAction("Login", "Account");
				}
				else
				{
					CreateMassageAlert("danger", "در ارسال لینک بازیابی خطایی رخ داده است . ایمیل خود را چک کرده و دوباره امتحان کنید .", "نا موفق");
					return View("ForgotPassword");
				}
			}
			CreateMassageAlert("danger", "ایمیل خود را دوباره چک کنید .", "نا موفق");
			return View("ForgotPassword");
		}


		[Route("Account/ResetPassword/{ActiveCode}/{BackUrl?}")]
		public async Task<IActionResult> ResetPassword(string ActiveCode, string BackUrl = "Account/Login")
		{
			var x = Request.Scheme + "://" + Request.Host;
			if (await userAction.IsActiveCodeExist(ActiveCode))
				return View(new ResetPasswordViewModel()
				{
					ActiveCode = ActiveCode
				});
			CreateMassageAlert("danger", "لینک ارسالی شما معتبر نمی باشد .", "ناموفق");
			return RedirectToAction("Login", "Account");

		}

		[HttpPost]
		[Route("/ResetPassword")]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordViewModel)
		{
			if (!ModelState.IsValid)
				return View(resetPasswordViewModel);

			if (await userAction.IsActiveCodeExist(resetPasswordViewModel.ActiveCode) == false)
			{
				CreateMassageAlert("danger", "لینک ارسالی شما معتبر نمی باشد .", "ناموفق");
				return RedirectToAction("Login", "Account");
			}
			if (await userAction.ChangePassowrd(resetPasswordViewModel))
			{
				CreateMassageAlert("success", "رمز عبور شما با موفقیت تغییر یافت .", "موفق ");
			}
			else
			{
				CreateMassageAlert("danger", "در تغییر رمز عبور شما مشکلی به وجود آمد دقایقی قبل دوباره تلاش کنید .", "بروز خطا  ");
			}
			return RedirectToAction("login", "Account");
		}

		#endregion


	}
}
