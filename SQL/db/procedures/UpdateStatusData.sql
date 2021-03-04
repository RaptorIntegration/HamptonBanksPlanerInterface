SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <August 31, 2010>
-- Description:	<This procedure processes the Status bitmap from the PLC>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateStatusData]
	@Map0 int,
	@Map1 int,
	@Map2 int,
	@Map3 int,
	@Map4 int,
	@Map5 int,
	@Map6 int,
	@Map7 int,
	@Map8 int,
	@Map9 int,
	@Map10 int,
	@Map11 int,
	@Map12 int,
	@Map13 int,
	@Map14 int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @Shift int
    declare @Run int
    declare @AlarmID smallint
    declare @WordCounter smallint
    declare @BitCounter smallint
    declare @TempMap int, @oldTempMap int
    declare @Status int
    declare @AlarmInProgress smallint
    declare @AlarmText varchar(255)
    declare @Multiplier int, @Data int
	declare @MaxAlarm smallint
	declare @tMap0 int,@tMap1 int,@tMap2 int,@tMap3 int,@tMap4 int,@tMap5 int,@tMap6 int,@tMap7 int,@tMap8 int,@tMap9 int,@tMap10 int,@tMap11 int,@tMap12 int,@tMap13 int,@tMap14 int

    select @Shift=max(shiftindex) from Shifts
    select @Run=max(runindex) from Runs
	select @MaxAlarm = (select max(alarmid) from alarmsettings where active=1 and alarmid<480)
	select @tmap0 = Map0 from AlarmDataTemp
	select @tmap1 = Map1 from AlarmDataTemp
	select @tmap2 = Map2 from AlarmDataTemp
	select @tmap3 = Map3 from AlarmDataTemp
	select @tmap4 = Map4 from AlarmDataTemp
	select @tmap5 = Map5 from AlarmDataTemp
	select @tMap6 = Map6 from AlarmDataTemp
	select @tMap7 = Map7 from AlarmDataTemp
	select @tMap8 = Map8 from AlarmDataTemp
	select @tmap9 = Map9 from AlarmDataTemp
	select @tmap10 = Map10 from AlarmDataTemp
	select @tmap11 = Map11 from AlarmDataTemp
	select @tmap12 = Map12 from AlarmDataTemp
	select @tmap13 = Map13 from AlarmDataTemp
	select @tmap14 = Map14 from AlarmDataTemp

    insert into DataReceivedStatus select getdate(),@Map0,@Map1,@Map2,@Map3,@Map4,@Map5,@Map6,@Map7,@Map8,@Map9,@Map10,@Map11,@Map12,@Map13,@Map14

	--Determine if any of the bits have changed
	select @WordCounter = 0
	select @AlarmID=0
	while (@WordCounter < 15)
	begin
		select @BitCounter = 0
		select @TempMap = 
		  CASE
			WHEN @WordCounter=0 THEN @Map0
			WHEN @WordCounter=1 THEN @Map1
			WHEN @WordCounter=2 THEN @Map2
			WHEN @WordCounter=3 THEN @Map3
			WHEN @WordCounter=4 THEN @Map4
			WHEN @WordCounter=5 THEN @Map5
			WHEN @WordCounter=6 THEN @Map6
			WHEN @WordCounter=7 THEN @Map7
			WHEN @WordCounter=8 THEN @Map8
			WHEN @WordCounter=9 THEN @Map9
			WHEN @WordCounter=10 THEN @Map10
			WHEN @WordCounter=11 THEN @Map11
			WHEN @WordCounter=12 THEN @Map12
			WHEN @WordCounter=13 THEN @Map13
			WHEN @WordCounter=14 THEN @Map14
		  END	
		select @oldTempMap = 
		  CASE
			WHEN @WordCounter=0 THEN @tMap0
			WHEN @WordCounter=1 THEN @tMap1
			WHEN @WordCounter=2 THEN @tMap2
			WHEN @WordCounter=3 THEN @tMap3
			WHEN @WordCounter=4 THEN @tMap4
			WHEN @WordCounter=5 THEN @tMap5
			WHEN @WordCounter=6 THEN @tMap6
			WHEN @WordCounter=7 THEN @tMap7
			WHEN @WordCounter=8 THEN @tMap8
			WHEN @WordCounter=9 THEN @tMap9
			WHEN @WordCounter=10 THEN @tMap10
			WHEN @WordCounter=11 THEN @tMap11
			WHEN @WordCounter=12 THEN @tMap12
			WHEN @WordCounter=13 THEN @tMap13
			WHEN @WordCounter=14 THEN @tMap14
		  END			 		
		if (@TempMap <> @oldTempMap)
		begin
			while (@BitCounter<32)
			begin
			  if @BitCounter = 31 Select @Multiplier = -2147483648
			  else Select @Multiplier = POWER(2,@BitCounter)
			  select @Status = @TempMap & @Multiplier
			  

		
			          
			  if (select active from AlarmSettings with(NOLOCK) where AlarmID=@AlarmID) > 0
			  begin   

				select @AlarmInProgress = (select count(*) from Alarms with(NOLOCK) where AlarmID=@AlarmID and stoptime is null)
				if @AlarmInProgress = 0
					select @AlarmInProgress = (select count(*) from AlarmsPrevious with(NOLOCK) where AlarmID=@AlarmID and stoptime is null)
					
					
				if @AlarmInProgress=0 AND @Status<>0  --turn alarm on
				begin
				  if (select datarequired from alarmsettings with(NOLOCK) where AlarmID=@AlarmID) = 1
				  begin	
				  
					update alarmsettings set Data = NULL where AlarmID=@AlarmID
					/*insert into DataRequestsAlarmData select getdate(),@AlarmID,0,0
					--wait until data has been receive before continuing
					declare @timecounter smallint
					select @timecounter = 0
					while @timecounter <= 500
					begin
						if (select data from alarmsettings with(nolock) where AlarmID=@AlarmID) is not null
						    break
						waitfor delay '0:0:0:50'
						select @timecounter = @timecounter + 50						
					end*/
				  end
				  select @AlarmText = AlarmText from AlarmSettings with(NOLOCK) where AlarmID=@AlarmID    
				  select @Data = Data from AlarmSettings with(NOLOCK) where AlarmID=@AlarmID
		
				  --check to see if this is a diverter failure alarm, and if so, extract the data and store for diverter failure report
				  if @AlarmID = 319-- and @Data is not null                   
				  begin
					if (select count(*) from ProductionDiverterFail where shiftindex=@shift and runindex=@run
					and BayID=999) > 0
						update ProductionDiverterFail set boardcount=Boardcount+1 where 
						shiftindex=@shift and runindex=@Run	and BayID=999
					else
						insert into ProductionDiverterFail select @shift,@run,999,1			
				  end
			      
				  if @AlarmText <> '' and @AlarmText is not null 
					insert into Alarms
					values
					(@Shift,@Run,@AlarmID,Getdate(),NULL,0,NULL,@Data, 0, 0)                
			      
				end

				else if @AlarmInProgress=1 and @Status=0 --turn alarm off
				begin  
				  if (select count(*) from Alarms with(NOLOCK) where AlarmID=@AlarmID and stoptime is null)>1
					delete from Alarms where AlarmID=@AlarmID and stoptime is null and starttime <>
					(select max(starttime) from alarms with(NOLOCK) where AlarmID=@AlarmID) 
			      

				  if (select count(*) from Alarms with(NOLOCK) where AlarmID=@AlarmID and stoptime is null)=0
				  begin
					update AlarmsPrevious
					set stoptime = getdate(), duration = convert(varchar,getdate()-starttime,14) where AlarmID=@AlarmID and stoptime is null
				  end
				  else
				  begin
					update Alarms
					set stoptime = getdate(), duration = convert(varchar,getdate()-starttime,14) where AlarmID=@AlarmID and stoptime is null 
				  end                          
				  update AlarmSettings set Data=NULL where AlarmID = @AlarmID
			        
			      
				end 
			  end
			  select @BitCounter = @BitCounter + 1
			  select @AlarmID = @AlarmID + 1
			  if @AlarmID>@MaxAlarm
				break
			end
		end
		else
			select @AlarmID = @AlarmID + 32
		select @WordCounter = @WordCounter + 1
		if @AlarmID>@MaxAlarm
			break
	end


	update AlarmDataTemp set Map0=@Map0,Map1=@Map1,Map2=@Map2,Map3=@Map3,Map4=@Map4,Map5=@Map5,Map6=@Map6,Map7=@Map7,Map8=@Map8,Map9=@Map9,Map10=@Map10,Map11=@Map11,Map12=@Map12,Map13=@Map13,Map14=@Map14

	if (select count(*) from DataReceivedStatus) > 100
		delete from DataReceivedStatus where id<=(select max(id)-100 from DataReceivedStatus)
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
