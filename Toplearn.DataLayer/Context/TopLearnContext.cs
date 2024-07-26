using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toplearn.DataLayer.Entities.User;

namespace Toplearn.DataLayer.Context
{
    public class TopLearnContext(DbContextOptions<TopLearnContext> options) : DbContext(options)
    {
	    #region User

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User_Role> UserRoles { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
	        modelBuilder.Entity<Role>().HasData(new Role()
	        {
                RoleId = 1,
                RoleDetail = "کاربر سایت"
	        });

            base.OnModelCreating(modelBuilder);
        }
    }
}
