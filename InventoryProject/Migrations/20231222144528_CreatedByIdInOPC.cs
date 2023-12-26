using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class CreatedByIdInOPC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "OrderProductReusables",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "OrderProductReusables",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "OrderProductConsumables",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "OrderProductConsumables",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductReusables_UserId",
                table: "OrderProductReusables",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductConsumables_UserId",
                table: "OrderProductConsumables",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductConsumables_AspNetUsers_UserId",
                table: "OrderProductConsumables",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductReusables_AspNetUsers_UserId",
                table: "OrderProductReusables",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductConsumables_AspNetUsers_UserId",
                table: "OrderProductConsumables");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductReusables_AspNetUsers_UserId",
                table: "OrderProductReusables");

            migrationBuilder.DropIndex(
                name: "IX_OrderProductReusables_UserId",
                table: "OrderProductReusables");

            migrationBuilder.DropIndex(
                name: "IX_OrderProductConsumables_UserId",
                table: "OrderProductConsumables");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "OrderProductReusables");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OrderProductReusables");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "OrderProductConsumables");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OrderProductConsumables");
        }
    }
}
