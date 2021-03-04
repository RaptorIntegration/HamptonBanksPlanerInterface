CREATE TABLE [dbo].[AlarmsPrevious] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [AlarmID] [int] NOT NULL,
   [StartTime] [datetime] NOT NULL,
   [StopTime] [datetime] NULL,
   [Downtime] [bit] NULL,
   [Duration] [varchar](50) NULL,
   [Data] [int] NULL,
   [PrimaryReasonID] [int] NOT NULL
      CONSTRAINT [DF_AlarmsPrevious_PrimaryReasonID] DEFAULT ((0)),
   [SecondaryReasonID] [int] NOT NULL
      CONSTRAINT [DF_AlarmsPrevious_SecondaryReasonID] DEFAULT ((0))

   ,CONSTRAINT [PK_AlarmsPrevious] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [AlarmID], [StartTime])
)


GO
