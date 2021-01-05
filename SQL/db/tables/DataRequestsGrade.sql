CREATE TABLE [dbo].[DataRequestsGrade] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [GradeID] [smallint] NULL,
   [GradeIDX] [smallint] NULL,
   [GradeStamps] [int] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsGrade] PRIMARY KEY CLUSTERED ([ID])
)


GO
