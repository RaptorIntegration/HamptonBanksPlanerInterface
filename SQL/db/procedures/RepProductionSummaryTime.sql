SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
create procEDURE [dbo].[RepProductionSummaryTime]
@starttime datetime,
@endtime datetime	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select @endtime=DATEADD(ss,59,@endtime)
	
    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int

	select @maxshiftid = (select max(shiftindex) from shifts)
	
	update websortsetup set starttime=@starttime, endtime=@endtime
	
	--if (select shiftend from shifts where shiftindex=@maxshiftid) is null
	update shifts set shiftend=getdate() where shiftindex=@maxshiftid
	update ReportHeader set ShiftIndexStart=(select MIN(shiftindex) from shifts where @starttime between ShiftStart and ShiftEnd) 
	update ReportHeader set ShiftIndexEnd=(select MIN(shiftindex) from shifts where @starttime between ShiftStart and ShiftEnd) 
	
	if (select shiftindexstart from ReportHeader) is null  --no shift start at this time
	begin
		update ReportHeader set ShiftIndexStart=(select MIN(shiftindex) from shifts where @endtime between ShiftStart and ShiftEnd) 
		update ReportHeader set ShiftIndexEnd=(select MIN(shiftindex) from shifts where @endtime between ShiftStart and ShiftEnd) 
	end
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

	create table #t (
	sortcode smallint,prodlabel1 varchar(1000),prodlabel varchar(1000),nominalthickness real,nominalwidth real ,gradelabel varchar(50),lengthnominal real,lengthlabel varchar(50),
	totalpieces int,totalvolume real)
			
	--select @recipeidstart=(select recipeid from Recipes where Online=1)
	--select @recipeidend= @recipeidstart

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoardsTime, runs
		where [timestamp] between @starttime and @endtime and sorted=1 and sortcode=1
		and runs.runindex = productionboardsTime.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
		insert into #t
			select sortcode='1',prodlabel1='No Data',prodlabel='No Data',nominalthickness=0,nominalwidth=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0
		else
		insert into #t
			select sortcode,prodlabel1=prodlabel,prodlabel=convert(varchar,thicknominal) + 'x' + convert(varchar,widthnominal),thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoardsTime,products,lengths,grades,runs
			where [timestamp] between @starttime and @endtime 
			and sorted=1 and sortcode=1
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsTime.prodid
			and lengths.lengthid = ProductionBoardsTime.lengthid
			and runs.runindex = ProductionBoardsTime.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsTime.shiftindex = @maxshiftid
			
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,sortcode
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoardsTime,runs 
		where [timestamp] between @starttime and @endtime  
		and sorted=1 and sortcode=1 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboardsTime.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPreviousTime,runs
		where [timestamp] between @starttime and @endtime 
		and sorted=1 and sortcode=1 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPreviousTime.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
		insert into #t
			select sortcode='1',prodlabel1='No Data',prodlabel='No Data',nominalthickness=0,nominalwidth=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0
		else
		begin
		insert into #t
			select sortcode,prodlabel1=prodlabel,prodlabel=convert(varchar,thicknominal) + 'x' + convert(varchar,widthnominal),thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoardsTime,products,lengths,grades,runs
			where [timestamp] between @starttime and @endtime 
			and sorted=1 and sortcode=1
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsTime.prodid
			and lengths.lengthid = ProductionBoardsTime.lengthid
			and runs.runindex = ProductionBoardsTime.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsTime.shiftindex between @shiftstart and @shiftend
			
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,sortcode
			union
			select sortcode,prodlabel1=prodlabel,prodlabel=convert(varchar,thicknominal) + 'x' + convert(varchar,widthnominal),thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoardsPreviousTime,products,lengths,grades,runs
			where [timestamp] between @starttime and @endtime 
			and sorted=1 and sortcode=1
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsPreviousTime.prodid
			and lengths.lengthid = ProductionBoardsPreviousTime.lengthid
			and runs.runindex = ProductionBoardsPreviousTime.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPreviousTime.shiftindex between @shiftstart and @shiftend
			
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,sortcode
		end
	end
	
	update #t set prodlabel='2x4',nominalthickness=2,nominalwidth=4 where prodlabel='4x2'
    select * from #t
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
