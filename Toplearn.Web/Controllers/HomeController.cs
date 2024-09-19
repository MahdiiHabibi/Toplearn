
using System.Security.Claims;
using IdentitySample.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Convertors;
using Toplearn.Core.Generator;
using Toplearn.Core.Security;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.Setting;
using Toplearn.DataLayer.Entities.User;
using Toplearn.Web.Security;
using WebMarkupMin.AspNet.Common.Resources;

namespace Toplearn.Web.Controllers
{
	public class HomeController(IUserPanelService userPanelService) : TopLearnController
	{
		public IActionResult Index()
		{
			return View();
		}


		[ValidateAntiForgeryToken]
		[HttpPost]
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


		[HttpPost]
		[ValidateAntiForgeryToken]

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

		#region Access Denied


		[Route("AccessDenied")]
		public IActionResult AccessDenied() => View();


		#endregion


		#region CK Editor

		[HttpPost]
		[Route("/CK-FileUpload")]
		public async Task<JsonResult> UploadImage([FromForm] IFormFile upload)
		{
			if (upload.Length <= 0) return null;
			if (!upload.IsImage())
			{
				return null;
			}

			//your custom code logic here

			//1)check if the file is Image

			//2)check if the file is too large

			//etc

			var extension = Path.GetExtension(upload.FileName).ToLower();

			var fileName = StringGenerate.GuidGenerateWithOutNum() + extension;
			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "CKEditorImages",
				fileName);

			while (System.IO.File.Exists(filePath))
			{
				fileName = StringGenerate.GuidGenerateWithOutNum() + extension;
				filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "CKEditorImages",
				   fileName);
			}

			//save file under wwwroot/images/CKEditorImages folder


			await using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await upload.CopyToAsync(stream);
			}

			var url = $"/images/CKEditorImages/{fileName}";


			return new JsonResult(new
			{
				Uploaded = 1,
				FileName = fileName,
				Url = url
			});
		}

		#endregion

	}

}
