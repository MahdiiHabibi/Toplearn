using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class erjg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFree",
                table: "CourseEpisodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFree",
                table: "CourseEpisodes");
        }
    }
}
