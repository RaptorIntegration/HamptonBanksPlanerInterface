CREATE TABLE [dbo].[DatabaseRestoreHistory] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [Restorefilename] [varchar](500) NULL

   ,CONSTRAINT [PK_DatabaseRestoreHistory] PRIMARY KEY CLUSTERED ([ID])
)


GO
