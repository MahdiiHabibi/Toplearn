using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.DTOs.Course
{
	public class ShowCoursesInSearchViewModel
	{
		public List<ShowCoursesWithBoxViewModel> ShowCoursesWithBoxViewModels { get; set; } = [];

		public int CurrentPage { get; set; } = 1;

		public int PageCount { get; set; }

		public int TotalCourses { get; set; }

		public string? Filter { get; set; }

		public string? PriceType { get; set; }

		public string? OrderType { get; set; }

		public int StartPrice { get; set; } = 0;

		public int EndPrice { get; set; } = 10000000;

		public List<Category> Categories { get; set; } = [];

		public List<int> CategoriesId { get; set; } = [];

	}
}
