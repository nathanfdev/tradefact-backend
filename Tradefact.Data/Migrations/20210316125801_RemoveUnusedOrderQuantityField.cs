using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class RemoveUnusedOrderQuantityField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderQuantityMinimum",
                table: "Products");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.AddColumn<int>(
                name: "OrderQuantityMinimum",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

        }
    }
}
