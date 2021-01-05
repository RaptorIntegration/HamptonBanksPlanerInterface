CREATE TABLE [dbo].[Commtest] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [timestamp] [datetime] NULL,
   [DATA0] [int] NULL,
   [DATA1] [int] NULL,
   [DATA2] [int] NULL,
   [DATA3] [int] NULL,
   [DATA4] [int] NULL,
   [DATA5] [int] NULL,
   [DATA6] [int] NULL,
   [DATA7] [int] NULL,
   [DATA8] [int] NULL,
   [DATA9] [int] NULL

   ,CONSTRAINT [PK_Commtest] PRIMARY KEY CLUSTERED ([ID])
)


GO
