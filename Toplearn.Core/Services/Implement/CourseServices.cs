using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.IdentityModel.Tokens;
using Toplearn.Core.DTOs;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.DTOs.Course;
using Toplearn.Core.DTOs.Teacher;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;
using Toplearn.DataLayer.Entities.User;

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

			return [.. categories];
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

		public async Task<List<Course>?> GetCourses(Expression<Func<Course, bool>>? expression, bool enableInclude = false)
		{
			try
			{
				IQueryable<Course> courses = Context.Courses;

				if (enableInclude)
				{
					courses = courses.Include(x => x.Episodes).Include(x => x.Teacher).Include(x=>x.CourseOff);
				}

				if (expression != null)
				{
					courses = courses.Where(expression);
				}

				return await courses.ToListAsync();

			}
			catch
			{
				return null;
			}
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

			ShowsCourseViewModel model = new()
			{
				Courses = courses.Skip(skip).Take(take).ToList(),
				CurrentPage = pageId,
				PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(courses.Count()) / take).ToString())
			};

			foreach (var course in model.Courses)
			{
				course.Episodes = (Context.CourseEpisodes.Where(x => x.CourseId == course.CourseId)).ToList();
			}

			return Task.FromResult(model);
		}

		public async Task<Course?> GetCourse(Func<Course, bool>? func, bool enableInclude = false)
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

		public async Task<bool> UpdateCourse(Course course)
		{
			try
			{
				Context.Update(course);
				await Context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> UpdateCourseEpisode(CourseEpisode courseEpisode)
		{
			try
			{
				Context.Update(courseEpisode);
				await Context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public object? GetCourseEpisode(Expression<Func<CourseEpisode, bool>>? expression, bool enableInclude)
		{
			try
			{
				IQueryable<CourseEpisode> courseEpisodes = Context.CourseEpisodes;

				if (enableInclude)
				{
					courseEpisodes = courseEpisodes.Include(x => x.Course);
				}

				if (expression != null)
				{
					courseEpisodes = courseEpisodes.Where(expression);
				}

				var model = courseEpisodes.ToList();

				if (model.Count == 1)
				{
					return model.First();
				}

				if (model.Count == 0)
				{
					return null;
				}

				else
				{
					return model;
				}

			}
			catch
			{
				return null;
			}
		}

		public Task<ShowCourseEpisodes> GetCourseEpisodes(int teacherId, int courseId = 0, int take = 5, int pageId = 1, string? filterFullname = "")
		{
			IQueryable<CourseEpisode> result = Context.CourseEpisodes;

			if (!string.IsNullOrEmpty(filterFullname))
			{
				result = result.Where(u => u.EpisodeTitle.Contains(filterFullname));
			}

			if (courseId != 0)
			{
				result = result.Where(x => x.CourseId == courseId);
			}

			result = result
				.Include(x => x.Course)
				.Where(x => x.Course.TeacherId == teacherId);


			// Show Item In Page
			int skip = (pageId - 1) * take;

			var model = new ShowCourseEpisodes()
			{
				CourseId = courseId,
				CurrentPage = pageId,
				PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(result.Count()) / take).ToString())
			};

			model.CourseEpisodes.AddRange(
				result.OrderBy(u => u.EpisodeNumber)
					.Skip(skip)
					.Take(take));

			if (model.PageCount == 0)
			{
				model.CourseEpisodes = null;
			}

			return Task.FromResult(model);
		}

		public async Task<bool> CheckTheTeacherIdWithCourseId(int teachId, int courseId)
		{
			IQueryable<Course> courses = Context.Courses.Where(x => x.TeacherId == teachId && x.CourseId == courseId);
			return await courses.AnyAsync();
		}

		public ShowCoursesInSearchViewModel GetCourse(int pageId = 1, string filter = ""
			, string priceType = "all", string orderByType = "date",
			int startPrice = 0, int endPrice = 0, List<int>? selectedGroups = null, int take = 8)
		{


			IQueryable<Course> result = Context.Courses.Include(x => x.Episodes);

			if (!string.IsNullOrEmpty(filter))
			{
				result = result.Where(c => c.CourseName.Contains(filter) || c.Tags!.Contains(filter.Replace("-", " ")));
			}

			result = priceType.ToLower() switch
			{
				"all" => result,
				"buy" => result.Where(c => c.CoursePrice > 0),
				"free" => result.Where(c => c.CoursePrice == 0),
				_ => result
			};

			bool totalPriceRes = false;

			result = orderByType.ToLower() switch
			{
				"date" => result.OrderByDescending(c => c.CreateTime),
				"updatedate" => result.OrderByDescending(c => c.LastUpdateTime),
				"price" => result.OrderByDescending(c => c.CoursePrice),
				"totaltime" => result.OrderByDescending(x => x.CourseVideosTime),
				"populer" => result.Include(x=>x.OrderDetails).ThenInclude(x=>x.Order).OrderByDescending(c=>c.OrderDetails.Count),
				_ => result
			};

			if (startPrice > 0)
			{
				result = result.Where(c => c.CoursePrice >= startPrice * 10);
			}
			else
			{
				startPrice = 0;
			}

			if (endPrice > 0)
			{
				result = result.Where(c => c.CoursePrice <= endPrice * 10);
			}
			else
			{
				endPrice = 10000000;
			}


			if (selectedGroups != null && selectedGroups.Count != 0)
			{
				result = selectedGroups
					.Aggregate(result, (current, group) =>
						current
							.Include(x => x.Category)
							.ThenInclude(x => x.ChildCategories)
							.Where(x =>
								x.CategoryId == group
										|| x.Category.ParentCategoryId == group));
			}

			var skip = (pageId - 1) * take;

			var model = new ShowCoursesInSearchViewModel()
			{
				EndPrice = endPrice,
				Filter = filter,
				OrderType = orderByType,
				PriceType = priceType,
				StartPrice = startPrice,
				CurrentPage = pageId,
				PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(result.Count()) / take).ToString("00")),
				TotalCourses = result.Count(),
				Categories = GetCategories(false)
			};

			model.ShowCoursesWithBoxViewModels.AddRange(result
				.Select(c => new ShowCoursesWithBoxViewModel()
				{
					CourseId = c.CourseId,
					CourseImagePath = c.CourseImagePath,
					CoursePrice = c.CoursePrice,
					CourseName = c.CourseName,
					CourseVideoTime = c.CourseVideosTime
				})
				.Skip(skip)
				.Take(take));

			if (selectedGroups != null) model.CategoriesId.AddRange(selectedGroups);

			return model;
		}

		public async Task<TimeSpan> CourseVideosTimeInquiry(int courseId)
		{
			var course = await GetCourse(x => x.CourseId == courseId, true);

			if (course == null)
			{
				return TimeSpan.Zero;
			}

			return course.Episodes.Select(x => x.EpisodeVideoTime)
				.Aggregate(TimeSpan.Zero, (current, timeSpan) => current + (TimeSpan)timeSpan);
		}

		public Course GetCourseForShow(int courseId)
		{
			return Context.Courses.Include(c => c.Episodes)
				.Include(c => c.Teacher)
				.Include(x=>x.CourseOff)
				.SingleOrDefault(c => c.CourseId == courseId)!;
		}

		public int GetCourseStudentCounts(int courseId)
		{
			return Context.Courses
				.Include(x => x.UserOfCourses)
				.First(x => x.CourseId == courseId).UserOfCourses?.Count ?? 0;
		}

		public ShowCommentsViewModel ShowComments(int courseId, int take, int pageId)
		{
			IQueryable<CourseComment> result = Context.CourseComments;


			if (courseId != 0)
			{
				result = result.Where(x => x.CourseId == courseId);
			}

			result = result
				.Include(x => x.User);


			// Show Item In Page
			int skip = (pageId - 1) * take;

			var model = new ShowCommentsViewModel
			{
				CurrentPage = pageId,
				PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(result.Count()) / take).ToString())
			};

			model.CourseComments.AddRange(result.OrderByDescending(u => u.CreateDate)
				.Skip(skip)
				.Take(take));

			if (model.PageCount == 0)
			{
				model.CourseComments = [];
			}

			return model;
		}

		public bool AddComment(CourseComment courseComment)
		{
			try
			{
				context.CourseComments.Add(courseComment);
				context.SaveChangesAsync();
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
