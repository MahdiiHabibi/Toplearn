using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toplearn.DataLayer.DataAnnotations;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Teacher
{
	public class AddCourseViewModel
	{
		[Display(Name = "نام دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		public string CourseName { get; set; }

		[Display(Name = "توضیحات دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public string CourseDetail { get; set; }

		[Display(Name = "قیمت")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MinLengthOfPrice(ErrorMessage = "قیمت دوره باید بیشتر از 50 هزار تومن باشد . دقت کنید قیمت را به ریال وارد کنید .")]
		public int CoursePrice { get; set; }

		[MaxLengthTagsOfCourses(ErrorMessage = "دقت کنید که هشتگ ها را درست وارد کنید ؛ همچنین " + "هشتگ ها نمیتوانند بیشتر از 1000 کاراکتر باشند .")]
		public string? Tags { get; set; }

		public IFormFile? CourseImageFile { get; set; }
		public IFormFile? CourseDemoVideo { get; set; }


		[Display(Name = "گروه ها")]
		[Required(ErrorMessage = "انتخاب یک گروه ضروری است .")]
		public int CategoryId { get; set; }

		[Display(Name = "زیر گروه ها ")]
		public int? SubCategoryId { get; set; }

		[Display(Name = "سطح دوره")]
		[Required(ErrorMessage = "انتخاب {0} ضروری است .")]
		public int Level { get; set; }

		[Display(Name = "وضعیت دوره")]
		[Required(ErrorMessage = "انتخاب {0} ضروری است .")]
		public int Status { get; set; }

	}
}
