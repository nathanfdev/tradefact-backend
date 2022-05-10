using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AddConsignmentData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "ConsignmentQuantity",
                table: "FreightMovements",
                nullable: false,
                defaultValue: 0)
                .Annotation("ColumnOrder", 25);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsignmentQuantity",
                table: "FreightMovements");

        }
    }
}
