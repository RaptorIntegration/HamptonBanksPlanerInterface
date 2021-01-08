CREATE TABLE [dbo].[WEBSortSetup] (
   [OnlineSetup] [smallint] NULL,
   [CurrentDatabaseName] [varchar](50) NULL,
   [MostRecentBackupDate] [datetime] NULL,
   [MostRecentRestoreDate] [datetime] NULL,
   [ServerIPAddress] [varchar](50) NULL,
   [TargetMode] [varchar](50) NULL,
   [BoardStatsVisible] [smallint] NULL,
   [NumThicks] [smallint] NULL,
   [NumWidths] [smallint] NULL,
   [NumLengths] [smallint] NULL,
   [NumPETs] [smallint] NULL,
   [NumMoistures] [smallint] NULL,
   [NumSpecs] [smallint] NULL,
   [NumProducts] [smallint] NULL,
   [NumPETLengths] [smallint] NULL,
   [BinFontSize] [smallint] NULL,
   [BinDetailWidth] [smallint] NULL,
   [BinButtonWidth] [smallint] NULL,
   [BinButtonCellSpacing] [smallint] NULL,
   [WhistleShortDuration] [smallint] NULL,
   [WhistleLongDuration] [smallint] NULL,
   [WEBSortProductKeyEncrypted] [int] NULL,
   [WEBSortProductKeyCurrent] [int] NULL,
   [TrimlossFactor] [real] NULL,
   [ChartWidth] [smallint] NULL,
   [activeproductlistzero] [smallint] NULL,
   [ChartWidth1] [int] NULL,
   [BinButtonWidth1] [int] NULL,
   [BinDetailWidth1] [int] NULL,
   [BinButtonCellSpacing1] [int] NULL,
   [nearsawoffset] [real] NULL,
   [farsawoffset] [real] NULL
)


GO