CREATE TABLE [dbo].[GradeMatrix] (
   [RecipeID] [int] NOT NULL,
   [PLCGradeID] [int] NOT NULL,
   [WEBSortGradeID] [int] NULL,
   [GradeStamps] [int] NULL,
   [Default] [bit] NULL

   ,CONSTRAINT [PK_GradeMatrix] PRIMARY KEY CLUSTERED ([RecipeID], [PLCGradeID])
)


GO
