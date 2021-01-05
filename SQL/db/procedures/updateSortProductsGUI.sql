SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: January 29, 2010
-- Description:	Updates the ProductsLabel column of the sorts table based on gui sort editing
-- =============================================
CREATE PROCEDURE [dbo].[updateSortProductsGUI]
@recipeid int,
@sortid int,
@productstring varchar(1000),
@lengthstring varchar(1000)
AS
BEGIN
	declare @i smallint, @j smallint, @p int, @l int, @productcount smallint, @lengthcount smallint, @gradeid smallint
	declare @prodid int, @lengthid int
	declare @s varchar(2),@product varchar(100), @species varchar(100), @grade varchar(100), @moisture varchar(100), @productlong varchar(100), @productslabel varchar(1000)
	declare @length varchar(100), @lengthlong varchar(100), @lengthlabel varchar(1000)
	declare @ptext varchar(1000), @ptextnew varchar(1000), @changed varchar(1000)
	
	select @changed=''
	
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
	sortid int,
	prodid int,
	lengthid int)
	
	select @ptext = (select productslabel from sorts where recipeid=@recipeid and sortid=@sortid)
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

	--create new sort products data for this sort
	delete from sortproducts where sortid=@sortid and recipeid=@recipeid
	delete from sortlengths where sortid=@sortid and recipeid=@recipeid
	select @i=1
	while @i<=(select max(id) from #tempproducts)
	begin
		select @p = (select prodid from #tempproducts where id=@i)
		delete from sortproducts where recipeid=@recipeid and sortid=@sortid and prodid=@p
		insert into sortproducts select @recipeid,@sortid,@p
		select @j=1
		while @j<=(select max(id) from #templengths)
		begin
			select @l = (select lengthid from #templengths where id=@j)
			delete from sortlengths where recipeid=@recipeid and sortid=@sortid and lengthid=@l
			insert into sortlengths select @recipeid,@sortid,@l
			insert into #sortproductsnew select @recipeid,@sortid,@p,@l
			select @j=@j+1
		end
		select @i=@i+1
	end
	delete from sortproductlengths where recipeid=@recipeid and sortid=@sortid
	insert into sortproductlengths select * from #sortproductsnew
	

	--update sorts table with new product text	
	if (select count(*) from sortproductlengths where recipeid=@recipeid and sortid=@sortid) = 0
		update sorts set productslabel='' where recipeid=@recipeid and sortid=@sortid
	else
	begin
		create table #tproducts(
		id int identity,
		prodlabel varchar(200))
		insert into #tproducts
		select '(' + prodlabel + ' ' + gradelabel + ' ' + lengthlabel + ')' from products,grades,lengths,sortproductlengths
		where recipeid=@recipeid and sortid=@sortid and products.gradeid=grades.gradeid and sortproductlengths.prodid=products.prodid
		and sortproductlengths.lengthid=lengths.lengthid order by thicknominal,widthnominal,gradelabel,lengthnominal

		select @j=1
		select @productslabel = ''
		while @j<= (select max(id) from #tproducts)
		begin
	
			select @productslabel = @productslabel + (select prodlabel from #tproducts where id=@j)	
			if @j< (select max(id) from #tproducts)
				select @productslabel = @productslabel + ', '
			select @j=@j+1				
		end
		update sorts set productslabel=@productslabel where recipeid=@recipeid and sortid=@sortid
	end		
		
	select @ptextnew = (select productslabel from sorts where recipeid=@recipeid and sortid=@sortid)
	if @ptext <> @ptextnew
		select changed=@ptextnew
	else
	    select changed='0'
    
    END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
