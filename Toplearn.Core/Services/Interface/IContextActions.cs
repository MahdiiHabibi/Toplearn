using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Toplearn.Core.Services.Interface
{
	public interface IContextActions<TEntity> where TEntity : class
	{
		public Task<bool> AddToContext(TEntity entity);

		public Task<bool> RemoveFromContext(TEntity entity);

		public Task<bool> UpdateTblOfContext(TEntity entity);

		public Task<bool> SaveContext();

		public Task<bool> Exists(Func<TEntity, bool>? fun);

		public Task<TEntity?> GetOne(Func<TEntity, bool>? fun = null);

		public Task<IList<TEntity?>> Get(Expression<Func<TEntity, bool>>? fun = null);
	}
}
