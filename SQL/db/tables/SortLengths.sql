CREATE TABLE [dbo].[SortLengths] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [LengthID] [int] NOT NULL

   ,CONSTRAINT [PK_SortLengthsActive] PRIMARY KEY CLUSTERED ([RecipeID], [SortID], [LengthID])
)


GO
