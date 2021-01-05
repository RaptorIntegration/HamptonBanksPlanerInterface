CREATE TABLE [dbo].[DataRequestsDiagnostic1] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [DiagnosticID] [smallint] NULL,
   [DiagnosticMap] [int] NULL,
   [Parameter] [int] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsDiagnostic1] PRIMARY KEY CLUSTERED ([ID])
)


GO
