SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <August 31, 2010>
-- Description:	<This procedure processes the Timing date received from the PLC>
-- =============================================
create PROCEDURE [dbo].[UpdateParameterData]
	@PLCID smallint,
	@Item1Data float

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	update [Parameters] set ItemValue = @Item1Data where ID=@PLCID and recipeid=(select min(recipeid) from recipes where online=1)
	


	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
