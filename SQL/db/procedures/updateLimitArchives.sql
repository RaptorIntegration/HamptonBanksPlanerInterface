SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 15, 2010>
-- Description:	<Limits the size of the archived production tables>
-- =============================================
CREATE PROCEDURE [dbo].[updateLimitArchives]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @maxshiftindex int, @archivedshiftcount smallint
	select @archivedshiftcount = 500
	select @maxshiftindex = (select max(shiftindex) from shifts)

    delete from ProductionBoardsPrevious where shiftindex < (@maxshiftindex-@archivedshiftcount)
	delete from ProductionPackagesPrevious where shiftindex < (@maxshiftindex-@archivedshiftcount)
	delete from ProductionDiverterFailPrevious where shiftindex < (@maxshiftindex-@archivedshiftcount)
	delete from AlarmsPrevious where shiftindex < (@maxshiftindex-50)
	delete from TargetSummaryPrevious where shiftindex < (@maxshiftindex-@archivedshiftcount)
	delete from DGSData where shiftindex < (@maxshiftindex-@archivedshiftcount)
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
