CREATE TABLE [dbo].[DataRequestsMoisture] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [MoistureID] [smallint] NULL,
   [MoistureMin] [smallint] NULL,
   [MoistureMax] [smallint] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsMoisture] PRIMARY KEY CLUSTERED ([ID])
)


GO
