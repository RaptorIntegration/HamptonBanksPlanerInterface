CREATE TABLE [dbo].[Reports] (
   [ReportID] [int] NOT NULL,
   [ReportName] [varchar](100) NULL,
   [CategoryName] [varchar](50) NULL,
   [CategoryID] [smallint] NULL,
   [AutomaticReport] [tinyint] NULL

   ,CONSTRAINT [PK_Reports] PRIMARY KEY CLUSTERED ([ReportID])
)


GO
