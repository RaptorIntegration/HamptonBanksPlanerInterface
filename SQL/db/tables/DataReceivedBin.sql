CREATE TABLE [dbo].[DataReceivedBin] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [FrameStart] [int] NULL,
   [BayNum] [int] NULL,
   [Count] [int] NULL,
   [SortXRef] [int] NULL,
   [Status] [int] NULL,
   [RdmWidthFlag] [bit] NULL,
   [TrimFlag] [bit] NULL,
   [Ack] [bit] NULL,
   [FrameEnd] [int] NULL

   ,CONSTRAINT [PK_PLCBinDataReceived] PRIMARY KEY CLUSTERED ([ID])
)


GO
