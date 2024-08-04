using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Toplearn.Web.Pages
{
    public class PagesModel : PageModel
    {
		public void CreateMassageAlert(string typeOfAlert, string descriptionOfAlert, string titleOfAlert, bool? isTake = false)
		{
			TempData["Massage_TypeOfAlert"] = typeOfAlert;
			TempData["Massage_DescriptionOfAlert"] = descriptionOfAlert;
			TempData["Massage_TitleOfAlert"] = titleOfAlert;
			if (isTake == true)
			{
				TempData.Keep();
			}
		}

	}
}
