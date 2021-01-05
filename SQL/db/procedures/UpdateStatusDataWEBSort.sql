SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <August 31, 2010>
-- Description:	<This procedure processes any WEBSort alarms>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateStatusDataWEBSort]
	@AlarmID int,
	@Status tinyint

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @Shift smallint
    declare @Run smallint
    declare @AlarmInProgress smallint
    declare @AlarmText varchar(255)
	declare @count smallint, @countprevious smallint


	select @count = (select count(*) from Alarms where AlarmID = @AlarmID and stoptime is null)
	select @countprevious = (select count(*) from AlarmsPrevious where AlarmID = @AlarmID and stoptime is null)
	select @count = @count+@countprevious

	if @Status = 0 and @count = 0
		return
    
    select @Shift=max(shiftindex) from Shifts
    select @Run=max(runindex) from Runs    
	
		          
	if (select active from AlarmSettings where AlarmID=@AlarmID) > 0
	begin   
		select @AlarmInProgress = (select count(*) from Alarms where AlarmID=@AlarmID and stoptime is null)
		if @AlarmInProgress = 0
			select @AlarmInProgress = (select count(*) from AlarmsPrevious where AlarmID=@AlarmID and stoptime is null)
		if @AlarmInProgress=0 AND @Status<>0  --turn alarm on
		begin
		  
		  select @AlarmText = AlarmText from AlarmSettings where AlarmID=@AlarmID    
		  
		  if @AlarmText <> '' and @AlarmText is not null 
			insert into Alarms
			values
			(@Shift,@Run,@AlarmID,Getdate(),NULL,0,NULL,NULL)                
		  
		end

		else if @AlarmInProgress=1 and @Status=0 --turn alarm off
		begin  
		  if (select count(*) from Alarms where AlarmID=@AlarmID and stoptime is null)>1
			delete from Alarms where AlarmID=@AlarmID and stoptime is null and starttime <>
			(select max(starttime) from alarms where AlarmID=@AlarmID) 
		  

		  if (select count(*) from Alarms where AlarmID=@AlarmID and stoptime is null)=0
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
		  
	

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
