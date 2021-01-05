SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <July 4, 2010>
-- Description:	<Retrieves data for the Bay Listing Crystal report>
-- =============================================
CREATE PROCEDURE [dbo].[RepBayListing1]
as	
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    create table #tempbay(
	BinThicknessLabel varchar(100),
	BinThickness varchar(50),
	Volume real)

	insert into #tempbay select convert(varchar,thicknominal) + '"',thicknominal,
	SUM(thicknominal*widthnominal*LengthNominal*BoardCount)/144
	from Bins,BinProductlengths,products,lengths
	where binproductlengths.ProdID=products.ProdID and lengths.LengthID=binproductlengths.LengthID
	and bins.BinID=BinProductLengths.BinID
	group by thicknominal



	select * from #tempbay order by binthickness,BinThicknessLabel
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
