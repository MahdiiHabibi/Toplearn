using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Web.ViewComponents
{
	public class ShowLastCoursesInIndexViewComponents(ICourseServices courseServices) : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return await Task.FromResult((IViewComponentResult)
				View("ShowLastCoursesInIndexViewComponents", courseServices.GetCourse(orderByType: "date",take:8).ShowCoursesWithBoxViewModels));

		}

	}
}
