CREATE TABLE [dbo].[ProductionPackagesGraders] (
   [PackageNumber] [int] NOT NULL,
   [BinID] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [GraderID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_ProdPackageGraders] PRIMARY KEY CLUSTERED ([PackageNumber], [BinID], [ProdID], [GraderID])
)


GO
