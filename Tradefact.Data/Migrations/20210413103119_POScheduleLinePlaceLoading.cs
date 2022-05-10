using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class POScheduleLinePlaceLoading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropColumn(
                name: "PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceOfLoadingId",
                table: "PurchaseOrderItemScheduleLines",
                maxLength: 8,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PlaceOfLoadingId",
                table: "PurchaseOrderItemScheduleLines",
                column: "PlaceOfLoadingId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Addresses_PlaceOfLoadingId",
                table: "PurchaseOrderItemScheduleLines",
                column: "PlaceOfLoadingId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Addresses_PlaceOfLoadingId",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PlaceOfLoadingId",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropColumn(
                name: "PlaceOfLoadingId",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.AddColumn<string>(
                name: "PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                type: "nvarchar(8)",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                column: "PortOfLoadingCode");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                column: "PortOfLoadingCode",
                principalTable: "Locations",
                principalColumn: "LocCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
