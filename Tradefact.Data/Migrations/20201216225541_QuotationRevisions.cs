using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class QuotationRevisions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_QuotationRequests_QuotationRequestId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_QuotationRequestId",
                table: "Quotations");

            //migrationBuilder.DropColumn(
            //    name: "Discriminator",
            //    table: "CargoItems");

            migrationBuilder.AddColumn<int>(
                name: "Revision",
                table: "Quotations",
                nullable: false,
                defaultValue: 0)
                .Annotation("ColumnOrder", 5);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_QuotationRequestId_Id",
                table: "Quotations",
                columns: new[] { "QuotationRequestId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_QuotationRequests_QuotationRequestId",
                table: "Quotations",
                column: "QuotationRequestId",
                principalTable: "QuotationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_QuotationRequests_QuotationRequestId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_QuotationRequestId_Id",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "Revision",
                table: "Quotations");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_QuotationRequestId",
                table: "Quotations",
                column: "QuotationRequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_QuotationRequests_QuotationRequestId",
                table: "Quotations",
                column: "QuotationRequestId",
                principalTable: "QuotationRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
