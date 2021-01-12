CREATE TABLE [dbo].[ProductionPackagesProductsPrevious] (
   [PackageNumber] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [LengthID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_ProductionPackagesProductsPrevious] PRIMARY KEY CLUSTERED ([PackageNumber], [ProdID], [LengthID])
)


GO
