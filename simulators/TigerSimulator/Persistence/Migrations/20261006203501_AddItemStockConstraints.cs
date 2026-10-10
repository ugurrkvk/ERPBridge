using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TigerSimulator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddItemStockConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Item_OnHandQuantity_NonNegative",
                table: "Item",
                sql: "[OnHandQuantity] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Item_ReservedQuantity_NonNegative",
                table: "Item",
                sql: "[ReservedQuantity] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Item_ReservedQuantity_NotAboveStock",
                table: "Item",
                sql: "[ReservedQuantity] <= [OnHandQuantity]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Item_UnitPrice_NonNegative",
                table: "Item",
                sql: "[UnitPrice] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Item_OnHandQuantity_NonNegative",
                table: "Item");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Item_ReservedQuantity_NonNegative",
                table: "Item");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Item_ReservedQuantity_NotAboveStock",
                table: "Item");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Item_UnitPrice_NonNegative",
                table: "Item");
        }
    }
}
