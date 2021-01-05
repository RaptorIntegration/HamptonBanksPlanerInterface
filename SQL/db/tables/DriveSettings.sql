CREATE TABLE [dbo].[DriveSettings] (
   [DriveID] [smallint] NOT NULL,
   [DriveLabel] [varchar](100) NULL,
   [Type] [int] NULL,
   [MaxSpeed] [real] NULL,
   [MaxDrive] [int] NULL,
   [GearingActual] [real] NULL,
   [Configuration] [int] NULL

   ,CONSTRAINT [PK_DriveSettings] PRIMARY KEY CLUSTERED ([DriveID])
)


GO
