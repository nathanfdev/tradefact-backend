using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AddProductSupplierCurrencies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.CreateTable(
                name: "ProductSupplierCurrency",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedByUser = table.Column<string>(nullable: true),
                    CreationDateInternal = table.Column<DateTime>(nullable: false),
                    LastChangeUser = table.Column<string>(nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    ProductSupplierId = table.Column<Guid>(nullable: false),
                    CurrencyCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSupplierCurrency", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSupplierCurrency_ProductSuppliers_ProductSupplierId",
                        column: x => x.ProductSupplierId,
                        principalTable: "ProductSuppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSupplierCurrency_ProductSupplierId",
                table: "ProductSupplierCurrency",
                column: "ProductSupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSupplierCurrency");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
