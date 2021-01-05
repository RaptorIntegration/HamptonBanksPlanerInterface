CREATE TABLE [dbo].[DataRequestsDrive] (
   [ID] [int] NOT NULL
      IDENTITY (1,1),
   [TimeStamp] [datetime] NULL,
   [DriveID] [int] NULL,
   [Command] [int] NULL,
   [Actual] [int] NULL,
   [MasterLink] [int] NULL,
   [MaxSpeed] [int] NULL,
   [Scale] [real] NULL,
   [SpeedMultiplier] [real] NULL,
   [Slave] [bit] NULL,
   [Master] [bit] NULL,
   [Independent] [bit] NULL,
   [Lineal] [bit] NULL,
   [Transverse] [bit] NULL,
   [Lugged] [bit] NULL,
   [Custom] [bit] NULL,
   [Write] [bit] NULL,
   [Processed] [bit] NULL

   ,CONSTRAINT [PK_DataRequestsDrive] PRIMARY KEY CLUSTERED ([ID])
)


GO
