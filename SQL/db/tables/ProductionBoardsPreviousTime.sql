CREATE TABLE [dbo].[ProductionBoardsPreviousTime] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [timestamp] [datetime] NOT NULL,
   [ProdID] [int] NOT NULL,
   [LengthID] [int] NOT NULL,
   [ThickActual] [real] NOT NULL,
   [WidthActual] [real] NOT NULL,
   [LengthIn] [real] NOT NULL,
   [LengthInID] [int] NOT NULL,
   [NET] [int] NOT NULL,
   [FET] [int] NOT NULL,
   [CN2] [int] NOT NULL,
   [Fence] [real] NOT NULL,
   [Sorted] [smallint] NOT NULL,
   [SortCode] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_ProductionBoardsprevioustime] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [timestamp], [ProdID], [LengthID], [ThickActual], [WidthActual], [LengthIn], [LengthInID], [NET], [FET], [CN2], [Fence], [Sorted], [SortCode])
)


GO
