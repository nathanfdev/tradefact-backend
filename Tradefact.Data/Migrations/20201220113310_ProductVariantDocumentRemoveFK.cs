using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ProductVariantDocumentRemoveFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductDocuments_Products_ProductId",
                table: "ProductDocuments");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductDocuments_Products_ProductId",
                table: "ProductDocuments",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
