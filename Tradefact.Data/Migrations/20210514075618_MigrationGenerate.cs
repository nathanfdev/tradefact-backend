using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class MigrationGenerate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserInvitations_AspNetUsers_UserId",
                table: "AspNetUserInvitations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserInvitations_UserId",
                table: "AspNetUserInvitations");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserInvitations",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserInvitations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserInvitations_UserId",
                table: "AspNetUserInvitations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserInvitations_AspNetUsers_UserId",
                table: "AspNetUserInvitations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
