CREATE TABLE [dbo].[ShiftWhistles] (
   [WhistleID] [smallint] NOT NULL,
   [WhistleBlow] [datetime] NULL,
   [Enabled] [tinyint] NULL,
   [Type] [smallint] NULL,
   [Repetitions] [smallint] NULL,
   [TempEnabled] [tinyint] NULL

   ,CONSTRAINT [PK_ShiftWhistles] PRIMARY KEY CLUSTERED ([WhistleID])
)


GO
