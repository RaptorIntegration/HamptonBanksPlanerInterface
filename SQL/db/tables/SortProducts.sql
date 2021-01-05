CREATE TABLE [dbo].[SortProducts] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [ProdID] [int] NOT NULL

   ,CONSTRAINT [PK_SortProductsActive] PRIMARY KEY CLUSTERED ([RecipeID], [SortID], [ProdID])
)


GO
