SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Josh Dollinger
-- Create date: 2019-04-15
-- Description:	Copy/Paste row from selected index to another
-- =============================================
CREATE PROCEDURE [dbo].[SortsMoveRow] 
	-- Add the parameters for the stored procedure here
	@RecipeID int = 0, 
	@SortIDTo int = 0,
	@SortIDFrom int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF (@SortIDFrom < @SortIDTo)
	BEGIN
		
		-- Cascade rows down to make room for insertion
		UPDATE Sorts 
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID > @SortIDTo
		
		UPDATE SortLengths
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID > @SortIDTo
		
		UPDATE SortProducts
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID > @SortIDTo
		
		UPDATE SortProductLengths
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID > @SortIDTo
		
		-- Move row to selected SortID
		UPDATE Sorts
		SET SortID = (@SortIDTo + 1)
		WHERE RecipeID = @RecipeID
		AND SortID = @SortIDFrom
		
		UPDATE SortLengths
		SET SortID = (@SortIDTo + 1)
		WHERE RecipeID = @RecipeID
		AND SortID = @SortIDFrom
		
		UPDATE SortProducts
		SET SortID = (@SortIDTo + 1)
		WHERE RecipeID = @RecipeID
		AND SortID = @SortIDFrom
		
		UPDATE SortProductLengths
		SET SortID = (@SortIDTo + 1)
		WHERE RecipeID = @RecipeID
		AND SortID = @SortIDFrom
		
		-- Reverse initial cascade step
		UPDATE Sorts
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID >= (@SortIDFrom + 1)
		
		UPDATE SortLengths
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID >= (@SortIDFrom + 1)
		
		UPDATE SortProducts
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID >= (@SortIDFrom + 1)
		
		UPDATE SortProductLengths
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID >= (@SortIDFrom + 1)
		
	END
	
	IF (@SortIDFrom > @SortIDTo)
	BEGIN
		
		-- Cascade rows down to make room for insertion
		UPDATE Sorts 
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID >= @SortIDTo
		
		UPDATE SortLengths
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID >= @SortIDTo
		
		UPDATE SortProducts
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID >= @SortIDTo
		
		UPDATE SortProductLengths
		SET SortID = SortID + 1
		WHERE RecipeID = @RecipeID
		AND SortID >= @SortIDTo
		
		-- Move row to selected SortID
		UPDATE Sorts
		SET SortID = @SortIDTo
		WHERE RecipeID = @RecipeID
		AND SortID = (@SortIDFrom + 1)
		
		UPDATE SortLengths
		SET SortID = @SortIDTo
		WHERE RecipeID = @RecipeID
		AND SortID = (@SortIDFrom + 1)
		
		UPDATE SortProducts
		SET SortID = @SortIDTo
		WHERE RecipeID = @RecipeID
		AND SortID = (@SortIDFrom + 1)
		
		UPDATE SortProductLengths
		SET SortID = @SortIDTo
		WHERE RecipeID = @RecipeID
		AND SortID = (@SortIDFrom + 1)
		
		-- Reverse initial cascade step
		UPDATE Sorts
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID > (@SortIDFrom + 1)
		
		UPDATE SortLengths
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID > (@SortIDFrom + 1)
		
		UPDATE SortProducts
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID > (@SortIDFrom + 1)
		
		UPDATE SortProductLengths
		SET SortID = SortID - 1
		WHERE RecipeID = @RecipeID
		AND SortID > (@SortIDFrom + 1)
		
	END	
		
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
