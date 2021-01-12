SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 27, 2010>
-- Description:	<Deletes recipe based on gui selection>
-- =============================================
CREATE PROCEDURE [dbo].[deleteRecipe]
	@RecipeID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    delete from recipes where recipeid=@RecipeID
	delete from sorts where recipeid=@RecipeID
	delete from sortproducts where recipeid=@RecipeID
	delete from sortlengths where recipeid=@RecipeID
	delete from sortproductlengths where recipeid=@RecipeID
	delete from gradematrix where recipeid=@RecipeID
	delete from drives where recipeid=@RecipeID
	update recipes set editing=1 where online=1
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
