SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: January 29, 2010
-- Description:	Updates the ProductsLabel column of the bins table based on current bin product data
-- =============================================
CREATE PROCEDURE [dbo].[updateBinProducts]
@binid int
AS
BEGIN
	declare @i smallint, @j smallint, @p int, @l int, @productcount smallint, @lengthcount smallint, @gradeid smallint
	declare @prodid int, @lengthid int
	declare @s varchar(2),@product varchar(100), @grade varchar(100), @productlong varchar(100), @productslabel varchar(1000)
	declare @length varchar(100), @lengthlong varchar(100), @lengthlabel varchar(1000)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	create table #tempproducts(
	id int identity,
	prodid int)
	create table #templengths(
	id int identity,
	lengthid int)
	create table #binproductsnew(
	binid int,
	prodid int,
	lengthid int,
	boardcount int)
	create table #binproductsold(
	binid int,
	prodid int,
	lengthid int,
	boardcount int)

	--store current bin products data to compare with later
	insert into #binproductsold select * from binproductlengths with(NOLOCK) where binid=@binid

	--process products
	insert into #tempproducts select distinct prodid from binproducts with(NOLOCK) where binid=@binid
		
	--process lengths
	insert into #templengths select distinct lengthid from binlengths with(NOLOCK) where binid=@binid

		

	--create new bin products data for this bin
	select @i=1
	while @i<=(select max(id) from #tempproducts)
	begin
		select @p = (select prodid from #tempproducts where id=@i)
		select @j=1
		while @j<=(select max(id) from #templengths)
		begin
			select @l = (select lengthid from #templengths where id=@j)
			insert into #binproductsnew select @binid,@p,@l,0
			select @j=@j+1
		end
		select @i=@i+1
	end
	--update counts where necessary
	update #binproductsnew set #binproductsnew.boardcount=#binproductsold.boardcount from #binproductsold where #binproductsnew.prodid=#binproductsold.prodid and #binproductsnew.lengthid=#binproductsold.lengthid
	if (select sum(boardcount) from binproductlengths where binid=@binid) > 1
	begin
		delete from binproductlengths where binid=@binid
		insert into binproductlengths select * from #binproductsnew
	end

	--update bins table with new product text	
	if (select count(*) from binproductlengths where binid=@binid) = 0
		update bins set productslabel='' where binid=@binid
	else
	begin
		create table #tproducts(
		id int identity,
		prodlabel varchar(200))
		insert into #tproducts
		select '(' + prodlabel + ' ' + gradelabel  + ' ' + lengthlabel + ') [' + convert(varchar,boardcount) + ']' from products,grades,lengths,binproductlengths
		where binid=@binid and products.gradeid=grades.gradeid and binproductlengths.prodid=products.prodid
		and binproductlengths.lengthid=lengths.lengthid order by thicknominal,widthnominal,gradelabel,lengthnominal
		--update #tproducts set prodlabel = replace(prodlabel,'[0]','')
		select @j=1
		select @productslabel = ''
		while @j<= (select max(id) from #tproducts)
		begin
	
			select @productslabel = @productslabel + (select prodlabel from #tproducts where id=@j)	
			if @j< (select max(id) from #tproducts)
				select @productslabel = @productslabel + ', '
			select @j=@j+1				
		end

		update bins set productslabel=@productslabel where binid=@binid
	end		
		


    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
