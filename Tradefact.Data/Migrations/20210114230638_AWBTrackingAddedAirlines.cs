using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AWBTrackingAddedAirlines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.CreateTable(
                name: "Airlines",
                columns: table => new
                {
                    IATA2LetterCcode = table.Column<string>(maxLength: 2, nullable: false),
                    AWBPrefix = table.Column<string>(maxLength: 3, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Tracking_Enabled = table.Column<bool>(nullable: true, defaultValue: false),
                    Tracking_Provider = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Airlines", x => new { x.AWBPrefix, x.IATA2LetterCcode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Airlines");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
