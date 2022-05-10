using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class InventoryChangesMarch2d : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSuppliers");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IncomingStockQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MinStockQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NotifyStockQuantityBelow",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderQuantityMaximum",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderQuantityMinimum",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_ASIN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_EAN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_GPC",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_GTIN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_ISBN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_JAN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_MPN",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_UPC",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Identifier_ePID",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "SKU",
                table: "Products",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
