SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: April 20, 2010
-- Description:	Checks to see if a ticket needs to be printed
-- =============================================
create PROCEDURE [dbo].[selectRaptorTicket]
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @print tinyint
	select @print = 0
	
	

	
	if (select count(*) from ProductionPackages with (nolock) where TicketPrinted = 0) > 0 
	begin 
		insert into RaptorTicketLog select getdate(),'Ticket print requirement detected.'
		select 'Print' = 1		
		return
	end	
	
	
	select 'Print'=@print
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
