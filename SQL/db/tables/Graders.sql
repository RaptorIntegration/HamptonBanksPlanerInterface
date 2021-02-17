CREATE TABLE [dbo].[Graders] (
   [GraderID] [smallint] NOT NULL
      IDENTITY (1,1),
   [GraderDescription] [varchar](50) NULL

   ,CONSTRAINT [PK_Graders] PRIMARY KEY CLUSTERED ([GraderID])
)


GO
