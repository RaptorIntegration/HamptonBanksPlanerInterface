SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Aug 4, 2010>
-- Description:	<Retrieves alarms or default messages to display on the display board>
-- =============================================
CREATE PROCEDURE [dbo].[selectDisplayMessages1]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	
	
	update alarmdefaultaccumulatedtime set accumulatedtime = accumulatedtime + abs(datediff(ss,getdate(),lasttimestamp))
	update AlarmDefaultAccumulatedTime set lasttimestamp = GETDATE()
	

	create table #tempmessages
	(
	ID int identity,
	AlarmID int,
	DisplayText varchar(100),
	Severity smallint,
	Priority smallint,
	DisplayTime real,
	BlankTime real,
	AlarmCount smallint,
	Data int,
	DataRequired bit)

	if (select count(*) from alarms,alarmsettings with(NOLOCK) where alarms.alarmid not  in (select alarmid from alarmexceptions) and  stoptime is null and alarms.alarmid=alarmsettings.alarmid and Active=1 and Priority<1000) = 0 and (select count(*) from alarmsprevious,alarmsettings with(NOLOCK) where alarmsprevious.alarmid not  in (select alarmid from alarmexceptions) and stoptime is null and alarmsprevious.alarmid=alarmsettings.alarmid and Active=1 and Priority<1000) = 0
	begin	--no active alarms, display default messages
		if (select count(*) from alarmdefaults with(NOLOCK) where active=1) = 0  --no defaults selected
		begin
			insert into #tempmessages select 2000,' ',0,1000,displaytime,blanktime	from currentstate,alarmgeneralsettings
			select * from #tempmessages order by alarmid
			return
		end

		declare @sqlstring nvarchar(500), @i smallint
		select @i = (select min(alarmid) from alarmdefaults where active=1 and columnname is not null)
		while @i<= (select max(alarmid) from alarmdefaults where active=1 and columnname is not null)
		begin
		    if (select columnname from AlarmDefaults where AlarmID=@i) not like '%reman%' and (select columnname from AlarmDefaults where AlarmID=@i) not like '%trimloss%' and (select columnname from AlarmDefaults where AlarmID=@i) not like '%perlug%'
				select @sqlstring = 'insert into #tempmessages select alarmid,Prefix + convert(varchar,convert(int,' + columnname + ')),0,1000,displaytime1,blanktime1,0,NULL,0 from currentstate,alarmgeneralsettings,alarmdefaults where active = 1 and alarmid=' + convert(varchar,@i) from alarmdefaults where columnname is not null and alarmid=@i
			else
				select @sqlstring = 'insert into #tempmessages select alarmid,Prefix + convert(varchar,round(convert(real,' + columnname + '),2)),0,1000,displaytime1,blanktime1,0,NULL,0 from currentstate,alarmgeneralsettings,alarmdefaults where active = 1 and alarmid=' + convert(varchar,@i) from alarmdefaults where columnname is not null and alarmid=@i
			EXECUTE sp_executesql @sqlstring
			select @i=(select min(alarmid) from alarmdefaults where alarmid>@i and active=1 and columnname is not null)
		end
		--update #tempmessages set DisplayText= (select CONVERT(varchar,binsfull) from CurrentState) + ' FULL BINS' where AlarmID=1009
		if (select active from alarmdefaults where category = 'Current Date & Time') = 1
		begin
			--Adaptive Display
				insert into #tempmessages select alarmid,convert(varchar,getdate(),100),0,1000 ,displaytime1,blanktime1,0,NULL,0
				from alarmdefaults,alarmgeneralsettings where category = 'Current Date & Time'
			
		end
		insert into #tempmessages select alarmid,prefix,0,1000 ,displaytime1,blanktime1,0,NULL,0
				from alarmdefaults,alarmgeneralsettings where active = 1 and category = 'User Message'
			insert into #tempmessages select alarmid, prefix + convert(varchar,abs(datediff(dd,getdate(),AccidentDate))),0,1000 ,displaytime1,blanktime1,0,NULL,0
				from alarmdefaults,alarmgeneralsettings where active = 1 and category = 'Safety'
				
		insert into #tempmessages select 2000+alarms.alarmid,displaytext,severity,priority,displaytime1,blanktime1,0,alarms.Data,datarequired from alarms,alarmsettings,alarmgeneralsettings 
		where stoptime is null /*and displaytext <> ''*/ and alarms.alarmid = alarmsettings.alarmid and active = 1 and Priority=1000
		insert into #tempmessages select 2000+alarmsprevious.alarmid,displaytext,severity,priority,displaytime1,blanktime1,0,alarmsprevious.Data,datarequired from alarmsprevious,alarmsettings,alarmgeneralsettings 
		where stoptime is null /*and displaytext <> ''*/ and alarmsprevious.alarmid = alarmsettings.alarmid and active = 1 and Priority=1000
		
	end
	else
	begin	--show alarms
		insert into #tempmessages select alarms.alarmid,displaytext,severity,priority,displaytime1,blanktime1,0,alarms.Data,datarequired from alarms,alarmsettings,alarmgeneralsettings with(NOLOCK)
		where stoptime is null /*and displaytext <> ''*/ and alarms.alarmid = alarmsettings.alarmid and active = 1 and alarms.alarmid not  in (select alarmid from alarmexceptions)
		insert into #tempmessages select alarmsprevious.alarmid,displaytext,severity,priority,displaytime1,blanktime1,0,alarmsprevious.Data,datarequired from alarmsprevious,alarmsettings,alarmgeneralsettings with(NOLOCK)
		where stoptime is null /*and displaytext <> ''*/ and alarmsprevious.alarmid = alarmsettings.alarmid and active = 1 and alarmsprevious.alarmid not  in (select alarmid from alarmexceptions)
	end
	delete from #tempmessages where priority > (select min(priority) from #tempmessages)
	UPDATE #tempmessages set DisplayText=' ' where DisplayText is null

	--check for variable data messages and if they exist, grab latest data from the PLC
	declare @aid smallint
	declare @timecounter smallint
	select @aid = (select min(AlarmID) from #tempmessages)
	while @aid<=(select max(AlarmID) from #tempmessages)
	begin
		if @aid>2000
		begin
		    declare @aid1 smallint
			select @aid1=@aid-2000
			if (select datarequired from alarmsettings where AlarmID=@aid1) = 1
			begin
				update alarmsettings set Data = NULL where AlarmID=@aid1
				insert into DataRequestsAlarmData select getdate(),@aid1,0,0
				--wait until data has been receive before continuing
				
				select @timecounter = 0
				while @timecounter <= 500
				begin
					if (select data from alarmsettings where AlarmID=@aid1) is not null
						break
					waitfor delay '0:0:0:50'
					select @timecounter = @timecounter + 50
				end
				
				update #tempmessages set Data = (select Data from alarmsettings where AlarmID = @aid1) where AlarmID = @aid1
				update alarms set Data = (select Data from alarmsettings where AlarmID = @aid1) where AlarmID = @aid1 and stoptime is null
				
				
			end
		end
		else
		begin
			if (select datarequired from alarmsettings where AlarmID=@aid) = 1
			begin
				update alarmsettings set Data = NULL where AlarmID=@aid
				insert into DataRequestsAlarmData select getdate(),@aid,0,0
				--wait until data has been receive before continuing
				
				select @timecounter = 0
				while @timecounter <= 500
				begin
					if (select data from alarmsettings where AlarmID=@aid) is not null
						break
					waitfor delay '0:0:0:50'
					select @timecounter = @timecounter + 50
				end
				
				update #tempmessages set Data = (select Data from alarmsettings where AlarmID = @aid) where AlarmID = @aid
				update alarms set Data = (select Data from alarmsettings where AlarmID = @aid) where AlarmID = @aid and stoptime is null
				
				
			
			end
		end
		select @aid = (select min(AlarmID) from #tempmessages where AlarmID>@aid)
		
	end
	
	--need to divide by 1000 for the sorter encoder actual position
	update #tempmessages set DisplayText = DisplayText + ' ' + convert(varchar,convert(real,Data)/1000) 
	where DataRequired = 1 and Data is not null and  AlarmID=162
	update #tempmessages set DisplayText = DisplayText + ' ' + convert(varchar,Data) 
	where DataRequired = 1 and Data is not null and  AlarmID<>162

	--if (select displaytype from alarmgeneralsettings) = 1  --Adaptive Display
		if (select min(priority) from #tempmessages) >= 1000  --check to see if we are only displaying default messages
			if (select count(*) from #tempmessages) > 1
				if (select multilinedefaults from alarmgeneralsettings) = 1  --check user choice for multi line displaying
				begin
					declare @j smallint, @nextdisplaytext varchar(100)
					select @j = (select min(id) from #tempmessages)
					while @j<=(select max(id) from #tempmessages)
					begin
						select @nextdisplaytext = (select displaytext from #tempmessages where id=@j+1)
						if @nextdisplaytext is null
							break
						update #tempmessages set displaytext = displaytext + char(13) + @nextdisplaytext where id=@j
						delete from #tempmessages where id=@j+1
						select @j=@j+2
					end
				end
				
	--except for the volume message, only display defaults every 3 minutes
    /*if (select accumulatedtime from AlarmDefaultAccumulatedTime) <= 300
    	delete from #tempmessages where AlarmID >1000
    else
		update AlarmDefaultAccumulatedTime set AccumulatedTime = 0
   */
	update #tempmessages set alarmcount = (select count(*) from #tempmessages)
	update #tempmessages set DisplayTime = 0.5 where Priority=0 and DataRequired = 1 and AlarmID <> 319
	

	
	select * from #tempmessages order by alarmid
   
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
