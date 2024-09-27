using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Course
{
	public class UserCourse
	{
		public UserCourse()
		{
			
		}

		[Key]
		public int Id { get; set; }

		public int CourseId { get; set; }

		public int UserId { get; set; }


		#region Relations

		[ForeignKey(nameof(UserId))]
		public virtual User.User? User { get; set; }

		[ForeignKey(nameof(CourseId))]
		public virtual Course? Course { get; set; }

		#endregion
	}
}
