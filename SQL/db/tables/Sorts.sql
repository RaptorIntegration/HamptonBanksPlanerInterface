CREATE TABLE [dbo].[Sorts] (
   [RecipeID] [int] NOT NULL,
   [SortID] [int] NOT NULL,
   [SortLabel] [varchar](100) NULL,
   [Active] [bit] NULL,
   [SortSize] [int] NULL,
   [Zone1Start] [smallint] NULL,
   [Zone1Stop] [smallint] NULL,
   [Zone2Start] [smallint] NULL,
   [Zone2Stop] [smallint] NULL,
   [PkgsPerSort] [smallint] NULL,
   [RW] [bit] NULL,
   [OrderCount] [smallint] NULL,
   [SortStamps] [int] NULL,
   [SortStampsLabel] [varchar](100) NULL,
   [SortSprays] [int] NULL,
   [SortSpraysLabel] [varchar](100) NULL,
   [BinID] [int] NULL,
   [TrimFlag] [bit] NULL,
   [ProductsLabel] [varchar](1000) NULL

   ,CONSTRAINT [PK_SortsOnline] PRIMARY KEY CLUSTERED ([RecipeID], [SortID])
)


GO
