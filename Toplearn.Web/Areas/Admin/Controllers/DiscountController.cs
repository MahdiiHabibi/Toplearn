using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Order;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	[CheckIVG]
	[CheckUIC]
	[Route("/Admin/Discount")]
	public class DiscountController(IAdminServices adminServices) : TopLearnController
	{
		public IActionResult Index(int pageId = 1, int take = 5, string filter = "")
		{
			var model = adminServices.GetDiscountsForShow(pageId, take, filter);

			ViewData["take"] = take;
			ViewData["Filter"] = filter;

			return View(model);
		}

		[Route("Add")]
		public IActionResult AddDiscount()
		{
			return View(new AddEditDiscountViewModel());
		}


		[Route("Add")]
		[HttpPost]
		public IActionResult AddDiscount(AddEditDiscountViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (adminServices.IsDiscountCodeExist(model.DiscountCode))
				{
					ModelState.AddModelError("DiscountCode", "این کد از قبل موجود است !");
					return View(model);
				}

				var discount = new OrderDiscount()
				{
					DiscountCode = model.DiscountCode,
					DiscountPercent = model.DiscountPercent
				};

				#region Check DateTimes

				if (!model.HaveStartDate)
				{
					discount.StartDate = model.StartDate;
				}

				if (!model.HaveEndDate)
				{
					discount.EndDate = model.EndDate;
				}

				#endregion

				#region Check Usable Count

				discount.UsableCount = !model.HaveUsableCount ? model.UsableCount : Int32.MaxValue;

				#endregion

				var res = adminServices.AddDiscount(discount);
				if (res)
					CreateMassageAlert("success", "عملیات با موفیت انجام شد .", "موفق");
				else
					CreateMassageAlert("danger", "مشکلی به وجود آمده است !", "خطا  ");
				return RedirectToAction("Index", "Discount");
			}
			return View(model);
		}


		[Route("Edit/{discountCode}")]
		public IActionResult EditDiscount(string discountCode)
		{
			throw new NotImplementedException();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<JsonResult> CheckDiscountCode(string discountCode)
		{
			return Task.FromResult<JsonResult>(adminServices.IsDiscountCodeExist(discountCode) ? Json("این کد از قبل موجود میباشد !") : Json(true));
		}


	}
}
