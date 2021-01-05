CREATE TABLE [dbo].[ShiftSchedule] (
   [ShiftID] [smallint] NOT NULL,
   [ShiftStartDay] [smallint] NULL,
   [ShiftStartTime] [datetime] NULL,
   [ShiftEndDay] [smallint] NULL,
   [ShiftEndTime] [datetime] NULL,
   [Enabled] [tinyint] NULL

   ,CONSTRAINT [PK_ShiftSchedule] PRIMARY KEY CLUSTERED ([ShiftID])
)


GO
