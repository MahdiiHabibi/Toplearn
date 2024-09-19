using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.DTOs
{
	public class ShowCoursesWithBoxViewModel
	{

		public int CourseId { get; set; }

		[Display(Name = "نام دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		public string CourseName { get; set; }


		[Display(Name = "عکس کاربر")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		public string CourseImagePath { get; set; }

		[Display(Name = "قیمت دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MinLength(500000, ErrorMessage = "{0} باید بیشتر از 50 هزار تومن باشد")]
		public int CoursePrice { get; set; }

		[Display(Name = "زمان")]
		[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
		public TimeSpan CourseVideoTime { get; set; }

	}
}
