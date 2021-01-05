CREATE TABLE [dbo].[DataRequestsWidth] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [WidthID] [int] NULL,
   [WidthMin] [real] NULL,
   [WidthMax] [real] NULL,
   [WidthNom] [real] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsWidth] PRIMARY KEY CLUSTERED ([ID])
)


GO
