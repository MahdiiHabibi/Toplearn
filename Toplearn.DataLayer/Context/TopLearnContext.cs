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

			modelBuilder.Entity<Role>().HasData(new Role()
			{
				RoleId = 1,
				RoleDetail = "کاربر سایت"
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

			base.OnModelCreating(modelBuilder);
		}
	}
}
