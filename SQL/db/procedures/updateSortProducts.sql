SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: January 29, 2010
-- Description:	Updates the ProductsLabel column of the sorts table based on current sort product data
-- =============================================
CREATE PROCEDURE [dbo].[updateSortProducts]
@recipeid int,
@sortid int
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
	create table #sortproductsnew(
	recipeid int,
	binid int,
	prodid int,
	lengthid int)
	

	--process products
	insert into #tempproducts select distinct prodid from sortproducts with(NOLOCK) where sortid=@sortid and recipeid=@recipeid
		
	--process lengths
	insert into #templengths select distinct lengthid from sortlengths with(NOLOCK) where sortid=@sortid and recipeid=@recipeid

		

	--create new sort products data for this bin
	select @i=1
	while @i<=(select max(id) from #tempproducts)
	begin
		select @p = (select prodid from #tempproducts where id=@i)
		select @j=1
		while @j<=(select max(id) from #templengths)
		begin
			select @l = (select lengthid from #templengths where id=@j)
			insert into #sortproductsnew select @recipeid,@sortid,@p,@l
			select @j=@j+1
		end
		select @i=@i+1
	end
	delete from sortproductlengths where sortid=@sortid and recipeid=@recipeid
	insert into sortproductlengths select * from #sortproductsnew
	

	--update bins table with new product text	
	if (select count(*) from sortproductlengths where sortid=@sortid and recipeid=@recipeid) = 0
		update sorts set productslabel='' where sortid=@sortid and recipeid=@recipeid
	else
	begin
		create table #tproducts(
		id int identity,
		prodlabel varchar(200))
		insert into #tproducts
		select '(' + prodlabel + ' ' + gradelabel + ' ' + lengthlabel + ')' from products,grades,lengths,sortproductlengths
		where sortid=@sortid  and recipeid=@recipeid and products.gradeid=grades.gradeid and sortproductlengths.prodid=products.prodid
		and sortproductlengths.lengthid=lengths.lengthid order by thicknominal,widthnominal,gradelabel,lengthnominal
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
		update sorts set productslabel=@productslabel where sortid=@sortid and recipeid=@recipeid
	end		
		


    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
