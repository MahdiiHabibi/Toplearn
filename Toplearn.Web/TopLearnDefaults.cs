using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Web
{
	public class TopLearnController:Controller
	{
		[HttpOptions]
		public  void CreateMassageAlert(string typeOfAlert, string descriptionOfAlert, string titleOfAlert, bool? isTake=false)
		{
			TempData["Massage_TypeOfAlert"] = typeOfAlert;
			TempData["Massage_DescriptionOfAlert"] = descriptionOfAlert;
			TempData["Massage_TitleOfAlert"] = titleOfAlert;
			if (isTake==true)
			{
				TempData.Keep();
			}
		}

		[HttpOptions]
		// Check The Url If there is No Problem : it returns the input , else : It returns the Url Of the Login Page
		public string CheckTheBackUrl(string BackUrl)
		{
			if (!BackUrl.IsNullOrEmpty() && Url.IsLocalUrl(BackUrl.Replace("%2", "/")))
			{
				return BackUrl;
			}

			return "%2UserPanel";
		}

		[HttpOptions]
		public int GetUserIdFromClaims()
		{
			return int.Parse(User.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value);
		}
		[HttpOptions]
		public async Task UpdateUserClaims(User user)
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


	}
}
