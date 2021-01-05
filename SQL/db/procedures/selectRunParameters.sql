SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Kevin Bushell
-- Create date: <October 16, 2010>
-- Description:	This procedure determines the run thickness and run width
-- =============================================
CREATE PROCEDURE [dbo].[selectRunParameters]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @RecipeID int
	declare @RunThickness real, @RunWidth real
	select @RecipeID  = (select RecipeID from Recipes where online=1)

	create table #tempthick
	(id int identity,
	ThicknessNominal real,
	Occurrences smallint)
	create table #tempwidth
	(id int identity,
	WidthNominal real,
	Occurrences smallint)
	
	--compile a list of thicknesses and widths in the online recipe, and then choose the most prevalent values
	insert into #tempthick select thicknominal, count(thicknominal) from products,sortproducts
	where products.prodid = sortproducts.prodid and recipeid=@RecipeID
	group by thicknominal
	insert into #tempwidth select widthnominal, count(widthnominal) from products,sortproducts
	where products.prodid = sortproducts.prodid and recipeid=@RecipeID
	group by widthnominal

	select @RunThickness = (select min(ThicknessNominal) from #tempthick where #tempthick.Occurrences = (select max(occurrences) from #tempthick))
	
	select @RunWidth = (select min(WidthNominal) from #tempwidth where #tempwidth.Occurrences = (select max(occurrences) from #tempwidth))

	select RunThickness = @RunThickness, RunWidth = @RunWidth, recipeid=@recipeid
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
