CREATE TABLE [dbo].[Recipes] (
   [RecipeID] [int] NOT NULL
      IDENTITY (1,1),
   [RecipeLabel] [varchar](100) NOT NULL,
   [TargetVolumePerHour] [real] NULL,
   [TargetPiecesPerHour] [int] NULL,
   [TargetLugFill] [smallint] NULL,
   [TargetUptime] [int] NULL,
   [Online] [smallint] NULL,
   [Editing] [smallint] NULL

   ,CONSTRAINT [PK_Recipes] PRIMARY KEY CLUSTERED ([RecipeID])
)


GO
