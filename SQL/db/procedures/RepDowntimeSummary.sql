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
	duration datetime,
	downtimeduration datetime,
	downtimedurationseconds int,
	downtimedurationfinal varchar(50),
	breakdurationminutes int,
	mindowntimeduration varchar(50),
	maxdowntimeduration varchar(50),
	subtotaldowntimeduration varchar(50),
	grandtotaldowntimeduration varchar(50),
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
	
--select * from #downtime	
	
	

	select @i = (select min(id) from #downtime)
	while @i<= (select max(id) from #downtime)
	begin
		select @aid = (select alarmid from #downtime where id=@i)
		select @start = (select starttime from #downtime where id=@i)
		select @stop = (select stoptime from #downtime where id=@i)
		
		--TO DO: examine priorities to determine if a higher priority alarm should take the downtime
		--for now, priority is not considered	
		
	
		--reconcile downtime alarms that start within another alarm, but ended after it
		if (select count(*) from #downtime where @start between starttime and stoptime and @stop > stoptime and id<>@i) > 0
		begin
			declare @id smallint
			select @id = (select min(id) from #downtime where id<>@i and stoptime = (select max(stoptime) from #downtime where id<>@i
			and @start between starttime and stoptime and @stop > stoptime))
			--update #downtime set downtimeduration = @stop-(select stoptime from #downtime where id=@id) where id = @i 	
			update #downtime set starttime =  (select stoptime from #downtime where id=@id) where id=@i		
		end
		
		select @i=@i+1
	end

	--eliminate any alarms that came and went entirely during another alarm
	select @i = (select min(id) from #downtime)
	while @i <= (select max(id) from #downtime)
	begin
		select @start = (select starttime from #downtime where id=@i)
		select @stop = (select stoptime from #downtime where id=@i)
		
		select @j = (select min(id) from #downtime)
		while @j <= (select max(id) from #downtime)
		begin
			if @j <> @i
			begin
				select @start1 = (select starttime from #downtime where id=@j)
				select @stop1 = (select stoptime from #downtime where id=@j)
				
				if (@start1 between @start and @stop and @stop1 between @start and @stop)
					delete from #downtime where id = @j
			end
			select @j=@j+1
		end
		select @i=@i+1
	end

	update #downtime set downtimeduration = convert(varchar,stoptime-starttime,14)
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
		select @dtdurationseconds = (select sum(datepart(hh,downtimeduration)*3600) + sum(datepart(mi,downtimeduration)*60) + sum(datepart(ss,downtimeduration)) from #downtime where id=@i) 

		update #downtime set downtimedurationseconds = @dtdurationseconds where id=@i
		update #downtime set downtimedurationseconds = downtimedurationseconds - @breakduration where id=@i
		
		select @seconds = (select downtimedurationseconds from #downtime where id=@i)
		select @minutes=@seconds/60
		select @hours=@seconds/3600
		select @minutes=@minutes-(@hours*60)
		select @seconds=@seconds-(@hours*3600)-(@minutes*60)
		
		select @downtimedurationText=''

		if @hours<10
			select @downtimedurationText='0' + convert(varchar(10),@hours)
		else
			select @downtimedurationText=convert(varchar(10),@hours)
		select @downtimedurationText=@downtimedurationText + ':'

		if @minutes<10
			select @downtimedurationText=@downtimedurationText + '0' + convert(varchar(10),@minutes)
		else
			select @downtimedurationText=@downtimedurationText + convert(varchar(10),@minutes)
		select @downtimedurationText=@downtimedurationText + ':'
		if @seconds<10
			select @downtimedurationText=@downtimedurationText + '0' + convert(varchar(10),@seconds)
		else

		select @downtimedurationText=@downtimedurationText + convert(varchar(10),@seconds)
		update #downtime set downtimedurationfinal = @downtimedurationText where id=@i
		
		select @i = (select min(id) from #downtime where id>@i)
	end
	delete from #downtime where downtimedurationseconds < 0
	delete from #downtime where datepart(hh,downtimeduration) = 0 and DATEPART(mi,downtimeduration) = 0 and DATEPART(ss,downtimeduration) = 0

	--calculate min/max and subtotal downtimes
	select @i = (select min(alarmid) from #downtime)
	while @i<= (select max(alarmid) from #downtime)
	begin
		--subtotal
		select @seconds = (select sum(downtimedurationseconds) from #downtime where alarmid=@i)
		select @minutes=@seconds/60
		select @hours=@seconds/3600
		select @minutes=@minutes-(@hours*60)
		select @seconds=@seconds-(@hours*3600)-(@minutes*60)
		
		select @subtotaldowntimedurationText=''

		if @hours<10
			select @subtotaldowntimedurationText='0' + convert(varchar(10),@hours)
		else
			select @subtotaldowntimedurationText=convert(varchar(10),@hours)
		select @subtotaldowntimedurationText=@subtotaldowntimedurationText + ':'

		if @minutes<10
			select @subtotaldowntimedurationText=@subtotaldowntimedurationText + '0' + convert(varchar(10),@minutes)
		else
			select @subtotaldowntimedurationText=@subtotaldowntimedurationText + convert(varchar(10),@minutes)
		select @subtotaldowntimedurationText=@subtotaldowntimedurationText + ':'
		if @seconds<10
			select @subtotaldowntimedurationText=@subtotaldowntimedurationText + '0' + convert(varchar(10),@seconds)
		else

		select @subtotaldowntimedurationText=@subtotaldowntimedurationText + convert(varchar(10),@seconds)
		update #downtime set subtotaldowntimeduration = @subtotaldowntimedurationText where alarmid=@i
		--min
		select @mindt = (select min(downtimedurationseconds) from #downtime where alarmid=@i) 
		select @mindowntimedurationText=''
		select @minutes=@mindt/60
		select @hours=@mindt/3600
		select @minutes=@minutes-(@hours*60)
		select @mindt=@mindt-(@hours*3600)-(@minutes*60)

		if @hours<10
			select @mindowntimedurationText='0' + convert(varchar(10),@hours)
		else
			select @mindowntimedurationText=convert(varchar(10),@hours)
		select @mindowntimedurationText=@mindowntimedurationText + ':'

		if @minutes<10
			select @mindowntimedurationText=@mindowntimedurationText + '0' + convert(varchar(10),@minutes)
		else
			select @mindowntimedurationText=@mindowntimedurationText + convert(varchar(10),@minutes)
		select @mindowntimedurationText=@mindowntimedurationText + ':'
		if @mindt<10
			select @mindowntimedurationText=@mindowntimedurationText + '0' + convert(varchar(10),@mindt)
		else

		select @mindowntimedurationText=@mindowntimedurationText + convert(varchar(10),@mindt)
		update #downtime set mindowntimeduration = @mindowntimedurationText where alarmid=@i
		--max
		select @maxdt = (select max(downtimedurationseconds) from #downtime where alarmid=@i) 
		select @maxdowntimedurationText=''
		select @minutes=@maxdt/60
		select @hours=@maxdt/3600
		select @minutes=@minutes-(@hours*60)
		select @maxdt=@maxdt-(@hours*3600)-(@minutes*60)

		if @hours<10
			select @maxdowntimedurationText='0' + convert(varchar(10),@hours)
		else
			select @maxdowntimedurationText=convert(varchar(10),@hours)
		select @maxdowntimedurationText=@maxdowntimedurationText + ':'

		if @minutes<10
			select @maxdowntimedurationText=@maxdowntimedurationText + '0' + convert(varchar(10),@minutes)
		else
			select @maxdowntimedurationText=@maxdowntimedurationText + convert(varchar(10),@minutes)
		select @maxdowntimedurationText=@maxdowntimedurationText + ':'
		if @maxdt<10
			select @maxdowntimedurationText=@maxdowntimedurationText + '0' + convert(varchar(10),@maxdt)
		else

		select @maxdowntimedurationText=@maxdowntimedurationText + convert(varchar(10),@maxdt)
		update #downtime set maxdowntimeduration = @maxdowntimedurationText where alarmid=@i
		select @i = (select min(alarmid) from #downtime where alarmid>@i)
	end

	--calculate grand total downtime 
	select @seconds = (select sum(downtimedurationseconds) from #downtime)
    select @minutes=@seconds/60
	select @hours=@seconds/3600
	select @minutes=@minutes-(@hours*60)
	select @seconds=@seconds-(@hours*3600)-(@minutes*60)

	select @grandtotaldowntimedurationText=''

	if @hours<10
		select @grandtotaldowntimedurationText='0' + convert(varchar(10),@hours)
	else
		select @grandtotaldowntimedurationText=convert(varchar(10),@hours)
	select @grandtotaldowntimedurationText=@grandtotaldowntimedurationText + ':'

	if @minutes<10
		select @grandtotaldowntimedurationText=@grandtotaldowntimedurationText + '0' + convert(varchar(10),@minutes)
	else
		select @grandtotaldowntimedurationText=@grandtotaldowntimedurationText + convert(varchar(10),@minutes)
	select @grandtotaldowntimedurationText=@grandtotaldowntimedurationText + ':'
	if @seconds<10
		select @grandtotaldowntimedurationText=@grandtotaldowntimedurationText + '0' + convert(varchar(10),@seconds)
	else

	select @grandtotaldowntimedurationText=@grandtotaldowntimedurationText + convert(varchar(10),@seconds)
	update #downtime set grandtotaldowntimeduration = @grandtotaldowntimedurationText

	
	
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
