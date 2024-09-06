using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class woiegjiower : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEpisode_Courses_CourseId",
                table: "CourseEpisode");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseEpisode",
                table: "CourseEpisode");

            migrationBuilder.RenameTable(
                name: "CourseEpisode",
                newName: "CourseEpisodes");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEpisode_CourseId",
                table: "CourseEpisodes",
                newName: "IX_CourseEpisodes_CourseId");

            migrationBuilder.AddColumn<int>(
                name: "EpisodeNumber",
                table: "CourseEpisodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseEpisodes",
                table: "CourseEpisodes",
                column: "EpisodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEpisodes_Courses_CourseId",
                table: "CourseEpisodes",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseEpisodes_Courses_CourseId",
                table: "CourseEpisodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseEpisodes",
                table: "CourseEpisodes");

            migrationBuilder.DropColumn(
                name: "EpisodeNumber",
                table: "CourseEpisodes");

            migrationBuilder.RenameTable(
                name: "CourseEpisodes",
                newName: "CourseEpisode");

            migrationBuilder.RenameIndex(
                name: "IX_CourseEpisodes_CourseId",
                table: "CourseEpisode",
                newName: "IX_CourseEpisode_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseEpisode",
                table: "CourseEpisode",
                column: "EpisodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseEpisode_Courses_CourseId",
                table: "CourseEpisode",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
