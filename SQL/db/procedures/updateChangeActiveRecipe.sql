SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <July 2, 2010>
-- Description:	<does necessary housekeeping when active recipe is changed>
-- =============================================
CREATE PROCEDURE [dbo].[updateChangeActiveRecipe] 
@RecipeID int, @ResetStatistics bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @currenttime datetime, @currentrunindex int, @newrunindex int, @currentshiftindex int
 
	select @currenttime = getdate()
	if (select count(*) from runs) = 0
		select @currentrunindex = 0
	else
		select @currentrunindex = (select max(runindex) from runs)
	select @newrunindex = @currentrunindex + 1
	select @currentshiftindex = (select max(shiftindex) from shifts)

	update runs set runend = @currenttime where runindex = @currentrunindex
	insert into runs select @newrunindex,@currenttime,null,@RecipeID,TargetVolumePerHour,TargetPiecesPerHour,TargetLugFill,TargetUptime
	from recipes where recipeid=@RecipeID

	insert into currentstateprevious select @currentshiftindex,@currentrunindex,CurrentVolume,CurrentPieces,CurrentShiftLugFill,CurrentUptime,CurrentVolumePerHour,CurrentPiecesPerHour,currentLPM from CurrentState
	update currentstate set currentvolume=0,currentpieces=0,currentshiftlugfill=0,currentuptime=0,currentvolumePerHour=0,currentpiecesPerHour=0


	-- clear the tally
	if @ResetStatistics = 1
		update RaptorCommSettings set DataRequests = DataRequests | 16384

	-- send PLC the run thickness and width
    update RaptorCommSettings set DataRequests = DataRequests | 32768
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
