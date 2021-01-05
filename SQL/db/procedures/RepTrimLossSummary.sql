SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Trim Loss Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepTrimLossSummary]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int
	declare @slashcode smallint
	select @maxshiftid = (select max(shiftindex) from shifts)
	select @slashcode = (select rejectflag from BoardRejects where RejectDescription like '%slash%')
	
	declare @trimlossfactor real
	
	select @trimlossfactor = (select trimlossfactor/100 from WEBSortSetup)

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
		lengthinrounded smallint,
		lengthnominal real,
		lengthlabel varchar(50),
		totalinputpieces int,
		totalinputvolume real,
		totaloutputvolume real,
		sorted smallint,
		sortcode smallint,
		cn2 smallint,
		trimlossfactor real
	)

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoards, runs
		where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=@slashcode))
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			insert into #temp select thicknominal=0,widthnominal=0,lengthin=0,lengthinrounded=0,lengthnominal=0,lengthlabel='No Data',totalinputpieces=0,totalinputvolume=0,totaloutputvolume=0,sorted=0,sortcode=0,cn2=0,trimlossfactor=0
		else
			insert into #temp
			select thicknominal,widthnominal,lengthin,floor(convert(smallint,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*lengthin*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)*@trimlossfactor,sorted,sortcode,cn2,@trimlossfactor
			from ProductionBoards,products,lengths,runs
			where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=@slashcode))  --sorted boards and slashed boards
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
		where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=@slashcode))
		and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where ((sorted=1 and (sortcode=1 or SortCode=22)) or (Sorted=0 and SortCode=@slashcode))
		and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			insert into #temp select thicknominal=0,widthnominal=0,lengthin=0,lengthinrounded=0,lengthnominal=0,lengthlabel='No Data',totalinputpieces=0,totalinputvolume=0,totaloutputvolume=0,sorted=0,sortcode=0,cn2=0,trimlossfactor=0
		else
		begin
			insert into #temp
			select thicknominal,widthnominal,lengthin,floor(convert(smallint,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*lengthin*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)*@trimlossfactor,sorted,sortcode,cn2,@trimlossfactor
			from ProductionBoards,products,lengths,runs
			where ((sorted=1 and (sortcode=1 or SortCode=22)) or (sorted=0 and sortcode = @slashcode))
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,SortCode,cn2
			union
			select thicknominal,widthnominal,lengthin,floor(convert(smallint,lengthin)),lengthnominal/12,lengthlabel,
			totalinputpieces=sum(boardcount),totalinputvolume=sum(thicknominal*widthnominal*lengthin*boardcount/12),totaloutputvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)*@trimlossfactor,sorted,sortcode,cn2,@trimlossfactor
			from ProductionBoardsPrevious,products,lengths,runs
			where ((sorted=1 and (sortcode=1 or SortCode=22)) or (sorted=0 and sortcode = @slashcode))
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,lengthin,lengthnominal,lengthlabel,sorted,SortCode,cn2
		end
	end
	
	update #temp set lengthlabel=(select rejectdescription from boardrejects where rejectflag=sortcode)
	where sorted=0 
	update #temp set totalinputvolume = 0,lengthin=0,lengthinrounded=0 where cn2 = 2
	--update #temp set cn2=0
	update #temp set lengthnominal=0 where sortcode=@slashcode
	update #temp set LengthInrounded=8,lengthin=8 where LengthIn=96
	update #temp set LengthInrounded=10,lengthin=10 where LengthIn=120
	update #temp set LengthInrounded=12,lengthin=12 where LengthIn=144
	update #temp set totalinputvolume=(thicknominal*widthnominal*lengthinrounded*totalinputpieces/12)
	
	update #temp set totalinputvolume=0 where cn2=2
	
	update #temp set lengthnominal=CEILING(lengthnominal)
	update #temp set totaloutputvolume=(thicknominal*widthnominal*lengthnominal*totalinputpieces/12)
	--select SUM(totalinputvolume) from #temp
	--select SUM(totaloutputvolume) from #temp
	select * from #temp 
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
