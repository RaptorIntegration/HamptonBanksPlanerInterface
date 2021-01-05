CREATE TABLE [dbo].[GradeMatrixOld] (
   [RecipeID] [int] NOT NULL,
   [PLCGradeID] [int] NOT NULL,
   [WEBSortGradeID] [int] NULL,
   [GradeStamps] [int] NULL,
   [Default] [bit] NULL

   ,CONSTRAINT [PK_GradeMatrixOld] PRIMARY KEY CLUSTERED ([RecipeID], [PLCGradeID])
)


GO
