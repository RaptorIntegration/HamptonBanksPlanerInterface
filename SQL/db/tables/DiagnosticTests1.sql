CREATE TABLE [dbo].[DiagnosticTests1] (
   [DeviceID] [smallint] NOT NULL,
   [DeviceLabel] [varchar](100) NULL,
   [DiagnosticOn] [bit] NULL,
   [CategoryID] [smallint] NULL,
   [CategoryName] [varchar](50) NULL,
   [Parameter] [int] NULL,
   [ParameterDescription] [varchar](100) NULL

   ,CONSTRAINT [PK_DiagnosticTests1] PRIMARY KEY CLUSTERED ([DeviceID])
)


GO
