SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Sept 5, 2010>
-- Description:	<Retrieves data for Crystal Downtime Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepDowntimeSummary]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int
	declare @shiftstarttime datetime, @shiftendtime datetime, @breakduration int
	declare @seconds int
    declare @minutes int, @dtdurationseconds int
    declare @hours int, @mindt int, @maxdt int, @mindtold int, @maxdtold int
    declare @subtotaldowntimedurationText varchar(50), @downtimedurationText varchar(50)
    declare @grandtotaldowntimedurationText varchar(50)
	declare @mindowntimedurationText varchar(50)
    declare @maxdowntimedurationText varchar(50)	
	declare @breakstart datetime, @breakend datetime

	select @maxshiftid = (select max(shiftindex) from shifts)

	select @shiftstart = (select shiftindexstart from reportheader)
	select @shiftend = (select shiftindexend from reportheader)
	select @shiftstarttime = (select shiftstart from shifts where shiftindex=@shiftstart)
	select @shiftendtime = (select shiftend from shifts where shiftindex=@shiftend)
	if @shiftendtime is null  --shift is still in progress
		select @shiftendtime = getdate()
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

	create table #downtime(
	id int identity,
	alarmid smallint,
	alarmtext varchar(100),
	priority smallint,
	starttime datetime,
	stoptime datetime,
	duration TIME(3),
	downtimeduration TIME(3),
	downtimedurationseconds int,
	downtimedurationfinal TIME(3),
	breakdurationminutes int,
	mindowntimeduration TIME(2),
	maxdowntimeduration TIME(2),
	subtotaldowntimeduration TIME(2),
	grandtotaldowntimeduration TIME(2),
	CONSTRAINT [PK_downtime] PRIMARY KEY CLUSTERED 
