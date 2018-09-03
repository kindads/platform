
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/24/2018 12:59:25
-- Generated from EDMX file: D:\Keorkj\Kinn\Fundary\Captivate\Sources\Repos\Captivate Express Web\KindAds\Models\KindadsDM.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [KindadsDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_PARTNER_PRODUCT_SETTINGSPRODUCT_TYPE]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PARTNER_PRODUCT_SETTINGS] DROP CONSTRAINT [FK_PARTNER_PRODUCT_SETTINGSPRODUCT_TYPE];
GO
IF OBJECT_ID(N'[dbo].[FK_PARTNERSPARTNER_SETTINGS_PARTNER_SETTINGS]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS] DROP CONSTRAINT [FK_PARTNERSPARTNER_SETTINGS_PARTNER_SETTINGS];
GO
IF OBJECT_ID(N'[dbo].[FK_PARTNERSPARTNER_SETTINGS_PARTNERS]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS] DROP CONSTRAINT [FK_PARTNERSPARTNER_SETTINGS_PARTNERS];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSAspNetUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRODUCTS] DROP CONSTRAINT [FK_PRODUCTSAspNetUser];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSPARTNERS_PARTNERS]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRODUCTSPARTNERS] DROP CONSTRAINT [FK_PRODUCTSPARTNERS_PARTNERS];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSPARTNERS_PRODUCTS]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRODUCTSPARTNERS] DROP CONSTRAINT [FK_PRODUCTSPARTNERS_PRODUCTS];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSPARTNERS1]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PARTNERS] DROP CONSTRAINT [FK_PRODUCTSPARTNERS1];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSPRODUCT_TYPE]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[PRODUCT_TYPE] DROP CONSTRAINT [FK_PRODUCTSPRODUCT_TYPE];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSSITES]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SITES] DROP CONSTRAINT [FK_PRODUCTSSITES];
GO
IF OBJECT_ID(N'[dbo].[FK_PRODUCTSTRANSACTIONS_CAPT]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TRANSACTIONS_CAPT] DROP CONSTRAINT [FK_PRODUCTSTRANSACTIONS_CAPT];
GO
IF OBJECT_ID(N'[dbo].[FK_SITESAspNetUser]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SITES] DROP CONSTRAINT [FK_SITESAspNetUser];
GO
IF OBJECT_ID(N'[dbo].[FK_TRANSACTION_TYPETRANSACTIONS_CAPT]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TRANSACTIONS_CAPT] DROP CONSTRAINT [FK_TRANSACTION_TYPETRANSACTIONS_CAPT];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[CATEGORIES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CATEGORIES];
GO
IF OBJECT_ID(N'[dbo].[PARTNER_PRODUCT_SETTINGS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PARTNER_PRODUCT_SETTINGS];
GO
IF OBJECT_ID(N'[dbo].[PARTNERS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PARTNERS];
GO
IF OBJECT_ID(N'[dbo].[PARTNERSPARTNER_SETTINGS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PARTNERSPARTNER_SETTINGS];
GO
IF OBJECT_ID(N'[dbo].[PRODUCT_TYPE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRODUCT_TYPE];
GO
IF OBJECT_ID(N'[dbo].[PRODUCTS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRODUCTS];
GO
IF OBJECT_ID(N'[dbo].[PRODUCTSPARTNERS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PRODUCTSPARTNERS];
GO
IF OBJECT_ID(N'[dbo].[SITES]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SITES];
GO
IF OBJECT_ID(N'[dbo].[TAGS]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TAGS];
GO
IF OBJECT_ID(N'[dbo].[TRANSACTION_TYPE]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TRANSACTION_TYPE];
GO
IF OBJECT_ID(N'[dbo].[TRANSACTIONS_CAPT]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TRANSACTIONS_CAPT];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Hometown] nvarchar(max)  NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL,
    [TokenAddress] nvarchar(max)  NULL,
    [WalletAddress] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'CATEGORIES'
CREATE TABLE [dbo].[CATEGORIES] (
    [IdCategory] smallint IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PARTNER_PRODUCT_SETTINGS'
CREATE TABLE [dbo].[PARTNER_PRODUCT_SETTINGS] (
    [IdSetting] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [PRODUCT_TYPE_IdProductType] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PARTNERS'
CREATE TABLE [dbo].[PARTNERS] (
    [IdPartner] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PRODUCT_TYPE'
CREATE TABLE [dbo].[PRODUCT_TYPE] (
    [IdProductType] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PRODUCTS'
CREATE TABLE [dbo].[PRODUCTS] (
    [IdProduct] uniqueidentifier  NOT NULL,
    [StartTime] nvarchar(max)  NOT NULL,
    [EndTime] nvarchar(max)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL,
    [Price] float  NOT NULL,
    [Image] nvarchar(max)  NOT NULL,
    [ShortDescription] nvarchar(max)  NOT NULL,
    [FullDescription] nvarchar(max)  NOT NULL,
    [SITE_IdSite] uniqueidentifier  NOT NULL,
    [PARTNER_IdPartner] uniqueidentifier  NOT NULL,
    [PRODUCT_TYPE_IdProductType] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'SITES'
CREATE TABLE [dbo].[SITES] (
    [IdSite] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [URL] nvarchar(max)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'TAGS'
CREATE TABLE [dbo].[TAGS] (
    [IdTag] smallint IDENTITY(1,1) NOT NULL,
    [Description] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TRANSACTION_TYPE'
CREATE TABLE [dbo].[TRANSACTION_TYPE] (
    [IdTransactionType] smallint IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TRANSACTIONS_CAPT'
CREATE TABLE [dbo].[TRANSACTIONS_CAPT] (
    [IdTransaction] int IDENTITY(1,1) NOT NULL,
    [HashFrom] nvarchar(max)  NOT NULL,
    [HashTo] nvarchar(max)  NOT NULL,
    [Amount] nvarchar(max)  NOT NULL,
    [BlockDate] nvarchar(max)  NOT NULL,
    [RegisterDate] nvarchar(max)  NOT NULL,
    [HashTransaction] nvarchar(max)  NOT NULL,
    [Gas] nvarchar(max)  NOT NULL,
    [TRANSACTION_TYPE_IdTransactionType] smallint  NOT NULL,
    [CAMPAIGN_IdCampaign] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'CAMPAIGNs1'
CREATE TABLE [dbo].[CAMPAIGNs1] (
    [IdCampaign] uniqueidentifier  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [RegisterDate] datetime  NOT NULL,
    [AdText] nvarchar(max)  NOT NULL,
    [AdUTM] nvarchar(max)  NOT NULL,
    [AdMedium] nvarchar(max)  NOT NULL,
    [AdURL] nvarchar(max)  NOT NULL,
    [AdImage] nvarchar(max)  NOT NULL,
    [AspNetUser_Id] nvarchar(128)  NOT NULL,
    [PRODUCT_IdProduct] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PARTNERSPARTNER_SETTINGS'
CREATE TABLE [dbo].[PARTNERSPARTNER_SETTINGS] (
    [PARTNER_PRODUCT_SETTINGS_IdSetting] uniqueidentifier  NOT NULL,
    [PARTNERS_IdPartner] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'PRODUCTSPARTNERS'
CREATE TABLE [dbo].[PRODUCTSPARTNERS] (
    [PARTNERS_IdPartner] uniqueidentifier  NOT NULL,
    [PRODUCT_TYPE_IdProductType] uniqueidentifier  NOT NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'TAGCATEGORY'
CREATE TABLE [dbo].[TAGCATEGORY] (
    [TAG_IdTag] smallint  NOT NULL,
    [CATEGORies_IdCategory] smallint  NOT NULL
);
GO

-- Creating table 'CATEGORYSITE'
CREATE TABLE [dbo].[CATEGORYSITE] (
    [CATEGORY_IdCategory] smallint  NOT NULL,
    [SITEs_IdSite] uniqueidentifier  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [IdCategory] in table 'CATEGORIES'
ALTER TABLE [dbo].[CATEGORIES]
ADD CONSTRAINT [PK_CATEGORIES]
    PRIMARY KEY CLUSTERED ([IdCategory] ASC);
GO

-- Creating primary key on [IdSetting] in table 'PARTNER_PRODUCT_SETTINGS'
ALTER TABLE [dbo].[PARTNER_PRODUCT_SETTINGS]
ADD CONSTRAINT [PK_PARTNER_PRODUCT_SETTINGS]
    PRIMARY KEY CLUSTERED ([IdSetting] ASC);
GO

-- Creating primary key on [IdPartner] in table 'PARTNERS'
ALTER TABLE [dbo].[PARTNERS]
ADD CONSTRAINT [PK_PARTNERS]
    PRIMARY KEY CLUSTERED ([IdPartner] ASC);
GO

-- Creating primary key on [IdProductType] in table 'PRODUCT_TYPE'
ALTER TABLE [dbo].[PRODUCT_TYPE]
ADD CONSTRAINT [PK_PRODUCT_TYPE]
    PRIMARY KEY CLUSTERED ([IdProductType] ASC);
GO

-- Creating primary key on [IdProduct] in table 'PRODUCTS'
ALTER TABLE [dbo].[PRODUCTS]
ADD CONSTRAINT [PK_PRODUCTS]
    PRIMARY KEY CLUSTERED ([IdProduct] ASC);
GO

-- Creating primary key on [IdSite] in table 'SITES'
ALTER TABLE [dbo].[SITES]
ADD CONSTRAINT [PK_SITES]
    PRIMARY KEY CLUSTERED ([IdSite] ASC);
GO

-- Creating primary key on [IdTag] in table 'TAGS'
ALTER TABLE [dbo].[TAGS]
ADD CONSTRAINT [PK_TAGS]
    PRIMARY KEY CLUSTERED ([IdTag] ASC);
GO

-- Creating primary key on [IdTransactionType] in table 'TRANSACTION_TYPE'
ALTER TABLE [dbo].[TRANSACTION_TYPE]
ADD CONSTRAINT [PK_TRANSACTION_TYPE]
    PRIMARY KEY CLUSTERED ([IdTransactionType] ASC);
GO

-- Creating primary key on [IdTransaction] in table 'TRANSACTIONS_CAPT'
ALTER TABLE [dbo].[TRANSACTIONS_CAPT]
ADD CONSTRAINT [PK_TRANSACTIONS_CAPT]
    PRIMARY KEY CLUSTERED ([IdTransaction] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [IdCampaign] in table 'CAMPAIGNs1'
ALTER TABLE [dbo].[CAMPAIGNs1]
ADD CONSTRAINT [PK_CAMPAIGNs1]
    PRIMARY KEY CLUSTERED ([IdCampaign] ASC);
GO

-- Creating primary key on [PARTNER_PRODUCT_SETTINGS_IdSetting], [PARTNERS_IdPartner] in table 'PARTNERSPARTNER_SETTINGS'
ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS]
ADD CONSTRAINT [PK_PARTNERSPARTNER_SETTINGS]
    PRIMARY KEY CLUSTERED ([PARTNER_PRODUCT_SETTINGS_IdSetting], [PARTNERS_IdPartner] ASC);
GO

-- Creating primary key on [PARTNERS_IdPartner], [PRODUCT_TYPE_IdProductType] in table 'PRODUCTSPARTNERS'
ALTER TABLE [dbo].[PRODUCTSPARTNERS]
ADD CONSTRAINT [PK_PRODUCTSPARTNERS]
    PRIMARY KEY CLUSTERED ([PARTNERS_IdPartner], [PRODUCT_TYPE_IdProductType] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- Creating primary key on [TAG_IdTag], [CATEGORies_IdCategory] in table 'TAGCATEGORY'
ALTER TABLE [dbo].[TAGCATEGORY]
ADD CONSTRAINT [PK_TAGCATEGORY]
    PRIMARY KEY CLUSTERED ([TAG_IdTag], [CATEGORies_IdCategory] ASC);
GO

-- Creating primary key on [CATEGORY_IdCategory], [SITEs_IdSite] in table 'CATEGORYSITE'
ALTER TABLE [dbo].[CATEGORYSITE]
ADD CONSTRAINT [PK_CATEGORYSITE]
    PRIMARY KEY CLUSTERED ([CATEGORY_IdCategory], [SITEs_IdSite] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AspNetUsers_Id] in table 'PRODUCTS'
ALTER TABLE [dbo].[PRODUCTS]
ADD CONSTRAINT [FK_PRODUCTSAspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRODUCTSAspNetUser'
CREATE INDEX [IX_FK_PRODUCTSAspNetUser]
ON [dbo].[PRODUCTS]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'SITES'
ALTER TABLE [dbo].[SITES]
ADD CONSTRAINT [FK_SITESAspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SITESAspNetUser'
CREATE INDEX [IX_FK_SITESAspNetUser]
ON [dbo].[SITES]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [TRANSACTION_TYPE_IdTransactionType] in table 'TRANSACTIONS_CAPT'
ALTER TABLE [dbo].[TRANSACTIONS_CAPT]
ADD CONSTRAINT [FK_TRANSACTION_TYPETRANSACTIONS_CAPT]
    FOREIGN KEY ([TRANSACTION_TYPE_IdTransactionType])
    REFERENCES [dbo].[TRANSACTION_TYPE]
        ([IdTransactionType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TRANSACTION_TYPETRANSACTIONS_CAPT'
CREATE INDEX [IX_FK_TRANSACTION_TYPETRANSACTIONS_CAPT]
ON [dbo].[TRANSACTIONS_CAPT]
    ([TRANSACTION_TYPE_IdTransactionType]);
GO

-- Creating foreign key on [PARTNER_PRODUCT_SETTINGS_IdSetting] in table 'PARTNERSPARTNER_SETTINGS'
ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS]
ADD CONSTRAINT [FK_PARTNERSPARTNER_SETTINGS_PARTNER_PRODUCT_SETTINGS]
    FOREIGN KEY ([PARTNER_PRODUCT_SETTINGS_IdSetting])
    REFERENCES [dbo].[PARTNER_PRODUCT_SETTINGS]
        ([IdSetting])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [PARTNERS_IdPartner] in table 'PARTNERSPARTNER_SETTINGS'
ALTER TABLE [dbo].[PARTNERSPARTNER_SETTINGS]
ADD CONSTRAINT [FK_PARTNERSPARTNER_SETTINGS_PARTNERS]
    FOREIGN KEY ([PARTNERS_IdPartner])
    REFERENCES [dbo].[PARTNERS]
        ([IdPartner])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PARTNERSPARTNER_SETTINGS_PARTNERS'
CREATE INDEX [IX_FK_PARTNERSPARTNER_SETTINGS_PARTNERS]
ON [dbo].[PARTNERSPARTNER_SETTINGS]
    ([PARTNERS_IdPartner]);
GO

-- Creating foreign key on [PARTNERS_IdPartner] in table 'PRODUCTSPARTNERS'
ALTER TABLE [dbo].[PRODUCTSPARTNERS]
ADD CONSTRAINT [FK_PRODUCTSPARTNERS_PARTNERS]
    FOREIGN KEY ([PARTNERS_IdPartner])
    REFERENCES [dbo].[PARTNERS]
        ([IdPartner])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [PRODUCT_TYPE_IdProductType] in table 'PRODUCTSPARTNERS'
ALTER TABLE [dbo].[PRODUCTSPARTNERS]
ADD CONSTRAINT [FK_PRODUCTSPARTNERS_PRODUCT_TYPE]
    FOREIGN KEY ([PRODUCT_TYPE_IdProductType])
    REFERENCES [dbo].[PRODUCT_TYPE]
        ([IdProductType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRODUCTSPARTNERS_PRODUCT_TYPE'
CREATE INDEX [IX_FK_PRODUCTSPARTNERS_PRODUCT_TYPE]
ON [dbo].[PRODUCTSPARTNERS]
    ([PRODUCT_TYPE_IdProductType]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRole]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUser'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUser]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [SITE_IdSite] in table 'PRODUCTS'
ALTER TABLE [dbo].[PRODUCTS]
ADD CONSTRAINT [FK_SITEPRODUCT]
    FOREIGN KEY ([SITE_IdSite])
    REFERENCES [dbo].[SITES]
        ([IdSite])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SITEPRODUCT'
CREATE INDEX [IX_FK_SITEPRODUCT]
ON [dbo].[PRODUCTS]
    ([SITE_IdSite]);
GO

-- Creating foreign key on [AspNetUser_Id] in table 'CAMPAIGNs1'
ALTER TABLE [dbo].[CAMPAIGNs1]
ADD CONSTRAINT [FK_AspNetUserCAMPAIGN]
    FOREIGN KEY ([AspNetUser_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserCAMPAIGN'
CREATE INDEX [IX_FK_AspNetUserCAMPAIGN]
ON [dbo].[CAMPAIGNs1]
    ([AspNetUser_Id]);
GO

-- Creating foreign key on [PARTNER_IdPartner] in table 'PRODUCTS'
ALTER TABLE [dbo].[PRODUCTS]
ADD CONSTRAINT [FK_PARTNERPRODUCT]
    FOREIGN KEY ([PARTNER_IdPartner])
    REFERENCES [dbo].[PARTNERS]
        ([IdPartner])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PARTNERPRODUCT'
CREATE INDEX [IX_FK_PARTNERPRODUCT]
ON [dbo].[PRODUCTS]
    ([PARTNER_IdPartner]);
GO

-- Creating foreign key on [PRODUCT_IdProduct] in table 'CAMPAIGNs1'
ALTER TABLE [dbo].[CAMPAIGNs1]
ADD CONSTRAINT [FK_PRODUCTCAMPAIGN]
    FOREIGN KEY ([PRODUCT_IdProduct])
    REFERENCES [dbo].[PRODUCTS]
        ([IdProduct])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRODUCTCAMPAIGN'
CREATE INDEX [IX_FK_PRODUCTCAMPAIGN]
ON [dbo].[CAMPAIGNs1]
    ([PRODUCT_IdProduct]);
GO

-- Creating foreign key on [CAMPAIGN_IdCampaign] in table 'TRANSACTIONS_CAPT'
ALTER TABLE [dbo].[TRANSACTIONS_CAPT]
ADD CONSTRAINT [FK_CAMPAIGNTRANSACTIONS_CAPT]
    FOREIGN KEY ([CAMPAIGN_IdCampaign])
    REFERENCES [dbo].[CAMPAIGNs1]
        ([IdCampaign])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CAMPAIGNTRANSACTIONS_CAPT'
CREATE INDEX [IX_FK_CAMPAIGNTRANSACTIONS_CAPT]
ON [dbo].[TRANSACTIONS_CAPT]
    ([CAMPAIGN_IdCampaign]);
GO

-- Creating foreign key on [TAG_IdTag] in table 'TAGCATEGORY'
ALTER TABLE [dbo].[TAGCATEGORY]
ADD CONSTRAINT [FK_TAGCATEGORY_TAG]
    FOREIGN KEY ([TAG_IdTag])
    REFERENCES [dbo].[TAGS]
        ([IdTag])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [CATEGORies_IdCategory] in table 'TAGCATEGORY'
ALTER TABLE [dbo].[TAGCATEGORY]
ADD CONSTRAINT [FK_TAGCATEGORY_CATEGORY]
    FOREIGN KEY ([CATEGORies_IdCategory])
    REFERENCES [dbo].[CATEGORIES]
        ([IdCategory])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TAGCATEGORY_CATEGORY'
CREATE INDEX [IX_FK_TAGCATEGORY_CATEGORY]
ON [dbo].[TAGCATEGORY]
    ([CATEGORies_IdCategory]);
GO

-- Creating foreign key on [CATEGORY_IdCategory] in table 'CATEGORYSITE'
ALTER TABLE [dbo].[CATEGORYSITE]
ADD CONSTRAINT [FK_CATEGORYSITE_CATEGORY]
    FOREIGN KEY ([CATEGORY_IdCategory])
    REFERENCES [dbo].[CATEGORIES]
        ([IdCategory])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [SITEs_IdSite] in table 'CATEGORYSITE'
ALTER TABLE [dbo].[CATEGORYSITE]
ADD CONSTRAINT [FK_CATEGORYSITE_SITE]
    FOREIGN KEY ([SITEs_IdSite])
    REFERENCES [dbo].[SITES]
        ([IdSite])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CATEGORYSITE_SITE'
CREATE INDEX [IX_FK_CATEGORYSITE_SITE]
ON [dbo].[CATEGORYSITE]
    ([SITEs_IdSite]);
GO

-- Creating foreign key on [PRODUCT_TYPE_IdProductType] in table 'PRODUCTS'
ALTER TABLE [dbo].[PRODUCTS]
ADD CONSTRAINT [FK_PRODUCT_TYPEPRODUCT]
    FOREIGN KEY ([PRODUCT_TYPE_IdProductType])
    REFERENCES [dbo].[PRODUCT_TYPE]
        ([IdProductType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRODUCT_TYPEPRODUCT'
CREATE INDEX [IX_FK_PRODUCT_TYPEPRODUCT]
ON [dbo].[PRODUCTS]
    ([PRODUCT_TYPE_IdProductType]);
GO

-- Creating foreign key on [PRODUCT_TYPE_IdProductType] in table 'PARTNER_PRODUCT_SETTINGS'
ALTER TABLE [dbo].[PARTNER_PRODUCT_SETTINGS]
ADD CONSTRAINT [FK_PRODUCT_TYPEPARTNER_PRODUCT_SETTINGS]
    FOREIGN KEY ([PRODUCT_TYPE_IdProductType])
    REFERENCES [dbo].[PRODUCT_TYPE]
        ([IdProductType])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PRODUCT_TYPEPARTNER_PRODUCT_SETTINGS'
CREATE INDEX [IX_FK_PRODUCT_TYPEPARTNER_PRODUCT_SETTINGS]
ON [dbo].[PARTNER_PRODUCT_SETTINGS]
    ([PRODUCT_TYPE_IdProductType]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
