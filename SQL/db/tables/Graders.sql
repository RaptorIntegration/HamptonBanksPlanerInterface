CREATE TABLE [dbo].[Graders] (
   [GraderID] [smallint] NOT NULL,
   [GraderDescription] [varchar](50) NULL,
   [StationID] [smallint] NULL

   ,CONSTRAINT [PK_Graders] PRIMARY KEY CLUSTERED ([GraderID])
)


GO
