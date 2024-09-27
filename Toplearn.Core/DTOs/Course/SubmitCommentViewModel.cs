using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs.Course
{
	public class SubmitCommentViewModel
	{
		[Display(Name = "متن دید گاه")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(700, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(10, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		public string Comment { get; set; }

		[Display(Name = "امتیاز دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[Range(1, 10, ErrorMessage = "{0} باید در بازه ی اعداد 1 تا 10 باشد .")]
		public int DegreeOfCourse { get; set; } = 0;

		[Display(Name = "امتیاز استاد ")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[Range(1, 10, ErrorMessage = "{0} باید در بازه ی اعداد 1 تا 10 باشد .")]
		public int DegreeOfTeacher { get; set; } = 0;

		public int  CourseId { get; set; }

		public int pageId { get; set; }
	}
}
