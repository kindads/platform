-- <Migration ID="35e71935-7b5d-4315-8768-41d3c92c47ff" />
GO

PRINT N'Altering [dbo].[PRODUCTS]'
GO
ALTER TABLE [dbo].[PRODUCTS] ADD
[RegistrationDate] [datetime] NULL CONSTRAINT [PRODUCTS_RegistrationDate] DEFAULT (getdate())
GO
