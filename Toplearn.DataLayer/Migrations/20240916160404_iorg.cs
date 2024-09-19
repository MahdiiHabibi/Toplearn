using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class iorg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "CourseVideosTime",
                table: "Courses",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseVideosTime",
                table: "Courses");
        }
    }
}
