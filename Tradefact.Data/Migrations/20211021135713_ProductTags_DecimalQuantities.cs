using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ProductTags_DecimalQuantities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "OrderQuantity",
                table: "PurchaseOrderItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitsPerPackage",
                table: "Products",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "Products",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ConsignmentQuantity",
                table: "FreightMovements",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "CargoItems",
                type: "decimal(18, 4)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "OrderQuantity",
                table: "PurchaseOrderItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<int>(
                name: "UnitsPerPackage",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<int>(
                name: "ConsignmentQuantity",
                table: "FreightMovements",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AlterColumn<int>(
                name: "Qty",
                table: "CargoItems",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
