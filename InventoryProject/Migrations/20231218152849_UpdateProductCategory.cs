using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_AspNetUsers_CreatedById",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "ProductCategories",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_CreatedById",
                table: "ProductCategories",
                newName: "IX_ProductCategories_ApplicationUserId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_AspNetUsers_ApplicationUserId",
                table: "ProductCategories",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_AspNetUsers_ApplicationUserId",
                table: "ProductCategories");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "ProductCategories",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_ApplicationUserId",
                table: "ProductCategories",
                newName: "IX_ProductCategories_CreatedById");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ProductCategories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_AspNetUsers_CreatedById",
                table: "ProductCategories",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
