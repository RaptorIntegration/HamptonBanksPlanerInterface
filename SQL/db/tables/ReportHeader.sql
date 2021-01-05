CREATE TABLE [dbo].[ReportHeader] (
   [ReportStart] [datetime] NULL,
   [ReportEnd] [datetime] NULL,
   [CompanyName] [varchar](100) NULL,
   [CompanyLogo] [varbinary](max) NULL,
   [CompanyDivision] [varchar](50) NULL,
   [CompanyCity] [varchar](50) NULL,
   [CompanyRegion] [varchar](50) NULL,
   [CompanyWebAddress] [varbinary](50) NULL,
   [RaptorName] [varchar](50) NULL,
   [RaptorLogo] [varbinary](max) NULL,
   [RaptorCity] [varchar](50) NULL,
   [RaptorRegion] [varchar](50) NULL,
   [RaptorPhone] [varchar](50) NULL,
   [RaptorWebAddress] [varchar](50) NULL,
   [ShiftIndexStart] [int] NULL,
   [ShiftIndexEnd] [int] NULL,
   [RunIndexStart] [int] NULL,
   [RunIndexEnd] [int] NULL,
   [RecipeID] [int] NULL,
   [RecipeLabel] [varchar](100) NULL,
   [PrintRecipeID] [int] NULL,
   [PrintRecipeLabel] [varchar](100) NULL,
   [PrintReportName] [varchar](200) NULL
)


GO
