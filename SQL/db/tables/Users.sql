CREATE TABLE [dbo].[Users] (
   [UserID] [smallint] NOT NULL,
   [UserName] [varchar](50) NULL,
   [Password] [varchar](50) NULL

   ,CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserID])
)


GO
