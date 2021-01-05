CREATE TABLE [dbo].[TargetSummaryPrevious] (
   [ShiftIndex] [int] NOT NULL,
   [TimeSegment] [int] NOT NULL,
   [VolumePerHour] [real] NULL,
   [PiecesPerHour] [int] NULL,
   [LugFill] [smallint] NULL,
   [Uptime] [int] NULL

   ,CONSTRAINT [PK_TargetSummaryPrevious] PRIMARY KEY CLUSTERED ([ShiftIndex], [TimeSegment])
)


GO
