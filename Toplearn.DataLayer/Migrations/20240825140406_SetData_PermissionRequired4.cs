using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class SetData_PermissionRequired4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 34,
                column: "PermissionDetail",
                value: "Admin_Roles_EditRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: 34,
                column: "PermissionDetail",
                value: "Admin_Roles_8EditRole");
        }
    }
}
