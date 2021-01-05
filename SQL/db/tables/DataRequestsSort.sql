CREATE TABLE [dbo].[DataRequestsSort] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [SortID] [int] NULL,
   [SortLabel] [varchar](100) NULL,
   [SortSize] [smallint] NULL,
   [PkgsPerSort] [smallint] NULL,
   [OrderCount] [smallint] NULL,
   [ProductMap0] [bigint] NULL,
   [ProductMap1] [bigint] NULL,
   [ProductMap2] [bigint] NULL,
   [ProductMap3] [bigint] NULL,
   [ProductMap4] [bigint] NULL,
   [ProductMap5] [bigint] NULL,
   [LengthMap] [bigint] NULL,
   [ProductMap0c] [bigint] NULL,
   [ProductMap1c] [bigint] NULL,
   [ProductMap2c] [bigint] NULL,
   [LengthMapc] [bigint] NULL,
   [ProductMap0Old] [bigint] NULL,
   [ProductMap1Old] [bigint] NULL,
   [ProductMap2Old] [bigint] NULL,
   [ProductMap3Old] [bigint] NULL,
   [ProductMap4Old] [bigint] NULL,
   [ProductMap5Old] [bigint] NULL,
   [SortStamps] [int] NULL,
   [SortSprays] [int] NULL,
   [Zone1] [int] NULL,
   [Zone2] [int] NULL,
   [TrimFlag] [bit] NULL,
   [RW] [bit] NULL,
   [Active] [bit] NULL,
   [ProductsOnly] [smallint] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsSort] PRIMARY KEY CLUSTERED ([ID])
)


GO
