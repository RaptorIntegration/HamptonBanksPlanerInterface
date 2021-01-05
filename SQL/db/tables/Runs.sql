CREATE TABLE [dbo].[Runs] (
   [RunIndex] [int] NOT NULL,
   [RunStart] [datetime] NULL,
   [RunEnd] [datetime] NULL,
   [RecipeID] [int] NULL,
   [TargetVolumePerHour] [real] NULL,
   [TargetPiecesPerHour] [int] NULL,
   [TargetLugFill] [smallint] NULL,
   [TargetUptime] [int] NULL

   ,CONSTRAINT [PK_Runs] PRIMARY KEY CLUSTERED ([RunIndex])
)


GO
