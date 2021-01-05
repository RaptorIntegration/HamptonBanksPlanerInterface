SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Aug 4, 2010>
-- Description:	<Checks to see if a higher priority message exists, and returns a 1 if so>
-- =============================================
CREATE PROCEDURE [dbo].[selectDisplayMessagePriority1]
@AlarmID smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--select 'Abort'=0,'HigherPriorityAlarmText'=99999
--return
	declare @alarmidhigherprioritytext varchar(100)
	if @alarmid>=1000
	begin
		if (select count(alarms.alarmid) from alarms,AlarmSettings where stoptime is null and Priority<1000  and active=1 and sortereligible='True' and alarms.AlarmID=AlarmSettings.AlarmID) > 0 or (select count(alarmsprevious.alarmid) from alarmsprevious,AlarmSettings where stoptime is null and Priority<1000 and Active=1  and sortereligible='True' and alarmsprevious.AlarmID=AlarmSettings.AlarmID) > 0
		begin
			select @alarmidhigherprioritytext = (select min(displaytext) from alarmsettings where alarmid in 
			(select alarmid from alarms where stoptime is null and alarmid in (select alarmid from alarmsettings where sortereligible='true' ) 
			union select alarmid from alarmsprevious where stoptime is null and alarmid in (select alarmid from alarmsettings where sortereligible='true' )))
			select 'Abort'=1,'HigherPriorityAlarmText'=@alarmidhigherprioritytext
		end
		else
			select 'Abort'=0,'HigherPriorityAlarmText'=99999
	end
	else
	begin
		if (select priority from alarmsettings where alarmid=@alarmid) > (select min(priority) from alarmsettings where active=1 and sortereligible='True' and alarmid in
		(select alarmid from alarms where stoptime is null union select alarmid from alarmsprevious where stoptime is null))
		begin
			
			select @alarmidhigherprioritytext = (select min(displaytext) from alarmsettings where alarmid in 
			(select alarmid from alarms where stoptime is null and alarmid in (select alarmid from alarmsettings where sortereligible='true' )
			union select alarmid from alarmsprevious where stoptime is null and alarmid in (select alarmid from alarmsettings where sortereligible='true' ))
			and priority < (select priority from alarmsettings where alarmid=@alarmid))
	    
			select 'Abort'=1,'HigherPriorityAlarmText'=@alarmidhigherprioritytext
		end
		else if (select count(alarmid) from alarms where stoptime is null and AlarmID in (select AlarmID from AlarmSettings where SorterEligible = 'True')) = 0 and (select count(alarmid) from alarmsprevious where stoptime is null and AlarmID in (select AlarmID from AlarmSettings where SorterEligible = 'True')) = 0
			select 'Abort'=1,'HigherPriorityAlarmText'='Default'
		else
			select 'Abort'=0,'HigherPriorityAlarmText'=99999
	end
   
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
