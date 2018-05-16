-- <Migration ID="307446c8-45cc-4488-8b02-c90c435fefa1" />
GO

PRINT N'Altering [dbo].[CAMPAIGNs1]'
GO
ALTER TABLE [dbo].[CAMPAIGNs1] ADD
[IdCampaign3rdParty] [varchar] (50) NULL
GO
