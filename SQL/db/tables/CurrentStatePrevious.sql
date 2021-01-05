CREATE TABLE [dbo].[CurrentStatePrevious] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL
      CONSTRAINT [DF_CurrentStatePrevious_RunIndex] DEFAULT ((1)),
   [CurrentVolume] [real] NULL,
   [CurrentPieces] [int] NULL,
   [CurrentShiftLugFill] [real] NULL,
   [CurrentUptime] [int] NULL,
   [CurrentVolumePerHour] [real] NULL,
   [CurrentPiecesPerHour] [int] NULL,
   [CurrentLPM] [smallint] NULL

   ,CONSTRAINT [PK_CurrentStatePrevious] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex])
)


GO
