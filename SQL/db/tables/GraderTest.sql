CREATE TABLE [dbo].[GraderTest] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [Graders] [int] NULL,
   [Grades] [int] NULL,
   [Lengths] [int] NULL,
   [Samplesize] [smallint] NULL,
   [SamplesRemaining] [smallint] NULL,
   [BayID] [smallint] NULL,
   [Interval] [smallint] NULL,
   [Active] [bit] NOT NULL,
   [stamp] [bit] NOT NULL,
   [Trim] [bit] NOT NULL,
   [Thickness] [int] NULL,
   [Width] [int] NULL

   ,CONSTRAINT [PK_GraderTest] PRIMARY KEY CLUSTERED ([ID])
)


GO
