CREATE TABLE [dbo].[SortProductLengthsChild] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [LengthID] [int] NOT NULL

   ,CONSTRAINT [PK_SortProductLengthsChild] PRIMARY KEY CLUSTERED ([RecipeID], [SortID], [ProdID], [LengthID])
)


GO
