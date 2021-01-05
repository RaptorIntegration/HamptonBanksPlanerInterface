CREATE TABLE [dbo].[DataRequestsAlarmData] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [AlarmID] [smallint] NULL,
   [AlarmData] [int] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsAlarmData] PRIMARY KEY CLUSTERED ([ID])
)


GO
