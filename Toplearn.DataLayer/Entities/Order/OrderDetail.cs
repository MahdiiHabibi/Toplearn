using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Order
{
	public class OrderDetail
	{
		public int OrderDetailId { get; set; }

		public int OrderId { get; set; }

		public int CourseId { get; set; }

		public int Price { get; set; }

		



		#region Relations

		[ForeignKey(nameof(OrderId))]
		public virtual Order Order { get; set; }

		[ForeignKey(nameof(CourseId))]
		public virtual Course.Course Course { get; set; }
		
		#endregion
	}
}
