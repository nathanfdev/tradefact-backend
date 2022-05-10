using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class POScheduleLinesKeyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId",
                table: "PurchaseOrderItemScheduleLines",
                columns: new[] { "PurchaseOrderId", "PurchaseOrderItemId" },
                principalTable: "PurchaseOrderItems",
                principalColumns: new[] { "PurchaseOrderId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_PurchaseOrderItemId",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemScheduleLines_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines",
                columns: new[] { "PurchaseOrderId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines",
                columns: new[] { "PurchaseOrderId", "Id" },
                principalTable: "PurchaseOrderItems",
                principalColumns: new[] { "PurchaseOrderId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
