CREATE TABLE [dbo].[DataRequestsPETLength] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [PETLengthID] [int] NULL,
   [SawIndex] [smallint] NULL,
   [LengthNom] [real] NULL,
   [PETPosition] [real] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsPETLength] PRIMARY KEY CLUSTERED ([ID])
)


GO
