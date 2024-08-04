using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Web.Pages.Admin.UserManager
{
	public class IndexModel(IAdminServices adminServices) : PageModel
	{
		private readonly IAdminServices _adminServices = adminServices;
		public ShowUserViewModel ShowUserViewModel { get; set; }

		public void OnGet(int pageId = 1, int take = 2, string filterUserName = "", string filterEmail = "", string filterFullname = "")
		{
			ViewData["take"] = take;
			ViewData["filterUserName"] = filterUserName;
			ViewData["filterEmail"] = filterEmail;
			ViewData["filterFullname"] = filterFullname;
			ShowUserViewModel = _adminServices.GetUsersForShow(pageId, take, filterEmail, filterUserName, filterFullname);
			
		}
	}
}
