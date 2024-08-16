using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toplearn.DataLayer.Entities.User;
using Toplearn.DataLayer.Entities.Wallet;

namespace Toplearn.DataLayer.Context
{
	public class TopLearnContext(DbContextOptions<TopLearnContext> options) : DbContext(options)
	{
		#region User

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<User_Role> UserRoles { get; set; }

		#endregion

		#region Wallet

		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<WalletType> WalletTypes { get; set; }

		#endregion


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region Set Required Data

			modelBuilder.Entity<Role>().HasData(
				new Role
				{
					RoleId = 1,
					RoleDetail = "کاربر سایت",
				},
				new Role
				{
					RoleId = 2,
					RoleDetail = "ادمین"
				},
				new Role()
				{
					RoleId = 3,
					RoleDetail = "استاد"
				},
				new Role()
				{
					RoleId = 4,
					RoleDetail = "صاحب سایت"
				});

			modelBuilder.Entity<WalletType>().HasData(new List<WalletType>()
			{
				new()
				{
					TypeId = 1,
					TypeTitle = "برداشت"
				},
				new()
				{
					TypeId = 2,
					TypeTitle = "واریز"
				},
				new() {
					TypeId = 3,
					TypeTitle = "خرید مستقیم دوره"
				}
			});

			#endregion

			#region Query Filter

			modelBuilder.Entity<Role>()
				.HasQueryFilter(x => x.IsActived);

			modelBuilder.Entity<User_Role>()
				.HasQueryFilter(x=>x.Role.IsActived && !x.User.IsDeleted);

			modelBuilder.Entity<User>()
				.HasQueryFilter(x => !x.IsDeleted);

			modelBuilder.Entity<Wallet>()
				.HasQueryFilter(x => !x.User.IsDeleted);

			#endregion

			base.OnModelCreating(modelBuilder);
		}
	}
}
