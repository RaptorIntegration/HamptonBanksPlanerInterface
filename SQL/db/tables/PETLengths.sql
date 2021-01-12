CREATE TABLE [dbo].[PETLengths] (
   [PETLengthID] [smallint] NOT NULL,
   [LengthLabel] [varchar](50) NULL,
   [SawIndex] [int] NULL,
   [LengthNominal] [real] NULL,
   [PETPosition] [real] NULL

   ,CONSTRAINT [PK_PETLengths] PRIMARY KEY CLUSTERED ([PETLengthID])
)


GO