(
	[id] ASC
	
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
	

	if @shiftstart = @maxshiftid --current shift only
	begin
		select @count = (select count(*) from Alarms, runs
		where runs.runindex = alarms.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) 
		if @count=0
			select AlarmID=NULL,starttime=NULL,stoptime=NULL,downtimeduration=NULL,alarmtext='No Downtimes',downtimedurationfinal=NULL,breakdurationminutes=0,mindowntimeduration=NULL,maxdowntimeduration=NULL,subtotaldowntimeduration=NULL,grandtotaldowntimeduration=NULL
		else
			insert into #downtime select alarms.AlarmID,alarmtext,priority,starttime,stoptime,duration,duration,0,NULL,0,NULL,NULL,NULL,NULL
			from Alarms,Alarmsettings,runs
			where Alarms.alarmid=alarmsettings.alarmid
			and runs.runindex = Alarms.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and Alarms.shiftindex = @maxshiftid
			and alarmsettings.downtime=1
			and abs(DATEDIFF(ms,starttime,stoptime)) >= 1000
			order by starttime
		
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from Alarms, runs
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = alarms.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious =(select count(*) from AlarmsPrevious, runs
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = AlarmsPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		select @count = @count + @countprevious
		
		if @count = 0
			select AlarmID=NULL,starttime=NULL,stoptime=NULL,downtimeduration=NULL,alarmtext='No Downtimes',downtimedurationfinal=NULL,breakdurationminutes=0,mindowntimeduration=NULL,maxdowntimeduration=NULL,subtotaldowntimeduration=NULL,grandtotaldowntimeduration=NULL
		else
		begin
			insert into #downtime select alarms.AlarmID,alarmtext,priority,starttime,stoptime,duration,duration,0,NULL,0,NULL,NULL,NULL,NULL
			from Alarms,Alarmsettings,runs
			where Alarms.alarmid=alarmsettings.alarmid
			and runs.runindex = Alarms.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and Alarms.shiftindex between @shiftstart and @shiftend
			and alarmsettings.downtime=1
			and abs(DATEDIFF(ms,starttime,stoptime)) >= 1000
			union
			select AlarmsPrevious.AlarmID,alarmtext,priority,starttime,stoptime,duration,duration,0,NULL,0,NULL,NULL,NULL,NULL
			from AlarmsPrevious,Alarmsettings,runs
			where AlarmsPrevious.alarmid=alarmsettings.alarmid
			and runs.runindex = AlarmsPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and AlarmsPrevious.shiftindex between @shiftstart and @shiftend
			and AlarmsPrevious.StopTime <= (select ShiftEnd from shifts where ShiftIndex = @shiftend)
			and alarmsettings.downtime=1
			and abs(DATEDIFF(ms,starttime,stoptime)) >= 1000
			order by starttime
			--select * from #downtime
		end
	end

	-- figure out which items are downtimes, and work out time intersections etc.
	declare @aid smallint, @i smallint, @j smallint
	declare @start datetime, @stop datetime
	declare @start1 datetime, @stop1 datetime
	
	--select * from #downtime where alarmid=64
	
	--only consider the time interval starting at the shiftstarttime point
	update #downtime set starttime=@shiftstarttime where starttime < @shiftstarttime
	update #downtime set stoptime = @shiftendtime where duration is null
	update #downtime set downtimeduration = stoptime-starttime where duration is null
	delete from #downtime where stoptime<@shiftstarttime

	--delete identical dtIn
	DELETE #downtime
	FROM #downtime dT1
	WHERE EXISTS
	(
		SELECT *
		FROM #downtime dT2
		WHERE dT1.starttime = dT2.starttime
		AND (
				dT1.stoptime < dT2.stoptime
				OR (dT1.stoptime = dT2.stoptime AND dT1.id < dT2.id)
			)
	);

    --adjust stoptime to the max dates for overlapping section
    UPDATE #downtime
    SET stoptime = COALESCE((
        SELECT MAX(stoptime)
        FROM #downtime as t1
        WHERE t1.starttime < #downtime.stoptime 
			AND t1.stoptime > #downtime.starttime
        ), stoptime);

    -- Do the actual updates of stoptime
    UPDATE #downtime
    SET stoptime = COALESCE((
        SELECT MIN(starttime)
        FROM #downtime as t2
        WHERE t2.id <> #downtime.id AND
            t2.starttime >= #downtime.starttime 
			AND t2.starttime < #downtime.stoptime
        ), stoptime);

	update #downtime set downtimeduration = CONVERT(TIME, stoptime - starttime)
    --select * from #downtime
	--update downtimeduration in seconds, for use later, and calculate and subtract break durations, and remove times in between shifts (if multiple shifts are selected)
	select @i = (select min(id) from #downtime)
	while @i<= (select max(id) from #downtime)
	begin
		select @start = (select starttime from #downtime where id=@i)
		select @stop = (select stoptime from #downtime where id=@i)
		select @breakduration = 0
		select @j=(select min(id) from shiftbreaks where enabled=1)
		while @j<=(select max(id) from shiftbreaks where enabled=1)
		begin
			
			--account for cases when the event occurs partially within the break  or within the break completely, and also if the event crosses over a day boundary
			select @breakstart = (select breakstart from shiftbreaks where id=@j)
			select @breakend = (select breakend from shiftbreaks where id=@j)
			
			if (select abs(datediff(dd,@start,@stop))) = 0 
			begin
				--event occurs completely within the break
				if @start between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
				and @stop between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					--select @breakduration = @breakduration + (select abs(datediff(ss,@start,@stop))) 
					delete from #downtime where id=@i
				--event start within the break
				else if @start between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					select @breakduration = @breakduration + (select abs(datediff(ss,@start,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)))))
				--event stops within the break
				else if @stop between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					select @breakduration = @breakduration + (select abs(datediff(ss,@stop,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)))))
				--event includes the entire break
				else if convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) between @start and @stop
					select @breakduration = @breakduration + (select abs(datediff(ss,@breakstart,@breakend)))
			end
			else
			begin  --handle one day separate from the other, separating them at midnight
				select @start = (select starttime from #downtime where id=@i)
				select @stop = (select convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + '23:59:59:999'))
				--event occurs completely within the break
				if @start between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
				and @stop between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					--select @breakduration = @breakduration + (select abs(datediff(ss,@start,@stop))) 
					delete from #downtime where id=@i
				--event start within the break
				else if @start between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					select @breakduration = @breakduration + (select abs(datediff(ss,@start,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)))))
				--event stops within the break
				else if @stop between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					select @breakduration = @breakduration + (select abs(datediff(ss,@stop,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)))))
				--event includes the entire break
				else if convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) between @start and @stop
					select @breakduration = @breakduration + (select abs(datediff(ss,@breakstart,@breakend)))

				if @breakduration is null select @breakduration = 0 
				select @start = (select convert(datetime, datename(mm,stoptime) + ' ' + datename(dd,stoptime) + ' ' + datename(yy,stoptime) + ' ' + '0:0:0:1') from #downtime where id=@i)
				select @stop = (select stoptime from #downtime where id=@i)
				--event occurs completely within the break
				if @start between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
				and @stop between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					--select @breakduration = @breakduration + (select abs(datediff(ss,@start,@stop))) 
					delete from #downtime where id=@i
				--event start within the break
				else if @start between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					select @breakduration = @breakduration + (select abs(datediff(ss,@start,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)))))
				--event stops within the break
				else if @stop between convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) and convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakend) + ':' + datename(mi,@breakend) + ':' + datename(ss,@breakend)) 
					select @breakduration = @breakduration + (select abs(datediff(ss,@stop,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)))))
				--event includes the entire break
				else if convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,@breakstart) + ':' + datename(mi,@breakstart) + ':' + datename(ss,@breakstart)) between @start and @stop
					select @breakduration = @breakduration + (select abs(datediff(ss,@breakstart,@breakend)))
			end

			select @start = (select starttime from #downtime where id=@i)
			select @stop = (select stoptime from #downtime where id=@i)
			if (select abs(datediff(dd,@start,@stop))) > 1
				select @breakduration = @breakduration * abs(datediff(dd,@start,@stop))
			if @breakduration is null select @breakduration = 0 
			
			select @j=(select min(id) from shiftbreaks where enabled=1 and id>@j)
		end
		--check for breaks between shifts, if multiple shifts are selected
		if (@shiftend-@shiftstart) > 0
		begin
			declare @counter int, @shiftbreak1 datetime, @shiftbreak2 datetime, @shiftbreakduration int
			select @shiftbreakduration = 0
			select @counter = @shiftstart
			while @counter < @shiftend
			begin
				select @shiftbreak1 = (select shiftend from shifts where shiftindex = @counter)
				select @shiftbreak2 = (select shiftstart from shifts where shiftindex = @counter+1)
				if @shiftbreak1 <> @shiftbreak2
				begin
					--event occurs during the shift break
					if @start > @shiftbreak1 and @stop < @shiftbreak2
						delete from #downtime where id=@i
					--event starts during the shift break
					else if @start between @shiftbreak1 and @shiftbreak2
						select @shiftbreakduration = @shiftbreakduration + abs(datediff(ss,@start,@shiftbreak2))
					--event stops during the shift break
					else if @stop between @shiftbreak1 and @shiftbreak2
						select @shiftbreakduration = @shiftbreakduration + abs(datediff(ss,@stop,@shiftbreak1))
					--entire shift break occurs during the alarm event
					if @start < @shiftbreak1 and @stop > @shiftbreak2
						select @shiftbreakduration = @shiftbreakduration + abs(datediff(ss,@shiftbreak1,@shiftbreak2))
					
					if @shiftbreakduration is null select @shiftbreakduration = 0
				end
				select @counter=@counter + 1
			end
			select @breakduration = @breakduration + @shiftbreakduration
		end

		
		
		update #downtime set breakdurationminutes = @breakduration/60 where id=@i
		set @dtdurationseconds = (select DATEDIFF(SECOND, 0, downtimeduration) from #downtime where id=@i) 

		update #downtime set downtimedurationseconds = @dtdurationseconds where id=@i
		update #downtime set downtimedurationseconds = downtimedurationseconds - @breakduration where id=@i

        update #downtime set downtimedurationfinal = (select DATEADD(ms, SUM(downtimedurationseconds) * 1000, 0) FROM #downtime where id=@i) where id=@i			
		
		select @i = (select min(id) from #downtime where id>@i)
	end
	delete from #downtime where downtimedurationfinal <= '00:00:00'	

    --subtotal
    UPDATE  #downtime
    SET     subtotaldowntimeduration = t
    FROM #downtime d
    INNER JOIN 
        (SELECT alarmid, DATEADD(ms, SUM(downtimedurationseconds) * 1000, 0) AS t 
        FROM #downtime 
        GROUP BY alarmid) AS s
    ON d.alarmid = s.alarmid 
    
    --min
    UPDATE  #downtime
    SET     mindowntimeduration = t
    FROM #downtime d
    INNER JOIN 
        (SELECT alarmid, DATEADD(ms, MIN(downtimedurationseconds) * 1000, 0) AS t 
        FROM #downtime 
        GROUP BY alarmid) AS s
    ON d.alarmid = s.alarmid 

    --max
    UPDATE  #downtime
    SET     maxdowntimeduration = t
    FROM #downtime d
    INNER JOIN 
        (SELECT alarmid, DATEADD(ms, MAX(downtimedurationseconds) * 1000, 0) AS t 
        FROM #downtime 
        GROUP BY alarmid) AS s
    ON d.alarmid = s.alarmid 

	--calculate grand total downtime 
    update #downtime set grandtotaldowntimeduration = (SELECT DATEADD(ms, SUM(downtimedurationseconds) * 1000, 0) FROM #downtime)
	
	
	if @count> 0
		select alarmid,alarmtext,starttime,stoptime,downtimeduration=convert(varchar,downtimeduration,14),downtimedurationfinal,breakdurationminutes,mindowntimeduration,maxdowntimeduration,subtotaldowntimeduration,grandtotaldowntimeduration from #downtime
		order by alarmid,downtimeduration desc
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
