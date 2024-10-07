using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Admin
{
	public class AddOffToCoursesViewModel
	{


		[Display(Name = "درصد تخیف")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[Range(1, 100, ErrorMessage = "{0} باید بین 0 درصد تا 100 درصد باشد")]
		public int OffPercent { get; set; } = 0;


		[Display(Name = "تاریخ اتمام مهلت")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public DateTime OffEndDate { get; set; }


		public required List<(int, string)> Courses { get; set; }

		[Display(Name = "دوره ها ")]
		[Required(ErrorMessage = "انتخاب یک دوره حداقل ضروری است .")]
		public List<int> CoursesId { get; set; }

	}

	public class AddOffToCategoriesViewModel
	{

		[Display(Name = "درصد تخیف")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[Range(1, 100, ErrorMessage = "{0} باید بین 0 درصد تا 100 درصد باشد")]
		public int OffPercent { get; set; } = 0;


		[Display(Name = "تاریخ اتمام مهلت")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public DateTime OffEndDate { get; set; }


		public required List<(int, string)>? Categories { get; set; }

		[Display(Name = "دوره ها ")]
		[Required(ErrorMessage = "انتخاب یک دوره حداقل ضروری است .")]
		public List<int> CategoriesId { get; set; }

	}
}
