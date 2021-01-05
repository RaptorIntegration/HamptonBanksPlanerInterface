CREATE TABLE [dbo].[lugfillcurrent1] (
   [id] [bigint] NOT NULL
      IDENTITY (1,1),
   [timestamp] [datetime] NULL,
   [volume] [real] NULL

   ,CONSTRAINT [PK_lugfillcurrent1] PRIMARY KEY CLUSTERED ([id])
)


GO
