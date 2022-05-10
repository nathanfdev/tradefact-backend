using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ProductSuppliersFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Organisations_SupplierId",
                table: "ProductSuppliers");

            migrationBuilder.DropIndex(
                name: "IX_ProductSuppliers_SupplierId",
                table: "ProductSuppliers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Organisations_Id",
                table: "ProductSuppliers",
                column: "Id",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Organisations_Id",
                table: "ProductSuppliers");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSuppliers_SupplierId",
                table: "ProductSuppliers",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Organisations_SupplierId",
                table: "ProductSuppliers",
                column: "SupplierId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
