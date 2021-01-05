CREATE TABLE [dbo].[Lengths] (
   [LengthID] [int] NOT NULL
      IDENTITY (1,1),
   [LengthLabel] [varchar](50) NULL,
   [LengthNominal] [real] NULL,
   [LengthMin] [real] NULL,
   [LengthMax] [real] NULL,
   [PETFlag] [bit] NULL,
   [PETLengthID] [int] NULL

   ,CONSTRAINT [PK_Lengths] PRIMARY KEY CLUSTERED ([LengthID])
)


GO
