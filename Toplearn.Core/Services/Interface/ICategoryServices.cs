using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toplearn.Core.DTOs.Admin;
using Toplearn.DataLayer.Entities.Course;

namespace Toplearn.Core.Services.Interface
{
	public interface ICategoryServices
	{
		public IContextActions<Category> ContextActionsForCategory { get; }

		public Task<IQueryable<Category>> GetCategories(bool enableIgnoreQueryFilter);

		public Task<ShowCategoriesViewModel> GetCategories(bool enableIgnoreQueryFilter = false, int pageId = 1, int take = 5, string categoryFilter = "");

		public Task<bool> UpdateCategory(Category  category);

		public Task<Category?> GetCategory(Func<Category,bool> func,bool enableIgnoreQueryFilter);


	}
}
