SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <September 13, 2010>
-- Description:	<This procedure selects the data for the WEBSort Drives Screen>
-- =============================================
create PROCEDURE [dbo].[selectInfeed4Drives]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @sqlstring nvarchar(800)


	select @sqlstring=''
	select @sqlstring = @sqlstring +  'select DriveID,DriveLabel,width4multiplier,width6multiplier,width8multiplier,width10multiplier,width12multiplier from [4inchspeeds] order by driveid'



	EXECUTE sp_executesql @sqlstring
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
