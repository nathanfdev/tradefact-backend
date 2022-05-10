using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ExternalPurchaseOrdersNullsPlusOrg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AlterColumn<string>(
                name: "PlaceOfIssue",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "ImportUserId",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ImportDate",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "GoodsReadyDate",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "GenericProductId",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfIssue",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "integration",
                table: "ExternalPurchaseOrders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlaceOfIssue",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "datetime2",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "OrderDate",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ImportUserId",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ImportDate",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "GoodsReadyDate",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GenericProductId",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfIssue",
                schema: "integration",
                table: "ExternalPurchaseOrders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
