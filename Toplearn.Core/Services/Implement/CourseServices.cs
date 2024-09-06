using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.IdentityModel.Tokens;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.DTOs.Teacher;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;

namespace Toplearn.Core.Services.Implement
{
	public class CourseServices(TopLearnContext context) : ICourseServices
	{
		public TopLearnContext Context { get; } = context;


		public List<Category> GetCategories(bool enableIgnoreQueryFilters)
		{
			IQueryable<Category> categories = Context.Categories;

			if (enableIgnoreQueryFilters)
			{
				categories = categories.IgnoreQueryFilters();
			}

			return categories.ToList();
		}

		public List<SelectListItem> GetCategories()
		{
			List<SelectListItem> model =
			[
				new SelectListItem
				{
					Text = "انتخاب کنید",
					Value = "-1"
				}
			];

			model.AddRange(context.Categories
				.Where(x => x.ParentCategoryId == null)
				.Select(x =>
					new SelectListItem()
					{
						Text = x.CategoryName,
						Value = x.CategoryId.ToString()
					}));

			return model;
		}

		public List<SelectListItem> GetChildCategories(int parentId)
		{
			List<SelectListItem> model =
			[
				new SelectListItem
				{
					Text = "انتخاب کنید" + "اگر نمیخواهید زیر گروه باشد چیزی انتخاب نکنید .",
					Value = "-1",
					Disabled = true,
					Selected = true
				}
			];

			model.AddRange(context.Categories
				.Where(x => x.ParentCategoryId == parentId)
				.Select(x =>
					new SelectListItem()
					{
						Text = x.CategoryName,
						Value = x.CategoryId.ToString(),
						Disabled = !x.IsActive,
					}));

			return model;
		}

		public List<SelectListItem> GetLevelOfCourses()
		{
			List<SelectListItem> model = [];
			int i = 0;
			foreach (var L in Enum.GetNames<CourseLevel>())
			{
				model.Add(new SelectListItem()
				{
					Text = L,
					Value = i.ToString()
				});
				i++;
			}

			return model;
		}

		public List<SelectListItem> GetStatusOfCourses()
		{
			List<SelectListItem> model = [];
			int i = 0;
			foreach (var L in Enum.GetNames<CourseStatus>())
			{
				model.Add(new SelectListItem()
				{
					Text = L.Replace("_", " "),
					Value = i.ToString()
				});
				i++;
			}
			return model;
		}

		public async Task<int> AddCourse(Course? model)
		{
			try
			{
				await context.Courses.AddAsync(model);
				await context.SaveChangesAsync();
				return model.CourseId;
			}
			catch
			{
				return 0;
			}
		}

		public async Task<List<Course?>> GetCourses()
		{
			return await Context.Courses.ToListAsync();
		}

		public async Task<List<Course?>> GetCourses(int teacherId)
		{
			return await Context.Courses.Where(x => x.TeacherId == teacherId).ToListAsync();
		}

		public Task<ShowsCourseViewModel> GetCoursesForShow(int userId, int pageId = 1, int take = 5, string nameFilter = "")
		{
			IQueryable<Course> courses = Context.Courses.Where(x => x.TeacherId == userId).Include(x => x.Category);

			if (nameFilter.IsNullOrEmpty() == false)
			{
				courses = courses.Where(x => x.CourseName.Contains(nameFilter));
			}


			int skip = (pageId - 1) * take;

			ShowsCourseViewModel model = new ShowsCourseViewModel
			{
				Courses = courses.Skip(skip).Take(take).ToList(),
				CurrentPage = pageId,
				PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(courses.Count()) / take).ToString())
			};

			foreach (var course in model.Courses)
			{
				course.Episodes = (Context.CourseEpisodes.Where(x=>x.CourseId == course.CourseId)).ToList();
			}

			return Task.FromResult(model);
		}

		public async Task<Course?> GetCourse(Func<Course?, bool> func, bool enableInclude = false)
		{
			try
			{
				return enableInclude ? Context.Courses.Include(x => x.Category).Include(x => x.Episodes).First(func) : Context.Courses.First(func);
			}
			catch
			{
				return null;
			}
		}

		public async Task<bool> AddCourseEpisode(CourseEpisode courseEpisode)
		{
			try
			{
				Context.CourseEpisodes.Add(courseEpisode);
				await Context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
