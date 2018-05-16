-- <Migration ID="d5b7f4e7-e9a1-44b2-9864-583950c7aede" />
GO

PRINT N'Altering [dbo].[SITES]'
GO
ALTER TABLE [dbo].[SITES] ADD
[Verified] [bit] NULL CONSTRAINT [SITES_Verified] DEFAULT ((0)),
[VerificationString] [varchar] (50) NULL
GO
