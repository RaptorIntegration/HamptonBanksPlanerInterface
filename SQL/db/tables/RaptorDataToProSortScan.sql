CREATE TABLE [dbo].[RaptorDataToProSortScan] (
   [id] [bigint] NOT NULL
      IDENTITY (1,1),
   [ProductID] [nchar](10) NULL,
   [ProgramGroup] [smallint] NULL,
   [NominalThickness] [int] NULL,
   [NominalWidth] [int] NULL,
   [Quality] [int] NULL,
   [LengthFrom] [int] NULL,
   [LengthTo] [int] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_RaptorDataToProSortScan] PRIMARY KEY CLUSTERED ([id])
)


GO
