CREATE TABLE [dbo].[AlarmsPrevious] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [AlarmID] [int] NOT NULL,
   [StartTime] [datetime] NOT NULL,
   [StopTime] [datetime] NULL,
   [Downtime] [bit] NULL,
   [Duration] [varchar](50) NULL,
   [Data] [int] NULL

   ,CONSTRAINT [PK_AlarmsPrevious] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [AlarmID], [StartTime])
)

CREATE NONCLUSTERED INDEX [StopTimeIndex, sysname,] ON [dbo].[AlarmsPrevious] ([StopTime])

GO
