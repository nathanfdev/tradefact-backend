using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ShipmentEventTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.CreateTable(
                name: "ShipmentEvents",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    EquipmentItemId = table.Column<string>(maxLength: 32, nullable: false)
                        .Annotation("ColumnOrder", 3),
                    EventId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 4),
                    TrackingNumber = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 2),
                    TimeOfEvent = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 10),
                    Voyage = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 11),
                    Activity = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 12),
                    Information = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 13)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentEvents", x => new { x.ShipmentId, x.EquipmentItemId, x.EventId });
                    table.ForeignKey(
                        name: "FK_ShipmentEvents_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentEvents");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
