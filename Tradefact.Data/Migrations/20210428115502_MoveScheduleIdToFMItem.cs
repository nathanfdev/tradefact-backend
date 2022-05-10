using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class MoveScheduleIdToFMItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CargoItems_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId",
                table: "CargoItems");

            migrationBuilder.DropForeignKey(
                name: "FK_CargoItems_PurchaseOrderItemScheduleLines_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems");

            migrationBuilder.DropIndex(
                name: "IX_CargoItems_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseOrderId",
                table: "FreightMovementItems",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseOrderItemScheduleLineId",
                table: "FreightMovementItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "FreightMovementItems");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderItemScheduleLineId",
                table: "FreightMovementItems");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CargoItems_PurchaseOrderId_PurchaseOrderItemId_PurchaseOrderItemScheduleLineId",
                table: "CargoItems",
                columns: new[] { "PurchaseOrderId", "PurchaseOrderItemId", "PurchaseOrderItemScheduleLineId" });

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
    }
}
