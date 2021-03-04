SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
CREATE procEDURE [dbo].[RepRejectBoardSummary]
	
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
	(rejectdescription varchar(50),
	prodlabel varchar(50),
	thicknominal real,
	widthnominal real,
	gradelabel varchar(50),
	lengthnominal real,
	lengthlabel varchar(50),
	totalpieces int,
	totalvolume real,
	shiftvolume real,
	shiftpieces int)

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoards, runs
		where sorted=0 and sortcode>=0
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend
		) = 0
		insert into #t
			select rejectdescription='No Data',prodlabel='No Data',thicknominal=0,widthnominal=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0,shiftvolume=0,shiftpieces=0
			
		else
			insert into #t
			select rejectdescription, prodlabel,thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*(lengthin*12)*boardcount/144),0,0
			from ProductionBoards,products,lengths,grades,runs, boardrejects
			where sorted=0 and sortcode>=0 and SortCode<>11
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by rejectdescription,thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			insert into #t
			select rejectdescription, prodlabel,thickactual,widthactual,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thickactual*widthactual*(lengthin*12)*boardcount/144),0,0
			from ProductionBoards,products,lengths,grades,runs, boardrejects
			where SortCode=11
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by rejectdescription,thickactual,widthactual,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			
			
			update #t set shiftvolume=(select sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoards,products,lengths,runs
			where sorted=1 and (sortcode=1)
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			
			)
			
			update #t set shiftpieces=(select sum(boardcount)
			from ProductionBoards,products,lengths,runs
			where sorted=1 and (sortcode=1)
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			)
			
			
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoards,runs 
		where sorted=0 and sortcode>=0 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend
		)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where sorted=0 and sortcode>=0 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend
		)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
		insert into #t
			select  rejectdescription='No Data',prodlabel='No Data',thicknominal=0,widthnominal=0,gradelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0,shiftvolume=0,shiftpieces=0
		else
		begin
			insert into #t
			select rejectdescription, prodlabel,thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*(lengthin*12)*boardcount/144),0,0
			from ProductionBoards,products,lengths,grades,runs, boardrejects
			where sorted=0 and sortcode>=0 and sortcode<>11
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			group by rejectdescription,thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			union
			select rejectdescription, prodlabel,thicknominal,widthnominal,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*(lengthin*12)*boardcount/144),0,0
			from ProductionBoardsPrevious,products,lengths,grades,runs, boardrejects
			where sorted=0 and sortcode>=0 and sortcode<>11
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by rejectdescription,thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			insert into #t
			select rejectdescription, prodlabel,thickactual,widthactual,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thickactual*widthactual*(lengthin*12)*boardcount/144),0,0
			from ProductionBoards,products,lengths,grades,runs, boardrejects
			where sortcode=11
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			group by rejectdescription,thickactual,widthactual,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			union
			select rejectdescription, prodlabel,thickactual,widthactual,gradelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thickactual*widthactual*(lengthin*12)*boardcount/144),0,0
			from ProductionBoardsPrevious,products,lengths,grades,runs, boardrejects
			where sortcode=11
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by rejectdescription,thickactual,widthactual,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			
			declare @sv real, @svp real, @sp int, @spp int
			select @sv=(select sum(thicknominal*widthnominal*(lengthin*12)*boardcount/144)
			from ProductionBoards,products,lengths,runs
			where sorted=1 and (sortcode=1)
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			and cn2<=1)
			if @sv is null select @sv=0
			
			select @svp=(select sum(thicknominal*widthnominal*(lengthin*12)*boardcount/144)
			from ProductionBoardsPrevious,products,lengths,runs
			where ((sorted=1 and sortcode=1) or (sorted=0 and sortcode=6))
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			and cn2<=1)
			if @svp is null select @svp=0
			select @sv = @sv+@svp
					
			update #t set shiftvolume=@sv
			
			select @sp=(select sum(boardcount)
			from ProductionBoards,products,lengths,runs
			where sorted=1 and (sortcode=1)
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			)
			if @sp is null select @sp=0
			
			select @spp=(select sum(boardcount)
			from ProductionBoardsPrevious,products,lengths,runs
			where sorted=1 and (sortcode=1)
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			)
			if @spp is null select @spp=0
			select @sp = @sp+@spp
					
			update #t set shiftpieces=@sp
		end
	end
	update #t set totalpieces=1,totalvolume=1 where totalpieces is null
	
	if (select count(*) from #t where rejectdescription='No Sort') = 0
		insert into #t select 'No Sort','',0,0,'',0,'',0,0,0,0
	
	select * from #t

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
