SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: January 29, 2010
-- Description:	Updates the ProductsLabel column of the bins table based on gui bin editing
-- =============================================
CREATE PROCEDURE [dbo].[updateBinProductsGUI]
@binid int,
@productstring varchar(1000),
@lengthstring varchar(1000)
AS
BEGIN
	declare @i smallint, @j smallint, @p int, @l int, @productcount smallint, @lengthcount smallint, @gradeid smallint
	declare @prodid int, @lengthid int
	declare @s varchar(2),@product varchar(100), @species varchar(100), @grade varchar(100), @moisture varchar(100), @productlong varchar(100), @productslabel varchar(1000)
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

	--process product string from gui
	select @i=1
	select @productcount=0
	while @i<=len(@productstring)
	begin
		select @s = substring(@productstring,@i,1)
		if @s='|'
			select @productcount = @productcount+1
		select @i=@i+1
	end
	select @i=1
	while @i<=@productcount
	begin
				
		select @productlong = (select substring(@productstring,1,charindex('|',@productstring)-1))
		select @product = (select substring(@productlong,1,charindex('~',@productlong)-2))
		select @grade = (select SUBSTRING(@productlong,CHARINDEX('~',@productlong)+2,LEN(@productlong)))
		select @productstring = (select substring(@productstring,charindex('|',@productstring)+1,len(@productstring)))
		select @prodid = (select min(prodid) from products where prodlabel = @product and gradeid = (select min(gradeid) from grades where gradelabel=@grade)
		)
		insert into #tempproducts select @prodid

		select @i=@i+1
	end
	--process length string from gui
	select @i=1
	select @lengthcount=0
	while @i<=len(@lengthstring)
	begin
		select @s = substring(@lengthstring,@i,1)
		if @s='|'
			select @lengthcount = @lengthcount+1
		select @i=@i+1
	end
	select @i=1
	while @i<=@lengthcount
	begin
		select @length = (select substring(@lengthstring,1,charindex('|',@lengthstring)-1))
		select @lengthstring = (select substring(@lengthstring,charindex('|',@lengthstring)+1,len(@lengthstring)))
		select @lengthid = (select min(lengthid) from lengths where lengthlabel = @length)
		insert into #templengths select @lengthid

		select @i=@i+1
	end
	
	Select * from #templengths
	select * from #tempproducts

	--create new bin products data for this bin
	delete from binproducts where binid=@binid
	delete from binlengths where binid=@binid
	select @i=1
	while @i<=(select max(id) from #tempproducts)
	begin
		select @p = (select prodid from #tempproducts where id=@i)
		delete from binproducts where binid=@binid and prodid=@p
		insert into binproducts select @binid,@p
		select @j=1
		while @j<=(select max(id) from #templengths)
		begin
			select @l = (select lengthid from #templengths where id=@j)
			delete from binlengths where binid=@binid and lengthid=@l
			insert into binlengths select @binid,@l
			insert into #binproductsnew select @binid,@p,@l,0
			select @j=@j+1
		end
		select @i=@i+1
	end
	--update counts where necessary
	update #binproductsnew set #binproductsnew.boardcount=#binproductsold.boardcount from #binproductsold where #binproductsnew.prodid=#binproductsold.prodid and #binproductsnew.lengthid=#binproductsold.lengthid
	delete from binproductlengths where binid=@binid
	insert into binproductlengths select * from #binproductsnew
	

	--update bins table with new product text	
	if (select count(*) from binproductlengths where binid=@binid) = 0
		update bins set productslabel='' where binid=@binid
	else
	begin
		create table #tproducts(
		id int identity,
		prodlabel varchar(200))
		insert into #tproducts
		select '(' + prodlabel + ' ' + gradelabel + ' ' + lengthlabel + ') [' + convert(varchar,boardcount) + ']' from products,grades,lengths,binproductlengths
		where binid=@binid 
		and products.gradeid=grades.gradeid and binproductlengths.prodid=products.prodid
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
