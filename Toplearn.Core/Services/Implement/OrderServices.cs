using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Order;
using Toplearn.DataLayer.Entities.Wallet;


namespace Toplearn.Core.Services.Implement
{
	public class OrderServices(TopLearnContext context, IWalletManager walletManager) : IOrderServices, IDisposable, IAsyncDisposable
	{


		public bool IsUserBuyCourse(int userId, int courseId)
		{
			var res = context.UserCourses.Any(x => x.UserId == userId && x.CourseId == courseId);

			return res;
		}

		public bool IsOrderHaveCourse(int userId, int courseId)
		{
			var order = context.Orders
				.Include(order => order.OrderDetails)
				.FirstOrDefault(x => !x.IsFinally && x.UserId == userId);
			if (order == null)
			{
				return false;
			}

			return order.OrderDetails
				.Any(x => x.CourseId == courseId);
		}

		public int AddCourseToOrder(Course course, int userId)
		{

			try
			{
				Order? order = context.Orders
					.Include(x => x.OrderDetails)
					.FirstOrDefault(x => x.UserId == userId && !x.IsFinally);

				if (order == null)
				{

					order = new Order()
					{
						UserId = userId,
						CreateTime = DateTime.Now,
						IsFinally = false,
						OrderSum = course.CoursePrice,
						OrderDetails =
						[
							new OrderDetail
							{
								CourseId = course.CourseId,
								Price = course.CoursePrice
							}
						]
					};

					context.Orders.Add(order);
					context.SaveChanges();
				}

				else
				{
					var orderDetail = new OrderDetail
					{
						CourseId = course.CourseId,
						Price = course.CoursePrice,
						OrderId = order.OrderId
					};

					order.OrderDetails.Add(orderDetail);

					// Add To context
					context.OrderDetails.Add(orderDetail);
					context.SaveChanges();

					OrderPriceSumInquiry(order.OrderId);
				}

				return order.OrderId;
			}

			catch (Exception exception)
			{
				return 0;
			}

		}

		public void OrderPriceSumInquiry(Order order,OrderDiscount? orderDiscount = null)
		{
			if (order == null)
			{
				return;
			}

			var orderDetails = context.OrderDetails.Where(x => x.OrderId == order.OrderId).ToList();
			var sum = orderDetails.Sum(x => x.Price);

			var discounts = GetOrderDiscounts(order.OrderId);
			if (orderDiscount != null)
			{
				discounts.Add(orderDiscount);
			}
			foreach (var discount in discounts)
			{
				sum -= (sum * discount.DiscountPercent) / 100;
			}

			order.OrderSum = sum;

			context.Orders.Update(order);

			context.SaveChanges();
		}

		public void OrderPriceSumInquiry(int orderId, OrderDiscount? orderDiscount = null)
		{
			OrderPriceSumInquiry(context.Orders.FirstOrDefault(x => x.OrderId == orderId),orderDiscount);
		}

		public List<OrderDiscount> GetOrderDiscounts(int orderId)
		{
			return context.OrdersToDiscounts
				.Include(x => x.Discount)
				.Where(x => x.OrderId == orderId).Select(x => x.Discount)
				.ToList();
		}

		public object? GetOrders(Expression<Func<Order, bool>>? func, bool enableInclude)
		{
			IQueryable<Order> orders = context.Orders;

			if (func != null)
			{
				orders = orders.Where(func);
			}

			if (enableInclude)
			{
				orders = orders.Include(x => x.OrderDetails).ThenInclude(x => x.Course);
			}

			var model = orders.ToList();

			if (model == null || model.Count == 0)
			{
				return null;
			}
			if (model.Count == 1)
			{
				return model[0];
			}

			return model;
		}

		public bool IsDiscountExist(string code)
		{
			return context.OrderDiscounts.Any(x => x.DiscountCode == code);
		}

		public OrderDiscount? GetDiscount(string code)
		{
			return context.OrderDiscounts.SingleOrDefault(x => x.DiscountCode == code);
		}

		public bool AddDiscountCodeToOrder(int orderId, OrderDiscount discount)
		{
			try
			{
				context.OrdersToDiscounts.Add(new OrderToDiscount
				{
					OrderId = orderId,
					DiscountId = discount.DiscountCode
				});

				discount.UsableCount -= 1;
				context.OrderDiscounts.Update(discount);
				OrderPriceSumInquiry(orderId,discount);
				
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool IsOrderExist(Func<Order, bool> func)
		{
			return context.Orders.Any(func);
		}

		public async Task<bool?> ConfirmOrder(int orderId, int userId)
		{

			Order order = context.Orders.Single(o => o.OrderId == orderId);

			if (await walletManager.GetBalanceOfUser(userId) < order.OrderSum)
			{
				return null;
			}

			order.IsFinally = true;
			context.Orders.Update(order);

			context.UserCourses.AddRange(context.OrderDetails.Where(x => x.OrderId == orderId).Select(x => new UserCourse()
			{
				CourseId = x.CourseId,
				UserId = userId
			}));

			context.Wallets.Add(new Wallet()
			{
				UserId = userId,
				Amount = order.OrderSum,
				Authority = 0,
				IsPay = true,
				CreateDate = DateTime.Now,
				RefId = 0,
				Description = "پرداخت  -  " + $@"<a href='/UserPanel/Order/{orderId}' target='_blank'> فاکتور {orderId} </a>",
				TypeId = 1
			});

			await context.SaveChangesAsync();

			var user = context.Users.Single(x => x.UserId == userId);

			user.WalletBalance = await walletManager.UserWalletBalanceInquiry(userId);

			context.Users.Update(user);
			await context.SaveChangesAsync();
			return true;
		}


		public void Dispose()
		{
			context.Dispose();
		}

		public async ValueTask DisposeAsync()
		{
			await context.DisposeAsync();
		}
	}
}
