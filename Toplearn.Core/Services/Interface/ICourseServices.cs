using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Toplearn.Core.DTOs;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.DTOs.Course;
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

		public Task<List<Course>?> GetCourses(Expression<Func<Course, bool>>? expression,
			bool enableInclude = false);

		public Task<List<Course?>> GetCourses(int teacherId);

		public Task<ShowsCourseViewModel> GetCoursesForShow(int userId, int pageId = 1, int take = 5, string nameFilter = "");

		public Task<Course?> GetCourse(Func<Course, bool>? func, bool enableInclude = false);

		public Task<bool> AddCourseEpisode(CourseEpisode courseEpisode);

		public Task<bool> UpdateCourse(Course course);

		public Task<bool> UpdateCourseEpisode(CourseEpisode courseEpisode);

		public object? GetCourseEpisode(Expression<Func<CourseEpisode,bool>>? expression,bool enableInclude);

		public Task<ShowCourseEpisodes> GetCourseEpisodes(int teacherId, int courseId = 0, int take = 5, int pageId = 1, string? filterFullname = "");

		public Task<bool> CheckTheTeacherIdWithCourseId(int teachId,int courseId);

		public ShowCoursesInSearchViewModel GetCourse(int pageId = 1, string filter = ""
			, string priceType = "all", string orderByType = "date",
			int startPrice = 0, int endPrice = 0, List<int>? selectedGroups = null, int take = 8);

		public Task<TimeSpan> CourseVideosTimeInquiry(int courseId);

		public Course GetCourseForShow(int courseId);
	}
}
