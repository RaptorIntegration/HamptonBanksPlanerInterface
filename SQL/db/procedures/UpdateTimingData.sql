SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <August 31, 2010>
-- Description:	<This procedure processes the Timing date received from the PLC>
-- =============================================
CREATE PROCEDURE [dbo].[UpdateTimingData]
	@PLCID smallint,
	@Item1Data int,
	@Item2Data int,
	@Item3Data int,
	@Item4Data int,
	@Item5Data int,
	@Item6Data int,
	@Item7Data int,
	@Item8Data int,
	@Item9Data int,
	@Item10Data int

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update timing set ItemValue = @Item1Data where ID=0 and PLCID=@PLCID
	update timing set ItemValue = @Item2Data where ID=1 and PLCID=@PLCID
	update timing set ItemValue = @Item3Data where ID=2 and PLCID=@PLCID
	update timing set ItemValue = @Item4Data where ID=3 and PLCID=@PLCID
	update timing set ItemValue = @Item5Data where ID=4 and PLCID=@PLCID
	update timing set ItemValue = @Item6Data where ID=5 and PLCID=@PLCID
	update timing set ItemValue = @Item7Data where ID=6 and PLCID=@PLCID
	update timing set ItemValue = @Item8Data where ID=7 and PLCID=@PLCID
	update timing set ItemValue = @Item9Data where ID=8 and PLCID=@PLCID
	update timing set ItemValue = @Item10Data where ID=9 and PLCID=@PLCID


	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
