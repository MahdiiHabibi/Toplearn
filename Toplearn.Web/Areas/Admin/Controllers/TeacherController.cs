
using System.Runtime.InteropServices.ComTypes;
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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
			var model = await courseServices.GetCoursesForShow(GetUserIdFromClaims(), pageId, take, nameFilter);
			return View(model);
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
					TeacherId = GetUserIdFromClaims(),
					CourseVideosTime = model.CourseDemoVideo == null ? TimeSpan.Zero : model.EpisodeVideoTime
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
						convertor.Image_resize(Path.Combine(courseImagePath, "image", courseImageName), Path.Combine(courseImagePath, "thumb", courseImageName), 340);
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
							var demoDirectory = Path.Combine(courseDirectory, "Intro");
							Directory.CreateDirectory(demoDirectory);

							await using (var stream = new FileStream(Path.Combine(demoDirectory, course.CourseName + "_Intro" + Path.GetExtension(model.CourseDemoVideo.FileName)), FileMode.Create))
							{
								await model.CourseDemoVideo.CopyToAsync(stream);
							}

							var courseIntroEpisode = new CourseEpisode()
							{
								CourseId = course.CourseId,
								CreateDate = DateTime.Now,
								EpisodeNumber = 0,
								EpisodeTitle = "توضیحات دوره",
								//TODO: EpisodeVideoTime
								EpisodeVideoTime = model.EpisodeVideoTime,
								EpisodeFileUrl = "\\" + Path.Combine("CourseInfo", "EpisodesFile",
									course.CourseName + "_" + course.CourseId,
									"Intro",
									course.CourseName + "_Intro" + Path.GetExtension(model.CourseDemoVideo.FileName)),
								IsFree = true
							};
							courseIntroEpisode.EpisodeFileUrl = courseIntroEpisode.EpisodeFileUrl;

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

				if (course.Episodes.Any(x => x.EpisodeNumber == 0))
				{
					model.EpisodeVideoTime = course.Episodes.Single(x => x.EpisodeNumber == 0).EpisodeVideoTime;
					TempData["LastEpisodeVideoTime"] = model.EpisodeVideoTime.ToString();
					TempData.Keep("LastEpisodeVideoTime");
				}


				#region Check Category Select List

				var categories = courseServices.GetCategories();

				if (course.Category.ParentCategoryId == null)
				{
					model.CategoryId = course.CategoryId;
					model.SubCategoryId = -1;

					model.Category = new SelectList(categories, "Value", "Text", course.CategoryId);
					var sub = GetSubCategoriesJson(course.CategoryId).Value as List<SelectListItem>;
					model.SubCategory = new SelectList(sub, "Value", "Text");
				}

				else
				{
					model.SubCategoryId = course.CategoryId;
					model.CategoryId = (int)course.Category.ParentCategoryId;

					model.Category = new SelectList(categories, "Value", "Text", course.Category.ParentCategoryId);

					var sub = GetSubCategoriesJson((int)course.Category.ParentCategoryId).Value as List<SelectListItem>;
					model.SubCategory = new SelectList(sub, "Value", "Text", course.CategoryId);
				}

				#endregion

				#region Status && Level

				var courseLevel = courseServices.GetLevelOfCourses();
				model.LevelOfCourse = new SelectList(courseLevel, "Value", "Text", model.Level);

				var courseStatus = courseServices.GetStatusOfCourses();
				model.StatusOfCourse = new SelectList(courseStatus, "Value", "Text", model.Status);


				#endregion

				#region Image && Demo Video

				model.LastImageName = course.CourseImagePath;

				model.LastFileDemoName = course.Episodes != null && course.Episodes.Any(x => x.EpisodeNumber == 0)
					? course.Episodes.First(x => x.EpisodeNumber == 0).EpisodeFileUrl
					: null;

				#endregion


				return View(model);
			}

			return RedirectToAction("Courses", "Teacher");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("EditCourse/{courseName}/{courseId:int}")]
		public async Task<IActionResult> EditCourse(EditCourseViewModel model)
		{
			if (ModelState.IsValid)
			{

				var course = await courseServices.GetCourse(x => x.CourseId == model.CourseId, true);

				if (course == null)
					return NotFound();


				course.CourseDetail = model.CourseDetail;
				course.CourseName = model.CourseName;
				course.CoursePrice = model.CoursePrice;

				#region Category

				if (model.CategoryId == null
					|| model.CategoryId == -1
					|| !(await contextActionsForCategory.Exists(x => x.CategoryId == model.CategoryId && x.ParentCategoryId == null)))
				{
					ModelState.AddModelError("CategoryId", "انتخاب یک گروه اجباری است !");
					GetRequirementsOfCourseSelectOptions();
					return View(model);
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


				if (model.CourseImageFile != null)
				{
					var courseImageName = course.CourseImagePath;
					string courseImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CourseInfo", "Images");

					if (model.CourseImageFile.IsImage())
					{

						System.IO.File.Delete(Path.Combine(courseImagePath, "image", courseImageName));
						System.IO.File.Delete(Path.Combine(courseImagePath, "thumb", courseImageName));

						courseImageName = StringGenerate.GuidGenerate() + Path.GetExtension(model.CourseImageFile.FileName);

						await using (var stream = new FileStream(Path.Combine(courseImagePath, "image", courseImageName), FileMode.Create))
						{
							await model.CourseImageFile.CopyToAsync(stream);
						}

						ImageConvertor convertor = new ImageConvertor();
						convertor.Image_resize(Path.Combine(courseImagePath, "image", courseImageName), Path.Combine(courseImagePath, "thumb", courseImageName), 340);
					}

					course.CourseImagePath = courseImageName;

				}

				#endregion

				#region Tags

				if (!model.Tags.IsNullOrEmpty())
				{
					course.Tags = string.Empty;
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



				var res = await courseServices.UpdateCourse(course);


				if (!res)
				{
					CreateMassageAlert("danger", "در ثبت اطلاعات مشکلی به وجود آمده است بعدا دوباره مراجعه کنید ", "ناموفق ");
				}
				else
				{
					#region Demo Video Of Course

					if (model.CourseDemoVideo == null)
					{
						var lastEpisodeVideoTime = TempData["LastEpisodeVideoTime"] as string;

						if (course.Episodes != null && lastEpisodeVideoTime != model.EpisodeVideoTime.ToString())
						{
							var x = course.Episodes.SingleOrDefault(x => x.EpisodeNumber == 0);

							if (x != null)
							{
								x.EpisodeVideoTime = model.EpisodeVideoTime;
								course.CourseVideosTime = await courseServices.CourseVideosTimeInquiry(course.CourseId);
								await courseServices.UpdateCourseEpisode(x);
							}
						}

						CreateMassageAlert("success", "عملیات با موفقیت انجام شد .	", "موفق ");
					}
					else
					{
						CourseEpisode? courseEpisodes = null;

						if (course.Episodes is { Count: > 0 })
						{
							courseEpisodes = course.Episodes.SingleOrDefault(x => x.EpisodeNumber == 0);
						}

						if (courseEpisodes == null)
						{
							courseEpisodes = new CourseEpisode()
							{
								CourseId = course.CourseId,
								CreateDate = DateTime.Now,
								EpisodeNumber = 0,
								EpisodeTitle = "توضیحات دوره",
								EpisodeVideoTime = model.EpisodeVideoTime,
								EpisodeFileUrl = Path.Combine("CourseInfo", "EpisodesFile",
									course.CourseName + "_" + course.CourseId, "Intro",
									course.CourseName + "_Intro" + Path.GetExtension(model.CourseDemoVideo.FileName)),
								IsFree = true
							};

							var courseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CourseInfo",
								"EpisodesFile", course.CourseName + "_" + course.CourseId);

							if (!Directory.Exists(courseDirectory))
							{
								Directory.CreateDirectory(courseDirectory);
							}

							var demoDirectory = Path.Combine(courseDirectory, "Intro");

							if (!Directory.Exists(demoDirectory))
							{
								Directory.CreateDirectory(demoDirectory);
							}

							var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseEpisodes.EpisodeFileUrl);

							await using (var stream = new FileStream(filePath, FileMode.Create))
							{
								await model.CourseDemoVideo.CopyToAsync(stream);
							}

							courseEpisodes.EpisodeFileUrl = "\\" + courseEpisodes.EpisodeFileUrl;

							res = await courseServices.AddCourseEpisode(courseEpisodes);

							if (res)
							{
								CreateMassageAlert("success", "عملیات با موفقیت انجام شد .	", "موفق ");
							}
							else
							{
								System.IO.File.Delete(filePath);
								CreateMassageAlert("danger", "در ثبت اطلاعات ویدیو مشکلی به وجود آمده است بعدا دوباره مراجعه کنید .", "ناموفق ");
							}


						}
						else
						{

							var x = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseEpisodes.EpisodeFileUrl.Remove(0, 1));
							System.IO.File.Delete(x);

							courseEpisodes.EpisodeFileUrl = "\\" + Path.Combine("CourseInfo", "EpisodesFile",
								course.CourseName + "_" + course.CourseId,
								"Intro",
								course.CourseName + "_Intro" + Path.GetExtension(model.CourseDemoVideo.FileName));

							await using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseEpisodes.EpisodeFileUrl.Remove(0, 1)), FileMode.Create))
							{
								await model.CourseDemoVideo.CopyToAsync(stream);
							}

							courseEpisodes.EpisodeVideoTime = model.EpisodeVideoTime;

							res = await courseServices.UpdateCourseEpisode(courseEpisodes);

							if (res)
							{
								CreateMassageAlert("success", "عملیات با موفقیت انجام شد .	", "موفق ");
							}
							else
							{
								System.IO.File.Delete(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", courseEpisodes.EpisodeFileUrl.Remove(0, 1)));
								CreateMassageAlert("danger", "در ثبت اطلاعات ویدیو مشکلی به وجود آمده است بعدا دوباره مراجعه کنید .", "ناموفق ");
							}
						}

						course.CourseVideosTime = await courseServices.CourseVideosTimeInquiry(course.CourseId);
					}

					#endregion

				}
				
				await courseServices.UpdateCourse(course);
				
				return RedirectToAction("Courses", "Teacher");
			}

			var categories = courseServices.GetCategories();
			model.Category = new SelectList(categories, "Value", "Text");

			var courseLevel = courseServices.GetLevelOfCourses();
			model.LevelOfCourse = new SelectList(courseLevel, "Value", "Text", model.Level);

			var courseStatus = courseServices.GetStatusOfCourses();
			model.StatusOfCourse = new SelectList(courseStatus, "Value", "Text", model.Status);

			return View(model);
		}


		#endregion

		#region Episodes

		[Route("Courses/Episodes")]
		public async Task<IActionResult> CourseEpisodes(int courseId, int take = 5, int pageId = 1, string nameFilter = "")
		{
			if (take <= 0)
			{
				return RedirectToAction("CourseEpisodes");
			}

			ViewData["nameFilter"] = nameFilter;
			ViewData["take"] = take;
			ViewData["pageId"] = pageId;
			ViewData["courseId"] = courseId;
			var courses = new List<(int, string)>()
			{
				new(0, "همه")
			};
			courses.AddRange((await courseServices.GetCourses(GetUserIdFromClaims())).Select(x => (x.CourseId, x.CourseName)).ToList());
			ViewBag.courses = courses;

			var model = await courseServices.GetCourseEpisodes(GetUserIdFromClaims(), courseId, take, pageId, nameFilter);

			if (model.CourseEpisodes == null)
			{
				return View(model);
			}

			if (model.PageCount < pageId)
			{
				return RedirectToAction("CourseEpisodes", "Teacher", new { take = take, nameFilter = nameFilter, pageId = 1, courseId = courseId });
			}

			return View(model);
		}

		[Route("Courses/Episode/{courseId:int}/{episodeId:int}")]
		public IActionResult CourseEpisode(int courseId, int episodeId)
		{
			return View();
		}

		[HttpGet]
		[Route("Courses/Episode/Add/{courseId:int}")]
		public async Task<IActionResult> AddEpisodeForCourse(int courseId)
		{
			var res = await courseServices.GetCourse(x => x.TeacherId == GetUserIdFromClaims() && x.CourseId == courseId);

			if (res == null)
			{
				return BadRequest();
			}

			ViewBag.CourseName = res.CourseName;
			return View(new AddCourseEpisode()
			{
				CourseId = courseId
			});
		}

		[HttpPost]
		[Route("Courses/Episode/Add/{courseId:int}")]
		public async Task<IActionResult> AddEpisodeForCourse(AddCourseEpisode model, int courseId)
		{
			if (ModelState.IsValid == false || model.EpisodeFile == null)
			{
				return View(model);
			}

			var course = await courseServices.GetCourse(x => x.CourseId == courseId && x.TeacherId == GetUserIdFromClaims(), true);

			if (course == null || model.CourseId != courseId)
			{
				return BadRequest();
			}

			var courseEpisode = new CourseEpisode
			{
				EpisodeNumber = course.Episodes == null || course.Episodes.Count == 0 ? 1 : course.Episodes.Count,
				CourseId = courseId,
				CreateDate = DateTime.Now,
				IsFree = model.IsFree,
				EpisodeTitle = model.EpisodeTitle,
				EpisodeVideoTime = model.EpisodeVideoTime
			};


			#region Episode File Services

			var coursePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CourseInfo",
				"EpisodesFile", course.CourseName + "_" + course.CourseId);

			if (Directory.Exists(coursePath) == false)
			{
				Directory.CreateDirectory(coursePath);
			}

			var episodePath = Path.Combine(coursePath, courseEpisode.EpisodeNumber.ToString("00"));

			Directory.CreateDirectory(episodePath);

			var filePath = Path.Combine(episodePath,
				course.CourseName + courseEpisode.EpisodeNumber.ToString("00") + Path.GetExtension(model.EpisodeFile.FileName));

			//Delete Ex File 
			if (System.IO.File.Exists(filePath))
			{
				System.IO.File.Delete(filePath);
			}

			// Add New File
			await using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await model.EpisodeFile.CopyToAsync(stream);
			}


			courseEpisode.EpisodeFileUrl = Path.Combine("CourseInfo",
				"EpisodesFile", course.CourseName + "_" + courseId,
				courseEpisode.EpisodeNumber.ToString("00"), course.CourseName + courseEpisode.EpisodeNumber.ToString("00") + Path.GetExtension(model.EpisodeFile.FileName));


			bool res = false;
			course.LastUpdateTime = DateTime.Now;
			course.CourseVideosTime += courseEpisode.EpisodeVideoTime;

			if (await courseServices.AddCourseEpisode(courseEpisode))
			{

				if (await courseServices.UpdateCourse(course))
				{
					res = true;
				}
			}
			else
			{
				Directory.Delete(filePath);
				res = false;
			}

			#endregion


			if (res)
			{
				CreateMassageAlert("success", "با موفقیت انجام شد .", "موفق ");
				return RedirectToAction("CourseEpisodes", "Teacher", new { courseId = courseId });
			}



			CreateMassageAlert("danger", "در انجام عملیات مشکلی به وجود آمد", "خطر ");
			return View(model);
		}

		[Route("Courses/Episode/ChangeFreeStatus/{episodeId:int}/{courseId:int}")]
		public async Task<IActionResult> ChangeEpisodeFreeStatus(int episodeId, int courseId)
		{
			var model = courseServices.GetCourseEpisode(x => x.CourseId == courseId && x.EpisodeId == episodeId && x.Course.TeacherId == GetUserIdFromClaims(), true) as CourseEpisode;

			if (model == null)
			{
				return BadRequest();
			}

			model.IsFree = !model.IsFree;

			if (!await courseServices.UpdateCourseEpisode(model))
			{
				CreateMassageAlert("danger", "مشکلی به وجود آمده است بعدا امتحان کنید", "نا موفق");
			}

			return RedirectToAction("CourseEpisodes", "Teacher", new { courseId = courseId });
		}


		[HttpGet]
		[Route("Courses/Episode/Edit/{episodeId:int}/{courseId:int}")]
		public IActionResult EditEpisodeForCourse(int episodeId, int courseId)
		{
			if (courseServices.GetCourseEpisode(x =>
					x.CourseId == courseId && x.EpisodeId == episodeId && x.Course.TeacherId == GetUserIdFromClaims(), true)
				is not CourseEpisode courseEpisode)
			{
				return BadRequest();
			}

			var model = new EditEpisodeViewModel()
			{
				CourseId = courseId,
				EpisodeId = episodeId,
				IsFree = courseEpisode.IsFree,
				EpisodeTitle = courseEpisode.EpisodeTitle,
				EpisodeVideoTime = (TimeSpan)courseEpisode.EpisodeVideoTime!
			};
			ViewBag.CourseName = courseEpisode.Course.CourseName;

			return View(model);
		}

		[HttpPost]
		[Route("Courses/Episode/Edit/{episodeId:int}/{courseId:int}")]
		public async Task<IActionResult> EditEpisodeForCourse(EditEpisodeViewModel model, int courseId, int episodeId)
		{
			if (ModelState.IsValid == false)
			{
				return View(model);
			}

			var courseEpisode = courseServices.GetCourseEpisode(x => x.EpisodeId == episodeId && x.CourseId == courseId && x.Course.TeacherId == GetUserIdFromClaims(), true) as CourseEpisode;

			if (courseEpisode == null || model.CourseId != courseId)
			{
				return BadRequest();
			}

			courseEpisode.EpisodeTitle = model.EpisodeTitle;
			courseEpisode.EpisodeVideoTime = model.EpisodeVideoTime;
			courseEpisode.IsFree = model.IsFree;


			#region Episode File Services


			string filePath = string.Empty;

			if (model.EpisodeFile != null)
			{
				var coursePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CourseInfo",
					"EpisodesFile", courseEpisode.Course.CourseName + "_" + courseEpisode.Course.CourseId);

				var episodePath = Path.Combine(coursePath, courseEpisode.EpisodeNumber.ToString("00"));


				filePath = Path.Combine(episodePath,
				   courseEpisode.Course.CourseName + courseEpisode.EpisodeNumber.ToString("00") + Path.GetExtension(model.EpisodeFile.FileName));

				if (Directory.Exists(coursePath) == false)
				{
					Directory.CreateDirectory(coursePath);
				}

				if (!Directory.Exists(episodePath))
				{
					Directory.CreateDirectory(episodePath);
				}

				await using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await model.EpisodeFile.CopyToAsync(stream);
				}


				courseEpisode.EpisodeFileUrl = Path.Combine("CourseInfo",
					"EpisodesFile", courseEpisode.Course.CourseName + "_" + courseId,
					courseEpisode.EpisodeNumber.ToString("00"), courseEpisode.Course.CourseName + courseEpisode.EpisodeNumber.ToString("00") + Path.GetExtension(model.EpisodeFile.FileName));

			}

			bool res = false;
			courseEpisode.Course.LastUpdateTime = DateTime.Now;

			if (await courseServices.UpdateCourseEpisode(courseEpisode))
			{
				courseEpisode.Course.CourseVideosTime = await courseServices.CourseVideosTimeInquiry(courseId);
				if (await courseServices.UpdateCourse(courseEpisode.Course))
				{
					res = true;
				}
			}

			else if (!res)
			{
				if (model.EpisodeFile != null)
				{
					Directory.Delete(filePath);
				}
				res = false;
			}

			#endregion


			if (res)
			{
				CreateMassageAlert("success", "با موفقیت انجام شد .", "موفق ");
				return RedirectToAction("CourseEpisodes", "Teacher", new { courseId = courseId });
			}



			CreateMassageAlert("danger", "در انجام عملیات مشکلی به وجود آمد", "خطر ");
			return View(model);
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
