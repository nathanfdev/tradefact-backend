using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class InvitationMemberOf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.AddColumn<Guid>(
                name: "MemberOfOrganisationId",
                table: "AspNetUserInvitations",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberOfOrganisationId",
                table: "AspNetUserInvitations");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                nullable: false,
                defaultValue: "");
        }
    }
}
