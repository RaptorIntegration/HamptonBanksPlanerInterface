SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		Josh Dollinger
-- Create date: 2019-04-15
-- Description:	Insert row at selected index
-- =============================================
CREATE PROCEDURE [dbo].[SortsInsertRow]
	-- Add the parameters for the stored procedure here
	@index int = 1,
	@RecipeID int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Cascade SortID's down one to make room for insertion in Sorts table
	UPDATE Sorts
	SET SortID = SortID + 1
	WHERE RecipeID = @RecipeID
	AND SortID >= @index
	
	-- Update SortID's in tables related to Sorts 
	UPDATE SortLengths
	SET SortID = SortID + 1
	WHERE RecipeID = @RecipeID
	AND SortID >= @index
	
	UPDATE SortProducts
	SET SortID = SortID + 1
	WHERE RecipeID = @RecipeID
	AND SortID >= @index
	
	UPDATE SortProductLengths
	SET SortID = SortID + 1
	WHERE RecipeID = @RecipeID
	AND SortID >= @index
	
	-- Insert "blank" row into Sorts at target location
	INSERT INTO Sorts (RecipeID, SortID, SortLabel, [Active] 
		,[SortSize] ,[Zone1Start] ,[Zone1Stop] ,[Zone2Start]
        ,[Zone2Stop] ,[PkgsPerSort] ,[RW] ,[OrderCount] ,[SortStamps]
        ,[SortStampsLabel] ,[SortSprays] ,[SortSpraysLabel] ,[BinID]
        ,[TrimFlag] ,[ProductsLabel]) 
	VALUES (@RecipeID, @index, 'Sort ' + CONVERT(VARCHAR(50), @index), 0,
		0, 1, 118, 1,
		118, 1, 0, 0, 0,
		'', 0, '', 0,
		1, '')	
	
	-- Retain Sorts table size
	DELETE FROM Sorts
	WHERE RecipeID = @RecipeID 
	AND SortID > 175	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
