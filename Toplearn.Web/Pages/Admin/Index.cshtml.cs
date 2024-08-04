using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Toplearn.Web.Pages.Admin
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }

        public void OnHead()
        {
	        HttpContext.Response.Headers.Add("Head Test", "Handled by OnHead!");

		}
	}
}
