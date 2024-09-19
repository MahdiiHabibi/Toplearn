using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Teacher
{
	public class AddCourseEpisode
	{
		
		[Display(Name = "عنوان قسمت ")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(1, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		public string EpisodeTitle { get; set; }

		[Required]
		public int CourseId { get; set; }


		[Display(Name = "زمان")]
		[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
		public TimeSpan EpisodeVideoTime { get; set; }


		public bool IsFree { get; set; } = false;

		[Display(Name = "فایل جلسه")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public IFormFile EpisodeFile { get; set; }
	}
}
