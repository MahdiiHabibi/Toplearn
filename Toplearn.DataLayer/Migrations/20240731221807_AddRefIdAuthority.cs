using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplearn.DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddRefIdAuthority : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Authority",
                table: "Wallets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RefId",
                table: "Wallets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "WalletTypes",
                columns: new[] { "TypeId", "TypeTitle" },
                values: new object[] { 3, "خرید مستقیم دوره" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WalletTypes",
                keyColumn: "TypeId",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Authority",
                table: "Wallets");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "Wallets");
        }
    }
}
