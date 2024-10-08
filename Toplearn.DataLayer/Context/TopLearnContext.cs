﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Toplearn.DataLayer.DataAnnotations;
using Toplearn.DataLayer.Entities.Course;
using Toplearn.DataLayer.Entities.Course.CourseRequirements;
using Toplearn.DataLayer.Entities.Order;
using Toplearn.DataLayer.Entities.Permission;
using Toplearn.DataLayer.Entities.Setting;
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

		#region App Setting

		public DbSet<AppSetting> AppSettings { get; set; }

		#endregion

		#region Permission

		public DbSet<Permission> Permissions { get; set; }

		public DbSet<RolesPermissions> RolesPermissions { get; set; }


		#endregion

		#region Course

		#region Categories

		public DbSet<Category> Categories { get; set; }

		#endregion


		public DbSet<Course> Courses { get; set; }

		public DbSet<CourseEpisode> CourseEpisodes { get; set; }

		public DbSet<UserCourse> UserCourses { get; set; }

		public DbSet<CourseComment> CourseComments { get; set; }

		public DbSet<CourseOff> CourseOffs { get; set; }
		#endregion

		#region Order

		public DbSet<Order> Orders { get; set; }

		public DbSet<OrderDetail> OrderDetails { get; set; }

		public DbSet<OrderDiscount> OrderDiscounts { get; set; }

		public DbSet<OrderToDiscount> OrdersToDiscounts { get; set; }

		#endregion


		
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			var cascadeFKs = modelBuilder.Model.GetEntityTypes()
				.SelectMany(t => t.GetForeignKeys())
				.Where(fk => fk is { IsOwnership: false, DeleteBehavior: DeleteBehavior.Cascade });

			foreach (var fk in cascadeFKs)
				fk.DeleteBehavior = DeleteBehavior.Restrict;


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

			modelBuilder.Entity<WalletType>()
				.HasData(new List<WalletType>()
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

			modelBuilder.Entity<Permission>()
				.HasData(
				new List<Permission>()
			{
			 	new ()
			 	{
			 		PermissionId  = 1,
			 		PermissionDetail ="Admin_Roles" ,
			 		PermissionPersianDetail ="مقام ها" ,
			 		PermissionUrl = "POST"
			 	},
			 	new ()
			 	{
			 		PermissionId  = 30,
			 		PermissionDetail = "Admin_Roles_Index",
			 		PermissionPersianDetail ="نمایش مقام ها" ,
			 		ParentId = 1,
			 		PermissionUrl = "/Admin/Roles"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 31,
			 		ParentId = 1,
			 		PermissionDetail = "Admin_Roles_UpdateUserRole",
			 		PermissionPersianDetail ="بروز رسانی مقام های کاربران" ,
			 		PermissionUrl = "POST"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 32,
			 		ParentId = 1,
			 		PermissionDetail = "Admin_Roles_AddRole",
			 		PermissionPersianDetail ="اضافه کردن مقام جدید " ,
			 		PermissionUrl = "POST"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 33,
			 		ParentId = 1,
			 		PermissionDetail = "Admin_Roles_ChangeRoleStatus",
			 		PermissionPersianDetail ="تغییر وضعیت مقام " ,
			 		PermissionUrl = "/Admin/RoleManager/ChangeRoleStatus"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 34,
			 		ParentId = 1,
			 		PermissionDetail = "Admin_Roles_EditRole",
			 		PermissionPersianDetail ="تغییر در اطلاعات مقام ها" ,
			 		PermissionUrl = "POST"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 35,
			 		PermissionDetail = "ChangeIvg",
			 		PermissionPersianDetail =  "تغییر کد احراز هویت " ,
			 		PermissionUrl = "/Admin/ChangeIvg"
			 	}
			 	,new ()
			 	{

			 		PermissionId = 2,
			 		PermissionDetail = "Admin_Home",
			 		PermissionPersianDetail ="ادمین" ,
			 		PermissionUrl = "/Admin"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 36,
			 		PermissionDetail = "Admin_Home_Index",
			 		PermissionPersianDetail ="داشبورد ادمین" ,
			 		PermissionUrl = "/Admin",
			 		ParentId = 2
			 	}
			 	,new ()
			 	{
			 		PermissionId = 3,
			 		PermissionDetail = "Admin_User",
			 		PermissionPersianDetail ="امور مربوط به کاربران" ,
			 		PermissionUrl = "/Admin/UserManager"
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 38,
			 		PermissionDetail = "Admin_User_Index",
			 		PermissionPersianDetail ="نمایش کاربران سایت" ,
			 		PermissionUrl = "/Admin/UserManager/",
			 		ParentId = 3
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 39,
			 		PermissionDetail = "Admin_UserManager_ActiveAccount",
			 		PermissionPersianDetail ="ارسال کد فعال سازی کاربر" ,
			 		PermissionUrl = "POST",
			 		ParentId = 3
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 40,
			 		PermissionDetail = "Admin_UserManager_RemoveUserImage",
			 		PermissionPersianDetail ="حذف آواتار شخصی کاربر" ,
			 		PermissionUrl = "POST",
			 		ParentId = 3
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 41,
			 		PermissionDetail = "Admin_UserManager_UserForShow",
			 		PermissionPersianDetail ="دیدن اطلاعات کاربر" ,
			 		PermissionUrl = "POST",
			 		ParentId = 3
			 	}
			 	,new ()
			 	{
			 		PermissionId  = 42,
			 		PermissionDetail = "Admin_UserManager_IncreaseTheWallet",
			 		PermissionPersianDetail ="افزایش کیف پول کاربر" ,
			 		PermissionUrl = "POST",
			 		ParentId = 3
			 	},
			 	new ()
			 	{
			 		PermissionId = 4,
			 		PermissionDetail = "Teacher",
			 		PermissionPersianDetail = "پنل استاد",
			 		PermissionUrl = "POST"
			 	},
			 	new ()
			 	{
			 		PermissionId = 43,
			 		ParentId = 4,
			 		PermissionDetail = "Teacher_Index",
			 		PermissionPersianDetail = "داشبورد پنل استاد",
			 		PermissionUrl = "/Teacher/Index"
			 	},
			 	new ()
			 	{
			 		PermissionId =44,
			 		PermissionDetail = "Teacher_AddCourse",
			 		PermissionPersianDetail = "اضافه کردن دوره ی جدید",
			 		PermissionUrl = "/Teacher/AddCourse",
			 		ParentId = 4
			 	},
					//new ()
					//{
					//	PermissionDetail = "",
					//	PermissionPersianDetail = "",
					//	PermissionUrl = "",
					//	ParentId = 4
					//},
					//new ()
					//{
					//	PermissionDetail = "",
					//	PermissionPersianDetail = "",
					//	PermissionUrl = "",
					//	ParentId = 4
					//}

			});
			modelBuilder.Entity<Category>()
				.HasData(new List<Category>()
			{
			 	new ()
			 	{
			 		CategoryId = 1,
			 		CategoryName = "برنامه نویسی سایت"
			 	},
			 	new ()
			 	{
			 		CategoryId = 2,
			 		CategoryName = "برنامه نویسی موبایل "
			 	},
			 	new ()
			 	{
			 		CategoryId = 3,
			 		CategoryName = "طراحی سایت"
			 	},
			 	new ()
			 	{
			 		CategoryId = 4,
			 		CategoryName = "بانک اطلاعاتی"
			 	}
			});
			#endregion

			#region Query Filter

			modelBuilder.Entity<Role>()
				.HasQueryFilter(x => x.IsActived);

			modelBuilder.Entity<User_Role>()
				.HasQueryFilter(x => x.Role.IsActived && !x.User.IsDeleted);

			modelBuilder.Entity<User>()
				.HasQueryFilter(x => !x.IsDeleted);

			modelBuilder.Entity<Wallet>()
				.HasQueryFilter(x => !x.User.IsDeleted);

			modelBuilder.Entity<Category>()
				.HasQueryFilter(c => c.IsActive);
			#endregion


			base.OnModelCreating(modelBuilder);
		}


	}
}
