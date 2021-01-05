CREATE TABLE [dbo].[DataRequestsTiming] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [PLCID] [int] NULL,
   [Item1Value] [int] NULL,
   [Item2Value] [int] NULL,
   [Item3Value] [int] NULL,
   [Item4Value] [int] NULL,
   [Item5Value] [int] NULL,
   [Item6Value] [int] NULL,
   [Item7Value] [int] NULL,
   [Item8Value] [int] NULL,
   [Item9Value] [int] NULL,
   [Item10Value] [int] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsTiming] PRIMARY KEY CLUSTERED ([ID])
)


GO
