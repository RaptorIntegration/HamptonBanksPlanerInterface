SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 5 2010>
-- Description:	<This procedure does all the work necessary when a shift ends>
-- =============================================
CREATE PROCEDURE [dbo].[upIncrementShift] 
	AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @currentshiftindex int, @currentrunindex int, @newshiftindex int, @currenttime datetime, @currentday smallint
	declare @filepath varchar(50),@filename varchar(50)
	select @currenttime = getdate()
	select @currentday = (select datepart(dw,@currenttime))
	
	--prevent running this again if it has already been processed
	--if (select ABS(datediff(mi,@currenttime,ShiftEnd)) from Shifts where ShiftIndex = (select MAX(shiftindex)-1 from Shifts)) <=2
	--	return

	if (select count(*) from shifts) = 0
		select @currentshiftindex = 0
	else
		select @currentshiftindex = (select max(shiftindex) from shifts)
	select @newshiftindex = @currentshiftindex + 1

	select @currentrunindex = (select max(runindex) from runs)
    -- increment the shift
	update shifts set shiftend = @currenttime where shiftindex=@currentshiftindex
	insert into shifts select @newshiftindex,@currenttime,null,targetvolumeperhour,targetpiecesperhour,targetLugfill,targetuptime from shifts where shiftindex=@currentshiftindex

	update reportheader set recipeid = (select RecipeID from Runs where RunIndex=@currentrunindex),recipelabel='<ALL>', shiftindexstart=(select max(shiftindex)-1 from shifts), shiftindexend=(select max(shiftindex)-1 from shifts),
		RunIndexStart = (select MAX(runindex) from runs), RunIndexEnd = (select MAX(runindex) from Runs)
		execute upReportHeader
		
	

	-- clear and move currentstate data
	insert into currentstateprevious select @currentshiftindex,@currentrunindex,CurrentVolume,CurrentPieces,CurrentShiftLugFill,CurrentUptime,CurrentVolumePerHour,CurrentPiecesPerHour,CurrentLPM from CurrentState
	update currentstate set currentvolumeperlug=0,currentvolume=0, currentinputvolume=0, currentpieces=0,preshiftvolume=0,preshiftpieces=0,displayvolume=0,displaypieces=0,currentshiftlugfill=0,currentlugfill=0,currentuptime=0,currentvolumePerHour=0,volumeperhour=0,currentpiecesPerHour=0,CurrentLPM=0,SorterEfficiency=0,CurrentReman=0,trimloss=0
	update VorneStatistics set [1x4volume]=0,[1x6volume]=0,[2x7volume]=0,[2x9volume]=0,[2x4volume]=0,[2x6volume]=0,[2x8volume]=0,[2x10volume]=0,[2x12volume]=0
	update VorneStatistics set [1x4percentage]=0,[1x6percentage]=0,[2x7percentage]=0,[2x9percentage]=0,[2x4percentage]=0,[2x6percentage]=0,[2x8percentage]=0,[2x10percentage]=0,[2x12percentage]=0
	-- deal with TargetSummary table
	delete from TargetSummaryPrevious where shiftindex = (select max(shiftindex) from TargetSummary)
	insert into TargetSummaryPrevious select * from TargetSummary
	declare @time int,@maxtime int,@dw smallint, @shiftid smallint, @ss datetime, @se datetime
	select @dw = (select datepart(dw,shiftstart) from shifts where shiftindex=@newshiftindex)
	select @time=1
	select @shiftid = (select shiftid from shiftschedule,shifts where convert(datetime,convert(varchar,datepart(yy,shiftstart)) + '-' + 
		convert(varchar,datepart(mm,shiftstart)) + '-' + convert(varchar,datepart(dd,shiftstart)) + ' ' + 
		convert(varchar,datepart(hh,shiftstart)) + ':' + convert(varchar,datepart(mi,shiftstart))) = 
		convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + convert(varchar,datepart(mm,@currenttime)) + '-' + 
		convert(varchar,datepart(dd,@currenttime)) + ' ' + convert(varchar,datepart(hh,shiftstarttime)) + ':' + 
		convert(varchar,datepart(mi,shiftstarttime))) 		
		and shiftstartday=@currentday and enabled=1 and shiftindex=@newshiftindex)
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
			insert into targetsummary select @newshiftindex,@time,0,0,0,0
			select @time=@time+1
		end
	end

	-- inform PLC of end of shift so it can clear its numbers
    update RaptorCommSettings set DataRequests = DataRequests | 65536

	-- move production data to archives	
	delete from ProductionBoardsPrevious where shiftindex=@currentshiftindex
	insert into ProductionBoardsPrevious select * from ProductionBoards where shiftindex=@currentshiftindex
	delete from ProductionBoards where shiftindex=@currentshiftindex
	delete from ProductionBoardsRerunPrevious where shiftindex=@currentshiftindex
	insert into ProductionBoardsRerunPrevious select * from ProductionBoardsRerun where shiftindex=@currentshiftindex
	delete from ProductionBoardsRerun where shiftindex=@currentshiftindex
	delete from ProductionPackagesProductsPrevious where packagenumber in (select packagenumber from ProductionPackages where shiftindex=@currentshiftindex)
	insert into ProductionPackagesProductsPrevious select * from ProductionPackagesProducts where packagenumber in (select packagenumber from ProductionPackages where shiftindex=@currentshiftindex)
	delete ProductionPackagesProducts
	delete from ProductionPackagesPrevious where shiftindex=@currentshiftindex
	insert into ProductionPackagesPrevious select * from ProductionPackages where shiftindex=@currentshiftindex
	delete from ProductionPackages where shiftindex=@currentshiftindex
	delete from AlarmsPrevious where shiftindex=@currentshiftindex
	insert into AlarmsPrevious select * from Alarms where shiftindex=@currentshiftindex
	delete from Alarms where shiftindex=@currentshiftindex
	delete from ProductionDiverterFailPrevious where shiftindex=@currentshiftindex
	insert into ProductionDiverterFailPrevious select * from ProductionDiverterFail where shiftindex=@currentshiftindex
	delete from ProductionDiverterFail where shiftindex=@currentshiftindex
	
	insert into DGSData select @newshiftindex,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0

	-- limit size of archives
	execute updateLimitArchives
	

	insert into RaptorShiftMasterLog select getdate(),'Incremented Shift'
	
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
