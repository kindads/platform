-- <Migration ID="9e1f51d3-b251-406a-953f-14b05f491917" />
GO

PRINT N'Creating [dbo].[CAMPAIGN_SETTINGS]'
GO
CREATE TABLE [dbo].[CAMPAIGN_SETTINGS]
(
[IdCampaignSetting] [uniqueidentifier] NOT NULL,
[SettingName] [nvarchar] (max) NOT NULL,
[SettingValue] [nvarchar] (max) NOT NULL,
[CAMPAIGNs1_IdCampaign] [uniqueidentifier] NOT NULL
)
GO
PRINT N'Creating primary key [PK_CAMPAIGN_SETTINGS] on [dbo].[CAMPAIGN_SETTINGS]'
GO
ALTER TABLE [dbo].[CAMPAIGN_SETTINGS] ADD CONSTRAINT [PK_CAMPAIGN_SETTINGS] PRIMARY KEY CLUSTERED  ([IdCampaignSetting])
GO
PRINT N'Adding foreign keys to [dbo].[CAMPAIGN_SETTINGS]'
GO
ALTER TABLE [dbo].[CAMPAIGN_SETTINGS] ADD CONSTRAINT [FK_CAMPAIGNs1_SETTINGS] FOREIGN KEY ([CAMPAIGNs1_IdCampaign]) REFERENCES [dbo].[CAMPAIGNs1] ([IdCampaign])
GO
