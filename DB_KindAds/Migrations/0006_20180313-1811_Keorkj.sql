-- <Migration ID="eee3b6c2-542b-4237-a6a3-35e7cb4e4b0a" />
GO

PRINT N'Dropping constraints from [dbo].[CAMPAIGNs1]'
GO
ALTER TABLE [dbo].[CAMPAIGNs1] DROP CONSTRAINT [Campaigns_IdStatus]
GO
PRINT N'Altering [dbo].[CAMPAIGNs1]'
GO
ALTER TABLE [dbo].[CAMPAIGNs1] DROP
COLUMN [IdStatus]
GO
