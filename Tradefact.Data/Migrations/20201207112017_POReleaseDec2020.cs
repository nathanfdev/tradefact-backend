using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Tradefact.Data.Migrations
{
    public partial class POReleaseDec2020 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "collab");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 4, nullable: false)
                        .Annotation("ColumnOrder", 1),
                    Description = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 2),
                    ISOTypeGroup = table.Column<string>(maxLength: 4, nullable: true)
                        .Annotation("ColumnOrder", 3),
                    ISOTypeGroupDescription = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 4),
                    Length = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 5),
                    Height = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 7),
                    Width = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 6),
                    AdditionalInformation = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 8),
                    Active = table.Column<bool>(nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 9)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code2 = table.Column<string>(maxLength: 4, nullable: false)
                        .Annotation("ColumnOrder", 1),
                    Name = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 3),
                    Code3 = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code2);
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 1)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyName = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 3),
                    CurrencyCode = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 2),
                    Description = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 4),
                    Active = table.Column<bool>(nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 5)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    CompanyLegalName = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 4),
                    Country = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 5),
                    Address = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 6),
                    City = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 7),
                    WebAddress = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 8),
                    BusinessId = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 9),
                    BusinessRegDocumentUrl = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 11),
                    BusinessRegDocumentName = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 10),
                    CompanyBio = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 13),
                    OrganisationId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 14),
                    RegistrationStatus = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 12)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationType",
                columns: table => new
                {
                    OrganisationTypeId = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    Name = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationType", x => x.OrganisationTypeId);
                });

            migrationBuilder.CreateTable(
                name: "PartnershipTypes",
                columns: table => new
                {
                    PartnershipTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProviderTypeId = table.Column<int>(nullable: false),
                    ClientTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnershipTypes", x => x.PartnershipTypeId);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseType",
                columns: table => new
                {
                    PurchaseTypeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseTypeName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseType", x => x.PurchaseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "QueuedTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Status = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 10),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 11),
                    Payload = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 20),
                    Result = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 21)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueuedTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuotationChargeType",
                columns: table => new
                {
                    QuotationChargeTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationChargeType", x => x.QuotationChargeTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentType",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                schema: "collab",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    IsOpen = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 22),
                    IsAlert = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 23),
                    UnreadCount = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 24),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 21),
                    Type = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 20)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocCode = table.Column<string>(maxLength: 8, nullable: false)
                        .Annotation("ColumnOrder", 1),
                    Name = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 3),
                    IATA = table.Column<string>(maxLength: 4, nullable: true)
                        .Annotation("ColumnOrder", 2),
                    Position_Latitude = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 9),
                    Position_Longitude = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 10),
                    Port = table.Column<bool>(nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 5),
                    Rail = table.Column<bool>(nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 6),
                    Road = table.Column<bool>(nullable: false),
                    Airport = table.Column<bool>(nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 7),
                    CountryCode = table.Column<string>(maxLength: 4, nullable: true)
                        .Annotation("ColumnOrder", 4),
                    IsGeneric = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 8)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.LocCode);
                    table.ForeignKey(
                        name: "FK_Locations_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code2",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRegistrationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    OrganisationRegistrationId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 4),
                    Email = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 5),
                    Role = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 6),
                    IsAdmin = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 7)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRegistrationUsers", x => new { x.OrganisationRegistrationId, x.Id });
                    table.ForeignKey(
                        name: "FK_OrganisationRegistrationUsers_OrganisationRegistrations_OrganisationRegistrationId",
                        column: x => x.OrganisationRegistrationId,
                        principalTable: "OrganisationRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 11),
                    ContactEmail = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 13),
                    ContactName = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 12),
                    ContactTelephone = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 14),
                    PaymentTerms = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 16),
                    TCs = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 40),
                    ParentId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 3),
                    Bank_BankIdentifierCode = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 30),
                    Bank_IBAN = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 31),
                    Bank_SortCode = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 32),
                    Bank_RoutingNo = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 33),
                    Bank_AccountNo = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 34),
                    Bank_IFSC = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 36),
                    Bank_BSB = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 37),
                    Bank_AccountName = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 35),
                    TaxId = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 15),
                    OrganisationTypeId = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    PlanInvitesAvailable = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 17),
                    InvitesIssued = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 18),
                    InvitesActioned = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 19)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisations_OrganisationType_OrganisationTypeId",
                        column: x => x.OrganisationTypeId,
                        principalTable: "OrganisationType",
                        principalColumn: "OrganisationTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Organisations_Organisations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carriers",
                columns: table => new
                {
                    SCAC = table.Column<string>(maxLength: 4, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Description = table.Column<string>(maxLength: 128, nullable: true),
                    Company = table.Column<string>(maxLength: 128, nullable: true),
                    CarrierGroup = table.Column<string>(maxLength: 128, nullable: true),
                    Regions = table.Column<string>(maxLength: 128, nullable: true),
                    Active = table.Column<bool>(nullable: false, defaultValue: true),
                    Tracking_Enabled = table.Column<bool>(nullable: true, defaultValue: false),
                    Tracking_OperatorValue = table.Column<string>(maxLength: 16, nullable: true),
                    Tracking_BillOfLading = table.Column<bool>(nullable: true, defaultValue: false),
                    Tracking_Container = table.Column<bool>(nullable: true, defaultValue: false),
                    ShipmentTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carriers", x => x.SCAC);
                    table.ForeignKey(
                        name: "FK_Carriers_ShipmentType_ShipmentTypeId",
                        column: x => x.ShipmentTypeId,
                        principalTable: "ShipmentType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                schema: "collab",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    RoomId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Comment = table.Column<string>(nullable: true),
                    IsPinned = table.Column<bool>(nullable: false),
                    PinnedAt = table.Column<DateTime>(nullable: true),
                    PostedBy_Avatar = table.Column<string>(maxLength: 256, nullable: true),
                    PostedBy_DisplayName = table.Column<string>(maxLength: 128, nullable: true),
                    Type = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => new { x.RoomId, x.Id });
                    table.ForeignKey(
                        name: "FK_Messages_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalSchema: "collab",
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    OrganisationId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Type = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 3),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 20),
                    AddressLine1 = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 21),
                    AddressLine2 = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 22),
                    AddressLine3 = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 23),
                    AddressLine4 = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 24),
                    City = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 25),
                    Province = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 27),
                    County = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 26),
                    IsDefault = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 4),
                    IsInvoiceAddress = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 5),
                    PostalCode = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 28),
                    CountryCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Countries_CountryCode",
                        column: x => x.CountryCode,
                        principalTable: "Countries",
                        principalColumn: "Code2",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    OrganisationId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Timestamp = table.Column<byte[]>(nullable: true)
                        .Annotation("ColumnOrder", 1005),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 999),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 998),
                    FullName = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 31),
                    FirstName = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 32),
                    MiddleName = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 33),
                    LastName = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 34),
                    Title = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 35),
                    Salutation = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 36),
                    Department = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 37),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 3),
                    LocationId = table.Column<Guid>(nullable: false),
                    Notes = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 38)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => new { x.OrganisationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Contacts_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    BlobUrl = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 23),
                    Name = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 20),
                    Extension = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 21),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 22),
                    DateUploaded = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 30),
                    CompanyId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 24),
                    IsRichText = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 26),
                    RichTextData = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 27)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Organisations_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Timestamp = table.Column<byte[]>(nullable: true)
                        .Annotation("ColumnOrder", 1005),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 999),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 998),
                    Note = table.Column<string>(nullable: true),
                    ParentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationNotes", x => new { x.OrganisationId, x.Id });
                    table.ForeignKey(
                        name: "FK_OrganisationNotes_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganisationNotes_Organisations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Partnerships",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    ProviderId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    ClientId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 3),
                    PartnershipTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partnerships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partnerships_Organisations_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partnerships_PartnershipTypes_PartnershipTypeId",
                        column: x => x.PartnershipTypeId,
                        principalTable: "PartnershipTypes",
                        principalColumn: "PartnershipTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Partnerships_Organisations_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FreightMovements",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    CompanyId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 64, nullable: true)
                        .Annotation("ColumnOrder", 20),
                    Reference = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 21),
                    ShipmentType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 11),
                    IncoTerms = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 12),
                    LoadType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 13),
                    TransactionType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 14),
                    PlaceOfLoadingId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 30),
                    PortOfLoadingId = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 31),
                    PortOfDischargeId = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 32),
                    PlaceOfDispatchId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 33),
                    GoodsReady = table.Column<DateTime>(nullable: false)
                        .Annotation("ColumnOrder", 15),
                    DeliveryDate = table.Column<DateTime>(nullable: true),
                    NumberOfItems = table.Column<int>(nullable: true)
                        .Annotation("ColumnOrder", 70),
                    Hazard = table.Column<bool>(nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 71),
                    InsuranceRequired = table.Column<bool>(nullable: true)
                        .Annotation("ColumnOrder", 50),
                    CustomsBrokerageRequired = table.Column<bool>(nullable: true)
                        .Annotation("ColumnOrder", 40),
                    InsuranceCurrency = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 51),
                    InsuranceValue = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 52),
                    Tags = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 100),
                    Notes = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 102),
                    HSCodes = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 101),
                    SupplierId = table.Column<Guid>(maxLength: 60, nullable: true),
                    BuyerId = table.Column<Guid>(maxLength: 61, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FreightMovements_Organisations_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreightMovements_Addresses_PlaceOfDispatchId",
                        column: x => x.PlaceOfDispatchId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreightMovements_Addresses_PlaceOfLoadingId",
                        column: x => x.PlaceOfLoadingId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreightMovements_Locations_PortOfDischargeId",
                        column: x => x.PortOfDischargeId,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreightMovements_Locations_PortOfLoadingId",
                        column: x => x.PortOfLoadingId,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContactEmails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    ContactId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Timestamp = table.Column<byte[]>(nullable: true)
                        .Annotation("ColumnOrder", 1005),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 999),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 998),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactEmails", x => new { x.ContactId, x.Id });
                    table.ForeignKey(
                        name: "FK_ContactEmails_Contacts_OrganisationId_ContactId",
                        columns: x => new { x.OrganisationId, x.ContactId },
                        principalTable: "Contacts",
                        principalColumns: new[] { "OrganisationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactPhoneNumbers",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    ContactId = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Timestamp = table.Column<byte[]>(nullable: true)
                        .Annotation("ColumnOrder", 1005),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 999),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 998),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    CountryCode = table.Column<string>(maxLength: 8, nullable: true),
                    Number = table.Column<string>(maxLength: 32, nullable: true),
                    AreaCode = table.Column<string>(maxLength: 8, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactPhoneNumbers", x => new { x.ContactId, x.Id });
                    table.ForeignKey(
                        name: "FK_ContactPhoneNumbers_Contacts_OrganisationId_ContactId",
                        columns: x => new { x.OrganisationId, x.ContactId },
                        principalTable: "Contacts",
                        principalColumns: new[] { "OrganisationId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ExternalProviderId = table.Column<long>(nullable: true),
                    FullName = table.Column<string>(maxLength: 128, nullable: true),
                    GivenName = table.Column<string>(maxLength: 64, nullable: true),
                    Surname = table.Column<string>(maxLength: 64, nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    ProfileImageId = table.Column<Guid>(nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: true),
                    ExternalProverUUID = table.Column<Guid>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Addresses_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Documents_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    Description = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 102),
                    Dimensions_Height = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 142),
                    Dimensions_Length = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 140),
                    Dimensions_Scale = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 145),
                    Dimensions_Width = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 141),
                    Dimensions_Weight = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 143),
                    Dimensions_weightMeasurement = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 144),
                    GoodsType = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 111),
                    HsCode = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 110),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 100),
                    Nickname = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 112),
                    Packing = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 113),
                    UnitsPerPackage = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 122),
                    SKU = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 101),
                    HazardousContents = table.Column<int>(maxLength: 50, nullable: false)
                        .Annotation("ColumnOrder", 132),
                    HazardClass = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 130),
                    HazardDescription = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 131),
                    HazardNotes = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 133),
                    HazardDocumentId = table.Column<Guid>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 134),
                    Reference = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 103),
                    LithiumBatteryPacking = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 135),
                    MagneticFieldContained = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 136),
                    Rotatable = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 121),
                    Stackable = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 120),
                    CompanyId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 90)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Organisations_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Documents_HazardDocumentId",
                        column: x => x.HazardDocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FreightMovementItems",
                columns: table => new
                {
                    FreightMovementId = table.Column<Guid>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    ContainerTypeCode = table.Column<string>(maxLength: 4, nullable: true),
                    CartonQty = table.Column<int>(nullable: false),
                    HazardCode = table.Column<string>(maxLength: 4, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FreightMovementItems", x => new { x.FreightMovementId, x.Id });
                    table.ForeignKey(
                        name: "FK_FreightMovementItems_ContainerTypes_ContainerTypeCode",
                        column: x => x.ContainerTypeCode,
                        principalTable: "ContainerTypes",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FreightMovementItems_FreightMovements_FreightMovementId",
                        column: x => x.FreightMovementId,
                        principalTable: "FreightMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    PartnershipId = table.Column<Guid>(nullable: false),
                    FreightMovementId = table.Column<Guid>(nullable: false),
                    Submitted = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 100),
                    Actioned = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 101),
                    State = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 102)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationRequests_FreightMovements_FreightMovementId",
                        column: x => x.FreightMovementId,
                        principalTable: "FreightMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationRequests_Partnerships_PartnershipId",
                        column: x => x.PartnershipId,
                        principalTable: "Partnerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    UserId = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 250),
                    GivenName = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 250),
                    EmailAddress = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 250),
                    InviteType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserInvitations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDocuments",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDocuments", x => new { x.ProductId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_ProductDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductDocuments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    ProductId = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 102),
                    Dimensions_Height = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 142),
                    Dimensions_Length = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 140),
                    Dimensions_Scale = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 145),
                    Dimensions_Width = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 141),
                    Dimensions_Weight = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 143),
                    Dimensions_weightMeasurement = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 144),
                    GoodsType = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 111),
                    HsCode = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 110),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 100),
                    Nickname = table.Column<string>(maxLength: 250, nullable: true)
                        .Annotation("ColumnOrder", 112),
                    Packing = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 113),
                    UnitsPerPackage = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 122),
                    SKU = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 101),
                    HazardousContents = table.Column<int>(maxLength: 50, nullable: false)
                        .Annotation("ColumnOrder", 132),
                    HazardClass = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 130),
                    HazardDescription = table.Column<string>(maxLength: 50, nullable: true)
                        .Annotation("ColumnOrder", 131),
                    HazardNotes = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 133),
                    HazardDocumentId = table.Column<Guid>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 134),
                    Reference = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 103),
                    MagneticFieldContained = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 136),
                    SupplierId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 90)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Documents_HazardDocumentId",
                        column: x => x.HazardDocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Organisations_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    IssueDate = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 14),
                    ExpiryDate = table.Column<DateTimeOffset>(nullable: false)
                        .Annotation("ColumnOrder", 15),
                    QuoteNumber = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 13),
                    ContactName = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 20),
                    ContactReference = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 21),
                    Reference = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 12),
                    TotalQuantity = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 260),
                    CurrencyId = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 190),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 191),
                    InverseExchangeRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 192),
                    BaseCurrencyTotalDiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 260),
                    Total_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 200),
                    Total_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 204),
                    Total_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 201),
                    Total_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 202),
                    Total_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 203),
                    BaseCurrency_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 250),
                    BaseCurrency_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 254),
                    BaseCurrency_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 251),
                    BaseCurrency_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 252),
                    BaseCurrency_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 253),
                    QuoteStatus = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 16),
                    Sent = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 22),
                    SentByEmail = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 23),
                    Booked = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 11),
                    ShipmentId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 3),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 299),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 302),
                    DetailedTermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 303),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 301),
                    FreightMovementId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 4),
                    QuotationRequestId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Routes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 300)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quotations_FreightMovements_FreightMovementId",
                        column: x => x.FreightMovementId,
                        principalTable: "FreightMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotations_QuotationRequests_QuotationRequestId",
                        column: x => x.QuotationRequestId,
                        principalTable: "QuotationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CargoItems",
                columns: table => new
                {
                    FreightMovementId = table.Column<Guid>(nullable: false),
                    FreightMovementItemId = table.Column<Guid>(nullable: false),
                    ItemId = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    ProductVariantId = table.Column<Guid>(nullable: true),
                    IsProductVariant = table.Column<bool>(nullable: false),
                    HsCode = table.Column<string>(maxLength: 32, nullable: true),
                    SKU = table.Column<string>(maxLength: 64, nullable: true),
                    ItemDescription = table.Column<string>(maxLength: 256, nullable: true),
                    Qty = table.Column<int>(nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    UOL = table.Column<string>(maxLength: 16, nullable: true),
                    CartonQty = table.Column<int>(nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18, 4)", nullable: false),
                    UOW = table.Column<string>(maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoItems", x => new { x.FreightMovementId, x.FreightMovementItemId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_CargoItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoItems_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoItems_FreightMovementItems_FreightMovementId_FreightMovementItemId",
                        columns: x => new { x.FreightMovementId, x.FreightMovementItemId },
                        principalTable: "FreightMovementItems",
                        principalColumns: new[] { "FreightMovementId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QuotationAvailableSchedules",
                columns: table => new
                {
                    QuotationId = table.Column<Guid>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarrierCode = table.Column<int>(nullable: false),
                    CarrierName = table.Column<int>(nullable: false),
                    TransitId = table.Column<string>(nullable: true),
                    DepartureTime = table.Column<DateTimeOffset>(nullable: false),
                    ArrivalTime = table.Column<DateTimeOffset>(nullable: false),
                    SCAC = table.Column<string>(nullable: true),
                    Route_Code = table.Column<string>(maxLength: 6, nullable: true),
                    Route_Name = table.Column<string>(maxLength: 50, nullable: true),
                    Vessel_IMO = table.Column<string>(maxLength: 6, nullable: true),
                    Vessel_MMSI = table.Column<string>(nullable: true),
                    Vessel_Name = table.Column<string>(maxLength: 50, nullable: true),
                    POD_Code = table.Column<string>(maxLength: 6, nullable: true),
                    POD_Name = table.Column<string>(maxLength: 50, nullable: true),
                    POL_Code = table.Column<string>(maxLength: 6, nullable: true),
                    POL_Name = table.Column<string>(maxLength: 50, nullable: true),
                    ReferenceCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationAvailableSchedules", x => new { x.QuotationId, x.Id });
                    table.ForeignKey(
                        name: "FK_QuotationAvailableSchedules_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationChargeItems",
                columns: table => new
                {
                    QuotationId = table.Column<Guid>(nullable: false),
                    LineId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Seq = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 3),
                    ServiceId = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 4),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 5),
                    Quantity = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 200),
                    UnitPrice = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 201),
                    UnitPriceIncludesTax = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 202),
                    TaxRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 203),
                    Margin = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 204),
                    Total_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 250),
                    Total_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 254),
                    Total_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 251),
                    Total_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 252),
                    Total_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 253),
                    BaseCurrency_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 200),
                    BaseCurrency_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 204),
                    BaseCurrency_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 201),
                    BaseCurrency_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 202),
                    BaseCurrency_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 203),
                    QuotationChargeTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationChargeItems", x => new { x.QuotationId, x.LineId });
                    table.ForeignKey(
                        name: "FK_QuotationChargeItems_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationChargeItems_Quotations_QuotationId1",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationChargeItems_Quotations_QuotationId2",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QuotationChargeItems_Quotations_QuotationId3",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    PartnershipId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    QuotationRequestId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 4),
                    FreightMovementId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 3),
                    ShipmentName = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 15),
                    SCAC = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 50),
                    BillofLadingNumber = table.Column<string>(maxLength: 36, nullable: true)
                        .Annotation("ColumnOrder", 53),
                    IMO = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 51),
                    VesselName = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 52),
                    ShipmentType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 11),
                    IncoTerms = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 12),
                    LoadType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 13),
                    Status = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 14),
                    Stage = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 120),
                    PlaceOfLoadingId = table.Column<Guid>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 30),
                    PortOfLoadingId = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 70),
                    PortOfDischargeId = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 81),
                    PlaceOfDispatchId = table.Column<Guid>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 84),
                    Latitude = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 121),
                    Longitude = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 122),
                    ETA = table.Column<DateTime>(nullable: false)
                        .Annotation("ColumnOrder", 80),
                    ETD = table.Column<DateTime>(nullable: false)
                        .Annotation("ColumnOrder", 73),
                    Tags = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 130),
                    Notes = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 131),
                    Booked = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 20),
                    BookedDate = table.Column<DateTime>(nullable: false)
                        .Annotation("ColumnOrder", 21),
                    Collected = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 32),
                    EstimatedCollectionDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 31),
                    CollectionDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 33),
                    InTransit = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 40),
                    InTransitDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 41),
                    EquipmentTrackAvailable = table.Column<bool>(nullable: false),
                    ShipmentTrackAvailable = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 61),
                    TrackinformationAdded = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 60),
                    ArrivedPOD = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 82),
                    ArrivedPODDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 83),
                    DepartedPOL = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 71),
                    DepartedPOLDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 72),
                    InCustoms = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 90),
                    IssueAtCustoms = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 91),
                    IssueAtCustomsDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 92),
                    IssueAtCustomCleared = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 93),
                    CustomsClearence = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 94),
                    CustomsClearenceDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 95),
                    Delivered = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 101),
                    EstimatedDeliveryDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 100),
                    DeliveryDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 102),
                    IsRescheduled = table.Column<bool>(nullable: false)
                        .Annotation("ColumnOrder", 110),
                    Rescheduled = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 111),
                    LastRescheduleTime = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 112),
                    Route = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 132),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    QuotationId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shipments_FreightMovements_FreightMovementId",
                        column: x => x.FreightMovementId,
                        principalTable: "FreightMovements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Partnerships_PartnershipId",
                        column: x => x.PartnershipId,
                        principalTable: "Partnerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Addresses_PlaceOfDispatchId",
                        column: x => x.PlaceOfDispatchId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Addresses_PlaceOfLoadingId",
                        column: x => x.PlaceOfLoadingId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Locations_PortOfDischargeId",
                        column: x => x.PortOfDischargeId,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Locations_PortOfLoadingId",
                        column: x => x.PortOfLoadingId,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Shipments_QuotationRequests_QuotationRequestId",
                        column: x => x.QuotationRequestId,
                        principalTable: "QuotationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EquipmentAllocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ShipmentId = table.Column<Guid>(nullable: false),
                    ETag = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    UpsertDate = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(nullable: true),
                    ContainerNo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipmentAllocations", x => new { x.ShipmentId, x.Id });
                    table.ForeignKey(
                        name: "FK_EquipmentAllocations_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    CompanyId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 5),
                    SupplierId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 7),
                    PurchaseOrderNumber = table.Column<string>(maxLength: 64, nullable: true)
                        .Annotation("ColumnOrder", 6),
                    Reference = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 14),
                    PurchaseOrderDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 11),
                    GoodsReadyDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 12),
                    TargetDeliveryDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 13),
                    IncotermsVersion = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 15),
                    Stage = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 16),
                    ShipmentType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 20),
                    IncoTerms = table.Column<int>(maxLength: 8, nullable: false)
                        .Annotation("ColumnOrder", 24),
                    LoadType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 22),
                    TransactionType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 23),
                    PlaceOfLoadingId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 30),
                    PortOfLoadingId = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 31),
                    PortOfDischargeId = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 32),
                    PlaceOfDispatchId = table.Column<Guid>(nullable: true)
                        .Annotation("ColumnOrder", 33),
                    Language = table.Column<string>(maxLength: 4, nullable: true)
                        .Annotation("ColumnOrder", 40),
                    PurchasingGroup = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 41),
                    CorrespncExternalReference = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 42),
                    CorrespncInternalReference = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 43),
                    Tags = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 350),
                    Containers = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 351),
                    CurrencyId = table.Column<string>(maxLength: 16, nullable: true)
                        .Annotation("ColumnOrder", 100),
                    ExchangeRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 101),
                    InverseExchangeRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 102),
                    ValidForDays = table.Column<int>(nullable: false),
                    DateOfIssue = table.Column<DateTime>(nullable: true),
                    PaymentTerms = table.Column<string>(type: "nvarchar(max)", nullable: true)
                        .Annotation("ColumnOrder", 300),
                    CustomsBrokerageRequired = table.Column<bool>(nullable: true)
                        .Annotation("ColumnOrder", 110),
                    NumberOfItems = table.Column<int>(nullable: true)
                        .Annotation("ColumnOrder", 290),
                    HSCodes = table.Column<string>(maxLength: 512, nullable: true)
                        .Annotation("ColumnOrder", 101),
                    InsuranceRequired = table.Column<bool>(nullable: true)
                        .Annotation("ColumnOrder", 120),
                    InsuranceCurrency = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 121),
                    InsuranceValue = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 122),
                    Notes = table.Column<string>(nullable: true),
                    Signature = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 360),
                    AdditionalInformation = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 361),
                    TaxRate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 260),
                    TaxRateDescription = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 261),
                    ItemsTotal_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 180),
                    ItemsTotal_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 184),
                    ItemsTotal_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 181),
                    ItemsTotal_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 182),
                    ItemsTotal_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 183),
                    ChargesTotal_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 190),
                    ChargesTotal_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 194),
                    ChargesTotal_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 191),
                    ChargesTotal_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 192),
                    ChargesTotal_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 193),
                    Total_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 200),
                    Total_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 204),
                    Total_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 201),
                    Total_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 202),
                    Total_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 203),
                    BaseCurrency_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 250),
                    BaseCurrency_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 254),
                    BaseCurrency_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 251),
                    BaseCurrency_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 252),
                    BaseCurrency_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 253),
                    Submitted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 80),
                    SubmittedDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 81),
                    Accepted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 82),
                    AcceptedDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 83),
                    InProduction = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 86),
                    InProductionDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 87),
                    PreShipment = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 88),
                    PreShipmentDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 89),
                    Shipping = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 90),
                    ShippedDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 91),
                    Rejected = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 84),
                    RejectedDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 85),
                    Cancelled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 92),
                    CancelledDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 93),
                    Completed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 94),
                    CompletedDate = table.Column<DateTime>(nullable: true)
                        .Annotation("ColumnOrder", 95),
                    LogisticsNotes = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 362),
                    AdditionalSupplierInformation = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 363),
                    ShipmentId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Addresses_PlaceOfDispatchId",
                        column: x => x.PlaceOfDispatchId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Addresses_PlaceOfLoadingId",
                        column: x => x.PlaceOfLoadingId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Locations_PortOfDischargeId",
                        column: x => x.PortOfDischargeId,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Locations_PortOfLoadingId",
                        column: x => x.PortOfLoadingId,
                        principalTable: "Locations",
                        principalColumn: "LocCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDocuments",
                columns: table => new
                {
                    ShipmentId = table.Column<Guid>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDocuments", x => new { x.ShipmentId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_ShipmentDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShipmentDocuments_Shipments_ShipmentId",
                        column: x => x.ShipmentId,
                        principalTable: "Shipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAdditionalCharges",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PurchaseOrderId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 0),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedByUser = table.Column<string>(nullable: true),
                    CreationDateInternal = table.Column<DateTime>(nullable: false),
                    LastChangeUser = table.Column<string>(nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(nullable: false),
                    Timestamp = table.Column<byte[]>(nullable: true),
                    LastModifiedOn = table.Column<DateTimeOffset>(nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(nullable: false),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 5),
                    Type = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 4),
                    Quantity = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 200),
                    Rate = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 201),
                    Total_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 250),
                    Total_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 254),
                    Total_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 251),
                    Total_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 252),
                    Total_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 253),
                    BaseCurrencyTotal_CurrencyId = table.Column<string>(maxLength: 12, nullable: true)
                        .Annotation("ColumnOrder", 200),
                    BaseCurrencyTotal_DiscountAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 204),
                    BaseCurrencyTotal_NetAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 201),
                    BaseCurrencyTotal_TaxAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 202),
                    BaseCurrencyTotal_TotalAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: true)
                        .Annotation("ColumnOrder", 203)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAdditionalCharges", x => new { x.PurchaseOrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAdditionalCharges_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderAttachedProductDocuments",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<Guid>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    PurchaseOrderProductId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderAttachedProductDocuments", x => new { x.PurchaseOrderId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAttachedProductDocuments_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderAttachedProductDocuments_ProductDocuments_PurchaseOrderProductId_DocumentId",
                        columns: x => new { x.PurchaseOrderProductId, x.DocumentId },
                        principalTable: "ProductDocuments",
                        principalColumns: new[] { "ProductId", "DocumentId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDocuments",
                columns: table => new
                {
                    PurchaseOrderId = table.Column<Guid>(nullable: false),
                    DocumentId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 100)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDocuments", x => new { x.PurchaseOrderId, x.DocumentId });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDocuments_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderEvents",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 0)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventType = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 20),
                    Description = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 21),
                    ActionedBy = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 30),
                    ActionDate = table.Column<DateTime>(nullable: false)
                        .Annotation("ColumnOrder", 31),
                    PurchaseOrderId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderEvents", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderEvents_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    PurchaseOrderId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 0),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    ProductId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 20),
                    SKU = table.Column<string>(maxLength: 64, nullable: true)
                        .Annotation("ColumnOrder", 23),
                    PurchaseOrderItemText = table.Column<string>(maxLength: 256, nullable: true)
                        .Annotation("ColumnOrder", 30),
                    OrderQuantity = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 40),
                    OrderQuantityUnit = table.Column<string>(maxLength: 32, nullable: true)
                        .Annotation("ColumnOrder", 41),
                    OrderPriceUnit = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 42),
                    NetPriceAmount = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 43),
                    NetPriceQuantity = table.Column<int>(nullable: false)
                        .Annotation("ColumnOrder", 44),
                    TaxCode = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 50),
                    TaxDeterminationDate = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 53),
                    TaxCountry = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 51),
                    TaxJurisdiction = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 52),
                    IsDeliveryComplete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 60),
                    IsFinallyInvoiced = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                        .Annotation("ColumnOrder", 61),
                    PurchaseOrderItemCategory = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 70),
                    AccountAssignmentCategory = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 71),
                    PurchaseContract = table.Column<string>(maxLength: 64, nullable: true)
                        .Annotation("ColumnOrder", 72),
                    ItemNetWeight = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 80),
                    ItemWeightUnit = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 81),
                    ItemVolume = table.Column<decimal>(type: "decimal(18, 4)", nullable: false)
                        .Annotation("ColumnOrder", 90),
                    ItemVolumeUnit = table.Column<string>(maxLength: 8, nullable: true)
                        .Annotation("ColumnOrder", 91)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => new { x.PurchaseOrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    PurchaseOrderId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 0),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    TextObjectType = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 30),
                    Language = table.Column<string>(maxLength: 4, nullable: true)
                        .Annotation("ColumnOrder", 31),
                    PlainLongText = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 32)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderNotes", x => new { x.PurchaseOrderId, x.Id });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderNotes_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItemNotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 2),
                    PurchaseOrderId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 0),
                    PurchaseOrderItemId = table.Column<Guid>(nullable: false)
                        .Annotation("ColumnOrder", 1),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                        .Annotation("ColumnOrder", 10),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1001),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1002),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true)
                        .Annotation("ColumnOrder", 1003),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false)
                        .Annotation("ColumnOrder", 1004),
                    TextObjectType = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 30),
                    Language = table.Column<string>(maxLength: 4, nullable: true)
                        .Annotation("ColumnOrder", 31),
                    PlainLongText = table.Column<string>(nullable: true)
                        .Annotation("ColumnOrder", 32)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItemNotes", x => new { x.PurchaseOrderId, x.PurchaseOrderItemId, x.Id });
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItemNotes_PurchaseOrderItems_PurchaseOrderId_Id",
                        columns: x => new { x.PurchaseOrderId, x.Id },
                        principalTable: "PurchaseOrderItems",
                        principalColumns: new[] { "PurchaseOrderId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "OrganisationType",
                columns: new[] { "OrganisationTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "SHIPPER" },
                    { 2, "SUPPLIER" },
                    { 3, "BUYER" }
                });

            migrationBuilder.InsertData(
                table: "QuotationChargeType",
                columns: new[] { "QuotationChargeTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "FREIGHT" },
                    { 2, "ORIGIN" },
                    { 3, "DESTINATION" },
                    { 4, "ADDITIONAL" }
                });

            migrationBuilder.InsertData(
                table: "ShipmentType",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 2, "AIR" },
                    { 1, "SEA" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CountryCode",
                table: "Addresses",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_OrganisationId",
                table: "Addresses",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserInvitations_UserId",
                table: "AspNetUserInvitations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LocationId",
                table: "AspNetUsers",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganisationId",
                table: "AspNetUsers",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProfileImageId",
                table: "AspNetUsers",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoItems_ProductId",
                table: "CargoItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoItems_ProductVariantId",
                table: "CargoItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Carriers_ShipmentTypeId",
                table: "Carriers",
                column: "ShipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactEmails_OrganisationId_ContactId",
                table: "ContactEmails",
                columns: new[] { "OrganisationId", "ContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_ContactPhoneNumbers_OrganisationId_ContactId",
                table: "ContactPhoneNumbers",
                columns: new[] { "OrganisationId", "ContactId" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompanyId",
                table: "Documents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightMovementItems_ContainerTypeCode",
                table: "FreightMovementItems",
                column: "ContainerTypeCode");

            migrationBuilder.CreateIndex(
                name: "IX_FreightMovements_CompanyId",
                table: "FreightMovements",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightMovements_PlaceOfDispatchId",
                table: "FreightMovements",
                column: "PlaceOfDispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightMovements_PlaceOfLoadingId",
                table: "FreightMovements",
                column: "PlaceOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightMovements_PortOfDischargeId",
                table: "FreightMovements",
                column: "PortOfDischargeId");

            migrationBuilder.CreateIndex(
                name: "IX_FreightMovements_PortOfLoadingId",
                table: "FreightMovements",
                column: "PortOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CountryCode",
                table: "Locations",
                column: "CountryCode");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationNotes_ParentId",
                table: "OrganisationNotes",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_OrganisationTypeId",
                table: "Organisations",
                column: "OrganisationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_ParentId",
                table: "Organisations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Partnerships_ClientId",
                table: "Partnerships",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Partnerships_PartnershipTypeId",
                table: "Partnerships",
                column: "PartnershipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Partnerships_ProviderId",
                table: "Partnerships",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDocuments_DocumentId",
                table: "ProductDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId",
                table: "Products",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_HazardDocumentId",
                table: "Products",
                column: "HazardDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_HazardDocumentId",
                table: "ProductVariants",
                column: "HazardDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductId",
                table: "ProductVariants",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_SupplierId",
                table: "ProductVariants",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderAttachedProductDocuments_PurchaseOrderProductId_DocumentId",
                table: "PurchaseOrderAttachedProductDocuments",
                columns: new[] { "PurchaseOrderProductId", "DocumentId" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDocuments_DocumentId",
                table: "PurchaseOrderDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderEvents_PurchaseOrderId",
                table: "PurchaseOrderEvents",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItemNotes_PurchaseOrderId_Id",
                table: "PurchaseOrderItemNotes",
                columns: new[] { "PurchaseOrderId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PlaceOfDispatchId",
                table: "PurchaseOrders",
                column: "PlaceOfDispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PlaceOfLoadingId",
                table: "PurchaseOrders",
                column: "PlaceOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PortOfDischargeId",
                table: "PurchaseOrders",
                column: "PortOfDischargeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_PortOfLoadingId",
                table: "PurchaseOrders",
                column: "PortOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_ShipmentId",
                table: "PurchaseOrders",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequests_FreightMovementId",
                table: "QuotationRequests",
                column: "FreightMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationRequests_PartnershipId",
                table: "QuotationRequests",
                column: "PartnershipId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_FreightMovementId",
                table: "Quotations",
                column: "FreightMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_QuotationRequestId",
                table: "Quotations",
                column: "QuotationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDocuments_DocumentId",
                table: "ShipmentDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_FreightMovementId",
                table: "Shipments",
                column: "FreightMovementId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_OrganisationId",
                table: "Shipments",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PlaceOfDispatchId",
                table: "Shipments",
                column: "PlaceOfDispatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PlaceOfLoadingId",
                table: "Shipments",
                column: "PlaceOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PortOfDischargeId",
                table: "Shipments",
                column: "PortOfDischargeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PortOfLoadingId",
                table: "Shipments",
                column: "PortOfLoadingId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_QuotationId",
                table: "Shipments",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_QuotationRequestId",
                table: "Shipments",
                column: "QuotationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Shipments_PartnershipId_Id",
                table: "Shipments",
                columns: new[] { "PartnershipId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserInvitations");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CargoItems");

            migrationBuilder.DropTable(
                name: "Carriers");

            migrationBuilder.DropTable(
                name: "ContactEmails");

            migrationBuilder.DropTable(
                name: "ContactPhoneNumbers");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "EquipmentAllocations");

            migrationBuilder.DropTable(
                name: "OrganisationNotes");

            migrationBuilder.DropTable(
                name: "OrganisationRegistrationUsers");

            migrationBuilder.DropTable(
                name: "PurchaseOrderAdditionalCharges");

            migrationBuilder.DropTable(
                name: "PurchaseOrderAttachedProductDocuments");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDocuments");

            migrationBuilder.DropTable(
                name: "PurchaseOrderEvents");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItemNotes");

            migrationBuilder.DropTable(
                name: "PurchaseOrderNotes");

            migrationBuilder.DropTable(
                name: "PurchaseType");

            migrationBuilder.DropTable(
                name: "QueuedTask");

            migrationBuilder.DropTable(
                name: "QuotationAvailableSchedules");

            migrationBuilder.DropTable(
                name: "QuotationChargeItems");

            migrationBuilder.DropTable(
                name: "QuotationChargeType");

            migrationBuilder.DropTable(
                name: "ShipmentDocuments");

            migrationBuilder.DropTable(
                name: "Messages",
                schema: "collab");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "FreightMovementItems");

            migrationBuilder.DropTable(
                name: "ShipmentType");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "OrganisationRegistrations");

            migrationBuilder.DropTable(
                name: "ProductDocuments");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "Rooms",
                schema: "collab");

            migrationBuilder.DropTable(
                name: "ContainerTypes");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropTable(
                name: "QuotationRequests");

            migrationBuilder.DropTable(
                name: "FreightMovements");

            migrationBuilder.DropTable(
                name: "Partnerships");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "PartnershipTypes");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "OrganisationType");
        }
    }
}
