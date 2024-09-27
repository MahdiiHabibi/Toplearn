using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.DataLayer.Entities.Course
{
	public class CourseEpisode
	{
		public CourseEpisode() { }

		[Key]
		public int EpisodeId { get; set; }

		[Display(Name = "عنوان قسمت ")]
		[Required(ErrorMessage = "{0} ضروری است .")]
		[MaxLength(100, ErrorMessage = "{0} شما نمیتواند بیشتر از {1} باشه ")]
		[MinLength(10, ErrorMessage = "{0} شما نمیتواند کمتر از {1} باشه ")]
		public string EpisodeTitle { get; set; }

		public string EpisodeFileUrl { get; set; }

		public int CourseId { get; set; }

		public DateTime CreateDate { get; set; }


		[Display(Name = "زمان")]
		[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
		public TimeSpan EpisodeVideoTime { get; set; }


		public int EpisodeNumber { get; set; }

		public bool IsFree { get; set; } = false;

		#region Relations

		[ForeignKey(nameof(CourseId))]
		public virtual Course Course { get; set; }


		#endregion



	}
}
