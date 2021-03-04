CREATE TABLE [dbo].[Alarms] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [AlarmID] [int] NOT NULL,
   [StartTime] [datetime] NOT NULL,
   [StopTime] [datetime] NULL,
   [Downtime] [bit] NULL,
   [Duration] [varchar](50) NULL,
   [Data] [int] NULL,
   [PrimaryReasonID] [int] NOT NULL
      CONSTRAINT [DF_Alarms_PrimaryReasonID] DEFAULT ((0)),
   [SecondaryReasonID] [int] NOT NULL
      CONSTRAINT [DF_Alarms_SecondaryReasonID] DEFAULT ((0))

   ,CONSTRAINT [PK_Alarms] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [AlarmID], [StartTime])
)


GO
