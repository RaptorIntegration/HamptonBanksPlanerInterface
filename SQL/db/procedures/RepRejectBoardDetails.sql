SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <October 14, 2010>
-- Description:	<Retrieves data for Crystal Rejct Board Summary Report>
-- =============================================
create PROCEDURE [dbo].[RepRejectBoardDetails]
	
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

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionBoards, runs
		where sorted=0 and SortCode=30
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			select rejectdescriptiond='No Data',prodlabeld='No Data',thicknominald=0,widthnominald=0,gradelabeld='No Data',lengthnominald=0,lengthlabeld='No Data',totalpiecesd=0,totalvolumed=0
			
		else
			select  rejectdescription as 'rejectdescriptiond', prodlabel as 'prodlabeld', thicknominal as 'thicknominald',widthnominal as 'widthnominald',gradelabel as 'gradelabeld',lengthnominal as 'lengthnominald' ,lengthlabel as 'lengthlabeld',totalpiecesd=sum(boardcount),totalvolumed=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoards,products,lengths,grades,runs, boardrejects
			where sorted=0 and SortCode=30
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by rejectdescription,thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionBoards,runs 
		where sorted=0 and SortCode=30 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = productionboards.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionBoardsPrevious,runs
		where sorted=0 and SortCode=30 and shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionBoardsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			select rejectdescriptiond='No Data',prodlabeld='No Data',thicknominald=0,widthnominald=0,gradelabeld='No Data',lengthnominald=0,lengthlabeld='No Data',totalpiecesd=0,totalvolumed=0
		else
		begin
			select  rejectdescription as 'rejectdescriptiond', prodlabel as 'prodlabeld', thicknominal as 'thicknominald',widthnominal as 'widthnominald',gradelabel as 'gradelabeld',lengthnominal as 'lengthnominald' ,lengthlabel as 'lengthlabeld',totalpiecesd=sum(boardcount),totalvolumed=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoards,products,lengths,grades,runs, boardrejects
			where sorted=0 and SortCode=30
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = productionBoards.prodid
			and lengths.lengthid = productionBoards.lengthid
			and runs.runindex = productionboards.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoards.shiftindex = @maxshiftid
			group by rejectdescription,thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			union
			select  rejectdescription as 'rejectdescriptiond', prodlabel as 'prodlabeld', thicknominal as 'thicknominald',widthnominal as 'widthnominald',gradelabel as 'gradelabeld',lengthnominal as 'lengthnominald' ,lengthlabel as 'lengthlabeld',totalpiecesd=sum(boardcount),totalvolumed=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)
			from ProductionBoardsPrevious,products,lengths,grades,runs, boardrejects
			where sorted=0 and SortCode=30
			and sortcode=rejectflag
			and products.gradeid=grades.gradeid
			and products.prodid = ProductionBoardsPrevious.prodid
			and lengths.lengthid = ProductionBoardsPrevious.lengthid
			and runs.runindex = ProductionBoardsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionBoardsPrevious.shiftindex between @shiftstart and @shiftend
			group by rejectdescription,thicknominal,widthnominal,prodlabel,gradelabel,lengthnominal,lengthlabel
			
			
		end
	end

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
