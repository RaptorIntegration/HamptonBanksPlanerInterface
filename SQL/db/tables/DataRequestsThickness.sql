CREATE TABLE [dbo].[DataRequestsThickness] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [ThickID] [int] NULL,
   [ThickMin] [real] NULL,
   [ThickMax] [real] NULL,
   [ThickNom] [real] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsThickness] PRIMARY KEY CLUSTERED ([ID])
)


GO
