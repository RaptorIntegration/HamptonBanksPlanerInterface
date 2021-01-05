CREATE TABLE [dbo].[DataRequestsMSRTest] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [TestID] [smallint] NULL,
   [Grades] [int] NULL,
   [Lengths] [int] NULL,
   [Samplesize] [smallint] NULL,
   [SamplesRemaining] [smallint] NULL,
   [Interval] [smallint] NULL,
   [Active] [bit] NOT NULL,
   [stamp] [bit] NOT NULL,
   [Trim] [bit] NOT NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsMSRTest] PRIMARY KEY CLUSTERED ([ID])
)


GO
