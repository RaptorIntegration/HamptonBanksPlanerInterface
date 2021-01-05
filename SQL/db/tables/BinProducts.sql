CREATE TABLE [dbo].[BinProducts] (
   [BinID] [int] NOT NULL,
   [ProdID] [int] NOT NULL

   ,CONSTRAINT [PK_BinProducts_1] PRIMARY KEY CLUSTERED ([BinID], [ProdID])
)


GO
