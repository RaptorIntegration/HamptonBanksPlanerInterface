SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <Feb 26, 2018>
-- Description:	<Processes Order Management data>
-- =============================================
CREATE PROCEDURE [dbo].[updateOrderManagement] 

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	return
	
	declare @RecipeID int
	
	if (select active from OrderManagementSettings) = 0
		return
	
	
	select @RecipeID =(select recipeid from recipes where online=1)
	--delete from OrderManagementAnticipation where SortID in (select SortID from Sorts where RecipeID=@RecipeID and OrderCount=0)
	
	--store current contents in temporary table so we can compare any changes after the fact.
	truncate table OrderManagementTemp
	insert into OrderManagementTemp select productionrundescription,websortproductid,nbeproductkey,
	websortproductdescription,websortspeciesdescription,websortgradeid,websortgradedescription,
	thicknessnominal,widthnominal,websortlengthid,lengthnominal,porter_cant_productactive,
	porter_bucking_productactive,nbe_edger_productactive,boardscannerq_productactive from OrderManagement
	
	truncate table OrderManagement
	
	insert into OrderManagement	
	select recipelabel,products.prodid,'',prodlabel,recipelabel,plcgradeid,gradelabel,thicknominal,widthnominal,lengths.lengthid,lengthnominal,
	sorts.active,sorts.active,sorts.active,sorts.active
	from products,grades,lengths,sorts,sortproductlengths,gradematrix,recipes
	where
	products.active=1 
	and products.deleted=0
	and products.gradeid=grades.gradeid
	and gradematrix.websortgradeid=products.gradeid
	and gradematrix.recipeid=sortproductlengths.recipeid
	and lengths.lengthid=sortproductlengths.lengthid
	and products.prodid=sortproductlengths.prodid
	and sorts.recipeid=sortproductlengths.recipeid
	and sorts.sortid=sortproductlengths.sortid
	and sortproductlengths.recipeid=@RecipeID
	and recipes.recipeid=@recipeid
	and products.prodid in (select distinct prodid from sortproductlengths where
	recipeid=@RecipeID
	and sortid in (select sortid from sorts where recipeid=@RecipeID
	and active=1))
	order by thicknominal,widthnominal,plcgradeid,lengthnominal
	
	
		
	--now go through and compare to the temp table to see if anything has changed, or based on anticipation,
	--update the notification table
	declare @counter int, @prodid int, @lengthid int
	declare @porter_cant_productactive bit, @porter_bucking_productactive bit, @NBE_Edger_productactive bit, @BoardScannerQ_productactive bit
	select @counter = (select min(tableindex) from ordermanagement)
	while @counter <= (select max(tableindex) from ordermanagement)
	begin
		select @prodid = (select websortproductid from ordermanagement where tableindex=@counter)
		select @lengthid = (select websortlengthid from ordermanagement where tableindex=@counter)
		
		--check for anticipation factors and potentially pre-emptively turn off products for certain machine centers.
/*		if (select count(*) from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid) > 0
		begin
			if (select OrderPiecesRemaining from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid) <= 
			(select Porter_Cant_Anticipation from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid)
				update OrderManagement set Porter_Cant_productactive=0 where WebSortProductid=@prodid and WebSortLengthid=@lengthid
			else
				update OrderManagement set Porter_Cant_productactive=1 where WebSortProductid=@prodid and WebSortLengthid=@lengthid
			if (select OrderPiecesRemaining from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid) <= 
			(select Porter_Bucking_Anticipation from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid)
				update OrderManagement set Porter_Bucking_productactive=0 where WebSortProductid=@prodid and WebSortLengthid=@lengthid
			else
				update OrderManagement set Porter_Bucking_productactive=1 where WebSortProductid=@prodid and WebSortLengthid=@lengthid
			if (select OrderPiecesRemaining from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid) <= 
			(select NBE_Edger_Anticipation from OrderManagementAnticipation where WebSortProductid=@prodid and WebSortLengthid=@lengthid)
				update OrderManagement set NBE_Edger_productactive=0 where WebSortProductid=@prodid and WebSortLengthid=@lengthid
			else
				update OrderManagement set NBE_Edger_productactive=1 where WebSortProductid=@prodid and WebSortLengthid=@lengthid
			
			
		end*/
		select @porter_cant_productactive = (select porter_cant_productactive from ordermanagement where tableindex=@counter)
		select @porter_bucking_productactive = (select porter_bucking_productactive from ordermanagement where tableindex=@counter)
		select @NBE_Edger_productactive = (select NBE_Edger_productactive from ordermanagement where tableindex=@counter)
		if (select count(*) from ordermanagementtemp where websortproductid=@prodid and websortlengthid=@lengthid and porter_cant_productactive=@porter_cant_productactive) = 0
			update OrderManagementNotification set Porter_Cant_NewData=1
		if (select count(*) from ordermanagementtemp where websortproductid=@prodid and websortlengthid=@lengthid and porter_bucking_productactive=@porter_bucking_productactive) = 0
			update OrderManagementNotification set Porter_Bucking_NewData=1
		if (select count(*) from ordermanagementtemp where websortproductid=@prodid and websortlengthid=@lengthid and NBE_Edger_productactive=@NBE_Edger_productactive) = 0
			update OrderManagementNotification set NBE_Edger_NewData=1
		
	
		select @counter=@counter+1
	end
	
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
