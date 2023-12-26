using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryProject.Migrations
{
    /// <inheritdoc />
    public partial class ProcessQuantityOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProcessQuantity",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessQuantity",
                table: "Orders");
        }
    }
}
