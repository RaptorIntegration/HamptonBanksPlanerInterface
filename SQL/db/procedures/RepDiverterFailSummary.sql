SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Dec 10, 2010>
-- Description:	<Retrieves data for Crystal Diverter Failure Summary Report>
-- =============================================
create PROCEDURE [dbo].[RepDiverterFailSummary]
	
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
		if (select count(*) from ProductionDiverterFail, runs
		where runs.runindex = ProductionDiverterFail.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			select BayID='No Data',boardcount=0
		else
			select BayID,boardcount=sum(boardcount)
			from ProductionDiverterFail,runs
			where runs.runindex = ProductionDiverterFail.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionDiverterFail.shiftindex = @maxshiftid
			group by BayID
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionDiverterFail,runs 
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionDiverterFail.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionDiverterFailPrevious,runs
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionDiverterFailPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			select BayID='No Data',boardcount=0
		else
		begin
			select BayID,boardcount=sum(boardcount)
			from ProductionDiverterFail,runs
			where runs.runindex = ProductionDiverterFail.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionDiverterFail.shiftindex between @shiftstart and @shiftend
			group by BayID
			union
			select BayID,boardcount=sum(boardcount)
			from ProductionDiverterFailPrevious,runs
			where runs.runindex = ProductionDiverterFailPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionDiverterFailPrevious.shiftindex between @shiftstart and @shiftend
			group by BayID
		end
	end


END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
