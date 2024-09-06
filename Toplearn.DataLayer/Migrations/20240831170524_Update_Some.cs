using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class Update_Some : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdateTime",
                table: "Courses",
                newName: "LastUpdateTime");

            migrationBuilder.AddColumn<string>(
                name: "UserDescription",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserDescription",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastUpdateTime",
                table: "Courses",
                newName: "UpdateTime");
        }
    }
}
