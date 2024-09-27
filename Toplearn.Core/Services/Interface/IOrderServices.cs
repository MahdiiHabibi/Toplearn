using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Order;

namespace Toplearn.Core.Services.Interface
{
	public interface IOrderServices
	{
		public bool IsUserBuyCourse(int userId,int courseId);

		public bool IsOrderHaveCourse(int userId,int courseId);

		public int AddCourseToOrder(Course course,int userId);

		public void OrderPriceSumInquiry(Order order, OrderDiscount? orderDiscount = null);

		public void OrderPriceSumInquiry(int orderId, OrderDiscount? orderDiscount = null);

		public List<OrderDiscount> GetOrderDiscounts(int orderId);

		public object? GetOrders(Expression<Func<Order, bool>>? func,bool enableInclude);

        public bool IsDiscountExist(string code);

		public OrderDiscount? GetDiscount(string code);

        public bool AddDiscountCodeToOrder(int orderId,OrderDiscount discount);

        public bool IsOrderExist(Func<Order, bool> func);

        public Task<bool?> ConfirmOrder(int orderId, int userId);
	}
}
