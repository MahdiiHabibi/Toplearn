using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Order
{
	public class Order
	{

		[Key]
		public int OrderId { get; set; }

		public int UserId { get; set; }

		[Required]
		public int OrderSum { get; set; } = 0;

		[Required]
		public DateTime CreateTime { get; set; }

		[Required]
		public bool IsFinally { get; set; }


		#region Relations

		public virtual List<OrderDetail> OrderDetails { get; set; } = [];

		[ForeignKey(nameof(UserId))]
		public virtual User.User? User { get; set; }

		public virtual List<OrderToDiscount> OrderToDiscounts { get; set; } = [];


		#endregion
	}
}
