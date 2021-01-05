SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <November 2, 2010>
-- Description:	<moves data around based on a sort move request>
-- =============================================
CREATE PROCEDURE [dbo].[updateMoveSort] 
@RecipeID int, @oldsortid smallint, @newsortid smallint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	if @oldsortid > @newsortid  --moving sort up
	begin
		update Sorts set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortLengths set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProducts set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProductLengths set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortLengthschild set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProductschild set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProductLengthschild set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update Sorts set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update SortLengths set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update SortProducts set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update SortProductLengths set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update SortLengthschild set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update SortProductschild set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update SortProductLengthschild set SortID = SortID+1 where SortID between @newsortid and (@oldsortid-1) and RecipeID=@RecipeID
		update Sorts set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortLengths set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProducts set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProductLengths set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortLengthschild set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProductschild set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProductLengthschild set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
	end
	else  --moving sort down
	begin
		update Sorts set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortLengths set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProducts set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProductLengths set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortLengthschild set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProductschild set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update SortProductLengthschild set SortID=999 where SortID=@oldsortid and RecipeID=@RecipeID
		update Sorts set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update SortLengths set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update SortProducts set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update SortProductLengths set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update SortLengthschild set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update SortProductschild set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update SortProductLengthschild set SortID = SortID-1 where SortID between (@oldsortid+1) and @newsortid and RecipeID=@RecipeID
		update Sorts set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortLengths set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProducts set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProductLengths set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortLengthschild set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProductschild set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		update SortProductLengthschild set SortID=@newsortid where SortID=999 and RecipeID=@RecipeID
		
	end
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
