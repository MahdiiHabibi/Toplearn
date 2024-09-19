using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Teacher
{
	public class ShowCourseEpisodes
	{

		public List<CourseEpisode> CourseEpisodes { get; set; } = [];

		public int CourseId { get; set; }

		public int CurrentPage { get; set; }

		public int PageCount { get; set; }
	}
}
