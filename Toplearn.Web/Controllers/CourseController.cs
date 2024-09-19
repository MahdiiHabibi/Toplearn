using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs.Course;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.Web.Security;

namespace Toplearn.Web.Controllers
{
	public class CourseController(ICourseServices courseServices) : TopLearnController
	{

		public IActionResult Index(int pageId = 1, string filter = "",
			string priceType = "all", string orderByType = "date",
			int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null, int take = 9)
		{
			ViewData["take"] = take;
			ViewData["pageId"] = pageId;
			var model = courseServices.GetCourse(pageId, filter, priceType, orderByType, startPrice, endPrice,
				selectedGroups, take);
			//if (model.PageCount ==0)
			//{
			//	return View(new ShowCoursesInSearchViewModel()
			//	{
			//		Categories = courseServices.GetCategories(false),
			//		TotalCourses = 0
			//	});
			//}

			if (model.PageCount < pageId && model.PageCount !=0)
			{
				return RedirectToAction("Index","Course",new { pageId = 1 ,filter = filter,priceType = priceType, orderByType = orderByType, startPrice = startPrice,endPrice = endPrice,selectedGroups = selectedGroups,take = take});
			}

			return View(model);
		}


		[Route("/course/{courseName}/{courseId:int}")]
		public IActionResult Index(int courseId,string courseName)
		{
			var model = courseServices.GetCourseForShow(courseId);
			
			return View("course",model);
		}
	}
}
