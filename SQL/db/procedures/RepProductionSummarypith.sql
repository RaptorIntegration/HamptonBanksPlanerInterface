SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Production Summary Report>
-- =============================================
create PROCEDURE [dbo].[RepProductionSummarypith]
	
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
	
	create table #t
	(
	prodlabel varchar(100),
	thicknominal real,
	widthnominal real,
	gradelabel varchar(50),
	lengthnominal real,
	lengthlabel varchar(50),
	totalpieces int,
	totalvolume real,
	grandtotalpieces int,
	totalpiecesnopithdata int
	)

	if @shiftstart = @maxshiftid --current shift only
	begin
	
		if (select count(*) from ProductionBoards, runs
		where sorted=1 and (SortCode= 1)
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
		insert into #t
			select prodlabel='No Data',thicknominal=0,widthnominal=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0,grandtotalpieces=0,totalpiecesnopithdata=0
		else
			insert into #t
			select prodlabel,thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),0,0
			from ProductionBoards,products,lengths,grades,runs
			where sorted=1 and (SortCode= 1)
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			
		update #t set grandtotalpieces = (select sum(boardcount) from productionboards,runs
		where sorted=1 and (sortcode=1 or sortcode=22)
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend
		and ProductionBoards.shiftindex = @maxshiftid)
		update #t set totalpiecesnopithdata = (select sum(boardcount) from productionboards,runs
		where sorted=1 and sortcode=22
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend
		and ProductionBoards.shiftindex = @maxshiftid)
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoards,runs 
		where sorted=1 and ( SortCode= 1) and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where sorted=1 and ( SortCode= 1) and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			insert into #t
				select prodlabel='No Data',thicknominal=0,widthnominal=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0,grandtotalpieces=0,totalpiecesnopithdata=0
		else
		begin
			insert into #t
			select prodlabel,thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),0,0
			from ProductionBoards,products,lengths,grades,runs
			where sorted=1 and ( SortCode= 1)
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			union
			select prodlabel,thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144),0,0
			from ProductionBoardsPrevious,products,lengths,grades,runs
			where sorted=1 and ( SortCode= 1)
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			declare @gtp int, @gtp2 int, @tpnpd int, @tpnpd2 int
			select @gtp = (select sum(boardcount) from productionboards,runs
			where sorted=1 and (sortcode=1 or sortcode=22)
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid)
			select @gtp2 = (select sum(boardcount) from productionboardsprevious,runs
			where sorted=1 and (sortcode=1 or sortcode=22)
			and runs.runindex = productionboardsprevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and productionboardsprevious.shiftindex between @shiftstart and @shiftend)
			if @gtp is null select @gtp=0
			if @gtp2 is null select @gtp2=0
			
			select @tpnpd = (select sum(boardcount) from productionboards,runs
			where sorted=1 and sortcode=22
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid)
			select @tpnpd2 = (select sum(boardcount) from productionboardsprevious,runs
			where sorted=1 and sortcode=22
			and runs.runindex = productionboardsprevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and productionboardsprevious.shiftindex between @shiftstart and @shiftend)
			if @tpnpd is null select @tpnpd=0
			if @tpnpd2 is null select @tpnpd2=0
			
			update #t set grandtotalpieces = @gtp + @gtp2
			update #t set totalpiecesnopithdata = @tpnpd + @tpnpd2
		end
	end

	select * from #t
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
