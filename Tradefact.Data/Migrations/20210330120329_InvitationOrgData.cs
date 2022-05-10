using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class InvitationOrgData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<Guid>(
                name: "InviteRequestedByOrganisationId",
                table: "AspNetUserInvitations",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "InviteRequestedByUserId",
                table: "AspNetUserInvitations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InviteRequestedByOrganisationId",
                table: "AspNetUserInvitations");

            migrationBuilder.DropColumn(
                name: "InviteRequestedByUserId",
                table: "AspNetUserInvitations");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
