SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <November 2, 2010>
-- Description:	<updates PlanerSpeed and ProductLength and acts on any change of ProductLength>
-- =============================================
CREATE PROCEDURE [dbo].[updateDriveCurrentState] 
@PlanerSpeed int, @ProductLength smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @oldProductLength smallint, @i smallint,@type smallint,@configuration smallint,@lengthid smallint
	declare @lineal bit, @transverse bit, @lugged bit, @custom bit
	declare @independent bit, @slave bit, @master bit, @masterlink int, @speedmultiplier real
	declare @sqlstring nvarchar(300)
	
	--update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024
	
	select @oldProductLength = (select productlength from DriveCurrentState)
	select @independent = 0
	select @slave=0
	select @master=0
	select @lugged=0
	select @custom=0
	select @transverse=0
	select @masterlink=0
	select @lineal=0
	
	update DriveCurrentState set PlanerSpeed=@PlanerSpeed,ProductLength=@ProductLength
	If @oldProductLength <> @ProductLength
	begin
	
		update RaptorCommSettings set DataRequests = DataRequests | 1024
		select @i=1
		while @i<=(select MAX(driveid) from DriveSettings)
		begin
			select @type = (select [TYPE] from DriveSettings where DriveID=@i)
			if @TYPE = -1  --stand alone
				select @independent = 1
            else if @TYPE = 0  --master
				select @Master = 1
            else  --slave
                select @Slave = 1
            select @masterlink = @type
            
            select @configuration = (select configuration from DriveSettings where DriveID=@i)
            if @configuration = 0  --lineal
				select @Lineal = 1
            else if @configuration = 1  --transverse
				select @Transverse = 1
            else if @configuration = 2 --lugged
				select @Lugged = 1
			else
                select @Custom = 1
                    
			select @lengthid = (select lengthid from lengths where lengthnominal=(select productlength*12 from drivecurrentstate))
			select @sqlstring = 'insert into datarequestsdrive select getdate(),' + convert(varchar,@i) + ',length' + convert(varchar,@lengthid) + 'multiplier,0,' + convert(varchar,@MasterLink) + ',maxspeed,gearingactual,length' + convert(varchar,@lengthid) + 'multiplier,' + convert(varchar,@Slave) + ',' + convert(varchar,@Master) + ',' + convert(varchar,@Independent) + ',' + convert(varchar,@Lineal) + ',' + convert(varchar,@Transverse) + ',' + convert(varchar,@Lugged) + ',' + convert(varchar,@Custom) + ',1,0 from drives,drivesettings where drives.driveid=drivesettings.driveid and drives.driveid=' + convert(varchar,@i) + ' and recipeid=(select recipeid from Recipes where Online=1)'
			--select @sqlstring
			
			EXECUTE sp_executesql @sqlstring
			select @i=@i+1
		end
		--update RaptorCommSettings set datarequests = datarequests-1024 where (datarequests & 1024)=1024
	
	end
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
