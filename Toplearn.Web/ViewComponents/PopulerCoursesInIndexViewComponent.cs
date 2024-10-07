using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Web.ViewComponents
{
	public class PopulerCoursesInIndexViewComponent(ICourseServices courseServices) : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var model = courseServices.GetCourse(orderByType: "populer", take: 8).ShowCoursesWithBoxViewModels;

			return await Task.FromResult((IViewComponentResult)
				View("PopulerCoursesInIndexViewComponent",model));

		}

	}
}
