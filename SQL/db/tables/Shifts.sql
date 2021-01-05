CREATE TABLE [dbo].[Shifts] (
   [ShiftIndex] [int] NOT NULL,
   [ShiftStart] [datetime] NULL,
   [ShiftEnd] [datetime] NULL,
   [TargetVolumePerHour] [real] NULL,
   [TargetPiecesPerHour] [int] NULL,
   [TargetLugFill] [smallint] NULL,
   [TargetUptime] [int] NULL

   ,CONSTRAINT [PK_Shifts] PRIMARY KEY CLUSTERED ([ShiftIndex])
)


GO
