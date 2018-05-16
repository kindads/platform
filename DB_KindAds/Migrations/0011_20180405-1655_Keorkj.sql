-- <Migration ID="c7839067-c80e-487e-9e7b-a3305b7035a4" />
GO

PRINT N'Creating [dbo].[CAMPAIGN_CHAT]'
GO
CREATE TABLE [dbo].[CAMPAIGN_CHAT]
(
[IdCampaignMessage] [uniqueidentifier] NOT NULL,
[CAMPAIGN_IdCampaign] [uniqueidentifier] NOT NULL,
[AspNetUser_IdCreator] [nvarchar] (128) NOT NULL,
[CampaignChatMessage] [nvarchar] (max) NOT NULL,
[RegisterDate] [datetime] NOT NULL,
[CampaignChatStatus] [int] NULL
)
GO
PRINT N'Creating primary key [PK_CAMPAIGN_CHAT] on [dbo].[CAMPAIGN_CHAT]'
GO
ALTER TABLE [dbo].[CAMPAIGN_CHAT] ADD CONSTRAINT [PK_CAMPAIGN_CHAT] PRIMARY KEY CLUSTERED  ([IdCampaignMessage])
GO
PRINT N'Creating index [IX_FK_AspNetUserCAMPAIGN_CHAT] on [dbo].[CAMPAIGN_CHAT]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_AspNetUserCAMPAIGN_CHAT] ON [dbo].[CAMPAIGN_CHAT] ([AspNetUser_IdCreator])
GO
PRINT N'Creating index [IX_FK_CAMPAIGNCAMPAIGN_CHAT] on [dbo].[CAMPAIGN_CHAT]'
GO
CREATE NONCLUSTERED INDEX [IX_FK_CAMPAIGNCAMPAIGN_CHAT] ON [dbo].[CAMPAIGN_CHAT] ([CAMPAIGN_IdCampaign])
GO
PRINT N'Adding foreign keys to [dbo].[CAMPAIGN_CHAT]'
GO
ALTER TABLE [dbo].[CAMPAIGN_CHAT] ADD CONSTRAINT [FK_CAMPAIGN_IdCampaign] FOREIGN KEY ([CAMPAIGN_IdCampaign]) REFERENCES [dbo].[CAMPAIGNs1] ([IdCampaign])
GO
ALTER TABLE [dbo].[CAMPAIGN_CHAT] ADD CONSTRAINT [FK_AspNetUserCAMPAIGN_CHAT] FOREIGN KEY ([AspNetUser_IdCreator]) REFERENCES [dbo].[AspNetUsers] ([Id])
GO
