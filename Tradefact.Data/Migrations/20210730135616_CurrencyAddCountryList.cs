using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class CurrencyAddCountryList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Currency",
                maxLength: 8,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CurrencyCountries",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(nullable: false),
                    CountryCode = table.Column<string>(maxLength: 4, nullable: false),
                    Active = table.Column<bool>(nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyCountries", x => new { x.CurrencyId, x.CountryCode });
                    table.ForeignKey(
                        name: "FK_CurrencyCountries_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code2",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyCountries_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyCountries_CountryCode",
                table: "CurrencyCountries",
                column: "CountryCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyCountries");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Currency");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
