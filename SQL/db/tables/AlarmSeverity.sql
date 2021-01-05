CREATE TABLE [dbo].[AlarmSeverity] (
   [SeverityID] [smallint] NOT NULL,
   [SeverityGraphic] [varbinary](max) NULL

   ,CONSTRAINT [PK_AlarmSeverity] PRIMARY KEY CLUSTERED ([SeverityID])
)


GO
