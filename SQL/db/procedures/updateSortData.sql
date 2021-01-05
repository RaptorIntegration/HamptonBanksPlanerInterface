SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 24, 2010>
-- Description:	<Processes incoming Sort Data from PLC>
-- =============================================
CREATE PROCEDURE [dbo].[updateSortData] 
@sortid int,
@Name varchar(100),
@PkgSize int,
@RdmWidthFlag bit,
@PkgPerSort smallint,
@OrderCount smallint,
@Stamps int,
@Sprays int,
@TrimFlag bit,
@zone1 int,
@zone2 int,
@active bit,
@ProductMap0 bigint,
@ProductMap1 bigint,
@ProductMap2 bigint,
@ProductMap3 bigint,
@ProductMap4 bigint,
@ProductMap5 bigint,
@LengthMap bigint,
@ProductMap0c bigint,
@ProductMap1c bigint,
@ProductMap2c bigint,
@LengthMapc bigint

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @RecipeID int
	declare @zone1start smallint, @zone1stop smallint, @zone2start smallint, @zone2stop smallint

	select @zone1start = @zone1 & 255
	select @zone1stop = (@zone1/256) & 255
	select @zone2start = @zone2 & 255
	select @zone2stop = (@zone2/256) & 255
	select @RecipeID =(select recipeid from recipes where online=1)
	
	if (select count(*) from sorts where sortid=@sortid and recipeid=@recipeid) = 0
		insert into sorts select @recipeid, @sortid,@Name,@active,@PkgSize,@zone1start,@zone1stop,@zone2start,@zone2stop,@pkgpersort,@RdmWidthFlag,
		@ordercount,@Stamps,'',@Sprays,'',0,@TrimFlag,''
	else
	begin
		update sorts set sortLabel=@Name,active=@active,SortSize=@PkgSize,zone1start=@zone1start,zone1stop=@zone1stop,zone2start=@zone2start,zone2stop=@zone2stop,
		pkgspersort=@Pkgpersort,RW=@RdmWidthFlag,ordercount=@ordercount,sortStamps=@Stamps,sortstampslabel='',
		sortSprays=@Sprays,sortsprayslabel='',binid=0,TrimFlag=@TrimFlag
		where sortid=@sortid  and recipeid=@recipeid				
		
	end

	declare @j smallint
	delete from sortproducts where sortid=@sortid and recipeid=@recipeid
	delete from sortlengths where sortid=@sortid and recipeid=@recipeid
	
	select @j=1
	while @j<96
	begin
		if @j<32
		begin
			if @j = 31
			begin
				if (@ProductMap0 & convert(bigint,-2147483648)) = -2147483648
					insert into sortproducts select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap0 & convert(bigint,power(2,@j)))=convert(bigint,power(2,@j))
					insert into sortproducts select @recipeid,@sortid,@j
			end
		end
		else if @j<64
		begin
			if @j-32 = 31
			begin
				if (@ProductMap1 & convert(bigint,-2147483648)) = -2147483648
					insert into sortproducts select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap1 & convert(bigint,power(2,@j-32)))=convert(bigint,power(2,@j-32))
					insert into sortproducts select @recipeid,@sortid,@j
			end
		end
		else if @j<96
		begin
			if @j-64 = 31
			begin
				if (@ProductMap2 & convert(bigint,-2147483648)) = -2147483648
					insert into sortproducts select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap2 & convert(bigint,power(2,@j-64)))=convert(bigint,power(2,@j-64))
					insert into sortproducts select @recipeid,@sortid,@j
			end
		end
		else if @j<128
		begin
			if @j-96 = 31
			begin
				if (@ProductMap3 & convert(bigint,-2147483648)) = -2147483648
					insert into sortproducts select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap3 & convert(bigint,power(2,@j-96)))=convert(bigint,power(2,@j-96))
					insert into sortproducts select @recipeid,@sortid,@j
			end
		end
		else if @j<160
		begin
			if @j-128 = 31
			begin
				if (@ProductMap4 & convert(bigint,-2147483648)) = -2147483648
					insert into sortproducts select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap4 & convert(bigint,power(2,@j-128)))=convert(bigint,power(2,@j-128))
					insert into sortproducts select @recipeid,@sortid,@j
			end
		end
		else 
		begin
			if @j-160 = 31
			begin
				if (@ProductMap5 & convert(bigint,-2147483648)) = -2147483648
					insert into sortproducts select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap5 & convert(bigint,power(2,@j-160)))=convert(bigint,power(2,@j-160))
					insert into sortproducts select @recipeid,@sortid,@j
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
				insert into sortlengths select @recipeid,@sortid,@j
		end
		else
		begin
			if (@LengthMap & convert(bigint,power(2,@j)))=convert(bigint,power(2,@j))
				insert into sortlengths select @recipeid,@sortid,@j
		end
		select @j=@j+1
	end
	
	delete from sortproductschild where sortid=@sortid and recipeid=@recipeid
	delete from sortlengthschild where sortid=@sortid and recipeid=@recipeid
	
	select @j=1
	while @j<96
	begin
		if @j<32
		begin
			if @j = 31
			begin
				if (@ProductMap0c & convert(bigint,-2147483648)) = -2147483648
					insert into sortproductschild select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap0c & convert(bigint,power(2,@j)))=convert(bigint,power(2,@j))
					insert into sortproductschild select @recipeid,@sortid,@j
			end
		end
		else if @j<64
		begin
			if @j-32 = 31
			begin
				if (@ProductMap1c & convert(bigint,-2147483648)) = -2147483648
					insert into sortproductschild select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap1c & convert(bigint,power(2,@j-32)))=convert(bigint,power(2,@j-32))
					insert into sortproductschild select @recipeid,@sortid,@j
			end
		end
		else
		begin
			if @j-64 = 31
			begin
				if (@ProductMap2c & convert(bigint,-2147483648)) = -2147483648
					insert into sortproductschild select @recipeid,@sortid,@j
			end
			else
			begin
				if (@ProductMap2c & convert(bigint,power(2,@j-64)))=convert(bigint,power(2,@j-64))
					insert into sortproductschild select @recipeid,@sortid,@j
			end
		end
		select @j=@j+1
	end
	select @j=1
	while @j<32
	begin
		if @j = 31
		begin
			if (@LengthMapc & convert(bigint,-2147483648)) = -2147483648
				insert into sortlengthschild select @recipeid,@sortid,@j
		end
		else
		begin
			if (@LengthMapc & convert(bigint,power(2,@j)))=convert(bigint,power(2,@j))
				insert into sortlengthschild select @recipeid,@sortid,@j
		end
		select @j=@j+1
	end
	exec updateSortProducts @recipeid,@sortid
	
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
