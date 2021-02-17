CREATE TABLE [dbo].[ChainInhibits] (
   [ID] [smallint] NOT NULL,
   [Description] [varchar](50) NULL,
   [X] [smallint] NULL,
   [Y] [smallint] NULL

   ,CONSTRAINT [PK_ChainInhibits] PRIMARY KEY CLUSTERED ([ID])
)


GO
