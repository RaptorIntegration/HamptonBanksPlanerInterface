CREATE TABLE [dbo].[2InchSpeeds] (
   [DriveID] [int] NOT NULL,
   [DriveLabel] [varchar](100) NULL,
   [Width4Multiplier] [real] NULL,
   [Width6Multiplier] [real] NULL,
   [Width8Multiplier] [real] NULL,
   [Width10Multiplier] [real] NULL,
   [Width12Multiplier] [real] NULL

   ,CONSTRAINT [PK_2InchSpeeds] PRIMARY KEY CLUSTERED ([DriveID])
)


GO
