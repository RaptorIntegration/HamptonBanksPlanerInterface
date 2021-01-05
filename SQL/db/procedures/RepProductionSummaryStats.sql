SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Production Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepProductionSummaryStats]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int
	declare @trimloss real, @slash real, @slashprevious real, @reman real, @remanprevious real, @cn2percent real
	
	declare @timesegment int, @maxshiftindex int, @maxrunindex int
	declare @lugfill smallint
	declare @sorted smallint, @sortcode int
	declare @totallugs real, @totallugsprevious real, @fulllugs real, @fulllugsprevious real,@fulllugscn2 real, @fulllugscn2previous real
	

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
	trimloss real,
	lugfull real,
	packages smallint,
	fullbays smallint,
	cn2percent real,
	inputvolume real
	)
	
	create table #temp
	(
		thicknominal real,
		widthnominal real,
		lengthin real,
		lengthinrounded real,
		lengthnominal real,
		lengthlabel varchar(50),
		totalinputpieces int,
		totalinputvolume real,
		totaloutputvolume real,
		sorted smallint,
		sortcode smallint,
		cn2 smallint
	)

	insert into #tstat select 0,0,0,0,0,0,0,0

	
	
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
		  select @reman = (select SUM(boardcount)/@fulllugs * 100 from ProductionBoards where SortCode between 6 and 8)
		if @reman is null select @reman=0
		
		--slash percentage
		select @slash = 0
		if @fulllugs >0
		  select @slash = (select SUM(boardcount)/@fulllugs * 100 from ProductionBoards where SortCode =11)
		if @slash is null select @slash=0
		
		--cn2 percentage
		select @fulllugscn2 = (select sum(boardcount) from ProductionBoards where sortcode>=0 and CN2<=1)
		select @cn2percent = 0
		if @fulllugscn2>0
			select @cn2percent = (select SUM(boardcount)/@fulllugscn2 *100 from ProductionBoards where CN2=2)
		if @cn2percent is null select @cn2percent=0
		
	end
	
	else --previous shifts and/or current shift
	begin
	
		select @lugfill = 0
		select @totallugs = (select sum(boardcount) from ProductionBoards)
		select @fulllugs = (select sum(boardcount) from ProductionBoards where sortcode>=0)
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
		if @fulllugs >0
		  select @reman = (select @reman/@fulllugs * 100)
		
		if @reman is null select @reman=0
		
		--slash percentage
		select @slash = 0
		select @slash = (select SUM(boardcount) from ProductionBoards where SortCode=11)
		if @slash is null select @slash = 0
		select @slashprevious = 0
		select @slashprevious = (select SUM(boardcount) from ProductionBoardsPrevious where SortCode=11 and shiftindex between @shiftstart and @shiftend)
		if @slashprevious is null select @slashprevious = 0
		select @slash = @slash + @slashprevious
		if @fulllugs >0
		  select @slash = (select @slash/@fulllugs * 100)
		
		if @slash is null select @slash=0
		
		--cn2 percentage
		select @fulllugscn2 = (select sum(boardcount) from ProductionBoards where sortcode>=0 and CN2<=1)
		select @cn2percent = 0
		if @fulllugscn2 is null select @fulllugscn2=0
		select @fulllugscn2previous = (select sum(boardcount) from ProductionBoardsprevious where sortcode>=0 and CN2<=1 and shiftindex between @shiftstart and @shiftend)
		if @fulllugscn2previous is null select @fulllugscn2previous=0
		select @fulllugscn2=@fulllugscn2+@fulllugscn2previous
		declare @cn2boards int,@cn2boardsprevious int
		select @cn2boards = (select SUM(boardcount) from ProductionBoards where CN2=2)
		if @cn2boards is null select @cn2boards=0
		select @cn2boardsprevious = (select SUM(boardcount) from ProductionBoardsprevious where CN2=2 and shiftindex between @shiftstart and @shiftend)
		if @cn2boardsprevious is null select @cn2boardsprevious=0
		select @cn2boards=@cn2boards+@cn2boardsprevious
		if @fulllugscn2>0
			select @cn2percent = @cn2boards/@fulllugscn2 *100
		if @cn2percent is null select @cn2percent=0
		
	end
	
	
	--trimloss
	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoards, runs
		where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=11))
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			insert into #temp select thicknominal=0,widthnominal=0,lengthin=0,lengthinrounded=0,lengthnominal=0,lengthlabel='No Data',totalinputpieces=0,totalinputvolume=0,totaloutputvolume=0,sorted=0,sortcode=0,cn2=0
		else
			insert into #temp
			select thicknominal,widthnominal,lengthin,floor(convert(smallint,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*lengthin*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),sorted,sortcode,cn2
			from ProductionBoards,products,lengths,runs
			where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=11))  --sorted boards and slashed boards
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,sortcode,cn2
			
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoards,runs 
		where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=11))
		and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=11))
		and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			insert into #temp select thicknominal=0,widthnominal=0,lengthin=0,lengthinrounded=0,lengthnominal=0,lengthlabel='No Data',totalinputpieces=0,totalinputvolume=0,totaloutputvolume=0,sorted=0,sortcode=0,cn2=0
		else
		begin
			insert into #temp
			select thicknominal,widthnominal,lengthin,floor(convert(smallint,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*lengthin*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),sorted,sortcode,cn2
			from ProductionBoards,products,lengths,runs
			where ((sorted=1 and (sortcode=1 or SortCode=22)) or (sorted=0 and sortcode = 11))
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,SortCode,cn2
			union
			select thicknominal,widthnominal,lengthin,floor(convert(smallint,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*lengthin*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),sorted,sortcode,cn2
			from ProductionBoardsPrevious,products,lengths,runs
			where ((sorted=1 and (sortcode=1 or SortCode=22)) or (sorted=0 and sortcode = 11))
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,SortCode,cn2
		end
	end
	
	update #temp set totalinputvolume = 0, lengthinrounded=0,lengthin=0 where cn2 = 2
	update #temp set lengthnominal=0 where sortcode=11
	update #temp set LengthInrounded=8,lengthin=8 where LengthIn=96
	update #temp set LengthInrounded=10,lengthin=10 where LengthIn=120
	update #temp set LengthInrounded=12,lengthin=12 where LengthIn=144
	update #temp set totalinputvolume=(thicknominal*widthnominal*lengthinrounded*totalinputpieces/12)
	update #temp set lengthnominal=CEILING(lengthnominal)
	update #temp set totalinputvolume=0 where cn2=2
	update #temp set totaloutputvolume=(thicknominal*widthnominal*lengthnominal*totalinputpieces/12)
	
	--select * from #temp
	
	if (select sum(totalinputvolume) from #temp) > 0
		select @trimloss = (select SUM(totalinputvolume-totaloutputvolume) / sum(totalinputvolume) *100) from #temp
	else
		select @trimloss=0
		
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
		
	
	update #tstat set lugfull=@lugfill,reman=@reman,trimloss=@trimloss,slash=@slash,packages=@numpacks,fullbays=@fullbays,cn2percent=@cn2percent
	update #tstat set inputvolume=(select SUM(totalinputvolume) from #temp)
	
	select * from #tstat

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
