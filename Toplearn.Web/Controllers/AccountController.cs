using Microsoft.AspNetCore.DataProtection;
using System.Numerics;
using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Toplearn.Core.Convertors;
using Toplearn.Core.DTOs;
using Toplearn.Core.DTOs.Accounts;
using Toplearn.Core.Security.Attribute.CheckUserForAccountControllerAttribute;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web.Controllers
{
	[CheckNotLogin]
	public class AccountController(IUserAction userAction, IUtilities utilities, IDataProtectionProvider dataProtectionProvider) : TopLearnController
	{
		private readonly IDataProtector _dataProtector = dataProtectionProvider.CreateProtector("IdentityValidationGuid");

		#region Register

		[HttpGet]
		[Route("Register")]
		public IActionResult Register(string BackUrl = null)
		{
			var registerViewModel = new RegisterViewModel
			{
				// Check The Url If there is No Problem : it returns the input , else : It returns the Url Of the Login Page
				BackUrl = CheckTheBackUrl(BackUrl).Replace("/", "%2")
			};

			return View(registerViewModel);
		}

		[HttpPost]
		[Route("Register")]
		public async Task<IActionResult> Register(RegisterViewModel registerViewModel, string BackUrl = null)
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
						return RedirectToAction("Login", "Account", new { BackUrl = CheckTheBackUrl(registerViewModel.BackUrl) });
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

		[Route("Login/{BackUrl?}")]
		public IActionResult Login(string BackUrl = null)
		{

			var loginViewModel = new LoginViewModel()
			{
				RememberMe = false,
				// Check The Url If there is No Problem : it returns the input , else : It returns the Url Of the Login Page
				BackUrl = CheckTheBackUrl(BackUrl).Replace("/", "%2")
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

						#region Set Identity Validation Guid (IVG)

						bool res = await SendIVG(user.UserId);

						#endregion

						#endregion
						
						
						if (res)
						{
							CreateMassageAlert("primary"
								, $"ورود به اکانت خود با موفقیت انجام شد"
								, $"سلام  {user.FullName}");
							return Redirect(loginViewModel.BackUrl.Replace("%2", "/"));
						}

						ModelState.AddModelError("Email","مشکلی به وجود آمده است از بروز بودن مرور گر خود اطمینان حاصل کنید .");
						return View(loginViewModel);

					}
				}
			}
			ModelState.AddModelError("Email", "رمز عبور یا ایمیل وارد شده درست نمی باشد و باهم تطابقت ندارند . ");
			return View(loginViewModel);
		}

		#endregion

		#region Logout

		[Route("Logout")]
		public IActionResult Logout()
		{
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			HttpContext.Response.Cookies.Delete("IVG");
			return RedirectToAction("Login", "Account");
		}

		#endregion

		#region Activation Account

		[Route("Account/ActiveAccount/{ActiveCode}/{BackUrl?}")]
		public async Task<IActionResult> ActiveAccount(string ActiveCode, string BackUrl = "2%UserPanel")
		{
			if (User.Identity.IsAuthenticated)
			{
				CreateMassageAlert("danger", "شما با اکانت دیگری وارد سایت شده اید .", "غیر قابل انجام  ");
				return RedirectToAction("index", "Home");
			}
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
					Url.Action("Login", "Account", new { BackUrl = CheckTheBackUrl(BackUrl) })
					: "/") ?? "/");

		}



		#endregion

		#region ForgotPassWord

		[Route("/ForgotPassword")]
		public IActionResult ForgotPassword() =>
			 User.Identity.IsAuthenticated ? RedirectToAction("index", "Home") : View();


		[Route("/ForgotPassword")]
		[HttpPost]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			if (User.Identity.IsAuthenticated)
			{
				CreateMassageAlert("danger", "شما با اکانت دیگری وارد سایت شده اید .", "غیر قابل انجام  ");
				return RedirectToAction("index", "Home");
			}

			ViewBag.Email = email;
			if (await userAction.IsEmailExist(email))
			{
				TempData["Url"] = Request.Scheme + "://" + Request.Host;
				bool res = await userAction.SendTheVerificationCodeWithEmail(await userAction.GetUserByEmail(email), "_ForgotPassword", "لینک بازیابی کلمه ی رمز عبور", Request.Scheme + "://" + Request.Host, "2%home2%index");

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
		public async Task<IActionResult> ResetPassword(string ActiveCode, string BackUrl = "%2UserPanel")
		{
			if (User.Identity.IsAuthenticated)
			{
				CreateMassageAlert("danger", "شما با اکانت دیگری وارد سایت شده اید .", "غیر قابل انجام  ");
				return RedirectToAction("index", "Home");
			}
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
			if (User.Identity.IsAuthenticated)
			{
				CreateMassageAlert("danger", "شما با اکانت دیگری وارد سایت شده اید .", "غیر قابل انجام  ");
				return RedirectToAction("index", "Home");
			}

			if (!ModelState.IsValid)
				return View(resetPasswordViewModel);

			if (await userAction.IsActiveCodeExist(resetPasswordViewModel.ActiveCode) == false)
			{
				CreateMassageAlert("danger", "لینک ارسالی شما معتبر نمی باشد .", "ناموفق");
				return RedirectToAction("Login", "Account");
			}
			if (await userAction.ChangePassword(resetPasswordViewModel))
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

		#region Access Denied

		[Route("AccessDenied")]
		public IActionResult AccessDenied() =>
			User.Identity is { IsAuthenticated: false } ? Redirect("/") : View();


		#endregion

		#region Set Identity Validation Guid (IVG)

		private async System.Threading.Tasks.Task<bool> SendIVG(int userId)
		{
			var ivg = await utilities.RoleValidationGuid();
			if (ivg == null)
			{
				return false;
			}

			try
			{
				HttpContext.Response.Cookies.Append("IVG", _dataProtector.Protect($"{ivg}|\\|{userId}"),
					new CookieOptions()
					{
						MaxAge = TimeSpan.FromMinutes(43200),
						HttpOnly = true,
						Secure = true,
						SameSite = SameSiteMode.Lax
					});
				return true;
			}

			catch
			{
				return false;
			}
		}

		#endregion
	}
}
