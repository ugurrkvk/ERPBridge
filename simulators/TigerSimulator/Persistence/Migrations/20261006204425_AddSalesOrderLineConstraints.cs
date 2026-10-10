using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TigerSimulator.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSalesOrderLineConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_SalesOrderLine_LineNumber_Positive",
                table: "SalesOrderLine",
                sql: "[LineNumber] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SalesOrderLine_Quantity_Positive",
                table: "SalesOrderLine",
                sql: "[Quantity] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_SalesOrderLine_UnitPrice_NonNegative",
                table: "SalesOrderLine",
                sql: "[UnitPrice] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_SalesOrderLine_LineNumber_Positive",
                table: "SalesOrderLine");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SalesOrderLine_Quantity_Positive",
                table: "SalesOrderLine");

            migrationBuilder.DropCheckConstraint(
                name: "CK_SalesOrderLine_UnitPrice_NonNegative",
                table: "SalesOrderLine");
        }
    }
}
