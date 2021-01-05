SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Target Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepTargetSummary]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int

	select @maxshiftid = (select max(shiftindex) from shifts)

	select @shiftstart = (select shiftindexstart from reportheader)
	select @shiftend = (select shiftindexend from reportheader)
	select @runstart = (select runindexstart from reportheader)
	select @runend = (select runindexend from reportheader)
	select @recipeid = (select recipeid from reportheader)
	if @recipeid = 0
	begin
		select @recipeidstart = 1
		select @recipeidend = 32767
	end
	else
	begin
		select @recipeidstart = @recipeid
		select @recipeidend = @recipeid
	end

	if @shiftstart = @maxshiftid --current shift only
	begin
		select * from shifts,targetsummary
			where shifts.shiftindex = @maxshiftid
					
		
	end
	
	else --previous shifts
	begin
		
			select * from shifts,targetsummaryprevious
			where shifts.shiftindex = @shiftstart
			and shifts.shiftindex = targetsummaryprevious.shiftindex
	end

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
