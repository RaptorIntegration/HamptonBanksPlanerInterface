CREATE TABLE [dbo].[SortProductLengths] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [LengthID] [int] NOT NULL

   ,CONSTRAINT [PK_SortProductLengthsActive] PRIMARY KEY CLUSTERED ([RecipeID], [SortID], [ProdID], [LengthID])
)


GO
