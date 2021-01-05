CREATE TABLE [dbo].[DataReceivedLug] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [FrameStart] [int] NULL,
   [LugNum] [int] NULL,
   [TrackNum] [int] NULL,
   [BayNum] [int] NULL,
   [ProductID] [int] NULL,
   [LengthID] [int] NULL,
   [ThickActual] [real] NULL,
   [WidthActual] [real] NULL,
   [LengthIn] [real] NULL,
   [Fence] [real] NULL,
   [Saws] [bigint] NULL,
   [NET] [int] NULL,
   [FET] [int] NULL,
   [CN2] [int] NULL,
   [BayCount] [int] NULL,
   [Volume] [real] NULL,
   [PieceCount] [int] NULL,
   [Flags] [int] NULL,
   [Devices] [int] NULL,
   [Ack] [bit] NULL,
   [FrameEnd] [int] NULL

   ,CONSTRAINT [PK_PLCLugData] PRIMARY KEY CLUSTERED ([ID])
)


GO
