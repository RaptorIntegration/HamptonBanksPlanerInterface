CREATE TABLE [dbo].[DataRequestsGraderTest] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
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
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsGraderTest] PRIMARY KEY CLUSTERED ([ID])
)


GO
