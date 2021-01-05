CREATE TABLE [dbo].[Products] (
   [ProdID] [int] NOT NULL
      IDENTITY (1,1),
   [Deleted] [smallint] NULL,
   [Active] [bit] NULL,
   [ProdLabel] [varchar](100) NULL,
   [GradeID] [int] NULL,
   [MoistureID] [int] NULL,
   [SpecID] [int] NULL,
   [ThickNominal] [real] NULL,
   [ThickMin] [real] NULL,
   [ThickMax] [real] NULL,
   [WidthNominal] [real] NULL,
   [WidthMin] [real] NULL,
   [WidthMax] [real] NULL,
   [ThicknessID] [smallint] NULL,
   [WidthID] [smallint] NULL

   ,CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProdID])
)


GO
