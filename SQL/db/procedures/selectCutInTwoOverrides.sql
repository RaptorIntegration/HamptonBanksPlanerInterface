SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 10, 2010>
-- Description:	<Retrieves production data by grade to display on the Boards screen>
-- =============================================
CREATE PROCEDURE [dbo].[selectCutInTwoOverrides]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @RecipeID int, @i int,@g int, @l int, @websortgradeid smallint
	declare @inputlengthnominal real
	declare @typeid smallint, @productlength smallint, @cn2 smallint
    
    insert into RaptorCommLog select GETDATE(),'Sending cut in two Overrides'
    select @inputlengthnominal = (select productlength*12 from DriveCurrentState)
    select @productlength = (select productlength from DriveCurrentState)
    
	
	
	select @RecipeID  = (select min(RecipeID) from Recipes where online=1)	

	create table #s(
	id int identity,
	gradeid smallint,
	parent smallint,
	child smallint,
	lengthid smallint,
	lengthnominal real,
	cn2 smallint,
	orders smallint,
	frequency smallint)
	
	   
    --first grab every grade and length combination that exists for the current recipe
    select @g=1
    while @g<=15
    begin
    
		select @websortgradeid = (select websortgradeid from GradeMatrix where RecipeID=@RecipeID and PLCGradeID=@g)
		
		if (select COUNT(*) from sorts,Sortproductlengths,products where sorts.RecipeID=@RecipeID and  ordercount>0 and sorts.Active=1 and
		sorts.sortid=sortproductlengths.sortid and sorts.recipeid=sortproductlengths.recipeid and products.gradeid=@websortgradeid
		and products.prodID=sortproductlengths.ProdID and binid>0 ) = 0
			insert into #s
			select @g,0,0,0,0,0,0,0
		else
		begin
			select @cn2 = (select lengthnominal/12 from Lengths where LengthID=(
			select MIN(lengthid) from SortProductLengths where RecipeID=@RecipeID and SortID=(
			select min(binid) from sorts,sortproductlengths,lengths,products where sorts.active=1 and ordercount>0 and  
			sorts.sortid=sortproductlengths.sortid and sorts.recipeid=sortproductlengths.recipeid and
			sorts.recipeid=@RecipeID and lengths.LengthID=SortProductLengths.LengthID and products.gradeid=@websortgradeid
			and products.prodID=sortproductlengths.ProdID and binid>0)))
			
			insert into #s 
			select @g,sorts.sortid,binid,SortProductLengths.LengthID,lengthnominal/12,@cn2,OrderCount,cn2frequency from sorts,sortproductlengths,lengths,products 
			where sorts.active=1 and ordercount>0 and 
			sorts.sortid=sortproductlengths.sortid and sorts.recipeid=sortproductlengths.recipeid and
			sorts.recipeid=@RecipeID and lengths.LengthID=SortProductLengths.LengthID and products.gradeid=@websortgradeid
			and products.prodID=sortproductlengths.ProdID and binid>0
			
	
			
		end		
		select @g=@g+1
	end
	
	select @i=1
	while @i<=(select MAX(id) from #s)
	begin
		if (select child from #s where id=@i) > 0
			update #s set cn2=(select lengthnominal/12 from Lengths where LengthID=(
			select MIN(lengthid) from SortProductLengths where RecipeID=@RecipeID and SortID=(select child from #s where id=@i)))
			where id=@i
		select @i=@i+1
	end
	
	--update #s set gradeid = (gradeid *10) + lengthid
	
	--fill in the holes with data
	select @g=0
	while @g<=19
	begin
		select @l=0
		while @l<=14
		begin
			if (select count(*) from #s where gradeid=@g and lengthid=@l) = 0
				insert into #s select @g,0,0,@l,0,0,0 ,0
			select @l=@l+1
		end
		select @g=@g+1
	end
	
	
	select * from #s
	order by gradeid,lengthid desc
	

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
