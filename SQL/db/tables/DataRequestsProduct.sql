CREATE TABLE [dbo].[DataRequestsProduct] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [ProductID] [smallint] NULL,
   [Active] [bit] NULL,
   [ThicknessID] [smallint] NULL,
   [WidthID] [smallint] NULL,
   [GradeID] [smallint] NULL,
   [MoistureID] [smallint] NULL,
   [SpecID] [smallint] NULL,
   [SpecialX] [smallint] NULL,
   [SpecialY] [smallint] NULL,
   [SpecialZ] [smallint] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsProduct] PRIMARY KEY CLUSTERED ([ID])
)


GO
