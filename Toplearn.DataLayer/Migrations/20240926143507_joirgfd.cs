using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class joirgfd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseOff_Courses_CourseId",
                table: "CourseOff");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseOff_Users_AdminId",
                table: "CourseOff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseOff",
                table: "CourseOff");

            migrationBuilder.RenameTable(
                name: "CourseOff",
                newName: "CourseOffs");

            migrationBuilder.RenameIndex(
                name: "IX_CourseOff_CourseId",
                table: "CourseOffs",
                newName: "IX_CourseOffs_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseOff_AdminId",
                table: "CourseOffs",
                newName: "IX_CourseOffs_AdminId");

            migrationBuilder.AddColumn<DateTime>(
                name: "OffEndDate",
                table: "CourseOffs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseOffs",
                table: "CourseOffs",
                column: "OffId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOffs_Courses_CourseId",
                table: "CourseOffs",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOffs_Users_AdminId",
                table: "CourseOffs",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseOffs_Courses_CourseId",
                table: "CourseOffs");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseOffs_Users_AdminId",
                table: "CourseOffs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseOffs",
                table: "CourseOffs");

            migrationBuilder.DropColumn(
                name: "OffEndDate",
                table: "CourseOffs");

            migrationBuilder.RenameTable(
                name: "CourseOffs",
                newName: "CourseOff");

            migrationBuilder.RenameIndex(
                name: "IX_CourseOffs_CourseId",
                table: "CourseOff",
                newName: "IX_CourseOff_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseOffs_AdminId",
                table: "CourseOff",
                newName: "IX_CourseOff_AdminId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseOff",
                table: "CourseOff",
                column: "OffId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOff_Courses_CourseId",
                table: "CourseOff",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseOff_Users_AdminId",
                table: "CourseOff",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
