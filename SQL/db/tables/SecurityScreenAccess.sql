CREATE TABLE [dbo].[SecurityScreenAccess] (
   [ScreenName] [varchar](50) NOT NULL,
   [UserID] [smallint] NOT NULL,
   [SecurityAccess] [smallint] NULL

   ,CONSTRAINT [PK_SecurityScreenAccess] PRIMARY KEY CLUSTERED ([ScreenName], [UserID])
)


GO
