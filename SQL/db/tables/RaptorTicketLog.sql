CREATE TABLE [dbo].[RaptorTicketLog] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [Timestamp] [datetime] NULL,
   [Text] [varchar](1000) NULL

   ,CONSTRAINT [PK_RaptorTicketLog] PRIMARY KEY CLUSTERED ([ID])
)


GO
