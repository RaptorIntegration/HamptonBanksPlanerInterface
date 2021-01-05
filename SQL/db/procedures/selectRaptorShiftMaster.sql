SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: April 20, 2010
-- Description:	Checks to see if shift needs to be incremented
-- =============================================
CREATE PROCEDURE [dbo].[selectRaptorShiftMaster]
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @print tinyint, @currenttime datetime, @currentday smallint, @endofshift tinyint
	select @print = 0
	select @endofshift = 0
	
	update RaptorCommSettings set DataRequests=DataRequests | 2
	
	select @currenttime = (select getdate())
	select @currentday = (select datepart(dw,@currenttime))

	if (select AutomaticShiftIncrementing from ReportSettings) = 0
	begin
		select 'Print' = 0
		return
	end

	--manual increment, print reports
	if (select PrintEndOfShiftReports from ReportSettings) = 1
	begin 
		insert into RaptorShiftMasterLog select getdate(),'Manually Incremented Shift Request'
		execute upIncrementShift
		update ReportSettings set PrintEndOfShiftReports = 0
		select 'Print' = 1		
		return
	end
	--manual print report set
	else if (select PrintEndOfShiftReports from ReportSettings) = 2
	begin 
		update ReportSettings set PrintEndOfShiftReports = 0
		select 'Print' = 1
		insert into RaptorShiftMasterLog select getdate(),'Manual Report Set Print Request'
		return
	end
	--print a single report
	else if (select PrintEndOfShiftReports from ReportSettings) = 3
	begin 
		update ReportSettings set PrintEndOfShiftReports = 0
		select 'Print' = 4
		insert into RaptorShiftMasterLog select getdate(),'Report Print Request'
		return
	end

	--check to see if current time matches shift end times
	if (select shiftid from shiftschedule where convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
	convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
	convert(varchar,datepart(hh,@currenttime)) + ':' + convert(varchar,datepart(mi,@currenttime))) = 
	convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
	convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,shiftendtime)) + ':' + 
	convert(varchar,datepart(mi,shiftendtime))) and shiftendday=@currentday and enabled=1) is not null
	begin
		select @endofshift = 1
		
		--check to see if there is any shift production to determine whether we need to print reports or not
		if (select count(*) from ProductionBoards where sorted=1) = 0
			select @print = 2
		else
			select @print = 1
		--increment the shift
		execute upIncrementShift		
	end

	--check to see if current time matches shift start times
	if @endofshift = 0
	begin
		if (select shiftid from shiftschedule where convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
		convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
		convert(varchar,datepart(hh,@currenttime)) + ':' + convert(varchar,datepart(mi,@currenttime))) = 
		convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,shiftstarttime)) + ':' + 
		convert(varchar,datepart(mi,shiftstarttime))) and shiftstartday=@currentday and enabled=1) is not null
		begin
			select @print = 3
			execute upStartShift
		end
	end
	
	/*
    --check to see if current time matches the end of break 1 or 5 (day shift first break end, afternoon shift first break end),
	--this is used to clear any pre-shift volume from the production display
	if @endofshift = 0
	begin
		declare @breakid smallint
		select @breakid =  (select id from shiftbreaks where convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
		convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
		convert(varchar,datepart(hh,@currenttime)) + ':' + convert(varchar,datepart(mi,@currenttime))) = 
		convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,breakend)) + ':' + 
		convert(varchar,datepart(mi,breakend))) and enabled=1) 
		if @breakid = 1 or @breakid = 5
		begin
			select @print = 3
			execute upPreShiftData
		end
	end
	
	--check to see if current time matches the start of break 3 or 7 (day shift third break start, afternoon shift third break start),
	--this is used to clear any pre-shift volume from the production display
	if @endofshift = 0
	begin
		declare @breakid1 smallint
		select @breakid1 =  (select id from shiftbreaks where convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
		convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
		convert(varchar,datepart(hh,@currenttime)) + ':' + convert(varchar,datepart(mi,@currenttime))) = 
		convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,breakstart)) + ':' + 
		convert(varchar,datepart(mi,breakstart))) and enabled=1) 
		if @breakid1 = 3 or @breakid1 = 7
		begin
			select @print = 3
			execute upPreShiftDataCorrection
		end
	end*/
	
	
	if @print = 1
	begin
		update reportheader set recipeid = 0,recipelabel='<ALL>', shiftindexstart=(select max(shiftindex)-1 from shifts), shiftindexend=(select max(shiftindex)-1 from shifts)
		execute upReportHeader
	end
	select 'Print'=@print
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
