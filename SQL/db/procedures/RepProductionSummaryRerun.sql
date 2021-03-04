SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
create procEDURE [dbo].[RepProductionSummaryRerun]
	
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
	
	create table #t (
	sortcode smallint,prodlabel1 varchar(1000),prodlabel varchar(1000),nominalthickness real,nominalwidth real ,gradelabel varchar(50),lengthnominal real,lengthlabel varchar(50),
	totalpieces int,totalvolume real)
			
	--select @recipeidstart=(select recipeid from Recipes where Online=1)
	--select @recipeidend= @recipeidstart

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoardsRerun, runs
		where sorted=1 and sortcode=1
		and runs.runindex = ProductionBoardsRerun.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
		insert into #t
			select sortcode='1',prodlabel1='No Data',prodlabel='No Data',nominalthickness=0,nominalwidth=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0
		else
		insert into #t
			select sortcode,prodlabel1=prodlabel,prodlabel=convert(varchar,thicknominal) + 'x' + convert(varchar,widthnominal),thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoardsRerun,products,lengths,grades,runs
			where sorted=1 and sortcode=1
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsRerun.prodid
			and lengths.lengthid = ProductionBoardsRerun.lengthid
			and runs.runindex = ProductionBoardsRerun.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsRerun.shiftindex = @maxshiftid
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,sortcode
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoardsRerun,runs 
		where sorted=1 and sortcode=1 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsRerun.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where sorted=1 and sortcode=1 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
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
			from ProductionBoardsRerun,products,lengths,grades,runs
			where sorted=1 and sortcode=1
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsRerun.prodid
			and lengths.lengthid = ProductionBoardsRerun.lengthid
			and runs.runindex = ProductionBoardsRerun.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsRerun.shiftindex between @shiftstart and @shiftend
			
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,sortcode
			union
			select sortcode,prodlabel1=prodlabel,prodlabel=convert(varchar,thicknominal) + 'x' + convert(varchar,widthnominal),thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoardsRerunPrevious,products,lengths,grades,runs
			where sorted=1 and sortcode=1
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsRerunPrevious.prodid
			and lengths.lengthid = ProductionBoardsRerunPrevious.lengthid
			and runs.runindex = ProductionBoardsRerunPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsRerunPrevious.shiftindex between @shiftstart and @shiftend
			
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,sortcode
		end
	end
	
	update #t set prodlabel='2x4',nominalthickness=2,nominalwidth=4 where prodlabel='4x2'
    select * from #t order by lengthnominal,lengthlabel
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
