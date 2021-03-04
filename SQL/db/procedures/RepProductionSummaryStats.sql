SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- alter date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Production Summary Report>
-- =============================================
CREATE procEDURE [dbo].[RepProductionSummaryStats]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int
	declare @trimloss real, @slash real, @slashprevious real, @reman real, @remanprevious real, @slashvolume real, @slashvolumeprevious real
	declare @optrejects real, @optrejectsprevious real,@skewedboards real, @skewedboardsprevious real
	declare @volumeperhour real, @volumeperhourprevious real, @piecesperhour real, @piecesperhourprevious real
	
	declare @timesegment int, @maxshiftindex int, @maxrunindex int
	declare @lugfill smallint
	declare @sorted smallint, @sortcode int
	declare @totallugs real, @totallugsprevious real, @fulllugs real, @fulllugsprevious real, @t real
	

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
	
	create table #tstat(
	reman real,
	slash real,
	slashvolume real,
	trimloss real,
	lugfull real,
	packages smallint,
	fullbays smallint,
	optrejects smallint,
	skewedboards smallint,
	volumeperhour real,
	piecesperhour real,
	multi2x4Count int,
	multi2x6Count int
	)
	
	create table #temp
	(
		thicknominal real,
		widthnominal real,
		lengthin real,
		lengthinrounded smallint,
		lengthnominal real,
		lengthlabel varchar(50),
		totalinputpieces int,
		totalinputvolume real,
		totaloutputvolume real,
		sorted smallint,
		sortcode smallint,
		cn2 smallint
	)

	insert into #tstat select 0,0,0,0,0,0,0,0,0,0,0,0,0

	
	
	if @shiftstart = @maxshiftid --current shift only
	begin
		select @lugfill = 0
		select @totallugs = (select sum(boardcount) from ProductionBoards)
		select @fulllugs = (select sum(boardcount) from ProductionBoards where sortcode>=0)
		if @totallugs is null select @totallugs=0
		if @fulllugs is null select @fulllugs=0
		if @totallugs > 0
			select @lugfill = @fulllugs/@totallugs*100

		--reman percentage
		select @reman = 0
		if @fulllugs >0
		  select @reman = (select SUM(boardcount) from ProductionBoards where SortCode between 6 and 8)
		if @reman is null select @reman=0
		
		--slash percentage
		select @slash = 0
		select @slashvolume = 0
		if @fulllugs >0
		  select @slash = (select SUM(boardcount) from ProductionBoards where SortCode =11)
		if @slash is null select @slash=0
		if @fulllugs >0
		  select @slashvolume = (select SUM(thickactual*widthactual*lengthin*boardcount/12) from ProductionBoards where SortCode =11)
		if @slashvolume is null select @slashvolume=0
		
		--opt rejects
		select @optrejects = 0
		if @fulllugs >0
		  select @optrejects = (select SUM(boardcount) from ProductionBoards where (sortcode=12 or (SortCode between 9 and 10)))
		if @optrejects is null select @optrejects=0
		
		--skews
		select @skewedboards = 0
		if @fulllugs >0
		  select @skewedboards = (select SUM(boardcount) from ProductionBoards where sortcode=13)
		if @skewedboards is null select @skewedboards=0
		
		--volume, pieces per hour		
		select @t = (select MAX(timesegment) from TargetSummary where volumeperhour>0) - (select MIN(timesegment) from TargetSummary where VolumePerHour>0) 
		if @t>600 select @t=600
		if @t>0
		begin
			select @volumeperhour = (select currentvolume from CurrentState) / (select @t/60.0)
			select @piecesperhour = (select currentpieces from CurrentState) / (select @t/60.0)
		end
		else 
		begin
			select @volumeperhour=0
			select @piecesperhour=0
		end
		if (select ABS(datediff(mi,getdate(),@shiftend))) < 30
		begin
			select @volumeperhour = (select currentvolume/10.0 from CurrentState) 
			select @piecesperhour = (select convert(real,currentpieces)/10.0 from CurrentState) 
		end
		
		update #tstat set multi2x4Count = (select multi2x4Count from CurrentState)
		update #tstat set multi2x6Count = (select multi2x6Count from CurrentState)
	end
	
	else --previous shifts and/or current shift
	begin
	
		select @lugfill = 0
		select @totallugs = (select sum(boardcount) from ProductionBoards where shiftindex between @shiftstart and @shiftend)
		select @fulllugs = (select sum(boardcount) from ProductionBoards where sortcode>=0 and shiftindex between @shiftstart and @shiftend)
		if @totallugs is null select @totallugs=0
		if @fulllugs is null select @fulllugs=0
		select @totallugsprevious = (select sum(boardcount) from ProductionBoardsPrevious where shiftindex between @shiftstart and @shiftend)
		select @fulllugsprevious = (select sum(boardcount) from ProductionBoardsPrevious where shiftindex between @shiftstart and @shiftend and sortcode>=0)
		if @totallugsprevious is null select @totallugsprevious=0
		if @fulllugsprevious is null select @fulllugsprevious=0
		select @fulllugs = @fulllugs + @fulllugsprevious
		select @totallugs = @totallugs + @totallugsprevious
		if @totallugs > 0
			select @lugfill = @fulllugs/@totallugs*100

		--reman percentage
		select @reman = 0
		select @reman = (select SUM(boardcount) from ProductionBoards where SortCode between 6 and 8 and shiftindex between @shiftstart and @shiftend)
		if @reman is null select @reman = 0
		select @remanprevious = 0
		select @remanprevious = (select SUM(boardcount) from ProductionBoardsPrevious where SortCode between 6 and 8 and shiftindex between @shiftstart and @shiftend)
		if @remanprevious is null select @remanprevious = 0
		select @reman = @reman + @remanprevious
		
		if @reman is null select @reman=0
		
		--slash percentage
		select @slash = 0
		select @slash = (select SUM(boardcount) from ProductionBoards where SortCode=11 and shiftindex between @shiftstart and @shiftend)
		if @slash is null select @slash = 0
		select @slashprevious = 0
		select @slashprevious = (select SUM(boardcount) from ProductionBoardsPrevious where SortCode=11 and shiftindex between @shiftstart and @shiftend)
		if @slashprevious is null select @slashprevious = 0
		select @slash = @slash + @slashprevious		
		if @slash is null select @slash=0
		
		select @slashvolume = 0
		select @slashvolume = (select SUM(thickactual*widthactual*lengthin*boardcount/12) from ProductionBoards where SortCode =11 and shiftindex between @shiftstart and @shiftend)
		if @slashvolume is null select @slashvolume=0
		select @slashvolumeprevious = 0
		select @slashvolumeprevious = (select SUM(thickactual*widthactual*lengthin*boardcount/12) from ProductionBoardsprevious where SortCode =11 and shiftindex between @shiftstart and @shiftend)
		if @slashvolumeprevious is null select @slashvolumeprevious=0
		select @slashvolume = @slashvolume + @slashvolumeprevious		
		if @slashvolume is null select @slashvolume=0
		
		--opt rejects
		select @optrejects = 0
		select @optrejects = (select SUM(boardcount) from ProductionBoards where shiftindex between @shiftstart and @shiftend and (sortcode=12 or (SortCode between 9 and 10)))
		if @optrejects is null select @optrejects = 0
		
		select @optrejectsprevious = 0
		select @optrejectsprevious = (select SUM(boardcount) from ProductionBoardsPrevious where (sortcode=12 or (SortCode between 9 and 10)) and shiftindex between @shiftstart and @shiftend)
		if @optrejectsprevious is null select @optrejectsprevious = 0
		select @optrejects = @optrejects + @optrejectsprevious		
		if @optrejects is null select @optrejects=0
		
		--skews
		select @skewedboards = 0
		select @skewedboards = (select SUM(boardcount) from ProductionBoards where shiftindex between @shiftstart and @shiftend and sortcode=13)
		if @skewedboards is null select @skewedboards = 0
		select @skewedboardsprevious = 0
		select @skewedboardsprevious = (select SUM(boardcount) from ProductionBoardsPrevious where sortcode=62 and shiftindex between @shiftstart and @shiftend)
		if @optrejectsprevious is null select @skewedboardsprevious = 0
		select @skewedboards = @skewedboards + @skewedboardsprevious		
		if @skewedboards is null select @skewedboards=0
		
		--volume, pieces per hour
		select @volumeperhour = (select currentvolume/10.0 from CurrentStateprevious where shiftindex = @shiftstart and RunIndex=@maxrunindex) 
		select @piecesperhour = (select convert(real,Currentpieces)/10.0 from CurrentStateprevious where shiftindex = @shiftstart and RunIndex=@maxrunindex) 
		
		
	end
	
	
	
		
	--number of packages
	declare @numpacks smallint, @numpacksprevious smallint
	
	if @shiftstart = @maxshiftid --current shift only
		select @numpacks = (select COUNT(*) from ProductionPackages)
	else --previous shifts and/or current shift
	begin
		select @numpacks = (select COUNT(*) from ProductionPackages where shiftindex between @shiftstart and @shiftend)
		select @numpacksprevious = (select COUNT(*) from ProductionPackagesPrevious where shiftindex between @shiftstart and @shiftend)
		select @numpacks = @numpacks+@numpacksprevious
	end
	
	--full bays
	declare @fullbays smallint
	
	select @fullbays = (select COUNT(*) from bins where BinStatus=2)
		
	
	update #tstat set lugfull=@lugfill,reman=@reman,trimloss=@trimloss,slash=@slash,packages=@numpacks,fullbays=@fullbays,optrejects=@optrejects,skewedboards=@skewedboards,
	volumeperhour=@volumeperhour,piecesperhour=@piecesperhour, slashvolume=@slashvolume
	
	select * from #tstat

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
