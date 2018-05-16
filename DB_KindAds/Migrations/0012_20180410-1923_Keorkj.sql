-- <Migration ID="9afce508-320f-4806-86e0-fcf6e185fee1" />
GO

PRINT N'Altering [dbo].[CAMPAIGNs1]'
GO
ALTER TABLE [dbo].[CAMPAIGNs1] ADD
[StartDate] [datetime] NULL,
[EndDate] [datetime] NULL
GO
