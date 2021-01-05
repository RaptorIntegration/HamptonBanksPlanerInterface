CREATE TABLE [dbo].[DatabaseBackupHistory] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [Backupfilename] [varchar](500) NULL,
   [Password] [varchar](50) NULL

   ,CONSTRAINT [PK_DatabaseBackupHistory] PRIMARY KEY CLUSTERED ([ID])
)


GO
