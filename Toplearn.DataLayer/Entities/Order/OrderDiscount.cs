using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Order
{
	public class OrderDiscount
	{
		public OrderDiscount()
		{
			DiscountCode = GetDiscountCode();
		}

		[Key]
		public string DiscountCode { get; set; }

		public int DiscountPercent { get; set; }

		public int? UsableCount { get; set; }

		public DateTime? StartDate { get; set; }

		public DateTime? EndDate { get; set; }

		public string GetDiscountCode()
		{
			var guid = Guid.NewGuid().ToString();
			var model = "";
			for (var i = 0; i < 5; i++)
			{
				model += guid[i];
			}

			return model;
		}



		#region Relations

		public virtual List<OrderToDiscount> OrderToDiscounts { get; set; }

		#endregion
	}
}
