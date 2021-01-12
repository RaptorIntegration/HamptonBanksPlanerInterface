SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 24, 2010>
-- Description:	<Processes incoming Bay Data from PLC>
-- =============================================
CREATE PROCEDURE [dbo].[updateBinData] 
@FrameStart int,
@BayNum int,
@Name varchar(100),
@PkgSize int,
@Count int,
@RdmWidthFlag bit,
@Status int,
@Stamps bigint,
@Sprays int,
@TrimFlag bit,
@SortXRef int,
@ProductMap0 bigint,
@ProductMap1 bigint,
@ProductMap2 bigint,
@ProductMap3 bigint,
@ProductMap4 bigint,
@ProductMap5 bigint,
@LengthMap bigint,
@Ack bit,
@FrameEnd int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @PackageSize int, @PkgsPerSort smallint
	
	if @name like '%nbsp%' select @name=''
	update BinS set BinLabel=REPLACE(binlabel,'amp;','')
	
	select @PackageSize = @PkgSize
	if @PackageSize=0 select @PackageSize=(select binsize from bins where binid=@BayNum)
	if @PackageSize = 0 select @PackageSize = 1

	declare @statuscurrent int
	select @statuscurrent = (select binstatus from bins where binid=@BayNum)

    --update RaptorCommSettings set datarequests = datarequests-2 where (datarequests & 2)=2
    
	--if (select count(*) from bins where binid=@BayNum) = 0
		--insert into bins select @BayNum,@Name,@Status,'',@PkgSize,@Count,@RdmWidthFlag,@Stamps,'',@Sprays,'',ceiling(@Count/@PackageSize*100),@SortXRef,@TrimFlag,'',''

	if @FrameStart = 0 and @FrameEnd = 1  --this procedure was launched as a result of a WEBSort bin read
	begin
		if @Status=0 --bin is spare
		begin				
			update Bins set BinLabel='',BinStatus=@Status,BinSize=0,BinCount=0,RW=0,BinStamps=0,
			BinSprays=0,BinPercent=0,sortid=0,TrimFlag=0,productslabel ='',timestampfull=null where binid=@BayNum
			delete from BinProductLengths where binid=@BayNum
			delete from BinProducts where binid=@BayNum
			delete from BinLengths where binid=@BayNum
			exec updateBinProducts @BayNum
		end
		else if @Status=1 --bin is active
		begin
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
			where binid=@BayNum
			delete from binproducts where binid=@BayNum
			delete from binlengths where binid=@BayNum
			insert into binproducts select @Baynum,prodid from sortproducts where sortid=@sortxref and recipeid=(select recipeid from recipes where online=1)
			insert into binlengths select @BayNum,lengthid from sortlengths where sortid=@sortxref and recipeid=(select recipeid from recipes where online=1)
			--exec updateBinProducts @BayNum			
		end
		else if @Status=2 --bin is full
		begin
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=getdate()
			where binid=@BayNum
		end
		else if @Status=3 --bin is disabled
		begin
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
			where binid=@BayNum
		end
		else
		begin
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
			where binid=@BayNum
		end
		

		--update products in the bin if the sort cross reference is unknown
		--if @SortXRef = 0	
		begin
			declare @j smallint
			delete from binproducts where binid=@BayNum
			delete from binlengths where binid=@BayNum
			--delete from binproductlengths where binid=@BayNum
			
			select @j=1
			while @j<96
			begin
				if @j<32
				begin
					if @j = 31
					begin
						if (@ProductMap0 & convert(bigint,-2147483648)) = -2147483648
							insert into binproducts select @BayNum,@j
					end
					else
					begin
						if (@ProductMap0 & convert(bigint,power(2,@j)))=convert(bigint,power(2,@j))
							insert into binproducts select @BayNum,@j
					end
				end
				else if @j<64
				begin
					if @j-32 = 31
					begin
						if (@ProductMap1 & convert(bigint,-2147483648)) = -2147483648
							insert into binproducts select @BayNum,@j
					end
					else
					begin
						if (@ProductMap1 & convert(bigint,power(2,@j-32)))=convert(bigint,power(2,@j-32))
							insert into binproducts select @BayNum,@j
					end
				end
				else if @j<96
				begin
					if @j-64 = 31
					begin
						if (@ProductMap2 & convert(bigint,-2147483648)) = -2147483648
							insert into binproducts select @BayNum,@j
					end
					else
					begin
						if (@ProductMap2 & convert(bigint,power(2,@j-64)))=convert(bigint,power(2,@j-64))
							insert into binproducts select @BayNum,@j
					end
				end
				else if @j<128
				begin
					if @j-96 = 31
					begin
						if (@ProductMap3 & convert(bigint,-2147483648)) = -2147483648
							insert into binproducts select @BayNum,@j
					end
					else
					begin
						if (@ProductMap3 & convert(bigint,power(2,@j-96)))=convert(bigint,power(2,@j-96))
							insert into binproducts select @BayNum,@j
					end
				end
				else if @j<160
				begin
					if @j-128 = 31
					begin
						if (@ProductMap4 & convert(bigint,-2147483648)) = -2147483648
							insert into binproducts select @BayNum,@j
					end
					else
					begin
						if (@ProductMap4 & convert(bigint,power(2,@j-128)))=convert(bigint,power(2,@j-128))
							insert into binproducts select @BayNum,@j
					end
				end
				else
				begin
					if @j-160 = 31
					begin
						if (@ProductMap5 & convert(bigint,-2147483648)) = -2147483648
							insert into binproducts select @BayNum,@j
					end
					else
					begin
						if (@ProductMap5 & convert(bigint,power(2,@j-160)))=convert(bigint,power(2,@j-160))
							insert into binproducts select @BayNum,@j
					end
				end
				select @j=@j+1
			end
			select @j=1
			while @j<32
			begin
				if @j = 31
				begin
					if (@LengthMap & convert(bigint,-2147483648)) = -2147483648
						insert into binlengths select @BayNum,@j
				end
				else
				begin
					if (@LengthMap & convert(bigint,power(2,@j)))=convert(bigint,power(2,@j))
						insert into binlengths select @BayNum,@j
				end
				select @j=@j+1
			end 
			exec updateBinProducts @BayNum	
		end
		
	end

	else  --this procedure was launched as a result of a PLC bin status change OR as a result of a editing the status of a bin on the WEBSort Bay screen
	begin
		if @FrameStart = @FrameEnd --this procedure was launched as a result of a PLC bin status change
			insert into DataReceivedBin select getdate(),@FrameStart,@BayNum,@Count,@SortXRef,@Status,@RdmWidthFlag,@TrimFlag,@Ack,@FrameEnd
		
		if @Status=0 and @statuscurrent <> 0 --bin reset from PLC or setting bin spare in WEBSort from non-spare state
		begin			
			--create a package record
			declare @maxshiftindex int, @maxrunindex int, @packagenumber int, @SortSize int, @Loop smallint
			select @maxshiftindex = (select max(shiftindex) from shifts)
			select @maxrunindex = (select max(runindex) from runs)
			
			select @PkgsPerSort = (select pkgspersort from Sorts where SortID=@SortXRef and RecipeID = (select RecipeID from Recipes where Online=1))
			if @PkgsPerSort is null select @PkgsPerSort=1
			
			select @SortSize = (select sortsize from sorts where sortid=@SortXRef and RecipeID = (select RecipeID from Recipes where Online=1))
			/* it is possible that the bay size was edited after the fact, putting an to multi-package logic */
			if @PkgsPerSort>1
				if (select binsize from bins where binID=@BayNum) <= @sortsize
					select @PkgsPerSort=1
			if @PkgsPerSort>1
			begin
				-- it is possible the bay did not achieve full double bin capacity 
			  if (select bincount from bins where binid=@BayNum) <= (select binsize/2 from bins where binid=@BayNum)
			  select @PkgsPerSort=1
			end

			if @PkgsPerSort>1
			begin
				declare @sz int, @ct int, @bcount int, @bcount1 int
				select @sz = (select BinSize from Bins where binid=@BayNum)
				select @ct = (select BinCount from Bins where binid=@BayNum)

				if @sz <= @ct   /* full double bin */
				begin
				  select @bcount = @sz/2
				  select @bcount1 = @bcount
				end
				else
				begin
				
				  if (@sz/2) < @ct  /* one full pack and one partial pack */
				  begin
					select @bcount = @sz/2
					select @bcount1 = @ct - @bcount
				  end
				
				end
			end

			declare @pkgcount int
			Select @Loop = 0
			while (@Loop < @PkgsPerSort)
			begin
				if @PkgsPerSort>1
				begin
					if @Loop = 0 select @pkgcount=@bcount
    				else select @pkgcount=@bcount1
				end
				else select @pkgcount = (select BinCount from Bins where binid=@BayNum)
				
				if (select count(*) from ProductionPackagesPrevious) = 0 and (select count(*) from ProductionPackages) = 0
					Select @PackageNumber=1
				else
				begin
					if (select count(*) from ProductionPackages) = 0
						select @PackageNumber = (select max(packagenumber)+1 from ProductionPackagesPrevious)
					else
						select @PackageNumber = (select max(packagenumber)+1 from ProductionPackages)
				end
				
				--delete from ProductionPackagesProducts where packagenumber=@packagenumber
				insert into ProductionPackages select @maxshiftindex,@maxrunindex,@packagenumber,0,timestampfull,getdate(),convert(smallint,(convert(real,BinSize)/convert(real,@PkgsPerSort))),@pkgCount,@BayNum,@SortXRef,
				BinLabel,ProductsLabel,0,convert(varchar,sortid),'','' from bins where binid=@BayNum and binsize>0
				insert into ProductionPackagesProducts select @packagenumber,prodid,lengthid,convert(smallint,(convert(real,boardcount)/convert(real,@PkgsPerSort))) from BinProductLengths where binid=@BayNum and boardcount>0
				
				--account for odd numbers when splitting the packaage
				if @PkgsPerSort>1
				begin
				  declare @delta smallint
				  if (select sum(boardcount) from ProductionPackagesProducts where PackageNumber=@PackageNumber) < (select packagecount from ProductionPackages where PackageNumber=@PackageNumber)
				  begin  
			        
					select @delta = (select packagecount from ProductionPackages where PackageNumber=@PackageNumber) - (select sum(boardcount) from ProductionPackagesProducts where PackageNumber=@PackageNumber) 
					update ProductionPackagesProducts set boardcount=boardcount + @delta where PackageNumber=@PackageNumber and boardcount=
					(select max(boardcount) from ProductionPackagesProducts where PackageNumber=@PackageNumber)

				  end
						else if (select sum(boardcount) from ProductionPackagesProducts where PackageNumber=@PackageNumber) > (select packagecount from ProductionPackages where PackageNumber=@PackageNumber)
				  begin  
			 
					select @delta = (select sum(boardcount) from ProductionPackagesProducts where PackageNumber=@PackageNumber) - (select packagecount from ProductionPackages where PackageNumber=@PackageNumber)
					update ProductionPackagesProducts set boardcount=boardcount - @delta where PackageNumber=@PackageNumber and boardcount=
					(select max(boardcount) from ProductionPackagesProducts where PackageNumber=@PackageNumber)

				  end
				end
				if (select packagelabel from ProductionPackages where packagenumber=@packagenumber) = '' or (select packagelabel from ProductionPackages where packagenumber=@packagenumber) is null
				begin
					declare @productstring varchar(100), @i smallint, @productcount smallint, @s varchar(50), @products varchar(100), @productsfinal varchar(100)
					select @productstring = (select productslabel from productionpackages where packagenumber=@packagenumber)
					select @productsfinal=''
					select @i=1
					select @productcount=1
					while @i<=len(@productstring)
					begin
						select @s = substring(@productstring,@i,1)
						if @s=','
							select @productcount = @productcount+1
						select @i=@i+1
					end
					select @i=1
					while @i<=@productcount
					begin
						select @products = (select substring(@productstring,1,charindex(')',@productstring)))
						select @productstring = (select substring(@productstring,charindex(',',@productstring)+2,len(@productstring)))
						select @productsfinal = @productsfinal + @products
						if @i<@productcount
							select @productsfinal = @productsfinal + ', '
						select @i=@i+1
					end
					update ProductionPackages set packagelabel=@productsfinal where packagenumber=@packagenumber
				end			

				--create file for TagTrack inventory system
				-- execute selectTicket @Packagenumber
				Select @Loop = @Loop + 1
			end
			
			/* update DGS data */
			--stacked loads
			update DGSData set StackedLoads = (select count(*) from ProductionPackages where TimeStampReset is not null and PackageCount>0) where ShiftIndex = @maxshiftindex		
			update Bins set BinLabel='',BinStatus=@Status,BinSize=0,BinCount=0,RW=0,BinStamps=0,
			BinSprays=0,BinPercent=0,sortid=0,TrimFlag=0,productslabel ='',timestampfull=null where binid=@BayNum
			delete from BinProductLengths where binid=@BayNum
			delete from BinProducts where binid=@BayNum
			delete from BinLengths where binid=@BayNum
			exec updateBinProducts @BayNum
		end
		else if @Status=0  --bin reset from WEBSort on a bin that was already Spare
		begin				
			update Bins set BinLabel='',BinStatus=@Status,BinSize=0,BinCount=0,RW=0,BinStamps=0,
			BinSprays=0,BinPercent=0,sortid=0,TrimFlag=0,productslabel ='',timestampfull=null where binid=@BayNum
			delete from BinProductLengths where binid=@BayNum
			delete from BinProducts where binid=@BayNum
			delete from BinLengths where binid=@BayNum
			exec updateBinProducts @BayNum
		end
		else if @Status=1 and @statuscurrent = 3 --bin turned active from disabled state
		begin
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
			where binid=@BayNum				
		end
		else if @Status=1 and @statuscurrent <> 1 --bin active 
		begin
		
			if @SortXRef > 0
			begin
			
				update Bins set BinLabel=sortlabel,BinStatus=@Status,BinSize=sortSize*PkgsPerSort,RW=sorts.rw,BinStamps=SortStamps,
				BinSprays=SortSprays,BinPercent=ceiling(convert(real,@Count)/convert(real,sortSize)*100),sortid=@SortXRef,
				TrimFlag=sorts.TrimFlag,timestampfull=null
				from sorts,bins  where sorts.sortid=@sortxref and recipeid=(select recipeid from recipes where online=1)
				and bins.binid=@BayNum
				
				delete from binproducts where binid=@BayNum
				delete from binlengths where binid=@BayNum
				insert into binproducts select @Baynum,prodid from sortproducts where sortid=@sortxref and recipeid=(select recipeid from recipes where online=1)
				insert into binlengths select @BayNum,lengthid from sortlengths where sortid=@sortxref and recipeid=(select recipeid from recipes where online=1)
				--delete from binproductlengths where prodid not in (select prodid from binproducts where binid=@BayNum)
				--delete from binproductlengths where lengthid not in (select lengthid from binlengths where binid=@BayNum)
				exec updateBinProducts @BayNum				
			end
			else
				update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
				BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
				where binid=@BayNum				
		end
		else if @Status=2 and @statuscurrent <> 2 --bin full
		begin
			--bin may be receiving pieces while a user is editing the bin to full, so we must verify the count
			if (select bincount from bins where binid=@BayNum) > @Count
				select @Count = (select bincount from bins where binid=@BayNum)
			update Bins set BinStatus=@Status,BinCount=@Count,RW=@RdmWidthFlag,sortid=@SortXRef,
			TrimFlag=@TrimFlag,timestampfull=getdate(),BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100)
			where binid=@BayNum
			
			if (select ordercount from Sorts where SortID=@SortXRef and RecipeID=(select RecipeID from Recipes where Online = 1)) > 0
			begin
				update Sorts set OrderCount=OrderCount-1 where SortID=@SortXRef and RecipeID=(select RecipeID from Recipes where Online = 1)
				if (select ordercount from Sorts where SortID=@SortXRef and RecipeID=(select RecipeID from Recipes where Online = 1)) = 0
				begin
					update Sorts set Active=0 where SortID=@SortXRef and RecipeID=(select RecipeID from Recipes where Online = 1)
					--delete from alarmsettingsinfeed where alarmid=@SortXRef
					--insert into alarmsettingsinfeed select @SortXRef,0,sortlabel + ': PACKAGE ORDER COMPLETE',2,3 from sorts where SortID=@SortXRef and RecipeID=(select RecipeID from Recipes where Online = 1)
				end
			--update RaptorCommSettings set DataRequests = DataRequests | 2
			end
		end
		else if @Status=3 and @statuscurrent <> 3 --bin disable
		begin
			--bin may be receiving pieces while a user is editing the bin to disabled, so we must verify the count
			if (select bincount from bins where binid=@BayNum) > @Count
				select @Count = (select bincount from bins where binid=@BayNum)
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
			where binid=@BayNum			
		end
		else
			update Bins set BinLabel=@Name,BinStatus=@Status,BinSize=@PackageSize,BinCount=@Count,RW=@RdmWidthFlag,BinStamps=@Stamps,
			BinSprays=@Sprays,BinPercent=ceiling(convert(real,@Count)/convert(real,@PackageSize)*100),sortid=@SortXRef,TrimFlag=@TrimFlag,timestampfull=null
			where binid=@BayNum	
			
		
		

		
	end		

	if @statuscurrent = 5 
	begin
		update Bins set BinStatus=5	where BinID=@BayNum		
		select @Status=5
	end
	
	update currentstate set binsfull = (select count(*) from Bins where BinStatus = 2)
	update currentstate set binsspare = (select count(*) from Bins where BinStatus = 0)
	update bins set BinStatusLabel = (select binstatuslabel from BinStatus where BinStatus=@Status) where binid=@BayNum

	/* update DGS data */
	--full bins in sorter
	update DGSData set FullBinsInSorter = (select count(*) from Bins where binstatus=2) where ShiftIndex = @maxshiftindex		
			
			
	if (select count(*) from DataReceivedBin) > 500
		delete from DataReceivedBin where id<=(select max(id)-500 from DataReceivedBin)
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
