using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class ShipmentEventTrackingExtendedVoyage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AlterColumn<string>(
                name: "Voyage",
                table: "ShipmentEvents",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("ColumnOrder", 11)
                .OldAnnotation("ColumnOrder", 11);

            migrationBuilder.AlterColumn<string>(
                name: "Activity",
                table: "ShipmentEvents",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .Annotation("ColumnOrder", 12)
                .OldAnnotation("ColumnOrder", 12);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Voyage",
                table: "ShipmentEvents",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true)
                .Annotation("ColumnOrder", 11)
                .OldAnnotation("ColumnOrder", 11);

            migrationBuilder.AlterColumn<string>(
                name: "Activity",
                table: "ShipmentEvents",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("ColumnOrder", 12)
                .OldAnnotation("ColumnOrder", 12);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
