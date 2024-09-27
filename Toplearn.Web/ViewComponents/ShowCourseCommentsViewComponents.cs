using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.Services.Interface;

namespace Toplearn.Web.ViewComponents
{
	public class ShowCourseCommentsViewComponents(ICourseServices courseServices) : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync(int courseId, int pageId)
		{
			var model = courseServices.ShowComments(courseId, 10, pageId);
			ViewData["courseId"] = courseId;
			ViewData["pageId"] = pageId;
			model.CourseComments = model.CourseComments.Where(x => x.AccessFromAdmin).ToList();
			return await Task.FromResult<IViewComponentResult>(View(model));
		}
	}
}
