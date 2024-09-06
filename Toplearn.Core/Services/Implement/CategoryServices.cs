using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.Services.Implement
{
	public class CategoryServices(TopLearnContext context, IContextActions<Category> contextActionsForCategory) : ICategoryServices
	{
		public TopLearnContext Context { get; } = context;
		public IContextActions<Category> ContextActionsForCategory { get; } = contextActionsForCategory;

		public async Task<IQueryable<Category>> GetCategories(bool enableIgnoreQueryFilter)
		{
			IQueryable<Category> categories = Context.Categories;

			if (enableIgnoreQueryFilter)
			{
				categories = categories.IgnoreQueryFilters();
			}

			return categories;
		}

		public async Task<ShowCategoriesViewModel> GetCategories(bool enableIgnoreQueryFilter = false, int pageId = 1, int take = 5, string categoryFilter = "")
		{
			IQueryable<Category> categories = Context.Categories;

			if (enableIgnoreQueryFilter)
			{
				categories = categories.IgnoreQueryFilters();
			}

			if (!categoryFilter.IsNullOrEmpty())
			{
				categories = categories.Where(x => x.CategoryName.Contains(categoryFilter));
			}

			categories = categories.Include(x => x.ChildCategories).Include(x => x.Courses);
			int skip = (pageId - 1) * take;

			var model = new ShowCategoriesViewModel();

			model.Categories.AddRange(categories.Skip(skip).Take(take));
			model.CurrentPage = pageId;
			model.PageCount = int.Parse(Math.Ceiling(Convert.ToDouble(categories.Count()) / take).ToString());

			return model;
		}

		public async Task<bool> UpdateCategory(Category category)
		{
			try
			{
				return await ContextActionsForCategory.UpdateTblOfContext(category);
			}
			catch
			{
				return false;
			}
		}

		public async Task<Category?> GetCategory(Func<Category, bool> func, bool enableIgnoreQueryFilter)
		{
			try
			{
				IQueryable<Category> categories = Context.Categories;

				if (enableIgnoreQueryFilter)
				{
					categories = categories.IgnoreQueryFilters();
				}

				var model = categories.Single(func);
				return model;
			}
			catch
			{
				return null;
			}
		}

	}
}
