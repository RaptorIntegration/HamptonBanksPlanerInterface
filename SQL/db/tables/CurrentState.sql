CREATE TABLE [dbo].[CurrentState] (
   [CurrentMessage] [varchar](255) NULL,
   [CurrentVolume] [real] NULL,
   [CurrentPieces] [int] NULL,
   [CurrentShiftLugFill] [real] NULL,
   [CurrentUptime] [int] NULL,
   [CurrentVolumePerHour] [real] NULL,
   [CurrentPiecesPerHour] [int] NULL,
   [CurrentLPM] [smallint] NULL,
   [LastSawPattern] [bigint] NULL,
   [MainEncoderPosition] [smallint] NULL,
   [SkipEncoderPosition] [smallint] NULL,
   [CurrentAlarmID] [smallint] NULL,
   [CombinedVolume] [real] NULL,
   [SorterEfficiency] [real] NULL,
   [BinsFull] [smallint] NULL,
   [MainEncoderActual] [real] NULL,
   [PreshiftVolume] [real] NULL,
   [DisplayVolume] [real] NULL,
   [PreShiftPieces] [int] NULL,
   [DisplayPieces] [int] NULL,
   [CurrentReman] [real] NULL,
   [CurrentLugFill] [real] NULL,
   [VolumePerHour] [real] NULL,
   [trimloss] [real] NULL,
   [CurrentInputVolume] [real] NULL,
   [BinsSpare] [smallint] NULL,
   [CurrentVolumePerLug] [real] NULL
)


GO
