SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: February 2, 2010
-- Description:	Creates simulated board records
-- =============================================
CREATE PROCEDURE [dbo].[BoardSim] 
	@NumBoards int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @i int, @binid int,@prodid int,@prodlabel varchar(50),@lengthid smallint,@lengthlabel varchar(50), @sorted smallint, @maxprodid smallint
	declare @thickactual real, @widthactual real, @sortcode int
	declare @timesegment int, @maxshiftindex int, @totallugs real, @fulllugs real
	declare @volumeperhour real,@piecesperhour int,@lugfill smallint,@uptime int
	
	select @maxshiftindex = (select max(shiftindex) from shifts)
	select @i=1
	while @i<=@NumBoards
	begin
		select @sorted=0
		select @sortcode = 0
		if (select @i % 10) = 0
		begin
			select @binid = 0
			select @prodid= 1
			select @lengthid= 1
							
			select @thickactual = 0
			select @widthactual = 0
		end
		else
		begin
			select @maxprodid = (select max(prodid) from products)
			select @binid = ceiling(rand()*50)
			select @prodid=-1
			while @prodid not in (select prodid from products)	
				select @prodid= ceiling(rand()*@maxprodid)
			select @lengthid= ceiling(rand()*7)
			if (select count(*) from products where prodid=@prodid)=0
				select @prodid=1
				
			select @thickactual = (select thicknominal from products where prodid=@prodid)
			select @widthactual = (select widthnominal from products where prodid=@prodid)
		end
		insert into boards
		select getdate(),@i,0,@binid,convert(varchar,@binid),@prodid,prodlabel + ' ~ ' + gradelabel,@lengthid,lengthlabel,thicknominal,widthnominal,@lengthid,lengthlabel,0,0,'',0,'',0,'',0,'',0,0,0
		from products,lengths,grades with(NOLOCK) where prodid=@prodid and lengthid=@lengthid and products.gradeid=grades.gradeid
		--to do add bin counts and percents
		select @i=@i+1
		if (select count(*) from boards) >50
			delete from boards where id<=(select max(id)-50 from boards)

		

		/* update CurrentState table */
		if @binid > 0 and @Binid in (select binid from bins where binstatus<4)
		begin
			select @sorted=1
			select @sortcode=1
			update CurrentState set currentpieces=currentpieces+1
			update CurrentState set currentvolume=currentvolume + (select thicknominal*widthnominal*lengthnominal/144 from lengths,products where products.prodid=@prodid and lengths.lengthid=@lengthid)
		end
		
		/* update ProductionBoards table */
		if (select count(*) from ProductionBoards where shiftindex=@maxshiftindex and runindex=(select max(runindex) from runs)
		and ProdID=@prodid and lengthID=@lengthid and thickactual=@thickactual and widthactual=@widthactual
		and lengthin=0 and lengthinid=@lengthid and net=0 and fet=0 and cn2=0 and fence=0 and sorted=@sorted and sortcode=@sortcode) > 0
			update ProductionBoards set boardcount=Boardcount+1 where 
			shiftindex=@maxshiftindex and runindex=(select max(runindex) from runs)
			and ProdID=@prodid and lengthID=@lengthid and thickactual=@thickactual and widthactual=widthactual
			and lengthin=0 and lengthinid=@lengthid and net=0 and fet=0 and cn2=0 and fence=0 and sorted=@sorted and sortcode=@sortcode
		else
begin

			insert into ProductionBoards select @maxshiftindex,max(runindex),@prodid, @lengthid,@thickactual,@widthactual,0,@lengthid,0,0,0,0,@sorted,@sortcode,1 from runs
		end
		/*target summary*/
		select @lugfill=0,@uptime=0
		select @totallugs = (select sum(boardcount) from ProductionBoards)
		select @fulllugs = (select sum(boardcount) from ProductionBoards where sortcode>0)
		if @totallugs is null select @totallugs=0
		if @fulllugs is null select @fulllugs=0
		select @lugfill = @fulllugs/@totallugs*100
		select @volumeperhour = (select currentvolume from currentstate) / 
		(select convert(real,abs(datediff(mi,shiftstart,getdate())))/60 from shifts where shiftindex=@maxshiftindex)
		select @piecesperhour = (select currentpieces from currentstate) / 
		(select convert(real,abs(datediff(mi,shiftstart,getdate())))/60 from shifts where shiftindex=@maxshiftindex)
		select @timesegment = (select abs(datediff(mi,getdate(),shiftstart)) from shifts where shiftindex = @maxshiftindex)	
		update TargetSummary set volumeperhour=@volumeperhour,piecesperhour=@piecesperhour,lugfill=@lugfill,uptime=@uptime where timesegment=@timesegment
        and shiftindex = @maxshiftindex

		-- fill in behind 
		/*update TargetSummary set piecesperhour = (select currentpieces from currentstate) / (convert(real,timesegment) /60)
		where piecesperhour=0 and timesegment<@timesegment
		update TargetSummary set volumeperhour = (select currentvolume from currentstate) / (convert(real,timesegment) /60)
		where volumeperhour=0 and timesegment<@timesegment
		update TargetSummary set lugfill = @lugfill where lugfill=0 and timesegment<@timesegment*/
		
		update CurrentState set CurrentVolumePerHour = @volumeperhour,currentpiecesperhour=@piecesperhour,Currentshiftlugfill=@lugfill,currentuptime=@uptime


		waitfor delay '0:0:1:000'	
	end
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
