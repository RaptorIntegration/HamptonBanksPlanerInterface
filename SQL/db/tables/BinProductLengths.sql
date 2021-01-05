CREATE TABLE [dbo].[BinProductLengths] (
   [BinID] [int] NOT NULL,
   [ProdID] [int] NOT NULL,
   [LengthID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_BinProducts] PRIMARY KEY CLUSTERED ([BinID], [ProdID], [LengthID])
)


GO
