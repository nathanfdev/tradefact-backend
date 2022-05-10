using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class IncreasedSizeOfHsCodes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "FreightMovements",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 60,
                oldNullable: true)
                .Annotation("ColumnOrder", 60);

            migrationBuilder.AlterColumn<string>(
                name: "HSCodes",
                table: "FreightMovements",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("ColumnOrder", 101)
                .OldAnnotation("ColumnOrder", 101);

            migrationBuilder.AlterColumn<Guid>(
                name: "BuyerId",
                table: "FreightMovements",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 61,
                oldNullable: true)
                .Annotation("ColumnOrder", 61);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "SupplierId",
                table: "FreightMovements",
                type: "uniqueidentifier",
                maxLength: 60,
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true)
                .OldAnnotation("ColumnOrder", 60);

            migrationBuilder.AlterColumn<string>(
                name: "HSCodes",
                table: "FreightMovements",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true)
                .Annotation("ColumnOrder", 101)
                .OldAnnotation("ColumnOrder", 101);

            migrationBuilder.AlterColumn<Guid>(
                name: "BuyerId",
                table: "FreightMovements",
                type: "uniqueidentifier",
                maxLength: 61,
                nullable: true,
                oldClrType: typeof(Guid),
                oldNullable: true)
                .OldAnnotation("ColumnOrder", 61);

        }
    }
}
