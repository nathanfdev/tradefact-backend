using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ExternalPurchaseOrdersSupplierId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierCode",
                schema: "integration",
                table: "ExternalPurchaseOrders");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                maxLength: 36,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierId",
                schema: "integration",
                table: "ExternalPurchaseOrders");

            migrationBuilder.AddColumn<string>(
                name: "SupplierCode",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
