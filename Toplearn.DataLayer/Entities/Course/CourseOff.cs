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
		public required int CourseId { get; set; }

		public required int UserId { get; set; }

		public required int RealCoursePrice { get; set; }

		public required DateTime OffEndDate { get; set; }

		public required int OffPrice { get; set; }


		#region Relations

		[ForeignKey(nameof(CourseId))]
		public virtual Course Course { get; set; }

		
		[ForeignKey(nameof(UserId))] 
		public User.User User { get; set; }

		#endregion
	}
}
