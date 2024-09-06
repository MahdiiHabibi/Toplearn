using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Web.ViewComponents
{
	public class ShowCategoriesInMainMenu(ICourseServices courseServices) : ViewComponent
	{

		public async Task<IViewComponentResult> InvokeAsync()
		{
			IEnumerable<Category> categories = courseServices.GetCategories(false);

			return await Task.FromResult((IViewComponentResult)
				View("ShowCategoriesInMainMenu",categories));
		}
	}
}
