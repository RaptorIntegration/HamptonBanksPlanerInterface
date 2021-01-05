CREATE TABLE [dbo].[ProductionDiverterFail] (
   [ShiftIndex] [int] NOT NULL,
   [RunIndex] [int] NOT NULL,
   [BayID] [int] NOT NULL,
   [BoardCount] [int] NULL

   ,CONSTRAINT [PK_ProductionDiverterFail] PRIMARY KEY CLUSTERED ([ShiftIndex], [RunIndex], [BayID])
)


GO
