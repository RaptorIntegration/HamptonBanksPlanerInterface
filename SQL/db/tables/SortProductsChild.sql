CREATE TABLE [dbo].[SortProductsChild] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [ProdID] [int] NOT NULL

   ,CONSTRAINT [PK_SortProductsChild] PRIMARY KEY CLUSTERED ([RecipeID], [SortID], [ProdID])
)


GO
