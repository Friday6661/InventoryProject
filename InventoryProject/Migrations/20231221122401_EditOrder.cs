using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class EditOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderProductReusables");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "OrderProductConsumables");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderProductReusables",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderProductReusables",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OrderProductConsumables",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OrderProductConsumables",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderProductReusables");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderProductReusables");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OrderProductConsumables");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OrderProductConsumables");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderProductReusables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "OrderProductConsumables",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
