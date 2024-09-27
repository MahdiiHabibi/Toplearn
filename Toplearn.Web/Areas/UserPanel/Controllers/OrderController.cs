using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Toplearn.Core.Security.Attribute.CheckIdentityCodeOfUserPolicy;
using Toplearn.Core.Security.Attribute.CheckIdentityValidationGuid;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Order;
using Toplearn.Web.Security;

namespace Toplearn.Web.Areas.UserPanel.Controllers
{
	[Area("UserPanel")]
	[Authorize]
	[CheckIVG]
	[CheckUIC]
	[Route("UserPanel/Order")]
	public class OrderController(IOrderServices orderServices, ICourseServices courseServices, IWalletManager walletManager) : TopLearnController
	{

		public IActionResult Index()
		{
			object? model = orderServices.GetOrders(x => x.UserId == GetUserIdFromClaims(), true);

			if (model == null)
			{
				model = null;
			}
			else if (model is not List<Order> && model is Order order)
			{
				model = new List<Order>()
				{
					order
				};
			}
			else if (model is not List<Order>)
			{
				return NotFound();
			}

			return View("Index", model as List<Order>);
		}


		[Route("{orderId:int}")]
		public IActionResult Index(int orderId)
		{
			var model = orderServices.GetOrders(x => x.OrderId == orderId, true) as Order;

			if (model == null)
			{
				return NotFound();
			}

			ViewBag.discounts = orderServices.GetOrderDiscounts(model.OrderId);

			return View("ShowOrder", model);
		}


		[HttpPost]
		public IActionResult UseDiscount(int orderId, string discountCode)
		{
			discountCode = discountCode.Trim();

			if (!orderServices.IsDiscountExist(discountCode))
			{
				CreateMassageAlert("danger", "این کد تخفیف یافت نشد .", "نا موفق  ");
				return Redirect($"/UserPanel/Order/{orderId}");
			}
			var discounts = orderServices.GetOrderDiscounts(orderId);

			var discount = orderServices.GetDiscount(discountCode);

			if (discounts.Any(x => x.DiscountCode.Equals(discount?.DiscountCode, StringComparison.CurrentCultureIgnoreCase)))
			{
				CreateMassageAlert("danger", "شما از این کد تخفیف قبلا استفاده کرده اید .", "نا موفق  ");
				return Redirect($"/UserPanel/Order/{orderId}");
			}

			if ((discount.EndDate != null &&
				 discount.StartDate != null &&
				 !(discount.StartDate <= DateTime.Now || DateTime.Now <= discount.EndDate))
				|| discount.UsableCount <= 0)
			{
				CreateMassageAlert("danger", "مهلت استفاده ی این کد تمام شده است ! ", "نا موفق  ");
				return Redirect($"/UserPanel/Order/{orderId}");
			}

			var res = orderServices.AddDiscountCodeToOrder(orderId, discount);

			
			if (!res)
				CreateMassageAlert("danger", "مشکلی به وجود آمده است !", "خطا  ");

			else
				CreateMassageAlert("success", "کد تخفیف با موفقیت به فاکتور شما ثبت شد .", "موفق  ");

			return Redirect($"/UserPanel/Order/{orderId}");
		}


		[Route("AddCourse/{courseId:int}")]
		public IActionResult AddCourseToCart(int courseId)
		{
			var course = courseServices.GetCourseForShow(courseId);

			if (courseId == 0 || course == null)
			{
				return NotFound();
			}

			var userId = GetUserIdFromClaims();
			if (orderServices.IsUserBuyCourse(userId, courseId))
			{
				return RedirectToAction("index", "Course", new { courseId = courseId });
			}

			if (orderServices.IsOrderHaveCourse(GetUserIdFromClaims(), courseId))
			{
				CreateMassageAlert("warning", "این دوره از قبل در فاکتور شما موجود است", "توجه  ");
				return Redirect($"/Course/TopLearn/{courseId}");
			}

			var res = orderServices.AddCourseToOrder(course, userId);


			if (res != 0)
				return Redirect($"/UserPanel/Order/{res}");

			CreateMassageAlert("danger", "مشکلی به وجود آمده است !", "خطا  ");
			return Redirect($"/Course/TopLearn/{courseId}");

		}


		[Route("Confirm/{orderId:int}")]
		public async Task<IActionResult> ConfirmOrder(int orderId)
		{
			if (!orderServices.IsOrderExist(o => o.OrderId == orderId && o.UserId == GetUserIdFromClaims() && !o.IsFinally))
			{
				return NotFound();
			}

			orderServices.OrderPriceSumInquiry(orderId);

			var res = await orderServices.ConfirmOrder(orderId, GetUserIdFromClaims());

				switch (res)
			{
				case null:
					CreateMassageAlert("danger", "کیف پول خود را شارژ کنید !", "نا موفق ");
					return RedirectToAction("index", "Wallet");
				case true:
					CreateMassageAlert("success", "عملیات با موفقیت انجام شد .", "موفق ");
					break;
				default:
					CreateMassageAlert("danger", "مشکلی به وجود آمده است !", "خطا  ");
					break;
			}

			return Redirect($"/UserPanel/Order/{orderId}");
		}
	}
}
