CREATE TABLE [dbo].[BoardRejects] (
   [RejectFlag] [smallint] NOT NULL,
   [RejectDescription] [varchar](50) NULL,
   [Colour] [varchar](50) NOT NULL
      CONSTRAINT [DF_BoardRejects_COlour] DEFAULT ('#1b3665')

   ,CONSTRAINT [PK_BoardRejects] PRIMARY KEY CLUSTERED ([RejectFlag])
)


GO
