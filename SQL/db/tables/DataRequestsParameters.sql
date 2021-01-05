CREATE TABLE [dbo].[DataRequestsParameters] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [PLCID] [int] NULL,
   [Item1Value] [float] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsParameters] PRIMARY KEY CLUSTERED ([ID])
)


GO
