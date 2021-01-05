SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 21, 2010>
-- Description:	<Processes PLC production piece count read>
-- =============================================
CREATE PROCEDURE [dbo].[UpdatePLCProductionData] 
@productid int,
@Lengthid int,
@Pieces int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


	declare @maxshiftindex int, @maxrunindex int
	declare @thickactual real, @widthactual real,@lengthin real
	
	if (select COUNT(*) from Products where ProdID = @productid) = 0
		return
	if (select COUNT(*) from lengths where LengthID = @lengthid) = 0
		return
	select @maxshiftindex = (select max(shiftindex) from shifts)
	select @maxrunindex = (select max(runindex) from runs)
	select @thickactual = (select thicknominal from Products where ProdID=@productid)
	select @widthactual = (select widthnominal from Products where ProdID=@productid)
	select @lengthin = (select lengthnominal/12 from Lengths where LengthID=@Lengthid)
	
	
	
	-- update production table
	if (select count(*) from ProductionBoards where shiftindex=@maxshiftindex and runindex=(select max(runindex) from runs)
	and ProdID=@productid and lengthID=@lengthid and thickactual=@thickactual and widthactual=@widthactual
	and lengthin=@LengthIn and net=0 and fet=0 and cn2=0 and fence=0 and sorted=1 and sortcode=1) > 0
		update ProductionBoards set boardcount=Boardcount+@pieces where 
		shiftindex=@maxshiftindex and runindex=(select max(runindex) from runs)
		and ProdID=@productid and lengthID=@lengthid and thickactual=@thickactual and widthactual=@widthactual
		and lengthin=@LengthIn and net=0 and fet=0 and cn2=0 and fence=0 and sorted=1 and sortcode=1
	else
		insert into ProductionBoards select @maxshiftindex,max(runindex),@productid, @lengthid,@thickactual,@widthactual,@lengthin,0,0,0,0,0,1,1,@pieces from runs
	
	insert into RaptorCommLog select GETDATE(),'Appended Production Data, ProductID ' + CONVERT(varchar,@productid) + ', LengthID ' + CONVERT(varchar,@lengthID) + ', Pieces ' + CONVERT(varchar,@pieces)
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
