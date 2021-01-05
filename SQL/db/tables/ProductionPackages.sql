CREATE TABLE [dbo].[ProductionPackages] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [PackageNumber] [int] NOT NULL,
   [CustomerPackageNumber] [int] NULL,
   [TimeStampFull] [datetime] NULL,
   [TimeStampReset] [datetime] NULL,
   [PackageSize] [int] NULL,
   [PackageCount] [int] NULL,
   [BayNum] [int] NULL,
   [SortID] [int] NULL,
   [PackageLabel] [varchar](50) NULL,
   [ProductsLabel] [varchar](1000) NULL,
   [TicketPrinted] [smallint] NULL,
   [LotNumber] [varchar](50) NULL,
   [Spare] [varchar](50) NULL,
   [Spare1] [varchar](50) NULL

   ,CONSTRAINT [PK_ProductionPackages] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [PackageNumber])
)


GO
