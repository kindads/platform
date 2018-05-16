-- <Migration ID="8a5d4c42-1359-4664-a373-a45244a1020b" />
GO

PRINT N'Dropping constraints from [dbo].[PRODUCTS]'
GO
ALTER TABLE [dbo].[PRODUCTS] DROP CONSTRAINT [PRODUCTS_RegistrationDate]
GO
PRINT N'Altering [dbo].[PRODUCTS]'
GO
ALTER TABLE [dbo].[PRODUCTS] DROP
COLUMN [RegistrationDate]
GO
