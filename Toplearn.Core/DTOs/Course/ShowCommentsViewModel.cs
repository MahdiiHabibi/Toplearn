﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Course
{
	public class ShowCommentsViewModel
	{
		public List<CourseComment> CourseComments { get; set; } = [];

		public int CurrentPage { get; set; }


		public int PageCount { get; set; }
	}
}
