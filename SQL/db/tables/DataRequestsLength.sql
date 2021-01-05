CREATE TABLE [dbo].[DataRequestsLength] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [LengthID] [int] NULL,
   [LengthMin] [real] NULL,
   [LengthMax] [real] NULL,
   [LengthNom] [real] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsLength] PRIMARY KEY CLUSTERED ([ID])
)


GO
