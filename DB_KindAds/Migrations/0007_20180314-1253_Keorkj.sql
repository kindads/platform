-- <Migration ID="1598a1e8-8c75-444d-8478-aca842d69cb0" />
GO

PRINT N'Altering [dbo].[SITES]'
GO
ALTER TABLE [dbo].[SITES] ADD
[RegistrationDate] [datetime] NULL CONSTRAINT [SITES_RegistrationDate] DEFAULT (getdate())
GO
