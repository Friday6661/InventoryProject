using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class editProduct1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_CreatedById",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Products",
                newName: "ApplicationUser");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CreatedById",
                table: "Products",
                newName: "IX_Products_ApplicationUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUser",
                table: "Products",
                column: "ApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_ApplicationUser",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ApplicationUser",
                table: "Products",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Products_ApplicationUser",
                table: "Products",
                newName: "IX_Products_CreatedById");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_CreatedById",
                table: "Products",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
