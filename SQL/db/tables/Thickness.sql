CREATE TABLE [dbo].[Thickness] (
   [ID] [int] NOT NULL,
   [Label] [varchar](50) NULL,
   [Nominal] [real] NULL,
   [Minimum] [real] NULL,
   [Maximum] [real] NULL

   ,CONSTRAINT [PK_Thickness] PRIMARY KEY CLUSTERED ([ID])
)


GO
