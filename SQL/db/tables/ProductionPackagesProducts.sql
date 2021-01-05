CREATE TABLE [dbo].[ProductionPackagesProducts] (
   [PackageNumber] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [LengthID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_ProductionPackagesProducts] PRIMARY KEY CLUSTERED ([PackageNumber], [ProdID], [LengthID])
)


GO
