CREATE TABLE [dbo].[Specs] (
   [SpecID] [int] NOT NULL,
   [SpecLabel] [varchar](50) NULL,
   [SpecDescription] [varchar](100) NULL

   ,CONSTRAINT [PK_Specs] PRIMARY KEY CLUSTERED ([SpecID])
)


GO
