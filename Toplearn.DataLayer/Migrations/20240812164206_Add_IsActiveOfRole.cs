using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsActiveOfRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActived",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 1,
                column: "IsActived",
                value: false);

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "IsActived", "RoleDetail" },
                values: new object[,]
                {
                    { 2, false, "ادمین" },
                    { 3, false, "استاد" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "IsActived",
                table: "Roles");
        }
    }
}
