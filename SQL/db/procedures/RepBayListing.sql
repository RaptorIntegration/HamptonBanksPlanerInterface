SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <July 4, 2010>
-- Description:	<Retrieves data for the Bay Listing Crystal report>
-- =============================================
CREATE PROCEDURE [dbo].[RepBayListing]
as	
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    create table #tempbay(
	BinID int,
	BinLabel varchar(100),
	BinStatusLabel varchar(50),
	BinSize int,
	BinCount int,
	RW bit,
	BinStamps varchar(100),
	BinSprays varchar(100),
	BinPercent smallint,
	Sortid int,
	TrimFlag bit,
	ProductsLabel varchar(1000),
	Volume real)

	insert into #tempbay select bins.BinID,BinLabel,binstatuslabel,BinSize,BinCount,RW,BinStamps,BinSprays,binpercent,sortid,TrimFlag,ProductsLabel,
	SUM(thicknominal*widthnominal*LengthNominal*BoardCount)/144
	from Bins,BinProductlengths,products,lengths
	where binproductlengths.ProdID=products.ProdID and lengths.LengthID=binproductlengths.LengthID
	and bins.BinID=BinProductLengths.BinID
	group by bins.BinID,BinLabel,binstatuslabel,BinSize,BinCount,RW,BinStamps,BinSprays,binpercent,sortid,TrimFlag,ProductsLabel

insert into #tempbay select bins.BinID,BinLabel,binstatuslabel,BinSize,BinCount,RW,BinStamps,BinSprays,binpercent,sortid,TrimFlag,ProductsLabel,
	0
	from Bins
	where bins.BinID not in (select BinID from #tempbay)
	group by bins.BinID,BinLabel,binstatuslabel,BinSize,BinCount,RW,BinStamps,BinSprays,binpercent,sortid,TrimFlag,ProductsLabel

	declare @i smallint, @j smallint, @stamps int, @stampsfinal varchar(100), @sprays int, @spraysfinal varchar(100)
	select @i=1
	while @i<=(select max(Binid) from Bins)
	begin
		select @j=1
		select @stampsfinal=''
		select @stamps = (select convert(int,Binstamps) from #tempbay where Binid=@i)
		select @sprays = (select convert(int,Binsprays) from #tempbay where Binid=@i)
		while @j<=(select max(stampid) from stamps)
		begin
			if (power(2,@j-1) & @stamps) = power(2,@j-1)
			begin
				select @stampsfinal = @stampsfinal + convert(varchar,@j)
				select @stampsfinal=@stampsfinal + ','
			end
			select @j=@j+1
		end
		if @stampsfinal <> ''
			update #tempbay set Binstamps = substring(@stampsfinal,1,len(@stampsfinal)-1) where Binid=@i
		select @j=1
		select @spraysfinal=''
		while @j<=(select max(sprayid) from sprays)
		begin
			if (power(2,@j-1) & @sprays) = power(2,@j-1)
			begin
				select @spraysfinal = @spraysfinal + convert(varchar,@j)
				select @spraysfinal=@spraysfinal + ','
			end
			select @j=@j+1
		end
		if @spraysfinal <> ''
			update #tempbay set Binsprays = substring(@spraysfinal,1,len(@spraysfinal)-1) where Binid=@i
		select @i=@i+1
	end

	select * from #tempbay order by binid
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
