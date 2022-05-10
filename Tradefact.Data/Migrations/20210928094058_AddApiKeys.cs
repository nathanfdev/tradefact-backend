using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AddApiKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsBulkUpload",
                table: "PurchaseOrderItems",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Symbol",
                table: "Currency",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExposedToOtherOrganization",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserPreferences",
                table: "AspNetUsers",
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

            migrationBuilder.CreateTable(
                name: "NewsFeeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekNo = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeeds", x => x.Id);
                });

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
                    CurrencyCode = table.Column<string>(nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
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

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                schema: "integration",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 36, nullable: false),
                    OrganisationId = table.Column<Guid>(maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "NewsFeedSections",
                columns: table => new
                {
                    NewsFeedId = table.Column<Guid>(nullable: false),
                    Reference = table.Column<string>(maxLength: 36, nullable: false),
                    Title = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeedSections", x => new { x.NewsFeedId, x.Reference });
                    table.ForeignKey(
                        name: "FK_NewsFeedSections_NewsFeeds_NewsFeedId",
                        column: x => x.NewsFeedId,
                        principalTable: "NewsFeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NewsFeedItems",
                columns: table => new
                {
                    NewsFeedId = table.Column<Guid>(nullable: false),
                    NewsSectionReference = table.Column<string>(maxLength: 36, nullable: false),
                    SeqNo = table.Column<int>(nullable: false),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsFeedItems", x => new { x.NewsFeedId, x.NewsSectionReference, x.SeqNo });
                    table.ForeignKey(
                        name: "FK_NewsFeedItems_NewsFeedSections_NewsFeedId_NewsSectionReference",
                        columns: x => new { x.NewsFeedId, x.NewsSectionReference },
                        principalTable: "NewsFeedSections",
                        principalColumns: new[] { "NewsFeedId", "Reference" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyCountries_CountryCode",
                table: "CurrencyCountries",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSupplierCurrency_ProductSupplierId",
                table: "ProductSupplierCurrency",
                column: "ProductSupplierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyCountries");

            migrationBuilder.DropTable(
                name: "NewsFeedItems");

            migrationBuilder.DropTable(
                name: "ProductSupplierCurrency");

            migrationBuilder.DropTable(
                name: "ApiKeys",
                schema: "integration");

            migrationBuilder.DropTable(
                name: "NewsFeedSections");

            migrationBuilder.DropTable(
                name: "NewsFeeds");

            migrationBuilder.DropColumn(
                name: "IsBulkUpload",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "Symbol",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "IsExposedToOtherOrganization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserPreferences",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
