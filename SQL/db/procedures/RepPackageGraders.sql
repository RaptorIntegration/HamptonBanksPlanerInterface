SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 14, 2010>
-- Description:	<Retrieves data for Crystal Production Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepPackageGraders]
@PackageNumber int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

 
	
	
		if (select count(*) from ProductionPackagesGraders
		where PackageNumber=@PackageNumber) = 0
			select Grader='None',Pieces=0
		else
			select Grader=graderDescription, Pieces= sum(boardcount) 
			from ProductionPackagesGraders,graders
			where  PackageNumber=@PackageNumber
			and graders.GraderID=ProductionPackagesGraders.GraderID
			group by GraderDescription
			order by sum(boardcount)
		
	
	
	

	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
