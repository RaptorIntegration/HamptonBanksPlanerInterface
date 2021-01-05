SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[updateStamps]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @recipeid int, @i smallint
	declare @gradeid int
	declare @newstamp int, @newstamp1 int
	
	select @recipeid = (select recipeid from GradeMatrixOld)
    select @gradeid = (select websortgradeid from GradeMatrixOld)
    select @newstamp1 = -2147483648
    select @newstamp = (select min(gradestamps) from GradeMatrix where RecipeID=@recipeid and WEBSortGradeID = @gradeid)
    if @newstamp is null select @newstamp=0
    select @newstamp1 = @newstamp | @newstamp1
    
    create table #temp(id int identity, sortid smallint)
    insert into #temp 
	select sorts.sortid from Sorts where sorts.RecipeID=@recipeid and sortid in (select sortid from sortproducts where recipeid=@recipeid and prodid in
    (select products.prodid from products,SortProducts where RecipeID=@recipeid and gradeid = @gradeid and products.ProdID=SortProducts.ProdID))
        
    select @i = (select MIN(id) from #temp)
    while @i<=(select MAX(id) from #temp)
    begin    
		--only adjust the stamps if there is only one grade (product) in the sort
		if (select COUNT(*) from SortProducts where RecipeID = @recipeid and SortID=(select sortid from #temp where id=@i)) = 1
			update Sorts set SortStamps = @newstamp where RecipeID = @RecipeID and SortID = (select sortid from #temp where id=@i)
		else
			delete from #temp where id=@i
		select @i=@i+1
    end
    select Sortid,gradestamps=@newstamp1 from #temp
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
