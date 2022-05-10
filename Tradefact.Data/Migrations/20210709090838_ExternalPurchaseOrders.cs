using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ExternalPurchaseOrders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.EnsureSchema(
                name: "integration");

            migrationBuilder.CreateTable(
                name: "ExternalPurchaseOrders",
                schema: "integration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExternalID = table.Column<string>(maxLength: 128, nullable: true),
                    PurchaseOrderNumber = table.Column<string>(maxLength: 128, nullable: true),
                    Reference = table.Column<string>(maxLength: 128, nullable: true),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    GoodsReadyDate = table.Column<DateTime>(nullable: false),
                    DateOfIssue = table.Column<DateTime>(nullable: false),
                    PlaceOfIssue = table.Column<DateTime>(maxLength: 128, nullable: false),
                    Status = table.Column<string>(maxLength: 30, nullable: true),
                    CurrencyRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    CurrencyCode = table.Column<string>(maxLength: 16, nullable: true),
                    SubTotal = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    TotalTax = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PaymentTerms = table.Column<string>(maxLength: 512, nullable: true),
                    SupplierCode = table.Column<string>(maxLength: 36, nullable: true),
                    SupplierName = table.Column<string>(maxLength: 256, nullable: true),
                    Tags = table.Column<string>(maxLength: 512, nullable: true),
                    Received = table.Column<DateTime>(nullable: false),
                    Imported = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ImportDate = table.Column<DateTime>(nullable: false),
                    ImportUserId = table.Column<Guid>(nullable: false),
                    ImportUserName = table.Column<string>(maxLength: 16, nullable: true),
                    GenericProductId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalPurchaseOrderLineItems",
                schema: "integration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ExternalPurchaseOrderId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LineItemID = table.Column<string>(maxLength: 36, nullable: true),
                    SKU = table.Column<string>(maxLength: 64, nullable: true),
                    Description = table.Column<string>(maxLength: 128, nullable: true),
                    SupplierReference = table.Column<string>(maxLength: 128, nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    TaxType = table.Column<string>(maxLength: 32, nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    LineAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPurchaseOrderLineItems", x => new { x.ExternalPurchaseOrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_ExternalPurchaseOrderLineItems_ExternalPurchaseOrders_ExternalPurchaseOrderId",
                        column: x => x.ExternalPurchaseOrderId,
                        principalSchema: "integration",
                        principalTable: "ExternalPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalPurchaseOrderLineItems",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "ExternalPurchaseOrders",
                schema: "integration");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
