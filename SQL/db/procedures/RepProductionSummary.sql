SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Production Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepProductionSummary]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int
	declare @trimlossfactor real
	
	select @trimlossfactor = (select trimlossfactor/100 from WEBSortSetup)

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

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoards, runs
		where sorted=1 and (sortcode=1 or SortCode=22)
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			select prodlabel='No Data',thicknominal=0,widthnominal=0,gradelabel='No Data',moisturelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0
		else
			select prodlabel,thicknominal,widthnominal,gradelabel,moisturelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144*@trimlossfactor)
			from ProductionBoards,products,lengths,grades,moistures,runs
			where sorted=1 and (sortcode=1 or SortCode=22)
			and products.gradeid=grades.gradeid
			and products.MoistureID=moistures.moistureid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,moisturelabel
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoards,runs 
		where sorted=1 and (sortcode=1 or SortCode=22) and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where sorted=1 and (sortcode=1 or SortCode=22) and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			select prodlabel='No Data',thicknominal=0,widthnominal=0,gradelabel='No Data',moisturelabel='No Data',lengthnominal=0,lengthlabel='No Data',totalpieces=0,totalvolume=0
		else
		begin
			select prodlabel,thicknominal,widthnominal,gradelabel,moisturelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144*@trimlossfactor)
			from ProductionBoards,products,lengths,grades,moistures,runs
			where sorted=1 and (sortcode=1 or SortCode=22)
			and products.gradeid=grades.gradeid
			and products.MoistureID=moistures.moistureid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,moisturelabel
			union
			select prodlabel,thicknominal,widthnominal,gradelabel,moisturelabel,lengthnominal,lengthlabel,totalpieces=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144*@trimlossfactor)
			from ProductionBoardsPrevious,products,lengths,grades,moistures,runs
			where sorted=1 and (sortcode=1 or SortCode=22)
			and products.gradeid=grades.gradeid
			and products.MoistureID=moistures.moistureid
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel,moisturelabel
		end
	end

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
