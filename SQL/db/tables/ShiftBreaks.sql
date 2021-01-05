CREATE TABLE [dbo].[ShiftBreaks] (
   [id] [smallint] NOT NULL,
   [breakstart] [datetime] NULL,
   [breakend] [datetime] NULL,
   [enabled] [tinyint] NULL

   ,CONSTRAINT [PK_ShiftBreaks] PRIMARY KEY CLUSTERED ([id])
)


GO
