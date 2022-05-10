using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class POScheduleLinesSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Countries_CountryofLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                column: "CountryofLoadingCode",
                principalTable: "Countries",
                principalColumn: "Code2",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines",
                column: "PortOfLoadingCode",
                principalTable: "Locations",
                principalColumn: "LocCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines",
                columns: new[] { "PurchaseOrderId", "Id" },
                principalTable: "PurchaseOrderItems",
                principalColumns: new[] { "PurchaseOrderId", "Id" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Countries_CountryofLoadingCode",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_Locations_PortOfLoadingCode",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItemScheduleLines_PurchaseOrderItems_PurchaseOrderId_Id",
                table: "PurchaseOrderItemScheduleLines");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
