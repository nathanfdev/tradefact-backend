IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetUserInvitations] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [UserId] nvarchar(max) NULL,
    [CompanyName] nvarchar(max) NULL,
    [GivenName] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    [InviteType] int NOT NULL,
    [MemberOfOrganisationId] uniqueidentifier NULL,
    [InviteRequestedByUserId] uniqueidentifier NULL,
    [InviteRequestedByOrganisationId] uniqueidentifier NULL,
    [LastActivationAttempt] datetime2 NULL,
    [ActivationStatus] int NOT NULL,
    [SubscriptionPlan] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserInvitations] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Countries] (
    [Code2] nvarchar(4) NOT NULL,
    [Name] nvarchar(256) NULL,
    [Code3] nvarchar(8) NULL,
    CONSTRAINT [PK_Countries] PRIMARY KEY ([Code2])
);

GO

CREATE TABLE [DeviceReports] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [DeviceId] varchar(32) NOT NULL,
    [ResponseType] nvarchar(128) NOT NULL,
    [ProtocolType] varchar(32) NOT NULL,
    [MsgSeqNo] int NOT NULL,
    [GPS_Latitude] float NULL,
    [GPS_Longitude] float NULL,
    [GPS_Alarm] nvarchar(64) NULL,
    [GPS_Status] nvarchar(250) NULL,
    [GPS_IsPrecise] bit NULL,
    [GPS_Altitude] float NULL,
    [GPS_Speed] float NULL,
    [GPS_Direction] float NULL,
    [GPS_Time] datetime2 NULL,
    [GPS_RecvTime] datetime2 NULL,
    [GPS_UseLBSLocation] bit NULL,
    [GPS_Address] nvarchar(500) NULL,
    [Temperature] decimal(18, 4) NULL,
    [Humidity] decimal(18, 4) NULL,
    [Battery] int NULL,
    [CommandMsgSeqNo] int NOT NULL,
    [CommandId] nvarchar(250) NULL,
    [CommandExecuteResult] nvarchar(250) NULL,
    [ExtraInfo] varchar(max) NULL,
    [RawData] varchar(max) NULL,
    CONSTRAINT [PK_DeviceReports] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Organisations] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [Name] nvarchar(250) NULL,
    [ContactEmail] nvarchar(250) NULL,
    [ContactName] nvarchar(250) NULL,
    [ContactTelephone] nvarchar(50) NULL,
    [PaymentTerms] int NOT NULL,
    [TCs] nvarchar(max) NULL,
    [Currency] nvarchar(max) NULL DEFAULT N'USD',
    [ParentId] uniqueidentifier NULL,
    [Bank_BankIdentifierCode] nvarchar(16) NULL,
    [Bank_IBAN] nvarchar(32) NULL,
    [Bank_SortCode] nvarchar(8) NULL,
    [Bank_RoutingNo] nvarchar(16) NULL,
    [Bank_AccountNo] nvarchar(32) NULL,
    [Bank_IFSC] nvarchar(32) NULL,
    [Bank_BSB] nvarchar(16) NULL,
    [Bank_AccountName] nvarchar(32) NULL,
    [TaxId] nvarchar(32) NULL,
    [OrganisationTypeId] int NOT NULL,
    [PlanInvitesAvailable] int NOT NULL,
    [InvitesIssued] int NOT NULL,
    [InvitesActioned] int NOT NULL,
    [ChargebeeSubscriptionId] nvarchar(max) NULL,
    [GenericSKUEnabled] bit NULL,
    CONSTRAINT [PK_Organisations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Organisations_Organisations_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [Organisations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Devices] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [DeviceId] nvarchar(max) NULL,
    [DeviceType] nvarchar(max) NULL,
    [SerialNumber] nvarchar(max) NULL,
    [IMEI] nvarchar(max) NULL,
    [SimProvider] nvarchar(max) NULL,
    [IMSI] nvarchar(max) NULL,
    [MSISDN] nvarchar(max) NULL,
    [HasFault] bit NOT NULL,
    [LastDeviceReportId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Devices_DeviceReports_LastDeviceReportId] FOREIGN KEY ([LastDeviceReportId]) REFERENCES [DeviceReports] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Addresses] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [OrganisationId] uniqueidentifier NOT NULL,
    [Type] int NOT NULL,
    [Name] nvarchar(250) NULL,
    [AddressLine1] nvarchar(250) NULL,
    [AddressLine2] nvarchar(250) NULL,
    [AddressLine3] nvarchar(250) NULL,
    [AddressLine4] nvarchar(250) NULL,
    [City] nvarchar(250) NULL,
    [Province] nvarchar(250) NULL,
    [County] nvarchar(250) NULL,
    [IsDefault] bit NOT NULL,
    [IsInvoiceAddress] bit NOT NULL,
    [PostalCode] nvarchar(50) NULL,
    [CountryCode] nvarchar(4) NULL,
    [Position_Latitude] decimal(18, 4) NULL,
    [Position_Longitude] decimal(18, 4) NULL,
    [B2BConnectionId] uniqueidentifier NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Addresses_Countries_CountryCode] FOREIGN KEY ([CountryCode]) REFERENCES [Countries] ([Code2]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Addresses_Organisations_OrganisationId] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [FullName] nvarchar(128) NULL,
    [GivenName] nvarchar(64) NULL,
    [Surname] nvarchar(64) NULL,
    [IsAdmin] bit NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [OrganisationId] uniqueidentifier NULL,
    [LocationId] uniqueidentifier NULL,
    [ExternalProverUUID] uniqueidentifier NULL,
    [RegistrationDate] datetime2 NULL,
    [InvitationId] uniqueidentifier NULL,
    [UserPreferences] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUsers_Addresses_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AspNetUsers_Organisations_OrganisationId] FOREIGN KEY ([OrganisationId]) REFERENCES [Organisations] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [OrganisationId] uniqueidentifier NOT NULL,
    [ShipmentName] nvarchar(100) NULL,
    [ReferenceType] nvarchar(100) NULL,
    [ReferenceNumber] nvarchar(100) NULL,
    [TransportMode] int NOT NULL,
    [ShipmentDate] datetime2 NOT NULL,
    [DeliveryAddressId] uniqueidentifier NOT NULL,
    [DestinationAddressId] uniqueidentifier NOT NULL,
    [OrderStatus] int NOT NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Addresses_DeliveryAddressId] FOREIGN KEY ([DeliveryAddressId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_Addresses_DestinationAddressId] FOREIGN KEY ([DestinationAddressId]) REFERENCES [Addresses] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [OrderLineItems] (
    [Id] uniqueidentifier NOT NULL,
    [Active] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreatedByUser] nvarchar(128) NULL,
    [CreationDateInternal] datetime2 NOT NULL,
    [LastChangeUser] nvarchar(128) NULL,
    [LastModifiedOnInternal] datetime2 NOT NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [LineNumber] int NOT NULL,
    [LabelText] nvarchar(250) NULL,
    [DeviceId] uniqueidentifier NULL,
    CONSTRAINT [PK_OrderLineItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrderLineItems_Devices_DeviceId] FOREIGN KEY ([DeviceId]) REFERENCES [Devices] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrderLineItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_Addresses_CountryCode] ON [Addresses] ([CountryCode]);

GO

CREATE INDEX [IX_Addresses_OrganisationId] ON [Addresses] ([OrganisationId]);

GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

GO

CREATE INDEX [IX_AspNetUsers_LocationId] ON [AspNetUsers] ([LocationId]);

GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

GO

CREATE INDEX [IX_AspNetUsers_OrganisationId] ON [AspNetUsers] ([OrganisationId]);

GO

CREATE INDEX [IX_Devices_LastDeviceReportId] ON [Devices] ([LastDeviceReportId]);

GO

CREATE INDEX [IX_OrderLineItems_DeviceId] ON [OrderLineItems] ([DeviceId]);

GO

CREATE INDEX [IX_OrderLineItems_OrderId] ON [OrderLineItems] ([OrderId]);

GO

CREATE INDEX [IX_Orders_DeliveryAddressId] ON [Orders] ([DeliveryAddressId]);

GO

CREATE INDEX [IX_Orders_DestinationAddressId] ON [Orders] ([DestinationAddressId]);

GO

CREATE INDEX [IX_Organisations_ParentId] ON [Organisations] ([ParentId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20211111151722_InitialCreate', N'3.1.4');

GO

