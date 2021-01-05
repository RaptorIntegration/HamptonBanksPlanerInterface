SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <May 5, 2010>
-- Description:	<This procedure updates the date in the ReportHeader table so that the correct header information appears on the report>
-- =============================================
CREATE PROCEDURE [dbo].[upReportHeader] 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @shiftstart datetime, @shiftend datetime
    declare @recipeid int

	select @shiftstart = (select shiftstart from shifts where shiftindex = (select shiftindexstart from reportheader))
	select @shiftend = (select shiftend from shifts where shiftindex = (select shiftindexend from reportheader))
	if @shiftend is null 
		select @shiftend=getdate()
	select @recipeid = (select recipeid from reportheader)

-- to do: determine recipelabels depending on how many recipeids are in the production tables
	/*if @recipeid = 0  --all recipes
	begin
		if (select count(distinct recipeid) from runs where @shiftstart between runstart and runend) = 1
			update reportheader set recipelabel = (select recipelabel from recipes where recipeid = (select max(recipeid) from runs where @shiftstart between runstart and runend))
	end*/

	update reportheader set reportstart = @shiftstart, reportend=@shiftend
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
