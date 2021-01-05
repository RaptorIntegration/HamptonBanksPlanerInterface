CREATE TABLE [dbo].[SortLengthsChild] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [LengthID] [int] NOT NULL

   ,CONSTRAINT [PK_SortLengthsChild] PRIMARY KEY CLUSTERED ([RecipeID], [SortID], [LengthID])
)


GO
