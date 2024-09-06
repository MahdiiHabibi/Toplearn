using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{

	[Area("Admin")]
	[Authorize]
	public class CategoryController(ICategoryServices categoryServices, ICourseServices courseServices) : TopLearnController
	{

		// GET: CategoryController
		public async Task<IActionResult> Index(int take = 5, int pageId = 1, string nameFilter = "")
		{
			if (take <= 0)
			{
				return RedirectToAction("Index");
			}

			ViewData["nameFilter"] = nameFilter;
			ViewData["take"] = take;
			ViewData["pageId"] = pageId;

			var model = await categoryServices.GetCategories(true, pageId, take, nameFilter);
			if (model.Categories == null || model.Categories.Count == 0)
			{
				return NotFound();
			}
			if (model.PageCount < pageId)
			{
				return RedirectToAction("Index", "Category", new { take = take, nameFilter = nameFilter, pageId = 1 });
			}

			return View(model);
		}


		#region Change Category Status

		// GET: CategoryController/ChangeCategoryStatus/5
		public async Task<IActionResult> ChangeCategoryStatus(int id)
		{
			var model = await categoryServices.GetCategory(x => x.CategoryId == id, true);

			if (model == null)
			{
				CreateMassageAlert("danger", " در لینک درخواستی شما مشکلی به وجود آمده است .", "نا موفق ");
			}
			else
			{
				model.IsActive = !model.IsActive;

				if (await categoryServices.UpdateCategory(model))
				{
					CreateMassageAlert("success", "عملیات با موفقیت انجام شد .", "موفق ");
				}
				else
				{
					CreateMassageAlert("danger", " در ثبت درخواست شما مشکلی به وجود آمده است .", "نا موفق ");
				}
			}

			return RedirectToAction("Index", "Category");
		}


		#endregion


		#region Create

		// GET: CategoryController/Create
		public IActionResult Create()
		{
			var categories = courseServices.GetCategories();
			categories[0].Text = "< بدون سرگروه >";
			ViewData["Categories"] = new SelectList(categories, "Value", "Text");

			return View(new AddCategoryViewModel());
		}

		// POST: CategoryController/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AddCategoryViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			// Check model name to Be Not Repeat
			if ((await CheckCategoryName(model.CategoryName)).Value?.ToString()!.ToLower() == "true")
			{
				if (await categoryServices.ContextActionsForCategory.AddToContext(new Category()
				{
					CategoryName = model.CategoryName,
					IsActive = true,
					ParentCategoryId = model.ParentCategoryId != -1 ? model.ParentCategoryId : null
				}))
				{
					CreateMassageAlert("success", "اطلاعات با موفقیت ثبت شد .", "موفق ");
					return RedirectToAction("Index");
				}

				CreateMassageAlert("danger", "ثبت اطلاعات با شکست مواجه شد .", " ناموفق");
				return RedirectToAction("Index");

			}
			else
			{
				ModelState.AddModelError("CategoryName", "این نام قبلا ثبت شده است .");
			}
			return View(model);
		}


		#endregion


		#region Edit

		// GET: CategoryController/Edit/5
		public async Task<IActionResult> Edit(int id)
		{
			var model = await categoryServices.GetCategory(x => x.CategoryId == id, true);

			if (model == null)
			{
				return NotFound();
			}

			ViewBag.Edit = model.CategoryName;

			var categories = courseServices.GetCategories();
			categories[0].Text = "< بدون سرگروه >";
			ViewData["Categories"] = new SelectList(categories, "Value", "Text", model.ParentCategoryId);

			return View(new EditCategoryViewModel()
			{
				CategoryName = model.CategoryName,
				ParentCategoryId = model.ParentCategoryId,
				CategoryId = model.CategoryId
			});
		}

		// POST: CategoryController/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(EditCategoryViewModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var category = await categoryServices.GetCategory(x => x.CategoryId == model.CategoryId, true);

			if (category == null)
			{
				return NotFound();
			}

			if (category.ParentCategoryId == null && model.ParentCategoryId > 0)
			{
				CreateMassageAlert("danger", "گروهی که انتخاب کرده اید خود سر گروه است و نمیتواند زیر گروه شود .", "خطر  ");
				return (View(model));
			}

			if (model.OldCategoryName == model.CategoryName)
			{
				CreateMassageAlert("danger", "نام جدید انتخاب کنید و در صورت خواستار لغو این عملیات دکمه ی لغو را بزنید .", "خطا  ");
			}

			category.CategoryName = model.CategoryName;
			category.ParentCategoryId = model.ParentCategoryId > 0 ? model.ParentCategoryId : null;
			var res = await categoryServices.UpdateCategory(category);

			if (res)
			{
				CreateMassageAlert("success", "این عملیات با موفقیت انجام شد .", "موفق ");
			}
			else
			{
				CreateMassageAlert("danger", "انجام عملیات با شکست مواجه شد .", "نا موفق ");
			}

			return RedirectToAction("Index", "Category");
		}

		#endregion


		#region Methods

		[Route("/Admin/Category/CheckCategoryName", Name = "CheckCategoryName")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<JsonResult> CheckCategoryName(string categoryName)
		{
			var res = await categoryServices.GetCategory(x =>
				string.Equals(x.CategoryName.Trim(), categoryName.Trim(), StringComparison.CurrentCultureIgnoreCase), true);

			return res == null ? Json(true) : Json("این نام مورد نظر تکراری میباشد .");
		}


		public async Task<JsonResult> GetCategoryName(int categoryId)
		{
			return Json((await categoryServices.GetCategory(x => x.CategoryId == categoryId, true))?.CategoryName);
		}

		#endregion

	}
}
