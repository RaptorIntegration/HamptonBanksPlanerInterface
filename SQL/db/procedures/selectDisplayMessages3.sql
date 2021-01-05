SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Aug 4, 2010>
-- Description:	<Retrieves alarms or default messages to display on the display board>
-- =============================================
CREATE pROCEDURE [dbo].[selectDisplayMessages3]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;	

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

	if (select count(*) from alarms,alarmsettings where alarms.alarmid not  in (select alarmid from alarmexceptions) and stoptime is null and alarms.alarmid=alarmsettings.alarmid and Active=1 and Priority<1000 and SorterEligible='True') = 0 and (select count(*) from alarmsprevious,alarmsettings where alarmsprevious.alarmid not  in (select alarmid from alarmexceptions) and stoptime is null and alarmsprevious.alarmid=alarmsettings.alarmid and Active=1 and Priority<1000) = 0
	begin	--no active alarms, display default messages
		if (select count(*) from alarmdefaults where active=1) = 0  --no defaults selected
		begin
			insert into #tempmessages select 2000,' ',0,1000,displaytime1,blanktime1	from currentstate,alarmgeneralsettings
			select * from #tempmessages order by alarmid
			return
		end

		declare @sqlstring nvarchar(500), @i smallint
		select @i = (select min(alarmid) from alarmdefaults where active=1 and columnname is not null)
		while @i<= (select max(alarmid) from alarmdefaults where active=1 and columnname is not null)
		begin
			select @sqlstring = 'insert into #tempmessages select alarmid,''Prefix''=InfomasterPrefix + convert(varchar,convert(int,' + columnname + ')),0,1000,displaytime2,blanktime2,0,NULL,0 from currentstate,alarmgeneralsettings,alarmdefaults where active = 1 and alarmid=' + convert(varchar,@i) from alarmdefaults where columnname is not null and alarmid=@i
			EXECUTE sp_executesql @sqlstring
			select @i=(select min(alarmid) from alarmdefaults where alarmid>@i and active=1 and columnname is not null)
		end
		update #tempmessages set DisplayText= (select CONVERT(varchar,binsfull) from CurrentState) + ' FULL BINS' where AlarmID=1009
		if (select active from alarmdefaults where category = 'Current Date & Time') = 1
		begin
			--Infomaster Display
				insert into #tempmessages select alarmid,substring(convert(varchar,getdate(),100),len(convert(varchar,getdate(),100))+1- charindex(' ',reverse(convert(varchar,getdate(),100))),8),0,1000 ,displaytime1,blanktime1,0,NULL,0
				from alarmdefaults,alarmgeneralsettings where category = 'Current Date & Time'
		end
		insert into #tempmessages select alarmid,infomasterprefix,0,1000 ,displaytime2,blanktime2,0,NULL,0
				from alarmdefaults,alarmgeneralsettings where active = 1 and category = 'User Message'
				
		insert into #tempmessages select 2000+alarms.alarmid,displaytext,severity,priority,displaytime2,blanktime2,0,alarms.Data,datarequired from alarms,alarmsettings,alarmgeneralsettings 
		where stoptime is null /*and displaytext <> ''*/ and alarms.alarmid = alarmsettings.alarmid and active = 1 and Priority=1000
		insert into #tempmessages select 2000+alarmsprevious.alarmid,displaytext,severity,priority,displaytime2,blanktime2,0,alarmsprevious.Data,datarequired from alarmsprevious,alarmsettings,alarmgeneralsettings 
		where stoptime is null /*and displaytext <> ''*/ and alarmsprevious.alarmid = alarmsettings.alarmid and active = 1 and Priority=1000
		
	end
	else
	begin	--show alarms
		insert into #tempmessages select alarms.alarmid,displaytext,severity,priority,displaytime2,blanktime2,0,alarms.Data,datarequired from alarms,alarmsettings,alarmgeneralsettings 
		where stoptime is null /*and displaytext <> ''*/ and alarms.alarmid = alarmsettings.alarmid and active = 1  and alarms.alarmid not  in (select alarmid from alarmexceptions)
		insert into #tempmessages select alarmsprevious.alarmid,displaytext,severity,priority,displaytime2,blanktime2,0,alarmsprevious.Data,datarequired from alarmsprevious,alarmsettings,alarmgeneralsettings 
		where stoptime is null /*and displaytext <> ''*/ and alarmsprevious.alarmid = alarmsettings.alarmid and active = 1 and alarmsprevious.alarmid not  in (select alarmid from alarmexceptions) 
	end
	delete from #tempmessages where priority > (select min(priority) from #tempmessages)
	UPDATE #tempmessages set DisplayText=' ' where DisplayText is null

	


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
