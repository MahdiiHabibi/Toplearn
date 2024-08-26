using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SetData_PermissionRequired2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 39,
                column: "PermissionDetail",
                value: "Admin_UserManager_ActiveAccount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 39,
                column: "PermissionDetail",
                value: "َAdmin_UserManager_ActiveAccount");
        }
    }
}
