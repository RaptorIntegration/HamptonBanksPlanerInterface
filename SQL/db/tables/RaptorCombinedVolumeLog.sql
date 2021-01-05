CREATE TABLE [dbo].[RaptorCombinedVolumeLog] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [Timestamp] [datetime] NULL,
   [Text] [varchar](1000) NULL

   ,CONSTRAINT [PK_RaptorCombinedVolumeLog] PRIMARY KEY CLUSTERED ([ID])
)


GO
