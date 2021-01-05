CREATE TABLE [dbo].[SawMileage] (
   [id] [smallint] NOT NULL,
   [SawID] [smallint] NULL,
   [SawDescription] [varchar](50) NULL,
   [Strokes] [int] NULL,
   [Mileage] [real] NULL,
   [StrokeThreshold] [int] NULL,
   [MileageThreshold] [int] NULL

   ,CONSTRAINT [PK_SawMileage] PRIMARY KEY CLUSTERED ([id])
)


GO
