using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Interface;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{

	[Area("Admin")]
	[Authorize]
	[CheckIVG]
	[CheckUIC]
	[Route("/Admin/Offs")]
	public class CoursesOffController(IAdminServices adminServices, ICourseServices courseServices, ICategoryServices categoryServices) : TopLearnController
	{
		[Route("")]
		public IActionResult Index(bool fromTeacher = false, int pageId = 1, int take = 5, string courseNameFilter = "")
		{
			if (take <= 0)
				return RedirectToAction("Index", "CoursesOff");

		
			var model = adminServices.GetOffsForShow(fromTeacher ? GetUserIdFromClaims() : 0, pageId, take, courseNameFilter);

			ViewData["pageId"] = pageId;
			ViewData["take"] = take;
			ViewData["CourseNameFilter"] = courseNameFilter;

			
			if (model.PageCount < pageId && model.CourseOffs.Count != 0)
			{
				return RedirectToAction("Index", "CoursesOff", new { pageId = 1, take = take, courseNameFilter = courseNameFilter });
			}

			if (fromTeacher)
				ViewBag.CoursesOffIndexTe = "active";
			else
				ViewBag.CoursesOffIndex = "active";


			return View(model);
		}




		[Route("Add/{type}")]
		public async Task<IActionResult> AddOff(string type = "Courses")
		{
			if (type == AddOffType.Courses.ToString())
			{
				return View("AddOffCourses", new AddOffToCoursesViewModel()
				{
					Courses = (await courseServices.GetCourses(x => x.CourseOff == null, true) ?? throw new NullReferenceException("", new ArgumentNullException("type"))).Select(x => (x.CourseId, x.CourseName)).ToList()
				});
			}
			if (type == AddOffType.Categories.ToString())
			{
				return View("AddOffCategories", new AddOffToCategoriesViewModel()
				{
					Categories = categoryServices
						.GetCategories(null)
						.Select(x => (x.CategoryId, x.CategoryName))
						.ToList()
				});
			}
			return NotFound();
		}

		[Route("TeacherAdd")]
		public async Task<IActionResult> AddOffForTeachersCourses()
		{
			ViewBag.AddOffForTeachersCourses = "active";

			return View("AddOffCourses", new AddOffToCoursesViewModel()
			{
				Courses = (await courseServices
					.GetCourses(x => x.CourseOff == null && x.TeacherId == GetUserIdFromClaims(), true)
						   ?? throw new NullReferenceException("", new ArgumentNullException("type")))
					.Select(x => (x.CourseId, x.CourseName))
					.ToList()
			});
		}

		[HttpPost]
		public IActionResult AddOffToCourses(AddOffToCoursesViewModel model)
		{
			if (ModelState.IsValid) return View("AddOffCourses", model);

			var res = adminServices.AddOffToCourses(model.CoursesId, model.OffPercent, model.OffEndDate, GetUserIdFromClaims());

			if (res)
			{
				CreateMassageAlert("success", "عملیات با موفقیت انجام شد  .", "موفق ");
				return RedirectToAction("Index");
			}

			else
			{
				CreateMassageAlert("danger", "عملیات با شکست رو به رو شد .", "ناموفق ");
				return View("AddOffCourses", model);
			}
		}

		[HttpPost]
		[Route("/Admin/Offs/AddOffToCategories")]
		public IActionResult AddOffToCategories(AddOffToCategoriesViewModel model)
		{

			if (!ModelState.IsValid)
			{
				model.Categories = categoryServices
					.GetCategories(null)
					.Select(x => (x.CategoryId, x.CategoryName))
					.ToList();

				return View("AddOffCategories", model);
			}

			var courses = (categoryServices.GetCoursesOfCategories(model.CategoriesId).Select(x => x.CourseId)
				.ToList());


			if (courses is { Count: <= 0 })
			{

				ModelState.AddModelError("All", "هیچ دوره ای در گروه انتخابی پیدا نشد !");

				model.Categories = categoryServices
					.GetCategories(null)
					.Select(x => (x.CategoryId, x.CategoryName))
					.ToList();

				return View("AddOffCategories", model);
			}


			var res = adminServices.AddOffToCourses(courses, model.OffPercent, model.OffEndDate, GetUserIdFromClaims());


			if (res)
			{
				CreateMassageAlert("success", "عملیات با موفقیت انجام شد  .", "موفق ");
				return RedirectToAction("Index");
			}

			else
			{
				model.Categories = categoryServices
					.GetCategories(null)
					.Select(x => (x.CategoryId, x.CategoryName))
					.ToList();

				CreateMassageAlert("danger", "عملیات با شکست رو به رو شد .", "ناموفق ");
				return View("AddOffCategories", model);
			}
		}


	}
}
