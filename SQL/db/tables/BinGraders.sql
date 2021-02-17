CREATE TABLE [dbo].[BinGraders] (
   [BinID] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [GraderID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_BinGraders] PRIMARY KEY CLUSTERED ([BinID], [ProdID], [GraderID])
)


GO
