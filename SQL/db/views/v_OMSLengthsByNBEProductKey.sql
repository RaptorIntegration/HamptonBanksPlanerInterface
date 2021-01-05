SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
/*	VIEW - Retrieve pivoted product lengths for NBE-keyed data

    NOTE THIS IS VALID FOR ALL 2' NOMINAL LENGTHS BETWEEN 8'and 20'
	IF THE NOMINAL VALUES CHANGE IN THE OMS DB THEN THE 'IN' VALUES
	BELOW WILL NEED TO CHANGE, AS WELL AS THE NBE PRODUCT PARAMETERS.

	Name				Date		Modification
	------------------	----------	------------------------------------------------------------
	D. McMahon			11/09/2017	Created
	--------------------------------------------------------------------------------------------
*/

CREATE VIEW dbo.v_OMSLengthsByNBEProductKey
AS

SELECT * FROM
(
	SELECT	NBEProductKey,
			IsNull(LengthNominal,0) 'Length',
			CAST(NBE_Edger_ProductActive AS SMALLINT) 'Active' 
	  FROM	dbo.OrderManagementTemp
) om  
 PIVOT  (
			Min(Active)
--          CURRENT PRODUCT LENGTHS BELOW: 
            FOR Length IN ([96], [120], [144], [168], [192], [216], [240])
		) AS pvt
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
