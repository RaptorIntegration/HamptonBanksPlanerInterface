CREATE TABLE [dbo].[Boardslugfill] (
   [id] [int] NOT NULL
      IDENTITY (1,1),
   [timestamp] [datetime] NULL,
   [LugNum] [int] NULL,
   [TrackNum] [int] NULL,
   [BinID] [int] NULL,
   [BinLabel] [varchar](50) NULL,
   [ProdID] [int] NULL,
   [ProdLabel] [varchar](100) NULL,
   [LengthID] [int] NULL,
   [LengthLabel] [varchar](100) NULL,
   [ThickActual] [real] NULL,
   [WidthActual] [real] NULL,
   [LengthIn] [real] NULL,
   [LengthInLabel] [varchar](50) NULL,
   [Saws] [varchar](50) NULL,
   [NET] [int] NULL,
   [NETLabel] [varchar](50) NULL,
   [FET] [int] NULL,
   [FETLabel] [varchar](50) NULL,
   [CN2] [int] NULL,
   [CN2Label] [varchar](50) NULL,
   [Fence] [real] NULL,
   [FenceLabel] [varchar](50) NULL,
   [BinCount] [int] NULL,
   [Flags] [int] NULL,
   [Devices] [int] NULL

   ,CONSTRAINT [PK_Boardslugfill] PRIMARY KEY CLUSTERED ([id])
)


GO
