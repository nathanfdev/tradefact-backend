using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class POScheduleLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItemScheduleLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PurchaseOrderId = table.Column<Guid>(nullable: false),
                    PurchaseOrderItemId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    RequestedGoodsReadyDate = table.Column<DateTime>(nullable: true),
                    ConfirmedGoodsReadyDate = table.Column<DateTime>(nullable: true),
                    RequestedDeliveryDate = table.Column<DateTime>(nullable: true),
                    ConfirmedDeliveryDate = table.Column<DateTime>(nullable: true),
                    ScheduleLineOrderQuantity = table.Column<int>(nullable: false),
                    ScheduleLineCommittedQuantity = table.Column<int>(nullable: false),
                    OrderQuantityUnit = table.Column<string>(nullable: true),
                    ScheduleLineOrderWeight = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    PortOfLoadingCode = table.Column<string>(maxLength: 8, nullable: true),
                    CountryofLoadingCode = table.Column<string>(maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItemScheduleLines", x => new { x.PurchaseOrderId, x.PurchaseOrderItemId, x.Id });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemScheduleLines_Countries_CountryofLoadingCode",
                        column: x => x.CountryofLoadingCode,
                        principalTable: "Countries",
                        principalColumn: "Code2",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode",
                        column: x => x.PortOfLoadingCode,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id",
                        columns: x => new { x.PurchaseOrderId, x.Id },
                        principalTable: "PurchaseOrderItems",
                        principalColumns: new[] { "PurchaseOrderId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemScheduleLines_CountryofLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                column: "CountryofLoadingCode");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                column: "PortOfLoadingCode");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines",
                columns: new[] { "PurchaseOrderId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderItemScheduleLines");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
