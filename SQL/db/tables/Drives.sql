CREATE TABLE [dbo].[Drives] (
   [RecipeID] [int] NOT NULL,
   [DriveID] [smallint] NOT NULL,
   [Command] [smallint] NULL,
   [Actual] [smallint] NULL,
   [Length1Multiplier] [real] NULL,
   [Length2Multiplier] [real] NULL,
   [Length3Multiplier] [real] NULL,
   [Length4Multiplier] [real] NULL,
   [Length5Multiplier] [real] NULL,
   [Length6Multiplier] [real] NULL,
   [Length7Multiplier] [real] NULL,
   [Length8Multiplier] [real] NULL,
   [Length9Multiplier] [real] NULL,
   [Length10Multiplier] [real] NULL

   ,CONSTRAINT [PK_Drives] PRIMARY KEY CLUSTERED ([RecipeID], [DriveID])
)


GO
