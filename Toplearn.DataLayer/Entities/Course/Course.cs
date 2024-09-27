using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Toplearn.DataLayer.DataAnnotations;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;
using Toplearn.DataLayer.Entities.Order;

namespace Toplearn.DataLayer.Entities.Course
{
	public class Course
	{
		public Course()
		{
			
		}


		[Key]
		public int CourseId { get; set; }

		[Required]
		public int TeacherId { get; set; }

		[Required]
		public int CategoryId { get; set; }


		[Display(Name = "نام دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		public string CourseName { get; set; }


		[Display(Name = "توضیحات دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public string CourseDetail { get; set; }


		[Display(Name = "عکس کاربر")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		public string CourseImagePath { get; set; }
		

		[Display(Name = "سطح دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public CourseLevel CourseLevel { get; set; }


		[Display(Name = "وضعیت دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		public CourseStatus CourseStatus { get; set; }


		[Display(Name = "قیمت دوره")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MinLength(500000,ErrorMessage = "{0} باید بیشتر از 50 هزار تومن باشد")]
		public int CoursePrice { get; set; }


		[Required]
		public DateTime CreateTime { get; set; }

		public DateTime LastUpdateTime { get; set; }

		[MaxLengthTagsOfCourses(ErrorMessage = "دقت کنید که هشتگ ها را درست وارد کنید ؛ همچنین " + "هشتگ ها نمیتوانند بیشتر از 1000 کاراکتر باشند .")]
		public string? Tags { get; set; }

		public TimeSpan CourseVideosTime { get; set; } = TimeSpan.Zero;

		#region Nav Props


		[ForeignKey(nameof(TeacherId))]
		public virtual User.User Teacher { get; set; }


		[ForeignKey(nameof(CategoryId))]
		public virtual Category Category { get; set; }

		public virtual List<CourseEpisode> Episodes { get; set; }

		public virtual List<UserCourse>? UserOfCourses { get; set; }

		public virtual List<OrderDetail> OrderDetails { get; set; }

		public virtual List<CourseComment> CourseComments { get; set; }

		public virtual CourseOff CourseOff { get; set; }


		#endregion


	}
}
