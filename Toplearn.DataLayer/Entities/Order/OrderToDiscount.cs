using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Order
{
	public class OrderToDiscount
	{
		[Key]
		public int Id { get; set; }

		public int OrderId { get; set; }

		public string DiscountId { get; set; }


		#region Relations

		[ForeignKey(nameof(OrderId))]
		public virtual Order Order { get; set; }

		[ForeignKey(nameof(DiscountId))]
		public virtual OrderDiscount Discount { get; set; }


		#endregion
	}
}
