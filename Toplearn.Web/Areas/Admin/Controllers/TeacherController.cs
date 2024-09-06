
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Toplearn.Core.Convertors;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.DTOs.Teacher;
using Toplearn.Core.Generator;
using Toplearn.Core.Security;
using TopLearn.Core.Security;
using Toplearn.Core.Security.Attribute.AuthorizeWithPermissionAttribute;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;
using Toplearn.Web.Security;
using Microsoft.AspNetCore.SignalR;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Route("Teacher")]
	[CheckIVG]
	[CheckUIC]

	public class TeacherController(ICourseServices courseServices, IContextActions<Category> contextActionsForCategory) : TopLearnController
	{

		public IActionResult Index()
		{
			return View();
		}


		#region Show Courses

		[Route("Courses")]
		[HttpGet]
		public async Task<IActionResult> Courses(int take = 5, int pageId = 1, string nameFilter = "")
		{
			ViewData["nameFilter"] = nameFilter;
			ViewData["take"] = take;
			ViewData["pageId"] = pageId;

			return View(await courseServices.GetCoursesForShow(GetUserIdFromClaims(), pageId, take, nameFilter));
		}

		#endregion

		#region Add

		//[Permission("Teacher_AddCourse")]
		[Route("AddCourse")]
		[HttpGet]
		public IActionResult AddCourse()
		{
			GetRequirementsOfCourseSelectOptions();

			return View(new AddCourseViewModel());
		}

		[Route("AddCourse")]
		[HttpPost]
		public async Task<IActionResult> AddCourse(AddCourseViewModel? model)
		{
			if (model == null)
			{
				CreateMassageAlert("danger", "مشکلی در ثبت اطلاعات به وحود آمد فایل ورودی شما نباید بزرگ تر از 500 مگابایت باشد .", "نا موفق ");

			}
			else if (ModelState.IsValid)
			{
				var course = new Course
				{
					CourseName = model.CourseName,
					CourseDetail = model.CourseDetail,
					CoursePrice = model.CoursePrice,
					CreateTime = DateTime.Now,
					TeacherId = GetUserIdFromClaims()
				};


				#region Category

				if (model.CategoryId == null
					|| model.CategoryId == -1
					|| !(await contextActionsForCategory.Exists(x => x.CategoryId == model.CategoryId && x.ParentCategoryId == null)))
				{
					ModelState.AddModelError("CategoryId", "انتخاب یک گروه اجباری است !");
					GetRequirementsOfCourseSelectOptions();
					return View();
				}
				else
				{
					if (model.SubCategoryId == null
						|| model.SubCategoryId == -1
						|| !(await contextActionsForCategory.Exists(x => x.CategoryId == model.SubCategoryId)))
					{
						course.CategoryId = model.CategoryId;
					}
					else
					{
						course.CategoryId = (int)model.SubCategoryId;
					}
				}

				#endregion

				#region Level

				course.CourseLevel = (CourseLevel)model.Level;

				#endregion

				#region Status

				course.CourseStatus = (CourseStatus)model.Status;

				#endregion

				#region Image Of Course

				var courseImageName = "no-photo.jpg";
				string courseImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CourseInfo", "Images");

				if (model.CourseImageFile != null)
				{
					if (model.CourseImageFile.IsImage())
					{
						courseImageName = StringGenerate.GuidGenerate() + Path.GetExtension(model.CourseImageFile.FileName);

						await using (var stream = new FileStream(Path.Combine(courseImagePath, "image", courseImageName), FileMode.Create))
						{
							await model.CourseImageFile.CopyToAsync(stream);
						}

						ImageConvertor convertor = new ImageConvertor();
						convertor.Image_resize(Path.Combine(courseImagePath, "image", courseImageName), Path.Combine(courseImagePath, "thumb", courseImageName), 150);
					}
				}

				course.CourseImagePath = courseImageName;


				#endregion

				#region Tags

				if (!model.Tags.IsNullOrEmpty())
				{
					var x = model.Tags.Split("-");
					foreach (var tag in x)
					{
						if (tag.StartsWith("#"))
						{
							var xx = tag.Remove(0, 1) + "-";
							course.Tags += xx;
						}
						else
							course.Tags += tag + "-";
					}
					course.Tags = course.Tags?.Remove(course.Tags.Length - 1);
				}
				else
				{
					ModelState.AddModelError(nameof(model.Tags), "حداقل یک هشتک وارد کنید !");
					return View(model);
				}
				#endregion



				var courseId = await courseServices.AddCourse(course);
				if (courseId == 0)
				{
					CreateMassageAlert("danger", "در ثبت اطلاعات مشکلی به وجود آمده است بعدا دوباره مراجعه کنید و", "ناموفق ");
				}
				else
				{
					#region Demo Video Of Course

					var res = false;

					if (model.CourseDemoVideo != null)
					{
						var courseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CourseInfo",
							"EpisodesFile", course.CourseName + "_" + course.CourseId);
						if (!Directory.Exists(courseDirectory))
						{
							Directory.CreateDirectory(courseDirectory);
							var demoDirectory = Path.Combine(courseDirectory,
								course.CourseName + "_" + course.CourseId + "_Intro");
							Directory.CreateDirectory(demoDirectory);

							await using (var stream = new FileStream(Path.Combine(demoDirectory, course.CourseName + "_Intro" + Path.GetExtension(model.CourseDemoVideo.FileName)), FileMode.Create))
							{
								await model.CourseDemoVideo.CopyToAsync(stream);
							}

							var courseIntroEpisode = new CourseEpisode()
							{
								CourseId = courseId,
								CreateDate = DateTime.Now,
								EpisodeNumber = 0,
								EpisodeTitle = "توضیحات دوره",
								//TODO: EpisodeVideoTime
								EpisodeVideoTime = "02:30:01",
								EpisodeFileUrl = "/" + Path.Combine("CourseInfo", "EpisodesFile",
									course.CourseName + "_" + course.CourseId,
									course.CourseName + "_" + course.CourseId + "_Intro",
									course.CourseName + "_Intro" + Path.GetExtension(model.CourseDemoVideo.FileName))
							};
							courseIntroEpisode.EpisodeFileUrl = courseIntroEpisode.EpisodeFileUrl.Replace("\\", "/");

							if (await courseServices.AddCourseEpisode(courseIntroEpisode))
							{
								res = true;
							}
							else
							{
								Directory.Delete(Path.Combine("CourseInfo", "EpisodesFile", course.CourseName + "_" + course.CourseId, course.CourseName + "_" + course.CourseId + "_Intro"));
								res = false;
							}

						}
					}
					else
					{
						CreateMassageAlert("success", "عملیات باموفقیت انجام شد ." + "حتما برای شروع دوره ویدیوی معرفی را آپلود کنید .", "موفق ");
						return RedirectToAction("Courses", "Teacher");
					}


					#endregion

					if (res) CreateMassageAlert("success", "عملیات باموفقیت انجام شد .", "موفق ");
					else CreateMassageAlert("warning", "عملیات باموفقیت انجام شد ." + "ولی در ثبت ویدیوی شما مشکلی به وجود آمد .", "توجه");
				}

				return RedirectToAction("Courses", "Teacher");
			}

			GetRequirementsOfCourseSelectOptions();

			return View(model);
		}


		#endregion

		#region Edit

		[Route("EditCourse/{courseName?}/{courseId:int}")]
		public async Task<IActionResult> EditCourse(int courseId, string? courseName = "")
		{
			var course = await courseServices.GetCourse(x => x.CourseId == courseId, true);

			if (course == null)
			{
				CreateMassageAlert("danger", "در لینک دریافتی برای دیدن دوره ی مورد نظر مشکلی وجود دارد .", "نا موفق ");
			}

			else if (course.TeacherId != GetUserIdFromClaims())
			{
				CreateMassageAlert("danger", "دوره ی مورد نظر مربوط به شما نیست .", "نا موفق ");
			}

			else
			{
				var model = new EditCourseViewModel
				{
					CourseId = course.CourseId,
					Tags = course.Tags,
					Status = (int)course.CourseStatus,
					Level = (int)course.CourseLevel,
					CoursePrice = course.CoursePrice,
					CourseName = course.CourseName,
					CourseDetail = course.CourseDetail
				};

				#region Check Category Select List

				var categories = courseServices.GetCategories();

				if (course.Category.ParentCategoryId == null)
				{
					model.CategoryId = course.CategoryId;
					model.SubCategoryId = -1;

					TempData["Categories"] = JsonConvert.SerializeObject(new SelectList(categories, "Value", "Text", course.CategoryId));
					var sub = GetSubCategoriesJson(course.CategoryId);
					TempData["SubCategories"] = JsonConvert.SerializeObject(new SelectList(sub.Value as List<SelectListItem>, "Value", "Text"));
				}

				else
				{
					model.SubCategoryId = course.CategoryId;
					model.CategoryId = (int)course.Category.ParentCategoryId;

					TempData["Categories"] = JsonConvert.SerializeObject(new SelectList(categories, "Value", "Text", course.Category.ParentCategoryId));

					var sub = GetSubCategoriesJson((int)course.Category.ParentCategoryId);
					TempData["SubCategories"] = JsonConvert.SerializeObject(new SelectList(sub.Value as List<SelectListItem>, "Value", "Text", course.CategoryId));
				}

				#endregion

				#region Status && Level

				var courseLevel = courseServices.GetLevelOfCourses();
				TempData["CourseLevel"] = JsonConvert.SerializeObject(new SelectList(courseLevel, "Value", "Text", model.Level));

				var courseStatus = courseServices.GetStatusOfCourses();
				TempData["CourseStatus"] = JsonConvert.SerializeObject(new SelectList(courseStatus, "Value", "Text", model.Level));
				var x = new SelectList(courseStatus, "Value", "Text", model.Level);
				
				#endregion

				#region Image && Demo Video

				model.LastImageName = course.CourseImagePath;

				model.LastFileDemoName = course.Episodes != null && course.Episodes.Any()
					? course.Episodes.First(x => x.EpisodeNumber == 0).EpisodeFileUrl
					: null;

				#endregion

				TempData.Keep();
				return View(model);
			}

			return RedirectToAction("Courses", "Teacher");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("EditCourse/{courseName?}/{courseId:int}")]
		public IActionResult EditCourse(EditCourseViewModel model, string? courseName, int courseId)
		{
			if (ModelState.IsValid)
			{
				var x = TempData["CourseStatus"] as string;
			}

			var p = courseServices.GetStatusOfCourses();
			var v = new SelectList(p, "Text", "Value", 1);
			var xx = JsonConvert.DeserializeObject<SelectList>(TempData["CourseStatus"].ToString()) as SelectList;
			System.IO.File.WriteAllText("D:\\m.json", TempData["CourseStatus"].ToString());
			return null;

		}


		#endregion

		#region Methods

		[Route("GetSubCategoriesJson")]
		public JsonResult GetSubCategoriesJson(int CatId)
		{
			return Json(courseServices.GetChildCategories(CatId));
		}



		[Route("GetCourseDescription")]
		public async Task<JsonResult> GetCourseDescription(string CourseId)
		{
			var model = (await courseServices.GetCourse(x => x.CourseId == int.Parse(CourseId))).CourseDetail;
			return Json(model);
		}


		private void GetRequirementsOfCourseSelectOptions()
		{
			var categories = courseServices.GetCategories();
			TempData["Categories"] = new SelectList(categories, "Value", "Text");

			var sub = new List<SelectListItem>()
			{
				new ()
				{
					Text = "ابتدا "+"یک گروه انتخاب کنید",
					Value = "-1",
					Selected = true
				}
			};
			ViewBag.SubCategories = new SelectList(sub, "Value", "Text");

			var courseLevel = courseServices.GetLevelOfCourses();
			ViewBag.CourseLevel = new SelectList(courseLevel, "Value", "Text");

			var courseStatus = courseServices.GetStatusOfCourses();
			ViewBag.CourseStatus = new SelectList(courseStatus, "Value", "Text");
		}


		#endregion


	}
}
