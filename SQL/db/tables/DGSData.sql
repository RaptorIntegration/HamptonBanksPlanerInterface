CREATE TABLE [dbo].[DGSData] (
   [ShiftIndex] [int] NOT NULL,
   [LugsRun] [int] NULL,
   [PiecesIn] [int] NULL,
   [RemanPieces] [int] NULL,
   [SlashedPieces] [int] NULL,
   [LinealIn] [real] NULL,
   [VolumeIn] [real] NULL,
   [TrimVolume] [real] NULL,
   [SlashedVolume] [real] NULL,
   [RemanVolume] [real] NULL,
   [BoneDryPieces2x4] [int] NULL,
   [BoneDryPieces2x6] [int] NULL,
   [BoneDryPieces2x10] [int] NULL,
   [BoneDryPieces2x12] [int] NULL,
   [StackedLoads] [int] NULL,
   [FullBinsInSorter] [int] NULL

   ,CONSTRAINT [PK_DGSData] PRIMARY KEY CLUSTERED ([ShiftIndex])
)


GO
