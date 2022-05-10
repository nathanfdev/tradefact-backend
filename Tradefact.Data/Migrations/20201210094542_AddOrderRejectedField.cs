using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class AddOrderRejectedField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OrderRejected",
                table: "PurchaseOrders",
                type: "bit",
                nullable: false,
                defaultValue: false)
                .Annotation("ColumnOrder", 70);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderRejected",
                table: "PurchaseOrders");

        }
    }
}
