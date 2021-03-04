CREATE TABLE [dbo].[TicketQueue] (
   [Packagenumber] [int] NOT NULL,
   [TimeStampReset] [varchar](50) NULL,
   [PackageCount] [int] NULL,
   [PackageLabel] [varchar](100) NULL,
   [ticketprinted] [smallint] NULL,
   [Printed] [bit] NULL

   ,CONSTRAINT [PK_TicketQueue] PRIMARY KEY CLUSTERED ([Packagenumber])
)


GO
