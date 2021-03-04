SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 5 2010>
-- Description:	<This procedure does all the work necessary when a shift starts>
-- =============================================
CREATE PROCEDURE [dbo].[upStartShift] 
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--jump out of this has already been processed
	if (select ABS(datediff(mi,getdate(),ShiftStart)) from Shifts where ShiftIndex = (select MAX(shiftindex) from Shifts)) <= 5
		return

	declare @currentshiftindex int, @currenttime datetime, @currentday smallint

    --take a snapshot of the preshift volume
    --update CurrentState set preshiftvolume=CurrentVolume, PreShiftPieces = currentpieces
    update CurrentState set preshiftvolume=0, PreShiftPieces = 0
	select @currenttime = getdate()
	select @currentday = (select datepart(dw,@currenttime))

	select @currentshiftindex = (select max(shiftindex) from shifts)
	
    -- update the shift start time
	update shifts set shiftstart = @currenttime where shiftindex=@currentshiftindex

	-- deal with TargetSummary table
	--insert into TargetSummaryPrevious select * from TargetSummary
	declare @time int,@maxtime int,@dw smallint,@shiftid smallint,@ss datetime,@se datetime
	select @dw = (select datepart(dw,shiftstart) from shifts where shiftindex=@currentshiftindex)
	select @time=1
	select @shiftid = (select shiftid from shiftschedule,shifts where convert(datetime,convert(varchar,datepart(yy,shiftstart)) + '-' + 
		convert(varchar,datepart(mm,shiftstart)) + '-' + convert(varchar,datepart(dd,shiftstart)) + ' ' + 
		convert(varchar,datepart(hh,shiftstart)) + ':' + convert(varchar,datepart(mi,shiftstart))) = 
		convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,shiftstarttime)) + ':' + 
		convert(varchar,datepart(mi,shiftstarttime))) and shiftstartday=@currentday and enabled=1 and shiftindex=@currentshiftindex)
	select @ss = (select convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,shiftstarttime)) + ':' + 
		convert(varchar,datepart(mi,shiftstarttime))) from shiftschedule where shiftid=@shiftid)
	select @se = (select convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,shiftendtime)) + ':' + 
		convert(varchar,datepart(mi,shiftendtime)))  from shiftschedule where shiftid=@shiftid)
	select @maxtime = (select abs(datediff(mi,@ss,@se)))
	if @maxtime is not null and @maxtime > 0
	begin
		truncate table targetsummary 
		while @time<=@maxtime
		begin
			insert into targetsummary select @currentshiftindex,@time,0,0,0,0
			select @time=@time+1
		end
	end
	
	-- inform PLC of start of shift
    update RaptorCommSettings set DataRequests = DataRequests | 131072

	insert into RaptorShiftMasterLog select getdate(),'Update Shift Start Time'
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
