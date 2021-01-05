SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 10, 2010>
-- Description:	<Retrieves production data by grade to display on the Boards screen>
-- =============================================
CREATE PROCEDURE [dbo].[selectBoardScreenGrades]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @trimlossfactor real
	select @trimlossfactor = (select trimlossfactor/100.0 from WEBSortSetup)

    select Grade=gradelabel ,Pieces = sum(boardcount), Volume=convert(int,sum(boardcount*thicknominal*widthnominal*lengthnominal/144*@trimlossfactor))
	from ProductionBoards,grades,products,lengths where products.prodid> 0  and sorted=1 and ProductionBoards.prodid=Products.ProdID and products.gradeid=grades.gradeid
	and lengths.lengthid=ProductionBoards.lengthid group by gradelabel order by Volume desc,gradelabel
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
