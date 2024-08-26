using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.ISendEmail;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.Core.Services.Implement
{
	public class PermissionServices : IPermissionServices, IDisposable, IAsyncDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private TopLearnContext _context;
		private IHttpContextAccessor _contextAccessor;
		public PermissionServices(IServiceProvider serviceProvider)
		{
			using (var services = serviceProvider.CreateScope())
			{
				_context = (TopLearnContext)
				   services.ServiceProvider.GetService(typeof(TopLearnContext))!;

				var listOfPermission = _context.Permissions.Include(x => x.RolesPermissionsList).Include(x => x.ParentPermission).ToList();
				Permissions = ImmutableHashSet.CreateRange(listOfPermission);
			}

			_serviceProvider = serviceProvider;
		}

		public ImmutableHashSet<Permission> Permissions { get; }



		public Task<List<Permission>> GetListOfPermissionsOfTopLearn()
		{
			return Task.FromResult(Permissions.ToList());
		}

		public async Task<List<Permission>> GetListOfPermissionsOfUser(int userId)
		{

			await using var scope = _serviceProvider.CreateAsyncScope();

			_context = (TopLearnContext)
							scope.ServiceProvider.GetService(typeof(TopLearnContext))!;


			var permissions =
			   _context.UserRoles
				   .Where(ur => ur.UserId == userId)
				   .Include(i => i.Role)
				   .ThenInclude(p => p.RolesPermissionsList)!
				   .ThenInclude(p => p.Permission)
				   .Select(x => x.Role)
				   .SelectMany(x => x.RolesPermissionsList!)
				   .Select(x => x.Permission)
				   .ToList();

			await scope.DisposeAsync();
			await DisposeAsync();

			return permissions;
		}

		public async Task<int[]> GetIdOfUserPermissionsFromCookie()
		{
			await using var scope = _serviceProvider.CreateAsyncScope();

			_contextAccessor = (IHttpContextAccessor)
				scope.ServiceProvider.GetService(typeof(IHttpContextAccessor))!;

			var httpContext = _contextAccessor.HttpContext;

			var permissionClaim = httpContext.User.FindFirst(TopLearnClaimTypes.Permission)?.Value;
			
			int[] permissionIds = permissionClaim.Split("|")
				.Select(x => 
					int.Parse(
						x.Split("_")[0]))
				.ToArray();

			await scope.DisposeAsync();

			return permissionIds;
		}

		public async Task<HashSet<Permission>?> GetUserPermissionsFromCookie()
		{
			HashSet<Permission> permissions = new HashSet<Permission>();
			var permissionIds = await GetIdOfUserPermissionsFromCookie();
			foreach (var id in permissionIds)
			{
				permissions.Add(Permissions.Single(x => x.PermissionId == id));
			}

			return permissions;
		}


		#region Dispose

		public void Dispose()
		{
			_context.Dispose();
		}

		public async ValueTask DisposeAsync()
		{
			await _context.DisposeAsync();
		}

		#endregion

	}
}
