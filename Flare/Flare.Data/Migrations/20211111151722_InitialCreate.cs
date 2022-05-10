using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Flare.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "AspNetUserInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    GivenName = table.Column<string>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    InviteType = table.Column<int>(nullable: false),
                    MemberOfOrganisationId = table.Column<Guid>(nullable: true),
                    InviteRequestedByUserId = table.Column<Guid>(nullable: true),
                    InviteRequestedByOrganisationId = table.Column<Guid>(nullable: true),
                    LastActivationAttempt = table.Column<DateTime>(nullable: true),
                    ActivationStatus = table.Column<int>(nullable: false),
                    SubscriptionPlan = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserInvitations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Code2 = table.Column<string>(maxLength: 4, nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    Code3 = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Code2);
                });

            migrationBuilder.CreateTable(
                name: "DeviceReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceId = table.Column<string>(unicode: false, maxLength: 32, nullable: false),
                    ResponseType = table.Column<string>(maxLength: 128, nullable: false),
                    ProtocolType = table.Column<string>(unicode: false, maxLength: 32, nullable: false),
                    MsgSeqNo = table.Column<int>(nullable: false),
                    GPS_Latitude = table.Column<double>(nullable: true),
                    GPS_Longitude = table.Column<double>(nullable: true),
                    GPS_Alarm = table.Column<string>(maxLength: 64, nullable: true),
                    GPS_Status = table.Column<string>(maxLength: 250, nullable: true),
                    GPS_IsPrecise = table.Column<bool>(nullable: true),
                    GPS_Altitude = table.Column<double>(nullable: true),
                    GPS_Speed = table.Column<double>(nullable: true),
                    GPS_Direction = table.Column<double>(nullable: true),
                    GPS_Time = table.Column<DateTime>(nullable: true),
                    GPS_RecvTime = table.Column<DateTime>(nullable: true),
                    GPS_UseLBSLocation = table.Column<bool>(nullable: true),
                    GPS_Address = table.Column<string>(maxLength: 500, nullable: true),
                    Temperature = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Humidity = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Battery = table.Column<int>(nullable: true),
                    CommandMsgSeqNo = table.Column<int>(nullable: false),
                    CommandId = table.Column<string>(maxLength: 250, nullable: true),
                    CommandExecuteResult = table.Column<string>(maxLength: 250, nullable: true),
                    ExtraInfo = table.Column<string>(unicode: false, nullable: true),
                    RawData = table.Column<string>(unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceReports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    ContactEmail = table.Column<string>(maxLength: 250, nullable: true),
                    ContactName = table.Column<string>(maxLength: 250, nullable: true),
                    ContactTelephone = table.Column<string>(maxLength: 50, nullable: true),
                    PaymentTerms = table.Column<int>(nullable: false),
                    TCs = table.Column<string>(nullable: true),
                    Currency = table.Column<string>(nullable: true, defaultValue: "USD"),
                    ParentId = table.Column<Guid>(nullable: true),
                    Bank_BankIdentifierCode = table.Column<string>(maxLength: 16, nullable: true),
                    Bank_IBAN = table.Column<string>(maxLength: 32, nullable: true),
                    Bank_SortCode = table.Column<string>(maxLength: 8, nullable: true),
                    Bank_RoutingNo = table.Column<string>(maxLength: 16, nullable: true),
                    Bank_AccountNo = table.Column<string>(maxLength: 32, nullable: true),
                    Bank_IFSC = table.Column<string>(maxLength: 32, nullable: true),
                    Bank_BSB = table.Column<string>(maxLength: 16, nullable: true),
                    Bank_AccountName = table.Column<string>(maxLength: 32, nullable: true),
                    TaxId = table.Column<string>(maxLength: 32, nullable: true),
                    OrganisationTypeId = table.Column<int>(nullable: false),
                    PlanInvitesAvailable = table.Column<int>(nullable: false),
                    InvitesIssued = table.Column<int>(nullable: false),
                    InvitesActioned = table.Column<int>(nullable: false),
                    ChargebeeSubscriptionId = table.Column<string>(nullable: true),
                    GenericSKUEnabled = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organisations_Organisations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceId = table.Column<string>(nullable: true),
                    DeviceType = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    IMEI = table.Column<string>(nullable: true),
                    SimProvider = table.Column<string>(nullable: true),
                    IMSI = table.Column<string>(nullable: true),
                    MSISDN = table.Column<string>(nullable: true),
                    HasFault = table.Column<bool>(nullable: false),
                    LastDeviceReportId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceReports_LastDeviceReportId",
                        column: x => x.LastDeviceReportId,
                        principalTable: "DeviceReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    AddressLine1 = table.Column<string>(maxLength: 250, nullable: true),
                    AddressLine2 = table.Column<string>(maxLength: 250, nullable: true),
                    AddressLine3 = table.Column<string>(maxLength: 250, nullable: true),
                    AddressLine4 = table.Column<string>(maxLength: 250, nullable: true),
                    City = table.Column<string>(maxLength: 250, nullable: true),
                    Province = table.Column<string>(maxLength: 250, nullable: true),
                    County = table.Column<string>(maxLength: 250, nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsInvoiceAddress = table.Column<bool>(nullable: false),
                    PostalCode = table.Column<string>(maxLength: 50, nullable: true),
                    CountryCode = table.Column<string>(nullable: true),
                    Position_Latitude = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    Position_Longitude = table.Column<decimal>(type: "decimal(18, 4)", nullable: true),
                    B2BConnectionId = table.Column<Guid>(nullable: true)
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
                    FullName = table.Column<string>(maxLength: 128, nullable: true),
                    GivenName = table.Column<string>(maxLength: 64, nullable: true),
                    Surname = table.Column<string>(maxLength: 64, nullable: true),
                    IsAdmin = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    LocationId = table.Column<Guid>(nullable: true),
                    ExternalProverUUID = table.Column<Guid>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InvitationId = table.Column<Guid>(nullable: true),
                    UserPreferences = table.Column<string>(nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ShipmentName = table.Column<string>(maxLength: 100, nullable: true),
                    ReferenceType = table.Column<string>(maxLength: 100, nullable: true),
                    ReferenceNumber = table.Column<string>(maxLength: 100, nullable: true),
                    TransportMode = table.Column<int>(nullable: false),
                    ShipmentDate = table.Column<DateTime>(nullable: false),
                    DeliveryAddressId = table.Column<Guid>(nullable: false),
                    DestinationAddressId = table.Column<Guid>(nullable: false),
                    OrderStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_DeliveryAddressId",
                        column: x => x.DeliveryAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_DestinationAddressId",
                        column: x => x.DestinationAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "OrderLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedByUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreationDateInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastChangeUser = table.Column<string>(maxLength: 128, nullable: true),
                    LastModifiedOnInternal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderId = table.Column<Guid>(nullable: false),
                    LineNumber = table.Column<int>(nullable: false),
                    LabelText = table.Column<string>(maxLength: 250, nullable: true),
                    DeviceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderLineItems_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderLineItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "IX_Devices_LastDeviceReportId",
                table: "Devices",
                column: "LastDeviceReportId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineItems_DeviceId",
                table: "OrderLineItems",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderLineItems_OrderId",
                table: "OrderLineItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryAddressId",
                table: "Orders",
                column: "DeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DestinationAddressId",
                table: "Orders",
                column: "DestinationAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_ParentId",
                table: "Organisations",
                column: "ParentId");
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
                name: "OrderLineItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "DeviceReports");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
