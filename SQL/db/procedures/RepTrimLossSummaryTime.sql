SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- alter date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Trim Loss Summary Report>
-- =============================================
create procEDURE [dbo].[RepTrimLossSummaryTime]
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
	declare @slashcode int
	select @maxshiftid = (select max(shiftindex) from shifts)
	
	update websortsetup set starttime=@starttime, endtime=@endtime
	
	--if (select shiftend from shifts where shiftindex=@maxshiftid) is null
	update shifts set shiftend=getdate() where shiftindex=@maxshiftid
	update ReportHeader set ShiftIndexStart=(select MIN(shiftindex) from shifts where @starttime between ShiftStart and ShiftEnd) 
	update ReportHeader set ShiftIndexEnd=(select MIN(shiftindex) from shifts where @starttime between ShiftStart and ShiftEnd) 
	
	
	select @slashcode = (select min(rejectflag) from BoardRejects where RejectDescription like '%slash%')

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
	
	create table #temp
	(
		thicknominal real,
		widthnominal real,
		lengthin real,
		lengthinrounded int,
		lengthnominal real,
		lengthlabel varchar(50),
		totalinputpieces int,
		totalinputvolume real,
		totaloutputvolume real,
		totalinputlineal real,
		totaloutputlineal real,
		sorted int,
		sortcode int,
		cn2 int,
		dimension varchar(50),
		prodid smallint
		
	)

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoardsTime, runs
		where [timestamp] between @starttime and @endtime and ((sorted=1 and sortcode=1) or (Sorted=0 and SortCode=@slashcode))
		and runs.runindex = ProductionBoardsTime.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			insert into #temp select thicknominal=0,widthnominal=0,lengthin=0,lengthinrounded=0,lengthnominal=0,lengthlabel='No Data',totalinputpieces=0,totalinputvolume=0,totaloutputvolume=0,totalinputlineal=0,totaloutputlineal=0,sorted=0,sortcode=0,cn2=0,dimension='',prodid=0
		else
			insert into #temp
			select thicknominal,widthnominal,lengthin=FLOOR(convert(int,lengthin)),floor(convert(int,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*FLOOR(convert(int,lengthin))*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),totalinputlineal=sum(lengthin*boardcount),totaloutputlineal=sum(lengthnominal*boardcount/12),sorted,sortcode,cn2,'',productionBoardsTime.prodid
			from ProductionBoardsTime,products,lengths,runs
			where [timestamp] between @starttime and @endtime
			and ((sorted=1 and sortcode=1) or (Sorted=0 and SortCode=@slashcode))
		    and  products.prodid = ProductionBoardsTime.prodid
			and lengths.lengthid = ProductionBoardsTime.lengthid
			
			and runs.runindex = ProductionBoardsTime.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsTime.shiftindex = @maxshiftid
			group by thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,sortcode,cn2,ProductionBoardsTime.prodid
			
			
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoardsTime,runs 
		where [timestamp] between @starttime and @endtime and ((sorted=1 and sortcode=1))
		and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsTime.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPreviousTime,runs
		where [timestamp] between @starttime and @endtime 
		and ((sorted=1  and sortcode=1) )
		and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPreviousTime.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			insert into #temp select thicknominal=0,widthnominal=0,lengthin=0,lengthinrounded=0,lengthnominal=0,lengthlabel='No Data',totalinputpieces=0,totalinputvolume=0,totaloutputvolume=0,totalinputlineal=0,totaloutputlineal=0,sorted=0,sortcode=0,cn2=0,dimension='',prodid=0
		else
		begin
			insert into #temp
			select thicknominal,widthnominal,lengthin=FLOOR(convert(int,lengthin)),floor(convert(int,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*FLOOR(convert(int,lengthin))*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),totalinputlineal=(lengthin*boardcount),totaloutputlineal=(lengthnominal*boardcount/12),sorted,sortcode,cn2,'',productionboardstime.prodid
			from ProductionBoardsTime,products,lengths,runs
			where [timestamp] between @starttime and @endtime 
			and ((sorted=1 and  sortcode=1) )
			and  products.prodid = ProductionBoardsTime.prodid
			
			and lengths.lengthid = ProductionBoardsTime.lengthid
			and runs.runindex = ProductionBoardsTime.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsTime.shiftindex between @shiftstart and @shiftend
			group by boardcount,thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,SortCode,cn2,productionboardsTime.prodid
			insert into #temp
			select thicknominal,widthnominal,lengthin=FLOOR(convert(int,lengthin)),floor(convert(int,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*FLOOR(convert(int,lengthin))*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),totalinputlineal=sum(lengthin*boardcount),totaloutputlineal=sum(lengthnominal*boardcount/12),sorted,sortcode,cn2,'',ProductionBoardsPreviousTime.prodid
			from ProductionBoardsPreviousTime,products,lengths,runs
			where [timestamp] between @starttime and @endtime 
			and ((sorted=1 and sortcode=1) )
			and products.prodid = ProductionBoardsPreviousTime.prodid
			
			and lengths.lengthid = ProductionBoardsPreviousTime.lengthid
			and runs.runindex = ProductionBoardsPreviousTime.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPreviousTime.shiftindex between @shiftstart and @shiftend
			group by boardcount,thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,SortCode,cn2,ProductionBoardsPreviousTime.prodid
		end
	end
	
	update #temp set lengthnominal=0,lengthin=0,lengthinrounded=0,totalinputvolume=0,totaloutputvolume=0 where sortcode=@slashcode
	update #temp set lengthlabel=(select rejectdescription from boardrejects where rejectflag=sortcode)
	where sorted=0
	update #temp set totalinputvolume = 0, totalinputlineal=0 where cn2 = 2
	
		
	
	update #temp set dimension = convert(varchar,thicknominal) + 'x' + convert(varchar,widthnominal)
	
	--select SUM(totalinputpieces) from #temp
	update #temp set lengthlabel='SLASH' where lengthlabel like '%mbf%'
	update #temp set lengthlabel='SLASH' where lengthlabel like '%Optimizer Slash%'
	update #temp set lengthlabel=convert(varchar,lengthnominal) + ''' ' + lengthlabel where sortcode=6
	
	update #temp set thicknominal=2,widthnominal=4 where dimension='4x2'
	update #temp set dimension='2x4' where dimension='4x2'
	--delete from #temp where lengthin=0
	--delete from #temp where thicknominal=0
	--delete from #temp where widthnominal=0
	
	
		select * from #temp order by thicknominal,widthnominal
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
