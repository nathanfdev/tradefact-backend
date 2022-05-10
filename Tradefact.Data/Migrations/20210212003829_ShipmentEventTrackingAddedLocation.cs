using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ShipmentEventTrackingAddedLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<string>(
                name: "LocationOfEvent",
                table: "ShipmentEvents",
                maxLength: 256,
                nullable: true)
                .Annotation("ColumnOrder", 14);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationOfEvent",
                table: "ShipmentEvents");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
