SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 27, 2010>
-- Description:	<Deletes recipe based on gui selection>
-- =============================================
CREATE PROCEDURE [dbo].[updateRaptorDataToProSortScan]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if (select active from OrderManagementSettings) = 0
		return
		
	declare @productgroup int
	
	select @productgroup=10  --dfir
	if (select recipeid from Recipes where Online=1) = 39  --white fir
		select @productgroup=18
	
    insert into RaptorDataToProSortScan
	select 0,@productgroup,thicknominal*1000,widthnominal*1000,plcgradeid,lengthnominal*1000,lengthnominal*1000,0
	from products,lengths,sorts,sortproductlengths,gradematrix
	where gradematrix.recipeid=(select recipeid from recipes where online=1)
	and sortproductlengths.recipeid=gradematrix.recipeid
	and sorts.recipeid=sortproductlengths.recipeid
	and sorts.sortid=sortproductlengths.sortid
	and sorts.active=1 and products.active=1
	and products.prodid=sortproductlengths.prodid
	and lengths.lengthid = sortproductlengths.lengthid
	and products.gradeid=gradematrix.websortgradeid

	delete from RaptorDataToProSortScan where id <= (select max(id)-2000 from RaptorDataToProSortScan)

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
