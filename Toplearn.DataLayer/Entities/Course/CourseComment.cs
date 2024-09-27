using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Course
{
	public class CourseComment
	{

		[Key]
		public int CommentId { get; set; }

		[Required]
		public int UserId { get; set; }

		[Required]
		public int CourseId { get; set; }

		[Display(Name = "کامنت")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(700, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(10, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		public string Comment { get; set; }


		public bool AccessFromAdmin { get; set; } = false;

		public DateTime CreateDate { get; set; }


		#region Relations

		[ForeignKey(nameof(UserId))]
		public virtual User.User User { get; set; }


		[ForeignKey(nameof(CourseId))]
		public virtual Course Course { get; set; }

		#endregion
	}
}
