using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class POScheduleLineCargoItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<Guid>(
                name: "PlaceOfLoadingId",
                table: "CargoItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseOrderId",
                table: "CargoItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseOrderItemId",
                table: "CargoItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseOrderItemScheduleLineId",
                table: "CargoItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoItems_PlaceOfLoadingId",
                table: "CargoItems",
                column: "PlaceOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoItems_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems",
                columns: new[] { "PurchaseOrderId", "PurchaseOrderItemId", "PurchaseOrderItemScheduleLineId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CargoItems_Addresses_PlaceOfLoadingId",
                table: "CargoItems",
                column: "PlaceOfLoadingId",
                principalTable: "Addresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CargoItems_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId",
                table: "CargoItems",
                columns: new[] { "PurchaseOrderId", "PurchaseOrderItemId" },
                principalTable: "PurchaseOrderItems",
                principalColumns: new[] { "PurchaseOrderId", "Id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CargoItems_PurchaseOrderItemScheduleLines_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems",
                columns: new[] { "PurchaseOrderId", "PurchaseOrderItemId", "PurchaseOrderItemScheduleLineId" },
                principalTable: "PurchaseOrderItemScheduleLines",
                principalColumns: new[] { "PurchaseOrderId", "PurchaseOrderItemId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargoItems_Addresses_PlaceOfLoadingId",
                table: "CargoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CargoItems_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId",
                table: "CargoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CargoItems_PurchaseOrderItemScheduleLines_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems");

            migrationBuilder.DropIndex(
                name: "IX_CargoItems_PlaceOfLoadingId",
                table: "CargoItems");

            migrationBuilder.DropIndex(
                name: "IX_CargoItems_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems");

            migrationBuilder.DropColumn(
                name: "PlaceOfLoadingId",
                table: "CargoItems");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "CargoItems");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderItemId",
                table: "CargoItems");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderItemScheduleLineId",
                table: "CargoItems");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
