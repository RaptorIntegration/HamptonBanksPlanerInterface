SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Sept 5, 2010>
-- Description:	<Retrieves data for Crystal Alarms Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepAlarms]
	
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
		if (select count(*) from Alarms, runs
		where runs.runindex = alarms.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			select AlarmID=NULL,starttime=NULL,stoptime=NULL,duration=NULL,data=NULL,alarmtext='No Alarms',severitygraphic=NULL
		else
			select alarms.AlarmID,starttime,stoptime,duration,alarms.data,alarmtext,severitygraphic
			from Alarms,Alarmsettings,alarmseverity,runs
			where Alarms.alarmid=alarmsettings.alarmid
			and severity=severityid
			and runs.runindex = Alarms.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and Alarms.shiftindex = @maxshiftid
			order by starttime
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from Alarms,runs 
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = Alarms.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from AlarmsPrevious,runs
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = AlarmsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			select AlarmID=NULL,starttime=NULL,stoptime=NULL,duration=NULL,data=NULL,alarmtext='No Alarms',severitygraphic=NULL
		else
		begin
			select alarms.AlarmID,starttime,stoptime,duration,alarms.data,alarmtext,severitygraphic
			from Alarms,Alarmsettings,alarmseverity,runs
			where Alarms.alarmid=alarmsettings.alarmid
			and severity=severityid
			and runs.runindex = Alarms.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and Alarms.shiftindex = @maxshiftid
			union
			select AlarmsPrevious.AlarmID,starttime,stoptime,duration,AlarmsPrevious.data,alarmtext,severitygraphic
			from AlarmsPrevious,Alarmsettings,alarmseverity,runs
			where AlarmsPrevious.alarmid=alarmsettings.alarmid
			and severity=severityid
			and runs.runindex = AlarmsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and AlarmsPrevious.shiftindex between @shiftstart and @shiftend
			order by starttime
		end
	end

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
