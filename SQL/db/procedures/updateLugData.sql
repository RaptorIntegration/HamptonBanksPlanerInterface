SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 21, 2010>
-- Description:	<Processes incoming Lug Data from PLC>
-- =============================================
CREATE PROCEDURE [dbo].[updateLugData] 
@FrameStart int,
@LugNum int,
@TrackNum int,
@BayNum int,
@ProductID int,
@LengthID int,
@ThickActual real,
@WidthActual real,
@LengthIn real,
@Fence real,
@graderid int,
@Saws bigint,
@NET int,
@FET int,
@CN2 int,
@BayCount int,
@Volume real,
@PieceCount int,
@Flags int,
@Devices int,
@Ack bit,
@FrameEnd int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @timesegment int, @maxshiftindex int, @maxrunindex int
	declare @volumeperhour real,@piecesperhour int,@lugfill smallint,@uptime int, @lugfill1 smallint
	declare @sorted smallint, @sortcode int, @previoussaws bigint
	declare @totallugs real, @fulllugs real, @nominalwidth real,@nominalthick real,@fulllugs2 real
	declare @totallugs1 real, @fulllugs1 real
	declare @lengthlabel varchar(50)
	declare @bitcounter smallint
	declare @sawbinary varchar(50), @min smallint
	declare @thicknom real, @widthnom real
	declare @trimlossfactor real, @trimlossfactor1 real, @volumeout real
	
	--override the exception code of no grade, since the plc is sorting them 
	if @flags=4 select @flags=0
	
	select @trimlossfactor=(select trimlossfactor/100.0 from WEBSortSetup)
	select @trimlossfactor1=(select (100.0 + (100.0-trimlossfactor))/100.0 from WEBSortSetup )
	select @volumeout = (select lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
	
	--account for trimsave pieces, count the input length as the output length
	--if @cn2 > 0
		--select @lengthin = (select lengthnominal/12 from lengths where lengthid=@lengthid)
	/*select @min = (select DATEPART(mi,getdate()))
	if @min between 0 and 9
		update lugfillcurrent set min10 = min10+@volumeout
	else if @min between 10 and 19
		update lugfillcurrent set min20 = min20+@volumeout
	else if @min between 20 and 29
		update lugfillcurrent set min30 = min30+@volumeout
	else if @min between 30 and 39
		update lugfillcurrent set min40 = min40+@volumeout
	else if @min between 40 and 49
		update lugfillcurrent set min50 = min50+@volumeout
	else if @min between 50 and 59
		update lugfillcurrent set min60 = min60+@volumeout*/
		
	delete from lugfillcurrent1 where ABS(datediff(mi, getdate(),timestamp)) > 60
	insert into lugfillcurrent1 select GETDATE(),@volumeout*@trimlossfactor
	
		
	
	declare @lengthinstring varchar(50)
	declare @lengthin_inches real, @lengthin_feet smallint
	select @lengthinstring=''
	select @lengthin_feet = CONVERT(smallint,@lengthin)
	select @lengthin_inches = round((@lengthin - @lengthin_feet) * 12,1)
	select @lengthinstring = convert(varchar,@lengthin_feet) + ''' ' + CONVERT(varchar,@lengthin_inches) + '"'
	if @lengthinstring = '0'' 0"' select @lengthinstring = '0'
	
	if @ProductID > 0
	begin
		select @thicknom = (select ThickNominal from Products where ProdID=@ProductID)
		select @widthnom = (select WidthNominal from Products where ProdID=@ProductID)
	end
	else
	begin
		select @thicknom=(select CEILING(@ThickActual))
		select @widthnom=0
	end
	if @thicknom is null select @thicknom = 0
	if @widthnom is null select @widthnom = 0
	
	
	
	insert into DataReceivedLug select getdate(),@FrameStart,@LugNum,@TrackNum,@BayNum,@ProductID,
	@LengthID,@ThickActual,@WidthActual,@LengthIn,@fence,@Saws,@NET,@FET,@CN2,@BayCount,@Volume,@PieceCount,@Flags,@Devices,@Ack,@FrameEnd	
	
	if @CN2 <> 2
	begin
		select @previoussaws = (select lastsawpattern from CurrentState)	
		update CurrentState set lastsawpattern=@saws
	end
	
    --create saw map for display purposes
	select @sawbinary=''
	select @bitcounter = (select MAX(lengthnominal/12) from Lengths)
	while @bitcounter>=0
	begin
		if (select @saws & POWER(2,@bitcounter)) = POWER(2,@bitcounter)
			select @sawbinary = @sawbinary + '1'
		else
			select @sawbinary = @sawbinary + '0'
		if @bitcounter % 4 = 0
			select @sawbinary = @sawbinary + ' '
		select @bitcounter=@bitcounter-1
	end
	
	--loop through the reject flags word to see what the lowest bit is
	if @Flags > 0 or @Flags = -2147483648
	begin
		declare @newflags int
		select @newflags=-1
		declare @i smallint
		select @i=31
		while @i>=0
		begin
			if @i = 31
			begin
				if (select @Flags & convert(bigint,-2147483648)) = -2147483648
					select @newFlags = @i		
			end
			else
			begin				
				if (select @Flags & power(2,@i)) = power(2,@i)
					select @newFlags = @i
			end
			select @i=@i-1
		end
		select @Flags = @newFlags
	end
	else
		select @Flags=-1
			
	select @maxshiftindex = (select max(shiftindex) from shifts)
	select @maxrunindex = (select max(runindex) from runs)
	select @sorted=0
	select @sortcode=0
	
	if (@Baynum = 0 and @Flags = -1)  --empty lugs
		select @sortcode=-1
	if (@LengthID not in (select lengthid from Lengths)) --plc sends lengthid out of range on some exceptions
		select @LengthID = 0
	
	select @lengthlabel = (select lengthlabel from Lengths where LengthID = @LengthID)	

	update CurrentState set CurrentVolume = @Volume*@trimlossfactor, CurrentPieces = @PieceCount
	update currentstate set DisplayVolume = (select @Volume*@trimlossfactor from CurrentState)
	update currentstate set DisplayPieces = (select @PieceCount from CurrentState)
	
	
	
	insert into Boards select getdate(),@LugNum,@TrackNum,@BayNum,convert(varchar,@BayNum),@ProductID,'',
	@LengthID,@lengthlabel,@ThickActual,@WidthActual,@LengthIn,@lengthinstring,@sawbinary,@NET,convert(varchar,@NET),
	@FET,convert(varchar,@FET),@CN2,convert(varchar,@CN2),@Fence,convert(varchar,@Fence),@BayCount,@Flags,@Devices
	
	insert into Boardslugfill select getdate(),@LugNum,@TrackNum,@BayNum,convert(varchar,@BayNum),@ProductID,'',
	@LengthID,@lengthlabel,@ThickActual,@WidthActual,@LengthIn,@lengthinstring,@sawbinary,@NET,convert(varchar,@NET),
	@FET,convert(varchar,@FET),@CN2,convert(varchar,@CN2),@Fence,convert(varchar,@Fence),@BayCount,@Flags,@Devices
	
	update Boards set ProdLabel = (select ProdLabel + ' ~ ' + gradelabel from Products,grades where ProdID = @ProductID and products.gradeid=grades.gradeid) where id=(select max(id) from Boards)
	update Boards set ProdLabel = '{None}' where prodid = 0	

	--we need to account for a cut in two board sequence where the first half did not sort, but the second half did.
	--if we leave it as is, the second half will have its input length ignored in trimloss reports, and the first half
	--will not show up to make up the difference in input length. This is no good.
	--so if we have an unsorted cut in two board marked as cut in two sequence 1, we need to alter the second half's
	--cut in two flag back to zero.
	if @CN2 = 2 and @Baynum in (select binid from bins where binstatus<>4)
	begin
		if (select CN2 from Boards where id = (select MAX(id) from Boards)) = 1	and (select BinStatus from Bins where BinID = (select binid from Boards where id = (select MAX(id) from Boards))) = 4
			Select @CN2 = 0
	end	
	
	--clear the saw pattern and input length on the second half of a cut in two board
	if @CN2 = 2
	begin
		select @Saws=0
		--select @LengthIn=0
	end
	
	--store saw mileage and stroke count
	select @nominalwidth = (select min(Nominal) from width where @WidthActual between Minimum and maximum)
	select @nominalthick = (select min(Nominal) from thickness where @thickActual between Minimum and maximum)
	
	if @nominalwidth is null select @nominalwidth = 0
	if @nominalthick is null select @nominalthick = 0

	if (@lengthid in (select lengthid from lengths where petflag=1))  --PET saw being used
	begin
		update sawmileage set mileage = mileage + @nominalwidth/12 where SawID=31
		update sawmileage set strokes = strokes + 1 where SawID=31
	end
	if @CN2 <> 2
	begin
		update sawmileage set mileage = mileage + @nominalwidth/12 where (@saws & POWER(2,sawid) = POWER(2,sawid)) and SawID<31
		update sawmileage set strokes = strokes + 1 where (@saws & POWER(2,sawid) = POWER(2,sawid))  and SawID<31
		and ((@previoussaws & POWER(2,sawid)) <> (@saws & POWER(2,sawid)))
	end
	
	--account for saw mileage in the PET trimmer
	/*if @LengthID in (select lengthid from Lengths where PETFlag=1)  
	begin
		select @Saws = 1  --zero saw is always on
		select @Saws = @Saws | (select POWER(2,sawindex) from PETLengths where PETLengthID in (select PETLengthID from Lengths where LengthID = @LengthID))
		select @saws = @Saws * POWER(2,27)  --bit shift to use the PET portion of the sawmileage map (bits 27-30)
		update sawmileage set mileage = mileage + @nominalwidth/12 where @saws & POWER(2,sawid) = POWER(2,sawid)
		update sawmileage set strokes = strokes + 1 where @saws & POWER(2,sawid) = POWER(2,sawid) 
		and ((@previoussaws & POWER(2,sawid)) <> (@saws & POWER(2,sawid)))
	end*/
	
	--check to see if the saw mileage thresholds have been achieved
	declare @sawcounter smallint
	select @sawcounter = 0
	while @sawcounter <=31
	begin
		if (select mileage from sawmileage where id = @sawcounter) > (select mileagethreshold from sawmileage where id = @sawcounter)
			execute UpdateStatusDataWEBSort 482,1
		if (select strokes from sawmileage where id = @sawcounter) > (select strokethreshold from sawmileage where id = @sawcounter)
			execute UpdateStatusDataWEBSort 483,1
		select @sawcounter = @sawcounter + 1
	end
	
	if (select count(*) from DataReceivedLug) > 150
		delete from DataReceivedLug where id<=(select max(id)-150 from DataReceivedLug)
	if (select count(*) from boards) >140
		delete from boards where id<=(select max(id)-140 from boards)
		
		if (select count(*) from boardslugfill) >500
		delete from boardslugfill where id<=(select max(id)-500 from boardslugfill)
	
	
	/* update DGS data */
	--lugs run
	update DGSData set LugsRun = LugsRun + 1 where ShiftIndex = @maxshiftindex
		
	if (@BayNum > 0)
	begin
		declare @lenghinrounded real
		select @lenghinrounded = (select floor(convert(smallint,@lengthin)))
		--pieces in
		update DGSData set PiecesIn = PiecesIn + 1 where ShiftIndex = @maxshiftindex
		
		--lineal in
		update DGSData set LinealIn = LinealIn + @lenghinrounded where ShiftIndex = @maxshiftindex
		
		--volume in
		update DGSData set VolumeIn = VolumeIn + ((@lenghinrounded*12)*@ThickNom*@WidthNom)/144 where ShiftIndex = @maxshiftindex
		
		--trim volume
		declare @volumein real,  @trimvolume real
		select @volumein = ((@lenghinrounded*12.0)*@ThickNom*@WidthNom)/144.0
		select @volumeout = (select lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
		select @trimvolume = @volumein - @volumeout
		update DGSData set TrimVolume = TrimVolume + @trimvolume where ShiftIndex = @maxshiftindex
		--insert into test select @lengthin,@ThickActual,@widthactual
		
	end
	
	if @Flags = (select rejectflag from BoardRejects where RejectDescription like '%Reman%')
	begin
		--Reman Pieces
		update DGSData set RemanPieces = RemanPieces + 1 where ShiftIndex = @maxshiftindex		
		--Reman volume
		update DGSData set RemanVolume = RemanVolume + ((@Lengthin*12)*@ThickActual*@WidthActual)/144 where ShiftIndex = @maxshiftindex		
	end
	
	if @Flags = (select rejectflag from BoardRejects where RejectDescription like '%Slash%')
	begin
		--Slash Pieces
		update DGSData set SlashedPieces = SlashedPieces + 1 where ShiftIndex = @maxshiftindex
		--Slash volume
		update DGSData set SlashedVolume = SlashedVolume + ((@Lengthin*12)*@ThickActual*@WidthActual)/144 where ShiftIndex = @maxshiftindex		
	end
	
	--Bone Dry Pieces
	if (select moistureid from Products where ProdID = @ProductID) = 1
	begin
		if (select widthnominal from Products where ProdID = @ProductID) = 4
		begin
			update DGSData set BoneDryPieces2x4 = BoneDryPieces2x4 + 1 where ShiftIndex = @maxshiftindex	
		end
		else if (select widthnominal from Products where ProdID = @ProductID) = 6
		begin
			update DGSData set BoneDryPieces2x6 = BoneDryPieces2x6 + 1 where ShiftIndex = @maxshiftindex	
		end
		else if (select widthnominal from Products where ProdID = @ProductID) = 10
		begin
			update DGSData set BoneDryPieces2x10 = BoneDryPieces2x10 + 1 where ShiftIndex = @maxshiftindex	
		end
		else if (select widthnominal from Products where ProdID = @ProductID) = 12
		begin
			update DGSData set BoneDryPieces2x12 = BoneDryPieces2x12 + 1 where ShiftIndex = @maxshiftindex	
		end
	
	end

	
	/* update bins tables */	
	if @Baynum > 0 and @Baynum in (select binid from bins where binstatus<>4) and (@Flags=-1 or @Flags=22)
	begin		
		
		update bins set bincount=@BayCount where binid=@BayNum
		if (select binsize from bins where binid=@BayNum) > 0
			update bins set BinPercent = (select ceiling(convert(real,bincount)/convert(real,binsize) * 100)) where binid=@BayNum
		else 
			update bins set BinPercent = 0 where binid=@BayNum
		if (select count(*) from BinProductLengths where BinID = @BayNum and ProdID=@ProductID and LengthID=@LengthID) = 0
			insert into BinProductLengths select @BayNum,@ProductID,@LengthID,1
		else
			update BinProductLengths set BoardCount=BoardCount+1 where BinID=@BayNum and ProdID=@ProductID and LengthID=@LengthID
		if (select count(*) from BinProducts where BinID = @BayNum and ProdID=@ProductID) = 0
			insert into BinProducts select @BayNum, @Productid
		if (select count(*) from BinLengths where BinID = @BayNum and Lengthid=@Lengthid) = 0
			insert into BinLengths select @BayNum, @Lengthid
		--update products label
		exec updateBinProducts @BayNum
				
		select @sorted=1
		select @sortcode=1	
		
	end
	else if @Flags>=0
	begin
		select @sorted=0
		select @sortcode=@Flags
		if (select binsize from Bins where BinID=@BayNum) > 0  --don't increment on a msr test board
		begin
			if (select rejectdescription from boardrejects where RejectFlag=@Flags) like '%test%'  --increment piece count in the test bin
			begin
				update bins set bincount=BinCount+1 where binid=@BayNum
				if (select binsize from bins where binid=@BayNum) > 0
					update bins set BinPercent = (select ceiling(convert(real,bincount)/convert(real,binsize) * 100)) where binid=@BayNum
				else 
					update bins set BinPercent = 0 where binid=@BayNum
			end
		end
	end
	
	
	

	/*target summary*/
	--first determine what break times to ignore
	declare @start datetime, @breaktimetemp real, @breaktimetemp1 real
	select @start = (select shiftstart from shifts where ShiftIndex=@maxshiftindex)

	--calculate sum of breaks that have come and gone
	select @breaktimetemp=(select SUM(abs(datediff(ss,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakstart) + ':' + datename(mi,breakstart) + ':' + datename(ss,breakstart)),convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakend) + ':' + datename(mi,breakend) + ':' + datename(ss,breakend))))) from shiftbreaks where enabled=1 and
	convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakstart) + ':' + datename(mi,breakstart) + ':' + datename(ss,breakstart))
	between @start and GETDATE() and
	convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakend) + ':' + datename(mi,breakend) + ':' + datename(ss,breakend))
	between @start and GETDATE())
	if @breaktimetemp is null select @breaktimetemp = 0

	--calculate any breaktime that is currently in progress
	select @breaktimetemp1=
	(select SUM(abs(datediff(ss,convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakstart) + ':' + datename(mi,breakstart) + ':' + datename(ss,breakstart)),GETDATE()))) from shiftbreaks where enabled=1 and
	GETDATE() between
	convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakstart) + ':' + datename(mi,breakstart) + ':' + datename(ss,breakstart))
	 and
	convert(datetime, datename(mm,@start) + ' ' + datename(dd,@start) + ' ' + datename(yy,@start) + ' ' + datename(hh,breakend) + ':' + datename(mi,breakend) + ':' + datename(ss,breakend)))
	if @breaktimetemp1 is null select @breaktimetemp1 = 0
	select @breaktimetemp = @breaktimetemp + @breaktimetemp1
	select @breaktimetemp = @breaktimetemp/3600
	
	select @lugfill=0,@uptime=0
	select @totallugs = (select sum(boardcount) from ProductionBoards where  ShiftIndex=@maxshiftindex)
	select @fulllugs = (select sum(boardcount) from ProductionBoards where  ShiftIndex=@maxshiftindex and sortcode>=0)
	select @fulllugs2 = (select sum(boardcount) from ProductionBoards where ShiftIndex=@maxshiftindex and sortcode=1)
	if @totallugs is null select @totallugs=0
	if @fulllugs is null select @fulllugs=0
	if @totallugs > 0
		select @lugfill = @fulllugs/@totallugs*100
		
	select @lugfill1=0
	select @totallugs1 = (select count(*) from boardslugfill)
	select @fulllugs1 = (select count(*) from boardslugfill where binid>0)
	if @totallugs1 is null select @totallugs1=0
	if @fulllugs1 is null select @fulllugs1=0
	if @totallugs1 > 0
		select @lugfill1 = @fulllugs1/@totallugs1*100
		
	if @fulllugs > 0
		update CurrentState set currentvolumeperlug = (select CurrentVolume from CurrentState) / @fulllugs2
	else
		update CurrentState set currentvolumeperlug = 0
		
	--reman percentage
	declare @percentreman real
	select @percentreman = 0
	if @fulllugs >0
	  select @percentreman = round((select SUM(boardcount)/@fulllugs * 100 from ProductionBoards where SortCode between 6 and 8),1)
	if @percentreman is null select @percentreman=0
	
	--trimloss
	declare @trimloss real
	select @trimloss = 0
	if @sortcode = 1 or @sortcode = (select RejectFlag from BoardRejects where RejectDescription like '%slash%')
		update CurrentState set CurrentInputVolume = CurrentInputVolume + ((@Lengthin*12)*@ThickNom*@WidthNom)/144
		
	if (select CurrentInputVolume from CurrentState) > 0
	begin
		select @trimloss = round((select (CurrentInputVolume-(currentvolume*@trimlossfactor1)) / CurrentInputVolume *100 from CurrentState),1)
	end
	else
		select @trimloss=0
	
    --update Vorne statistics
	--update Vorne statistics
	if @sorted=1
	begin
		if @nominalthick=2 and @nominalwidth = 4
		begin
			update VorneStatistics set [2x4volume]=[2x4volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x4percentage] = (select ROUND([2x4volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalthick=2 and @nominalwidth = 6
		begin
			update VorneStatistics set [2x6volume]=[2x6volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x6percentage] = (select ROUND([2x6volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalwidth = 8
		begin
			update VorneStatistics set [2x8volume]=[2x8volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x8percentage] = (select ROUND([2x8volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalwidth = 10
		begin
			update VorneStatistics set [2x10volume]=[2x10volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x10percentage] = (select ROUND([2x10volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalwidth = 12
		begin
			update VorneStatistics set [2x12volume]=[2x12volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x12percentage] = (select ROUND([2x12volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalthick=2 and @nominalwidth = 7
		begin
			update VorneStatistics set [2x7volume]=[2x7volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x7percentage] = (select ROUND([2x7volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalthick=2 and @nominalwidth = 9
		begin
			update VorneStatistics set [2x9volume]=[2x9volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [2x9percentage] = (select ROUND([2x9volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalthick=1 and @nominalwidth = 4
		begin
			update VorneStatistics set [1x4volume]=[1x4volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [1x4percentage] = (select ROUND([1x4volume]/CurrentVolume*100,1) from CurrentState)
		end
		else if @nominalthick=1 and @nominalwidth = 6
		begin
			update VorneStatistics set [1x6volume]=[1x6volume] + (select @trimlossfactor * lengthnominal * thicknominal *widthnominal/144.0 from Products,Lengths where products.ProdID=@ProductID and lengths.LengthID=@LengthID)
			if (select currentvolume from CurrentState) > 0
				update VorneStatistics set [1x6percentage] = (select ROUND([1x6volume]/CurrentVolume*100,1) from CurrentState)
		end
	
	end
	
	--if (select targetmode from WEBSortSetup) = 'Shift'
	begin
		if (select convert(real,abs(datediff(s,shiftstart,getdate())))/3600-@breaktimetemp from shifts where shiftindex=@maxshiftindex) > 0
			select @volumeperhour = (select displayvolume from currentstate) / 
			(select convert(real,abs(datediff(s,shiftstart,getdate())))/3600-@breaktimetemp from shifts where shiftindex=@maxshiftindex)
		else select @volumeperhour = 0
		if (select convert(real,abs(datediff(s,shiftstart,getdate())))/3600-@breaktimetemp from shifts where shiftindex=@maxshiftindex) > 0
			select @piecesperhour = (select displaypieces from currentstate) / 
			(select convert(real,abs(datediff(s,shiftstart,getdate())))/3600-@breaktimetemp from shifts where shiftindex=@maxshiftindex)
		else select @piecesperhour = 0
		select @timesegment = (select abs(datediff(mi,getdate(),shiftstart)) from shifts where shiftindex = @maxshiftindex)	
	end
	/*else
	begin
		if (select convert(real,abs(datediff(s,runstart,getdate())))/3600-@breaktimetemp from runs where runindex=@maxrunindex) > 0
			select @volumeperhour = (select currentvolume from currentstate) / 
			(select convert(real,abs(datediff(s,runstart,getdate())))/3600-@breaktimetemp from runs where runindex=@maxrunindex)
		else select @volumeperhour = 0
		if (select convert(real,abs(datediff(s,runstart,getdate())))/3600-@breaktimetemp from runs where runindex=@maxrunindex) > 0
			select @piecesperhour = (select currentpieces from currentstate) / 
			(select convert(real,abs(datediff(s,runstart,getdate())))/3600-@breaktimetemp from runs where runindex=@maxrunindex)
		else select @piecesperhour = 0
		select @timesegment = (select abs(datediff(mi,getdate(),runstart)) from runs where runindex = @maxrunindex)	
	end*/
	
	update CurrentState set CurrentVolumePerHour = @volumeperhour,currentpiecesperhour=@piecesperhour,Currentshiftlugfill=@lugfill,Currentlugfill=@lugfill1,currentuptime=@uptime,currentreman=@percentreman, trimloss=@trimloss
    update CurrentState set VolumePerHour = (select SUM(volume) from lugfillcurrent1)
    select @volumeperhour = (select volumeperhour from CurrentState)
	update TargetSummary set volumeperhour=@volumeperhour,piecesperhour=(select COUNT(*) from lugfillcurrent1),lugfill=@lugfill1,uptime=@uptime where timesegment=@timesegment
    and shiftindex = @maxshiftindex

	-- fill in behind 
	/*update TargetSummary set piecesperhour = (select currentpieces from currentstate) / (convert(real,timesegment) /60)
	where piecesperhour=0 and timesegment<@timesegment
	update TargetSummary set volumeperhour = (select currentvolume from currentstate) / (convert(real,timesegment) /60)
	where volumeperhour=0 and timesegment<@timesegment*/
	
	/*Sorter Efficiency*/
	/*Actual Lugs Filled /( 110 * number of minutes run) displayed as percentage*/
	/*declare @sortereff real
	if (@timesegment > 0)
		select @sortereff = ROUND(@fulllugs / (110 * @timesegment) *100,0)
	else
		select @sortereff = 0*/
	

	
	
	if (select count(*) from ProductionBoards where shiftindex=@maxshiftindex and runindex=(select max(runindex) from runs)
	and ProdID=@productid and lengthID=@lengthid and thickactual=@thickactual and widthactual=@widthactual
	and lengthin=@LengthIn and net=@net and fet=@fet and cn2=@cn2 and fence=@fence and sorted=@sorted and sortcode=@sortcode) > 0
		update ProductionBoards set boardcount=Boardcount+1 where 
		shiftindex=@maxshiftindex and runindex=(select max(runindex) from runs)
		and ProdID=@productid and lengthID=@lengthid and thickactual=@thickactual and widthactual=@widthactual
		and lengthin=@LengthIn and net=@net and fet=@fet and cn2=@cn2 and fence=@fence and sorted=@sorted and sortcode=@sortcode
	else
		insert into ProductionBoards select @maxshiftindex,max(runindex),@productid, @lengthid,@thickactual,@widthactual,@lengthin,0,@net,@fet,@cn2,@fence,@sorted,@sortcode,1 from runs
	
	--update user message with current infeed length
	--update AlarmDefaults set Prefix = (select  'LEN: ' + CONVERT(varchar,productlength) from DriveCurrentState)
	--where AlarmID=1011
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
