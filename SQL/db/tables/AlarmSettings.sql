CREATE TABLE [dbo].[AlarmSettings] (
   [AlarmID] [smallint] NOT NULL,
   [Active] [bit] NULL,
   [Priority] [smallint] NULL,
   [AlarmText] [varchar](100) NULL,
   [DisplayText] [varchar](100) NULL,
   [Severity] [smallint] NULL,
   [Downtime] [bit] NULL,
   [DataRequired] [bit] NULL,
   [Data] [int] NULL,
   [SorterEligible] [bit] NULL

   ,CONSTRAINT [PK_AlarmSettings] PRIMARY KEY CLUSTERED ([AlarmID])
)


GO
