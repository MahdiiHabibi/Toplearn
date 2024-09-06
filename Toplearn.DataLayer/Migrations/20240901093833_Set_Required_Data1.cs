using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Set_Required_Data1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "ParentId", "PermissionDetail", "PermissionPersianDetail", "PermissionUrl" },
                values: new object[,]
                {
                    { 4, null, "Teacher", "پنل استاد", "POST" },
                    { 43, 4, "Teacher_Index", "داشبورد پنل استاد", "/Teacher/Index" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 4);
        }
    }
}
