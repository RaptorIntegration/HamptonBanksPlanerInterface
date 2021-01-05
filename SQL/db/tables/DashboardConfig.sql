CREATE TABLE [dbo].[DashboardConfig] (
   [CardRefreshSeconds] [int] NOT NULL
      CONSTRAINT [DF_DashboardConfig_CardRefreshSeconds] DEFAULT ((10)),
   [SmallCardRefreshSeconds] [int] NOT NULL
      CONSTRAINT [DF_DashboardConfig_SmallCardRefreshSeconds] DEFAULT ((10)),
   [CardMemory] [int] NOT NULL
      CONSTRAINT [DF_DashboardConfig_CardMemory] DEFAULT ((10))
)


GO
