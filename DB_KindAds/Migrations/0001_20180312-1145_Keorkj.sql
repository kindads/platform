-- <Migration ID="606a6884-1cf1-45fb-9ac0-27dfe7b3c6eb" />
GO

PRINT N'Creating [dbo].[PRODUCTS]'
GO
CREATE TABLE [dbo].[PRODUCTS]
(
[IdProduct] [uniqueidentifier] NOT NULL,
[StartTime] [nvarchar] (max) NOT NULL,
[EndTime] [nvarchar] (max) NOT NULL,
[AspNetUsers_Id] [nvarchar] (128) NOT NULL,
[Price] [float] NOT NULL,
[Image] [nvarchar] (max) NOT NULL,
[ShortDescription] [nvarchar] (max) NOT NULL,
[FullDescription] [nvarchar] (max) NOT NULL,
[SITE_IdSite] [uniqueidentifier] NOT NULL,
[PARTNER_IdPartner] [uniqueidentifier] NOT NULL,
[PRODUCT_TYPE_IdProductType] [uniqueidentifier] NOT NULL,
[RegistrationDate] [datetime] NULL CONSTRAINT [PRODUCTS_RegistrationDate] DEFAULT (getdate())
)
GO
PRINT N'Creating primary key [PK_PRODUCTS] on [dbo].[PRODUCTS]'
GO
ALTER TABLE [dbo].[PRODUCTS] ADD CONSTRAINT [PK_PRODUCTS] PRIMARY KEY CLUSTERED  ([IdProduct])
GO
PRINT N'Creating index [IX_FK_PRODUCTSAspNetUser] on [dbo].[PRODUCTS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PRODUCTSAspNetUser] ON [dbo].[PRODUCTS] ([AspNetUsers_Id])
GO
PRINT N'Creating index [IX_FK_PARTNERPRODUCT] on [dbo].[PRODUCTS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PARTNERPRODUCT] ON [dbo].[PRODUCTS] ([PARTNER_IdPartner])
GO
PRINT N'Creating index [IX_FK_PRODUCT_TYPEPRODUCT] on [dbo].[PRODUCTS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PRODUCT_TYPEPRODUCT] ON [dbo].[PRODUCTS] ([PRODUCT_TYPE_IdProductType])
GO
PRINT N'Creating index [IX_FK_SITEPRODUCT] on [dbo].[PRODUCTS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_SITEPRODUCT] ON [dbo].[PRODUCTS] ([SITE_IdSite])
GO
PRINT N'Creating [dbo].[AspNetUsers]'
GO
CREATE TABLE [dbo].[AspNetUsers]
(
[Id] [nvarchar] (128) NOT NULL,
[Hometown] [nvarchar] (max) NULL,
[Email] [nvarchar] (256) NULL,
[EmailConfirmed] [bit] NOT NULL,
[PasswordHash] [nvarchar] (max) NULL,
[SecurityStamp] [nvarchar] (max) NULL,
[PhoneNumber] [nvarchar] (max) NULL,
[PhoneNumberConfirmed] [bit] NOT NULL,
[TwoFactorEnabled] [bit] NOT NULL,
[LockoutEndDateUtc] [datetime] NULL,
[LockoutEnabled] [bit] NOT NULL,
[AccessFailedCount] [int] NOT NULL,
[UserName] [nvarchar] (256) NOT NULL,
[TokenAddress] [nvarchar] (max) NULL,
[WalletAddress] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating primary key [PK_AspNetUsers] on [dbo].[AspNetUsers]'
GO
ALTER TABLE [dbo].[AspNetUsers] ADD CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED  ([Id])
GO
PRINT N'Creating [dbo].[CAMPAIGNs1]'
GO
CREATE TABLE [dbo].[CAMPAIGNs1]
(
[IdCampaign] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (max) NOT NULL,
[RegisterDate] [datetime] NOT NULL,
[AdText] [nvarchar] (max) NOT NULL,
[AdURL] [nvarchar] (max) NOT NULL,
[AdImage] [nvarchar] (max) NOT NULL,
[UTM_Source] [varchar] (255) NULL,
[UTM_Medium] [varchar] (255) NULL,
[UTM_Campaign] [varchar] (255) NULL,
[AspNetUser_Id] [nvarchar] (128) NOT NULL,
[PRODUCT_IdProduct] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_CAMPAIGNs1] on [dbo].[CAMPAIGNs1]'
GO
ALTER TABLE [dbo].[CAMPAIGNs1] ADD CONSTRAINT [PK_CAMPAIGNs1] PRIMARY KEY CLUSTERED  ([IdCampaign])
GO
PRINT N'Creating index [IX_FK_AspNetUserCAMPAIGN] on [dbo].[CAMPAIGNs1]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_AspNetUserCAMPAIGN] ON [dbo].[CAMPAIGNs1] ([AspNetUser_Id])
GO
PRINT N'Creating index [IX_FK_PRODUCTCAMPAIGN] on [dbo].[CAMPAIGNs1]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PRODUCTCAMPAIGN] ON [dbo].[CAMPAIGNs1] ([PRODUCT_IdProduct])
GO
PRINT N'Creating [dbo].[TRANSACTIONS_CAPT]'
GO
CREATE TABLE [dbo].[TRANSACTIONS_CAPT]
(
[IdTransaction] [int] NOT NULL IDENTITY(1, 1),
[HashFrom] [nvarchar] (max) NOT NULL,
[HashTo] [nvarchar] (max) NOT NULL,
[Amount] [nvarchar] (max) NOT NULL,
[BlockDate] [nvarchar] (max) NOT NULL,
[RegisterDate] [nvarchar] (max) NOT NULL,
[HashTransaction] [nvarchar] (max) NOT NULL,
[Gas] [nvarchar] (max) NOT NULL,
[TRANSACTION_TYPE_IdTransactionType] [smallint] NOT NULL,
[CAMPAIGN_IdCampaign] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_TRANSACTIONS_CAPT] on [dbo].[TRANSACTIONS_CAPT]'
GO
ALTER TABLE [dbo].[TRANSACTIONS_CAPT] ADD CONSTRAINT [PK_TRANSACTIONS_CAPT] PRIMARY KEY CLUSTERED  ([IdTransaction])
GO
PRINT N'Creating index [IX_FK_CAMPAIGNTRANSACTIONS_CAPT] on [dbo].[TRANSACTIONS_CAPT]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_CAMPAIGNTRANSACTIONS_CAPT] ON [dbo].[TRANSACTIONS_CAPT] ([CAMPAIGN_IdCampaign])
GO
PRINT N'Creating index [IX_FK_TRANSACTION_TYPETRANSACTIONS_CAPT] on [dbo].[TRANSACTIONS_CAPT]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_TRANSACTION_TYPETRANSACTIONS_CAPT] ON [dbo].[TRANSACTIONS_CAPT] ([TRANSACTION_TYPE_IdTransactionType])
GO
PRINT N'Creating [dbo].[CATEGORIES]'
GO
CREATE TABLE [dbo].[CATEGORIES]
(
[IdCategory] [smallint] NOT NULL IDENTITY(1, 1),
[Description] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating primary key [PK_CATEGORIES] on [dbo].[CATEGORIES]'
GO
ALTER TABLE [dbo].[CATEGORIES] ADD CONSTRAINT [PK_CATEGORIES] PRIMARY KEY CLUSTERED  ([IdCategory])
GO
PRINT N'Creating [dbo].[CATEGORYSITE]'
GO
CREATE TABLE [dbo].[CATEGORYSITE]
(
[CATEGORY_IdCategory] [smallint] NOT NULL,
[SITEs_IdSite] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_CATEGORYSITE] on [dbo].[CATEGORYSITE]'
GO
ALTER TABLE [dbo].[CATEGORYSITE] ADD CONSTRAINT [PK_CATEGORYSITE] PRIMARY KEY CLUSTERED  ([CATEGORY_IdCategory], [SITEs_IdSite])
GO
PRINT N'Creating index [IX_FK_CATEGORYSITE_SITE] on [dbo].[CATEGORYSITE]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_CATEGORYSITE_SITE] ON [dbo].[CATEGORYSITE] ([SITEs_IdSite])
GO
PRINT N'Creating [dbo].[SITES]'
GO
CREATE TABLE [dbo].[SITES]
(
[IdSite] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (max) NOT NULL,
[URL] [nvarchar] (max) NOT NULL,
[AspNetUsers_Id] [nvarchar] (128) NOT NULL
)
GO
PRINT N'Creating primary key [PK_SITES] on [dbo].[SITES]'
GO
ALTER TABLE [dbo].[SITES] ADD CONSTRAINT [PK_SITES] PRIMARY KEY CLUSTERED  ([IdSite])
GO
PRINT N'Creating index [IX_FK_SITESAspNetUser] on [dbo].[SITES]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_SITESAspNetUser] ON [dbo].[SITES] ([AspNetUsers_Id])
GO
PRINT N'Creating [dbo].[AspNetRoles]'
GO
CREATE TABLE [dbo].[AspNetRoles]
(
[Id] [nvarchar] (128) NOT NULL,
[Name] [nvarchar] (256) NOT NULL
)
GO
PRINT N'Creating primary key [PK_AspNetRoles] on [dbo].[AspNetRoles]'
GO
ALTER TABLE [dbo].[AspNetRoles] ADD CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED  ([Id])
GO
PRINT N'Creating [dbo].[AspNetUserRoles]'
GO
CREATE TABLE [dbo].[AspNetUserRoles]
(
[UserId] [nvarchar] (128) NOT NULL,
[RoleId] [nvarchar] (128) NOT NULL
)
GO
PRINT N'Creating primary key [PK_AspNetUserRoles] on [dbo].[AspNetUserRoles]'
GO
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED  ([UserId], [RoleId])
GO
PRINT N'Creating [dbo].[PARTNERS]'
GO
CREATE TABLE [dbo].[PARTNERS]
(
[IdPartner] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (max) NOT NULL,
[Status] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating primary key [PK_PARTNERS] on [dbo].[PARTNERS]'
GO
ALTER TABLE [dbo].[PARTNERS] ADD CONSTRAINT [PK_PARTNERS] PRIMARY KEY CLUSTERED  ([IdPartner])
GO
PRINT N'Creating [dbo].[PARTNER_PRODUCT_SETTINGS]'
GO
CREATE TABLE [dbo].[PARTNER_PRODUCT_SETTINGS]
(
[IdSetting] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (max) NOT NULL,
[Value] [nvarchar] (max) NOT NULL,
[PRODUCT_TYPE_IdProductType] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_PARTNER_PRODUCT_SETTINGS] on [dbo].[PARTNER_PRODUCT_SETTINGS]'
GO
ALTER TABLE [dbo].[PARTNER_PRODUCT_SETTINGS] ADD CONSTRAINT [PK_PARTNER_PRODUCT_SETTINGS] PRIMARY KEY CLUSTERED  ([IdSetting])
GO
PRINT N'Creating index [IX_FK_PRODUCT_TYPEPARTNER_PRODUCT_SETTINGS] on [dbo].[PARTNER_PRODUCT_SETTINGS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PRODUCT_TYPEPARTNER_PRODUCT_SETTINGS] ON [dbo].[PARTNER_PRODUCT_SETTINGS] ([PRODUCT_TYPE_IdProductType])
GO
PRINT N'Creating [dbo].[PARTNERSPARTNER_SETTINGS]'
GO
CREATE TABLE [dbo].[PARTNERSPARTNER_SETTINGS]
(
[PARTNER_PRODUCT_SETTINGS_IdSetting] [uniqueidentifier] NOT NULL,
[PARTNERS_IdPartner] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_PARTNERSPARTNER_SETTINGS] on [dbo].[PARTNERSPARTNER_SETTINGS]'
GO
ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS] ADD CONSTRAINT [PK_PARTNERSPARTNER_SETTINGS] PRIMARY KEY CLUSTERED  ([PARTNER_PRODUCT_SETTINGS_IdSetting], [PARTNERS_IdPartner])
GO
PRINT N'Creating index [IX_FK_PARTNERSPARTNER_SETTINGS_PARTNERS] on [dbo].[PARTNERSPARTNER_SETTINGS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PARTNERSPARTNER_SETTINGS_PARTNERS] ON [dbo].[PARTNERSPARTNER_SETTINGS] ([PARTNERS_IdPartner])
GO
PRINT N'Creating [dbo].[PRODUCT_TYPE]'
GO
CREATE TABLE [dbo].[PRODUCT_TYPE]
(
[IdProductType] [uniqueidentifier] NOT NULL,
[Name] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating primary key [PK_PRODUCT_TYPE] on [dbo].[PRODUCT_TYPE]'
GO
ALTER TABLE [dbo].[PRODUCT_TYPE] ADD CONSTRAINT [PK_PRODUCT_TYPE] PRIMARY KEY CLUSTERED  ([IdProductType])
GO
PRINT N'Creating [dbo].[PRODUCTSPARTNERS]'
GO
CREATE TABLE [dbo].[PRODUCTSPARTNERS]
(
[PARTNERS_IdPartner] [uniqueidentifier] NOT NULL,
[PRODUCT_TYPE_IdProductType] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_PRODUCTSPARTNERS] on [dbo].[PRODUCTSPARTNERS]'
GO
ALTER TABLE [dbo].[PRODUCTSPARTNERS] ADD CONSTRAINT [PK_PRODUCTSPARTNERS] PRIMARY KEY CLUSTERED  ([PARTNERS_IdPartner], [PRODUCT_TYPE_IdProductType])
GO
PRINT N'Creating index [IX_FK_PRODUCTSPARTNERS_PRODUCT_TYPE] on [dbo].[PRODUCTSPARTNERS]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_PRODUCTSPARTNERS_PRODUCT_TYPE] ON [dbo].[PRODUCTSPARTNERS] ([PRODUCT_TYPE_IdProductType])
GO
PRINT N'Creating [dbo].[TAGCATEGORY]'
GO
CREATE TABLE [dbo].[TAGCATEGORY]
(
[TAG_IdTag] [smallint] NOT NULL,
[CATEGORies_IdCategory] [smallint] NOT NULL
)
GO
PRINT N'Creating primary key [PK_TAGCATEGORY] on [dbo].[TAGCATEGORY]'
GO
ALTER TABLE [dbo].[TAGCATEGORY] ADD CONSTRAINT [PK_TAGCATEGORY] PRIMARY KEY CLUSTERED  ([TAG_IdTag], [CATEGORies_IdCategory])
GO
PRINT N'Creating index [IX_FK_TAGCATEGORY_CATEGORY] on [dbo].[TAGCATEGORY]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_TAGCATEGORY_CATEGORY] ON [dbo].[TAGCATEGORY] ([CATEGORies_IdCategory])
GO
PRINT N'Creating [dbo].[TAGS]'
GO
CREATE TABLE [dbo].[TAGS]
(
[IdTag] [smallint] NOT NULL IDENTITY(1, 1),
[Description] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating primary key [PK_TAGS] on [dbo].[TAGS]'
GO
ALTER TABLE [dbo].[TAGS] ADD CONSTRAINT [PK_TAGS] PRIMARY KEY CLUSTERED  ([IdTag])
GO
PRINT N'Creating [dbo].[TRANSACTION_TYPE]'
GO
CREATE TABLE [dbo].[TRANSACTION_TYPE]
(
[IdTransactionType] [smallint] NOT NULL IDENTITY(1, 1),
[Name] [nvarchar] (max) NOT NULL
)
GO
PRINT N'Creating primary key [PK_TRANSACTION_TYPE] on [dbo].[TRANSACTION_TYPE]'
GO
ALTER TABLE [dbo].[TRANSACTION_TYPE] ADD CONSTRAINT [PK_TRANSACTION_TYPE] PRIMARY KEY CLUSTERED  ([IdTransactionType])
GO
PRINT N'Creating [dbo].[TRANSACTIONS_EXTERNAL]'
GO
CREATE TABLE [dbo].[TRANSACTIONS_EXTERNAL]
(
[IdTransactionext] [uniqueidentifier] NOT NULL,
[HashFrom] [nvarchar] (max) NOT NULL,
[HashTo] [nvarchar] (max) NOT NULL,
[Amount] [float] NOT NULL,
[RegisterDate] [nvarchar] (max) NOT NULL,
[HashTransaction] [nvarchar] (max) NOT NULL,
[Gas] [nvarchar] (max) NOT NULL,
[TRANSACTION_TYPE_IdTransactionType] [smallint] NOT NULL
)
GO
PRINT N'Creating primary key [PK_TRANSACTIONS_EXTERNAL] on [dbo].[TRANSACTIONS_EXTERNAL]'
GO
ALTER TABLE [dbo].[TRANSACTIONS_EXTERNAL] ADD CONSTRAINT [PK_TRANSACTIONS_EXTERNAL] PRIMARY KEY CLUSTERED  ([IdTransactionext])
GO
PRINT N'Creating index [IX_FK_TRANSACTION_TYPETRANSACTIONS_EXT] on [dbo].[TRANSACTIONS_EXTERNAL]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_TRANSACTION_TYPETRANSACTIONS_EXT] ON [dbo].[TRANSACTIONS_EXTERNAL] ([TRANSACTION_TYPE_IdTransactionType])
GO
PRINT N'Creating [dbo].[AspNetUserClaims]'
GO
CREATE TABLE [dbo].[AspNetUserClaims]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[ClaimType] [nvarchar] (max) NULL,
[ClaimValue] [nvarchar] (max) NULL,
[UserId] [nvarchar] (128) NOT NULL
)
GO
PRINT N'Creating [dbo].[AspNetUserLogins]'
GO
CREATE TABLE [dbo].[AspNetUserLogins]
(
[UserId] [nvarchar] (128) NOT NULL,
[LoginProvider] [nvarchar] (128) NOT NULL,
[ProviderKey] [nvarchar] (128) NOT NULL
)
GO
PRINT N'Creating primary key [PK_AspNetUserLogins] on [dbo].[AspNetUserLogins]'
GO
ALTER TABLE [dbo].[AspNetUserLogins] ADD CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED  ([UserId], [LoginProvider], [ProviderKey])
GO
PRINT N'Adding foreign keys to [dbo].[AspNetUserRoles]'
GO
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[AspNetRoles] ([Id]) ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] ADD CONSTRAINT [FK_dbo.AspNetUserRoles_dbo.AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
GO
PRINT N'Adding foreign keys to [dbo].[CAMPAIGNs1]'
GO
ALTER TABLE [dbo].[CAMPAIGNs1] ADD CONSTRAINT [FK_AspNetUserCAMPAIGN] FOREIGN KEY ([AspNetUser_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[CAMPAIGNs1] ADD CONSTRAINT [FK_PRODUCTCAMPAIGN] FOREIGN KEY ([PRODUCT_IdProduct]) REFERENCES [dbo].[PRODUCTS] ([IdProduct])
GO
PRINT N'Adding foreign keys to [dbo].[PRODUCTS]'
GO
ALTER TABLE [dbo].[PRODUCTS] ADD CONSTRAINT [FK_PRODUCTSAspNetUser] FOREIGN KEY ([AspNetUsers_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
GO
ALTER TABLE [dbo].[PRODUCTS] ADD CONSTRAINT [FK_PARTNERPRODUCT] FOREIGN KEY ([PARTNER_IdPartner]) REFERENCES [dbo].[PARTNERS] ([IdPartner])
GO
ALTER TABLE [dbo].[PRODUCTS] ADD CONSTRAINT [FK_SITEPRODUCT] FOREIGN KEY ([SITE_IdSite]) REFERENCES [dbo].[SITES] ([IdSite])
GO
ALTER TABLE [dbo].[PRODUCTS] ADD CONSTRAINT [FK_PRODUCT_TYPEPRODUCT] FOREIGN KEY ([PRODUCT_TYPE_IdProductType]) REFERENCES [dbo].[PRODUCT_TYPE] ([IdProductType])
GO
PRINT N'Adding foreign keys to [dbo].[SITES]'
GO
ALTER TABLE [dbo].[SITES] ADD CONSTRAINT [FK_SITESAspNetUser] FOREIGN KEY ([AspNetUsers_Id]) REFERENCES [dbo].[AspNetUsers] ([Id])
GO
PRINT N'Adding foreign keys to [dbo].[TRANSACTIONS_CAPT]'
GO
ALTER TABLE [dbo].[TRANSACTIONS_CAPT] ADD CONSTRAINT [FK_CAMPAIGNTRANSACTIONS_CAPT] FOREIGN KEY ([CAMPAIGN_IdCampaign]) REFERENCES [dbo].[CAMPAIGNs1] ([IdCampaign])
GO
ALTER TABLE [dbo].[TRANSACTIONS_CAPT] ADD CONSTRAINT [FK_TRANSACTION_TYPETRANSACTIONS_CAPT] FOREIGN KEY ([TRANSACTION_TYPE_IdTransactionType]) REFERENCES [dbo].[TRANSACTION_TYPE] ([IdTransactionType])
GO
PRINT N'Adding foreign keys to [dbo].[CATEGORYSITE]'
GO
ALTER TABLE [dbo].[CATEGORYSITE] ADD CONSTRAINT [FK_CATEGORYSITE_CATEGORY] FOREIGN KEY ([CATEGORY_IdCategory]) REFERENCES [dbo].[CATEGORIES] ([IdCategory])
GO
ALTER TABLE [dbo].[CATEGORYSITE] ADD CONSTRAINT [FK_CATEGORYSITE_SITE] FOREIGN KEY ([SITEs_IdSite]) REFERENCES [dbo].[SITES] ([IdSite])
GO
PRINT N'Adding foreign keys to [dbo].[TAGCATEGORY]'
GO
ALTER TABLE [dbo].[TAGCATEGORY] ADD CONSTRAINT [FK_TAGCATEGORY_CATEGORY] FOREIGN KEY ([CATEGORies_IdCategory]) REFERENCES [dbo].[CATEGORIES] ([IdCategory])
GO
ALTER TABLE [dbo].[TAGCATEGORY] ADD CONSTRAINT [FK_TAGCATEGORY_TAG] FOREIGN KEY ([TAG_IdTag]) REFERENCES [dbo].[TAGS] ([IdTag])
GO
PRINT N'Adding foreign keys to [dbo].[PARTNERSPARTNER_SETTINGS]'
GO
ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS] ADD CONSTRAINT [FK_PARTNERSPARTNER_SETTINGS_PARTNER_PRODUCT_SETTINGS] FOREIGN KEY ([PARTNER_PRODUCT_SETTINGS_IdSetting]) REFERENCES [dbo].[PARTNER_PRODUCT_SETTINGS] ([IdSetting])
GO
ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS] ADD CONSTRAINT [FK_PARTNERSPARTNER_SETTINGS_PARTNERS] FOREIGN KEY ([PARTNERS_IdPartner]) REFERENCES [dbo].[PARTNERS] ([IdPartner])
GO
PRINT N'Adding foreign keys to [dbo].[PRODUCTSPARTNERS]'
GO
ALTER TABLE [dbo].[PRODUCTSPARTNERS] ADD CONSTRAINT [FK_PRODUCTSPARTNERS_PARTNERS] FOREIGN KEY ([PARTNERS_IdPartner]) REFERENCES [dbo].[PARTNERS] ([IdPartner])
GO
ALTER TABLE [dbo].[PRODUCTSPARTNERS] ADD CONSTRAINT [FK_PRODUCTSPARTNERS_PRODUCT_TYPE] FOREIGN KEY ([PRODUCT_TYPE_IdProductType]) REFERENCES [dbo].[PRODUCT_TYPE] ([IdProductType])
GO
PRINT N'Adding foreign keys to [dbo].[PARTNER_PRODUCT_SETTINGS]'
GO
ALTER TABLE [dbo].[PARTNER_PRODUCT_SETTINGS] ADD CONSTRAINT [FK_PRODUCT_TYPEPARTNER_PRODUCT_SETTINGS] FOREIGN KEY ([PRODUCT_TYPE_IdProductType]) REFERENCES [dbo].[PRODUCT_TYPE] ([IdProductType])
GO
PRINT N'Adding foreign keys to [dbo].[TRANSACTIONS_EXTERNAL]'
GO
ALTER TABLE [dbo].[TRANSACTIONS_EXTERNAL] ADD CONSTRAINT [FK_TRANSACTION_TYPETRANSACTIONS_EXT] FOREIGN KEY ([TRANSACTION_TYPE_IdTransactionType]) REFERENCES [dbo].[TRANSACTION_TYPE] ([IdTransactionType])
GO
