using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.DTOs.Teacher;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.Services.Interface
{
	public interface ICourseServices
	{
		public List<Category> GetCategories(bool enableIgnoreQueryFilters);

		public List<SelectListItem> GetCategories();

		public List<SelectListItem> GetChildCategories(int parentId);

		public List<SelectListItem> GetLevelOfCourses();

		public List<SelectListItem> GetStatusOfCourses();

		public Task<int> AddCourse(Course? model);

		public Task<List<Course?>> GetCourses();

		public Task<List<Course?>> GetCourses(int teacherId);

		public Task<ShowsCourseViewModel> GetCoursesForShow(int userId, int pageId = 1, int take = 5, string nameFilter = "");

		public Task<Course?> GetCourse(Func<Course?, bool> func, bool enableInclude = false);

		public Task<bool> AddCourseEpisode(CourseEpisode courseEpisode);


	}
}
