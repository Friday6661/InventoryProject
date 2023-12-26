using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class EditOrder1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedById",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Orders",
                newName: "ApplicationUser");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CreatedById",
                table: "Orders",
                newName: "IX_Orders_ApplicationUser");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUser",
                table: "Orders",
                column: "ApplicationUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_ApplicationUser",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ApplicationUser",
                table: "Orders",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_ApplicationUser",
                table: "Orders",
                newName: "IX_Orders_CreatedById");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CreatedById",
                table: "Orders",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
