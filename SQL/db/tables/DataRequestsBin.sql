CREATE TABLE [dbo].[DataRequestsBin] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [BinID] [int] NULL,
   [BinLabel] [varchar](100) NULL,
   [BinStatus] [smallint] NULL,
   [BinSize] [int] NULL,
   [BinCount] [int] NULL,
   [ProductMap0] [bigint] NULL,
   [ProductMap1] [bigint] NULL,
   [ProductMap2] [bigint] NULL,
   [ProductMap3] [bigint] NULL,
   [ProductMap4] [bigint] NULL,
   [ProductMap5] [bigint] NULL,
   [LengthMap] [bigint] NULL,
   [ProductMap0Old] [bigint] NULL,
   [ProductMap1Old] [bigint] NULL,
   [ProductMap2Old] [bigint] NULL,
   [ProductMap3Old] [bigint] NULL,
   [ProductMap4Old] [bigint] NULL,
   [ProductMap5Old] [bigint] NULL,
   [BinStamps] [int] NULL,
   [BinSprays] [int] NULL,
   [SortID] [int] NULL,
   [TrimFlag] [bit] NULL,
   [RW] [bit] NULL,
   [ProductsOnly] [smallint] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsBin] PRIMARY KEY CLUSTERED ([ID])
)


GO
