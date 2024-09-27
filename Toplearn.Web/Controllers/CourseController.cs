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

			if (model.PageCount < pageId && model.PageCount != 0)
			{
				return RedirectToAction("Index", "Course", new { pageId = 1, filter = filter, priceType = priceType, orderByType = orderByType, startPrice = startPrice, endPrice = endPrice, selectedGroups = selectedGroups, take = take });
			}

			return View(model);
		}


		[Route("/course/{courseName}/{courseId:int}")]
		public IActionResult Index(int courseId, string courseName, int pageId = 1)
		{
			var model = courseServices.GetCourseForShow(courseId);

			if (model == null)
			{
				return NotFound();
			}

			ViewBag.Comment = "";
			ViewBag.DegreeOfTeacher = null;
			ViewBag.DegreeOfCourse = null;
			ViewBag.Errors = TempData["Errors"]!;

			ViewBag.CourseStudentCount = courseServices.GetCourseStudentCounts(courseId);
			ViewData["pageId"] = pageId;

			return View("course", model);
		}


		[HttpPost]
		public IActionResult SubmitComment(SubmitCommentViewModel submitCommentViewModel)
		{
			if (User.Identity == null || User.Identity.IsAuthenticated == false)
			{
				return RedirectToAction("Login", "Account", new { BackUrl = $"/course/TopLearn/{submitCommentViewModel.CourseId}?pageId={submitCommentViewModel.pageId}" });
			}

			if (!ModelState.IsValid)
			{
				List<string> errors = new List<string>();
				foreach (var error in ModelState
							 .Where(x =>
								 x.Value != null && !x.Value.ValidationState
									 .ToString()
									 .Equals("Valid", StringComparison.CurrentCultureIgnoreCase))
							 .Select(x => x.Value.Errors))
				{
					errors.AddRange(error.Select(x => x.ErrorMessage));
				}

				errors.DistinctBy(x => x);
				TempData["Errors"] = errors;
				ViewBag.Comment = submitCommentViewModel.Comment;
				ViewBag.DegreeOfTeacher = submitCommentViewModel.DegreeOfTeacher;
				ViewBag.DegreeOfCourse = submitCommentViewModel.DegreeOfCourse;

				return Redirect($"/Course/TopLearn/{submitCommentViewModel.CourseId}?pageId={submitCommentViewModel.pageId}");
			}

			var comment = new CourseComment()
			{
				UserId = GetUserIdFromClaims(),
				AccessFromAdmin = false,
				Comment = submitCommentViewModel.Comment,
				CourseId = submitCommentViewModel.CourseId,
				CreateDate = DateTime.Now
			};

			bool res = courseServices.AddComment(comment);
			
			if (res)
				CreateMassageAlert("success", "دید گاه شما ثبت شد بعد از تایید به نمایش خواهد آمد .", "موفق ");
			else
				CreateMassageAlert("danger", "عملیات با شکست رو به رو شد .", "ناموفق ");


			return Redirect($"/Course/TopLearn/{submitCommentViewModel.CourseId}?pageId={submitCommentViewModel.pageId}");
		}
	}
}
