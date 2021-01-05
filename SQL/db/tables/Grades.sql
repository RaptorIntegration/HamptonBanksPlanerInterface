CREATE TABLE [dbo].[Grades] (
   [GradeID] [int] NOT NULL
      IDENTITY (1,1),
   [GradeLabel] [varchar](50) NULL,
   [GradeDescription] [varchar](100) NULL,
   [GradeLabelTicket] [varchar](50) NULL

   ,CONSTRAINT [PK_Grades] PRIMARY KEY CLUSTERED ([GradeID])
)


GO
