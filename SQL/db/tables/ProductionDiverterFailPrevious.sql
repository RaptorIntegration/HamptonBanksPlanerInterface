CREATE TABLE [dbo].[ProductionDiverterFailPrevious] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [BayID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_ProductionDiverterFailPrevious] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [BayID])
)


GO
