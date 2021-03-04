SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: April 20, 2010
-- Description:	Checks to see if a ticket needs to be printed
-- =============================================
CREATE PROCEDURE [dbo].[selectTicketQueue]
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	
	
	truncate table Ticketqueue
	insert into TicketQueue
	select  Packagenumber,TimeStampReset,packagecount,PackageLabel,TicketPrinted,Printed
    from ProductionPackages
	where   printed=0 
	
	
	union
	select  Packagenumber,TimeStampReset,packagecount,PackageLabel,TicketPrinted,Printed
    from ProductionPackagesPrevious
	where   printed=0  
        
    select * from Ticketqueue order by convert(datetime,timestampreset) desc
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
