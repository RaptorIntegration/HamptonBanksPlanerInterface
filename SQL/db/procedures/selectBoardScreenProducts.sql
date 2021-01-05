SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 10, 2010>
-- Description:	<Retrieves production data by product to display on the Boards screen>
-- =============================================
CREATE PROCEDURE [dbo].[selectBoardScreenProducts]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @trimlossfactor real
	select @trimlossfactor = (select trimlossfactor/100.0 from WEBSortSetup)

    select Product=prodlabel ,thicknominal,widthnominal,Pieces = sum(boardcount), Volume=convert(int,sum(boardcount*thicknominal*widthnominal*lengthnominal/144*@trimlossfactor))
	from ProductionBoards,products,lengths where products.prodid>0 and sorted=1 and ProductionBoards.prodid=Products.ProdID
	and lengths.lengthid=ProductionBoards.lengthid group by prodlabel,thicknominal,widthnominal order by Volume desc,thicknominal,widthnominal
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
