CREATE TABLE [dbo].[Moistures] (
   [MoistureID] [int] NOT NULL,
   [MoistureLabel] [varchar](50) NULL,
   [MoistureDescription] [varchar](100) NULL,
   [MoistureMin] [real] NULL,
   [MoistureMax] [real] NULL

   ,CONSTRAINT [PK_Moistures] PRIMARY KEY CLUSTERED ([MoistureID])
)


GO
