SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: January 28, 2010
-- Description:	Retrieves Bin Details from the database
-- =============================================
CREATE PROCEDURE [dbo].selectBinData
	AS
BEGIN
	declare @i smallint, @j smallint
	declare @productslabel varchar(1000)
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	

	
	truncate table binstemp
	insert into binstemp select binid,binlabel,binstatus,null,binsize,bincount,rw,binstamps,null,binsprays,null,binpercent,sortid,trimflag,null,'' from bins with(NOLOCK)
	update binstemp set binstatuslabel='Empty' where binstatus=0
	update binstemp set binstatuslabel='Active' where binstatus=1
	update binstemp set binstatuslabel='Full' where binstatus=2
	update binstemp set binstatuslabel='Disabled' where binstatus=3
	update binstemp set binstatuslabel='Reject' where binstatus=4

	select @i=1
	While @i<=(select max(binid) from binstemp)
	begin
		if (select count(*) from binproducts where binid=@i) = 0
			update binstemp set productslabel='' where binid=@i
		else
		begin
			truncate table tproducts
			insert into tproducts
			select prodlabel + ' - ' + gradelabel + ' - ' + lengthlabel + ' [' + convert(varchar,boardcount) + ']' 
			from products,grades,lengths,binproductlengths
			where binid=@i and products.gradeid=grades.gradeid and binproductlengths.prodid=products.prodid
			and binproductlengths.lengthid=lengths.lengthid
			select @j=1
			select @productslabel = ''
			while @j<= (select max(id) from tproducts)
			begin
		
				select @productslabel = @productslabel + (select prodlabel from tproducts where id=@j)	
				if @j< (select max(id) from tproducts)
					select @productslabel = @productslabel + ',   '
				select @j=@j+1				
			end
			update binstemp set productslabel=@productslabel where binid=@i
		end		
		select @i=@i+1
	end
	


    select 'Bin' = binid, 'Bin Label'  = binlabel, 'Status' =binstatuslabel, 'Size' =binsize, 'Count' =bincount, 'Stamps'=binstampslabel,
	'Sprays'=binsprayslabel, 'Percent Full'=binpercent, 'Sort Link'=sortid, 'Products' =productslabel FROM binstemp
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
