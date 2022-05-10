using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AddressGeoLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<decimal>(
                name: "Position_Latitude",
                table: "Addresses",
                type: "decimal(18, 4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Position_Longitude",
                table: "Addresses",
                type: "decimal(18, 4)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position_Latitude",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "Position_Longitude",
                table: "Addresses");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
