using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class ekogojrwqerwg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseOffs_Users_AdminId",
                table: "CourseOffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseOffs",
                table: "CourseOffs");

            migrationBuilder.DropIndex(
                name: "IX_CourseOffs_AdminId",
                table: "CourseOffs");

            migrationBuilder.DropIndex(
                name: "IX_CourseOffs_CourseId",
                table: "CourseOffs");

            migrationBuilder.DropColumn(
                name: "OffId",
                table: "CourseOffs");

            migrationBuilder.RenameColumn(
                name: "RealPrice",
                table: "CourseOffs",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "CourseOffs",
                newName: "RealCoursePrice");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseOffs",
                table: "CourseOffs",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOffs_UserId",
                table: "CourseOffs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOffs_Users_UserId",
                table: "CourseOffs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseOffs_Users_UserId",
                table: "CourseOffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseOffs",
                table: "CourseOffs");

            migrationBuilder.DropIndex(
                name: "IX_CourseOffs_UserId",
                table: "CourseOffs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CourseOffs",
                newName: "RealPrice");

            migrationBuilder.RenameColumn(
                name: "RealCoursePrice",
                table: "CourseOffs",
                newName: "AdminId");

            migrationBuilder.AddColumn<int>(
                name: "OffId",
                table: "CourseOffs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseOffs",
                table: "CourseOffs",
                column: "OffId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOffs_AdminId",
                table: "CourseOffs",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseOffs_CourseId",
                table: "CourseOffs",
                column: "CourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOffs_Users_AdminId",
                table: "CourseOffs",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
