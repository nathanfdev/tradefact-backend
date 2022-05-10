using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AddMediaDocumentDefaultImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");*/

            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultImage",
                table: "ProductDocuments",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefaultImage",
                table: "ProductDocuments");

            /*migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");*/
        }
    }
}
