CREATE TABLE [dbo].[Bins] (
   [BinID] [int] NOT NULL,
   [BinLabel] [varchar](100) NULL,
   [BinStatus] [smallint] NULL,
   [BinStatusLabel] [varchar](50) NULL,
   [BinSize] [int] NULL,
   [BinCount] [int] NULL,
   [RW] [bit] NULL,
   [BinStamps] [int] NULL,
   [BinStampsLabel] [varchar](100) NULL,
   [BinSprays] [int] NULL,
   [BinSpraysLabel] [varchar](100) NULL,
   [BinPercent] [smallint] NULL,
   [SortID] [int] NULL,
   [TrimFlag] [bit] NULL,
   [ProductsLabel] [varchar](1000) NULL,
   [TimeStampFull] [datetime] NULL

   ,CONSTRAINT [PK_Bins] PRIMARY KEY CLUSTERED ([BinID])
)


GO
