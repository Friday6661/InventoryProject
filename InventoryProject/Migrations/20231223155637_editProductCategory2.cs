using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class editProductCategory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_AspNetUsers_ApplicationUserId",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "ProductCategories",
                newName: "ApplicationUser");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_ApplicationUserId",
                table: "ProductCategories",
                newName: "IX_ProductCategories_ApplicationUser");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_AspNetUsers_ApplicationUser",
                table: "ProductCategories",
                column: "ApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_AspNetUsers_ApplicationUser",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "ApplicationUser",
                table: "ProductCategories",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_ApplicationUser",
                table: "ProductCategories",
                newName: "IX_ProductCategories_ApplicationUserId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_AspNetUsers_ApplicationUserId",
                table: "ProductCategories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
