using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class IncreaseDescriptionSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PurchaseOrderAdditionalCharges",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true)
                .Annotation("ColumnOrder", 5)
                .OldAnnotation("ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProductVariants",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true)
                .Annotation("ColumnOrder", 102)
                .OldAnnotation("ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true)
                .Annotation("ColumnOrder", 102)
                .OldAnnotation("ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "ItemDescription",
                table: "CargoItems",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PurchaseOrderAdditionalCharges",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("ColumnOrder", 5)
                .OldAnnotation("ColumnOrder", 5);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProductVariants",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("ColumnOrder", 102)
                .OldAnnotation("ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Products",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("ColumnOrder", 102)
                .OldAnnotation("ColumnOrder", 102);

            migrationBuilder.AlterColumn<string>(
                name: "ItemDescription",
                table: "CargoItems",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 512,
                oldNullable: true);

        }
    }
}
