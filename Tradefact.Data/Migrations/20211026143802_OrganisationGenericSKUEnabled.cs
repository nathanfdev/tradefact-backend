using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class OrganisationGenericSKUEnabled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<bool>(
                name: "GenericSKUEnabled",
                table: "Organisations",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ExternalProducts",
                schema: "integration",
                columns: table => new
                {
                    BatchId = table.Column<Guid>(nullable: false),
                    SeqNo = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ProductId = table.Column<Guid>(nullable: true),
                    CompanyId = table.Column<Guid>(nullable: true),
                    SKU = table.Column<string>(maxLength: 150, nullable: true),
                    Identifier_GTIN = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_UPC = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_EAN = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_JAN = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_ASIN = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_ISBN = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_MPN = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_ePID = table.Column<string>(maxLength: 20, nullable: true),
                    Identifier_GPC = table.Column<string>(maxLength: 20, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Nickname = table.Column<string>(maxLength: 250, nullable: true),
                    Dimensions_Height = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Dimensions_Length = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Dimensions_Scale = table.Column<string>(maxLength: 16, nullable: true),
                    Dimensions_Width = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Dimensions_Weight = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Dimensions_weightMeasurement = table.Column<string>(maxLength: 16, nullable: true),
                    GoodsType = table.Column<string>(maxLength: 250, nullable: true),
                    HsCode = table.Column<string>(maxLength: 250, nullable: true),
                    Packing = table.Column<string>(maxLength: 50, nullable: true),
                    UnitsPerPackage = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    HazardousContents = table.Column<int>(maxLength: 50, nullable: false),
                    HazardClass = table.Column<string>(maxLength: 32, nullable: true),
                    HazardDescription = table.Column<string>(maxLength: 50, nullable: true),
                    HazardNotes = table.Column<string>(maxLength: 512, nullable: true),
                    Reference = table.Column<string>(maxLength: 256, nullable: true),
                    LithiumBatteryPacking = table.Column<string>(maxLength: 8, nullable: true),
                    MagneticFieldContained = table.Column<bool>(nullable: false),
                    Rotatable = table.Column<bool>(nullable: false),
                    Stackable = table.Column<bool>(nullable: false),
                    Barcode = table.Column<string>(maxLength: 250, nullable: true),
                    Tags = table.Column<string>(maxLength: 512, nullable: true),
                    StockQuantity = table.Column<int>(nullable: false),
                    MinStockQuantity = table.Column<int>(nullable: false),
                    NotifyStockQuantityBelow = table.Column<int>(nullable: false),
                    OrderQuantityMaximum = table.Column<int>(nullable: false),
                    IncomingStockQuantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalProducts", x => new { x.BatchId, x.SeqNo });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalProducts",
                schema: "integration");

            migrationBuilder.DropColumn(
                name: "GenericSKUEnabled",
                table: "Organisations");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
