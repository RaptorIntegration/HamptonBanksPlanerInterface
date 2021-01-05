SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <July 12, 2010>
-- Description:	<Retrieves Product data for the Product Listing crystal report>
-- =============================================
CREATE PROCEDURE [dbo].[RepProducts]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


 SELECT "Products"."ProdID", specs.speclabel,moistures.moisturelabel,"Grades"."GradeLabel", "Products"."Active", "Products"."ProdLabel", "Products"."ThickNominal", "Products"."ThickMin", "Products"."ThickMax", "Products"."WidthNominal", "Products"."WidthMin", "Products"."WidthMax"
 FROM   ("RaptorWebSort"."dbo"."Products" "Products" INNER JOIN "RaptorWebSort"."dbo"."Grades" "Grades" ON "Products"."GradeID"="Grades"."GradeID"
 INNER JOIN "RaptorWebSort"."dbo"."specs" "specs" ON "Products"."specID"="specs"."specID"
 INNER JOIN "RaptorWebSort"."dbo"."moistures" "moistures" ON "Products"."moistureID"="moistures"."moistureID") 
 where prodid>0 and deleted=0 ORDER BY "Products"."ThickNominal", "Products"."WidthNominal", specs.speclabel,moistures.moisturelabel,"Grades"."GradeLabel"
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
