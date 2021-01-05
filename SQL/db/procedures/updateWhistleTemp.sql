SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <November 2, 2010>
-- Description:	<activate temporary whistle sequence>
-- =============================================
create PROCEDURE [dbo].[updateWhistleTemp] 
@WhistleID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @timediff1 int, @timediff2 int
	declare @time datetime, @time1 datetime, @time2 datetime, @currenttime datetime
	
	select @currenttime = GETDATE()
	
	update ShiftWhistles set enabled=0 where WhistleID>=37
	update ShiftWhistles set tempenabled=Enabled 
	
	create table #temp
	(id smallint identity,whistleid smallint,whistleblow datetime,enabled tinyint,type smallint,repetitions smallint,tempenabled tinyint)
	
	update shiftwhistles set WhistleBlow = (select convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
			convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
			convert(varchar,datepart(hh,WhistleBlow)) + ':' + convert(varchar,datepart(mi,WhistleBlow))) from ShiftWhistles
			where WhistleID = @WhistleID) where WhistleID = @WhistleID
    update shiftwhistles set WhistleBlow = (select convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
			convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
			convert(varchar,datepart(hh,WhistleBlow)) + ':' + convert(varchar,datepart(mi,WhistleBlow))) from ShiftWhistles
			where WhistleID = @WhistleID+1) where WhistleID = @WhistleID+1
	update shiftwhistles set WhistleBlow = (select convert(datetime,convert(varchar,datepart(yy,@currenttime)) + '-' + 
			convert(varchar,datepart(mm,@currenttime)) + '-' + convert(varchar,datepart(dd,@currenttime)) + ' ' + 
			convert(varchar,datepart(hh,WhistleBlow)) + ':' + convert(varchar,datepart(mi,WhistleBlow))) from ShiftWhistles
			where WhistleID = @WhistleID+2) where WhistleID = @WhistleID+2
			
	insert into #temp select * from ShiftWhistles where whistleid between @WhistleID and @WhistleID+2 order by enabled desc,whistleblow
	
	select @time = (select WhistleBlow from #temp where ID=1)
	select @time1 = (select WhistleBlow from #temp where ID=2)
	select @time2 = (select WhistleBlow from #temp where ID=3)
	
	select @timediff1 = (select ABS(datediff(mi,@time,@time1)))
	select @timediff2 = (select ABS(datediff(mi,@time,@time2)))
		
	update ShiftWhistles set WhistleBlow = GETDATE() where WhistleID>=37	
	update ShiftWhistles set WhistleBlow = (select DATEADD(mi,@timediff1,WhistleBlow) from ShiftWhistles where WhistleID=37) where WhistleID=38
	update ShiftWhistles set WhistleBlow = (select DATEADD(mi,@timediff2,WhistleBlow) from ShiftWhistles where WhistleID=37) where WhistleID=39	
	
	update ShiftWhistles set Type = (select Type from #temp where ID=1) where WhistleID=37
	update ShiftWhistles set Type = (select Type from #temp where ID=2) where WhistleID=38
	update ShiftWhistles set Type = (select Type from #temp where ID=3) where WhistleID=39
	
	update ShiftWhistles set Repetitions = (select Repetitions from #temp where ID=1) where WhistleID=37
	update ShiftWhistles set Repetitions = (select Repetitions from #temp where ID=2) where WhistleID=38
	update ShiftWhistles set Repetitions = (select Repetitions from #temp where ID=3) where WhistleID=39
	
	update ShiftWhistles set Enabled = (select Enabled from #temp where ID=1) where WhistleID=37
	update ShiftWhistles set Enabled = (select Enabled from #temp where ID=2) where WhistleID=38
	update ShiftWhistles set Enabled = (select Enabled from #temp where ID=3) where WhistleID=39
	
	update ShiftWhistles set Enabled = 0 where WhistleID between @WhistleID and @WhistleID+2
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
