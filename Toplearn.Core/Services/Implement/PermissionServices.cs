using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Toplearn.Core.DTOs.Admin;
using Toplearn.Core.Security.Identity;
using Toplearn.Core.Services.Interface;
using Toplearn.Core.Services.Interface.ISendEmail;
using Toplearn.DataLayer.Context;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.User;
using static System.Formats.Asn1.AsnWriter;
using System.Data;
using Humanizer;
using IdentitySample.Repositories;
using Microsoft.IdentityModel.Tokens;

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

			List<Permission> permissions = [];

			var userRoles = _context.UserRoles.Where(x => x.UserId == userId)
				.Select(x => x.RoleId).ToList();
			foreach (var roleId in userRoles.ToList())
			{
				permissions.AddRange(
					_context.RolesPermissions
						.Where(x => x.RoleId == roleId)
						.Include(x => x.Permission)
						.Select(x => x.Permission)
						.ToList());
			}

			await scope.DisposeAsync();
			await DisposeAsync();

			return permissions;
		}

		public async Task<int[]?> GetIdOfUserPermissionsFromCookie()
		{
			await using var scope = _serviceProvider.CreateAsyncScope();

			_contextAccessor = (IHttpContextAccessor)
				scope.ServiceProvider.GetService(typeof(IHttpContextAccessor))!;

			var httpContext = _contextAccessor.HttpContext;

			var permissionClaim = httpContext.User.FindFirst(TopLearnClaimTypes.Permission)?.Value;

			await scope.DisposeAsync();

			return permissionClaim.IsNullOrEmpty() ? null : permissionClaim!.Split("|")
				.Select(x =>
					int.Parse(
						x.Split("\\")[0]))
				.ToArray();
		}

		public async Task<string[]> GetDetailOfUserPermissionsFromCookie()
		{
			return ((await GetUserPermissionsFromCookie())!).Select(x => x.PermissionDetail).ToArray();
		}

		public async Task<ImmutableHashSet<Permission>?> GetUserPermissionsFromCookie()
		{
			var permissionIds = await GetIdOfUserPermissionsFromCookie();
			if (permissionIds == null)
				return ImmutableHashSet.CreateRange(new List<Permission>());

			var permissions = permissionIds
				.Select(id =>
					Permissions
						.Single(x => x.PermissionId == id))
				.ToList();

			return ImmutableHashSet.CreateRange(permissions);
		}

		public async Task<List<PermissionOfRole>> GetPermissionForShowInRole(Role role)
		{
			List<PermissionOfRole> permissions = new List<PermissionOfRole>();

			foreach (var permission in Permissions.Where(x => x.ParentId == null))
			{
				var modelPermission = new PermissionOfRole()
				{
					ParentId = null,
					PermissionDetail = permission.PermissionDetail,
					PermissionId = permission.PermissionId,
					PermissionPersianDetail = permission.PermissionPersianDetail,
					PermissionUrl = permission.PermissionUrl,
					IsSelected = permission.ParentPermission.Any(),
					ParentsPermission = []
				};

				if (permission.ParentPermission.Count != 0)
				{
					foreach (var parentPermission in permission.ParentPermission)
					{
						modelPermission.ParentsPermission.Add(new PermissionOfRole()
						{
							ParentId = permission.PermissionId,
							PermissionDetail = parentPermission.PermissionDetail,
							PermissionId = parentPermission.PermissionId,
							PermissionPersianDetail = parentPermission.PermissionPersianDetail,
							PermissionUrl = parentPermission.PermissionUrl,
							IsSelected =
							role.RolesPermissionsList != null
							&&
								role.RolesPermissionsList.Any(x => x.PermissionId == parentPermission.PermissionId)
						});
					}
				}

				permissions.Add(modelPermission);
			}

			return permissions;
		}

		public async Task<bool> UpdateRolePermissions(List<PermissionOfRole> permissions, int roleId)
		{
			try
			{
				var permissionId = new List<int>();
				foreach (var Ppermission in permissions)
				{

					if (Ppermission.ParentsPermission == null) continue;

					permissionId.AddRange(Ppermission.ParentsPermission
						.Where(x => x.IsSelected)
						.Select(permission => permission.PermissionId));

				}

				await using var scope = _serviceProvider.CreateAsyncScope();
				_context = (TopLearnContext)
					scope.ServiceProvider.GetService(typeof(TopLearnContext))!;
				_context.RolesPermissions.RemoveRange(_context.RolesPermissions.Where(x => x.RoleId == roleId));
				await _context.RolesPermissions.AddRangeAsync(permissionId.Select(x => new RolesPermissions()
				{
					PermissionId = x,
					RoleId = roleId
				}));
				await _context.SaveChangesAsync();

				IUtilities utilities = scope.ServiceProvider.GetService<IUtilities>()!;
				await utilities.ChangeIVGOfTopLearn();

				await DisposeAsync();
				await scope.DisposeAsync();

				return true;
			}
			catch
			{
				return false;
			}
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
