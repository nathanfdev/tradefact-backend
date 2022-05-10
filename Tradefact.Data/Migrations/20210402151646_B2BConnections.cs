using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class B2BConnections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "CargoItems");

            migrationBuilder.CreateTable(
                name: "B2B_Connections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrgansationId = table.Column<Guid>(nullable: false),
                    LinkedOrganisationId = table.Column<Guid>(nullable: false),
                    Tags = table.Column<string>(nullable: true),
                    Rating = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B2B_Connections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_B2B_Connections_Organisations_LinkedOrganisationId",
                        column: x => x.LinkedOrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_B2B_Connections_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "B2B_ConnectionContacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConnectionId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    FullName = table.Column<string>(maxLength: 250, nullable: true),
                    FirstName = table.Column<string>(maxLength: 250, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 250, nullable: true),
                    LastName = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(maxLength: 250, nullable: true),
                    Salutation = table.Column<string>(maxLength: 250, nullable: true),
                    Department = table.Column<string>(maxLength: 250, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LocationId = table.Column<Guid>(nullable: false),
                    Notes = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B2B_ConnectionContacts", x => new { x.ConnectionId, x.Id });
                    table.ForeignKey(
                        name: "FK_B2B_ConnectionContacts_B2B_Connections_ConnectionId",
                        column: x => x.ConnectionId,
                        principalTable: "B2B_Connections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "B2B_ConnectionContactEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContactId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ConnectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B2B_ConnectionContactEmails", x => new { x.ContactId, x.Id });
                    table.ForeignKey(
                        name: "FK_B2B_ConnectionContactEmails_B2B_ConnectionContacts_ConnectionId_ContactId",
                        columns: x => new { x.ConnectionId, x.ContactId },
                        principalTable: "B2B_ConnectionContacts",
                        principalColumns: new[] { "ConnectionId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "B2B_ConnectionContactPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContactId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Timestamp = table.Column<byte[]>(nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    CountryCode = table.Column<string>(maxLength: 8, nullable: true),
                    Number = table.Column<string>(maxLength: 32, nullable: true),
                    AreaCode = table.Column<string>(maxLength: 8, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ConnectionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B2B_ConnectionContactPhoneNumbers", x => new { x.ContactId, x.Id });
                    table.ForeignKey(
                        name: "FK_B2B_ConnectionContactPhoneNumbers_B2B_ConnectionContacts_ConnectionId_ContactId",
                        columns: x => new { x.ConnectionId, x.ContactId },
                        principalTable: "B2B_ConnectionContacts",
                        principalColumns: new[] { "ConnectionId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_B2B_ConnectionContactEmails_ConnectionId_ContactId",
                table: "B2B_ConnectionContactEmails",
                columns: new[] { "ConnectionId", "ContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_B2B_ConnectionContactPhoneNumbers_ConnectionId_ContactId",
                table: "B2B_ConnectionContactPhoneNumbers",
                columns: new[] { "ConnectionId", "ContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_B2B_Connections_LinkedOrganisationId",
                table: "B2B_Connections",
                column: "LinkedOrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_B2B_Connections_OrganisationId",
                table: "B2B_Connections",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_B2B_Connections_OrganisationID",
                table: "B2B_Connections",
                column: "OrgansationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B2B_ConnectionContactEmails");

            migrationBuilder.DropTable(
                name: "B2B_ConnectionContactPhoneNumbers");

            migrationBuilder.DropTable(
                name: "B2B_ConnectionContacts");

            migrationBuilder.DropTable(
                name: "B2B_Connections");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "CargoItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
