using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Course
{
	public class CourseOff
	{

		[Key]
		public int OffId { get; set; }

		public required int CourseId { get; set; }

		public required int AdminId { get; set; }

		public required int RealPrice { get; set; }

		public required int OffPrice { get; set; }

		public DateTime OffEndDate { get; set; }

		#region Relations

		[ForeignKey(nameof(AdminId))]
		public virtual User.User Admin { get; set; }


		[ForeignKey(nameof(CourseId))]
		public virtual Course Course { get; set; }

		#endregion
	}
}
