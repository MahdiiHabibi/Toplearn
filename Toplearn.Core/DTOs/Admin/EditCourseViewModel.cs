using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toplearn.Core.DTOs.Teacher;
using Toplearn.DataLayer.DataAnnotations;
using Toplearn.DataLayer.Entities.Course;
namespace Toplearn.Core.DTOs.Admin
{
	public class EditCourseViewModel : AddCourseViewModel
	{
		[Required(ErrorMessage = "در دریافت کد احراز دوره مشکلی به وجود آمده است .")]
		
		public int CourseId { get; set; }

		public string LastImageName { get; set; }

		public string? LastFileDemoName { get; set; }

	}
}
