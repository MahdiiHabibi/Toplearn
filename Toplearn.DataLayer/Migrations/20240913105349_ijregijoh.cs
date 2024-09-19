using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ijregijoh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "CategoryName", "IsActive", "ParentCategoryId" },
                values: new object[,]
                {
                    { 1, "برنامه نویسی سایت", true, null },
                    { 2, "برنامه نویسی موبایل ", true, null },
                    { 3, "طراحی سایت", true, null },
                    { 4, "بانک اطلاعاتی", true, null }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "ParentId", "PermissionDetail", "PermissionPersianDetail", "PermissionUrl" },
                values: new object[,]
                {
                    { 1, null, "Admin_Roles", "مقام ها", "POST" },
                    { 2, null, "Admin_Home", "ادمین", "/Admin" },
                    { 3, null, "Admin_User", "امور مربوط به کاربران", "/Admin/UserManager" },
                    { 4, null, "Teacher", "پنل استاد", "POST" },
                    { 35, null, "ChangeIvg", "تغییر کد احراز هویت ", "/Admin/ChangeIvg" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "IsActived", "RoleDetail" },
                values: new object[,]
                {
                    { 1, true, "کاربر سایت" },
                    { 2, true, "ادمین" },
                    { 3, true, "استاد" },
                    { 4, true, "صاحب سایت" }
                });

            migrationBuilder.InsertData(
                table: "WalletTypes",
                columns: new[] { "TypeId", "TypeTitle" },
                values: new object[,]
                {
                    { 1, "برداشت" },
                    { 2, "واریز" },
                    { 3, "خرید مستقیم دوره" }
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "ParentId", "PermissionDetail", "PermissionPersianDetail", "PermissionUrl" },
                values: new object[,]
                {
                    { 30, 1, "Admin_Roles_Index", "نمایش مقام ها", "/Admin/Roles" },
                    { 31, 1, "Admin_Roles_UpdateUserRole", "بروز رسانی مقام های کاربران", "POST" },
                    { 32, 1, "Admin_Roles_AddRole", "اضافه کردن مقام جدید ", "POST" },
                    { 33, 1, "Admin_Roles_ChangeRoleStatus", "تغییر وضعیت مقام ", "/Admin/RoleManager/ChangeRoleStatus" },
                    { 34, 1, "Admin_Roles_EditRole", "تغییر در اطلاعات مقام ها", "POST" },
                    { 36, 2, "Admin_Home_Index", "داشبورد ادمین", "/Admin" },
                    { 38, 3, "Admin_User_Index", "نمایش کاربران سایت", "/Admin/UserManager/" },
                    { 39, 3, "Admin_UserManager_ActiveAccount", "ارسال کد فعال سازی کاربر", "POST" },
                    { 40, 3, "Admin_UserManager_RemoveUserImage", "حذف آواتار شخصی کاربر", "POST" },
                    { 41, 3, "Admin_UserManager_UserForShow", "دیدن اطلاعات کاربر", "POST" },
                    { 42, 3, "Admin_UserManager_IncreaseTheWallet", "افزایش کیف پول کاربر", "POST" },
                    { 43, 4, "Teacher_Index", "داشبورد پنل استاد", "/Teacher/Index" },
                    { 44, 4, "Teacher_AddCourse", "اضافه کردن دوره ی جدید", "/Teacher/AddCourse" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "CategoryId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "WalletTypes",
                keyColumn: "TypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "WalletTypes",
                keyColumn: "TypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "WalletTypes",
                keyColumn: "TypeId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 4);
        }
    }
}
