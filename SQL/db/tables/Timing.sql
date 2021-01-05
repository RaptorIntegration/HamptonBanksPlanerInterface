CREATE TABLE [dbo].[Timing] (
   [PLCID] [smallint] NOT NULL,
   [ID] [int] NOT NULL,
   [CategoryName] [varchar](100) NULL,
   [ItemName] [varchar](100) NULL,
   [ItemValue] [int] NULL,
   [PreviousValue] [int] NULL,
   [min] [int] NULL,
   [max] [int] NULL

   ,CONSTRAINT [PK_Timing] PRIMARY KEY CLUSTERED ([PLCID], [ID])
)


GO
