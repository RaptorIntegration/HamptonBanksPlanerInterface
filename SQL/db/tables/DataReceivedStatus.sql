CREATE TABLE [dbo].[DataReceivedStatus] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [Map0] [int] NULL,
   [Map1] [int] NULL,
   [Map2] [int] NULL,
   [Map3] [int] NULL,
   [Map4] [int] NULL,
   [Map5] [int] NULL,
   [Map6] [int] NULL,
   [Map7] [int] NULL,
   [Map8] [int] NULL,
   [Map9] [int] NULL,
   [Map10] [int] NULL,
   [Map11] [int] NULL,
   [Map12] [int] NULL,
   [Map13] [int] NULL,
   [Map14] [int] NULL

   ,CONSTRAINT [PK_PLCStatusDataReceived] PRIMARY KEY CLUSTERED ([ID])
)


GO
