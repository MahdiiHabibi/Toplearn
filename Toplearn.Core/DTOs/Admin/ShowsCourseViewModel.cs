using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Admin
{
	public class ShowsCourseViewModel 
	{
		public List<DataLayer.Entities.Course.Course> Courses { get; set; } = [];

		public int CurrentPage { get; set; }
		public int PageCount { get; set; }

	}
}
