using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toplearn.Core.Convertors;
using Toplearn.Core.Services.Interface;
using Toplearn.DataLayer.Context;

namespace Toplearn.Core.Services.Implement
{
	// If you have a question, why in the layers where this repository is used, why the context is not used directly:
	// 1.This method is dynamic and can be used for any object in any project
	// 2.in the future development, only one part of the project can be  changed so that the whole project can benefit from that change, not changing several parts of project

	// I Initialize the Fields with Primary ctor
	public class ContextActions<TEntity>(TopLearnContext db) : IContextActions<TEntity> where TEntity : class
	{
		// Set The dynamic object and get it from Context
		private readonly DbSet<TEntity> _entities = db.Set<TEntity>();


		public async Task<bool> AddToContext(TEntity entity)
		{
			try
			{
				await db.AddAsync(entity);
				// It is the same SaveChanges the Database, To prevent its repetition, a method was created to be used for all dynamic object
				return await SaveContext();
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> RemoveFromContext(TEntity entity)
		{
			try
			{
				db.Remove(entity);
				// It is the same SaveChanges the Database, To prevent its repetition, a method was created to be used for all dynamic object
				return await SaveContext();
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> UpdateTblOfContext(TEntity entity)
		{
			try
			{
				var local = db.Set<TEntity>()
					.Local
					.FirstOrDefault(x => GetProperties<TEntity>.GetValueOfKeyPropertyOfTbl(x) == GetProperties<TEntity>.GetValueOfKeyPropertyOfTbl(entity));

				if (local != null)
				{
					db.Entry(local).State = EntityState.Detached;
				}

				db.Entry(entity).State = EntityState.Modified;

				// It is the same SaveChanges the Database, To prevent its repetition, a method was created to be used for all dynamic object
				return await SaveContext();
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> SaveContext()
		{
			try
			{
				// If SaveChangesAsync does its job correctly it will send the number one .
				int res = await db.SaveChangesAsync();
				// If the result of the above method is True, the number one is returned, and in this case, one is equal to one < (1==1)== true >. But if any other number is returned, the result is false
				return res == 1;
			}
			catch
			{
				return false;
			}
		}

		public async Task<bool> Exists(Func<TEntity, bool> fun) =>
			_entities.ToList().Where(fun).Any();


		public async Task<TEntity?> GetOne(Func<TEntity, bool>? fun = null)
		{
			return _entities.Where(fun).SingleOrDefault();
		}

		public async Task<IList<TEntity?>> Get(Expression<Func<TEntity, bool>>? fun = null)
		{
			IQueryable<TEntity?> query = _entities;
			// If the input function is null, it means that all data is requested Otherwise, we have data limits
			if (fun != null)
			{
				query = query.Where(fun);
			}

			return query.ToList();
		}

	}
}
