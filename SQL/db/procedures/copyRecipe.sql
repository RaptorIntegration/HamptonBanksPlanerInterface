SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 27, 2010>
-- Description:	<Deletes recipe based on gui selection>
-- =============================================
CREATE PROCEDURE [dbo].[copyRecipe]
	@RecipeID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @recipelabel varchar(100), @newrecipeid int
	select @recipelabel = (select recipelabel from recipes where recipeid=@recipeid)
	
	update Recipes set editing=0
    insert into recipes select 'Copy of ' + @recipelabel,0,0,0,0,0,1 
	select @newrecipeid = (select max(recipeid) from recipes)
	
	delete from sorts where recipeid=@newrecipeid
	delete from sortproducts where recipeid=@newrecipeid
	delete from sortlengths where recipeid=@newrecipeid
	delete from sortproductlengths where recipeid=@newrecipeid
	delete from sortproductschild where recipeid=@newrecipeid
	delete from sortlengthschild where recipeid=@newrecipeid
	delete from sortproductlengthschild where recipeid=@newrecipeid
	delete from gradematrix where recipeid=@newrecipeid
	delete from drives where recipeid=@newrecipeid

	insert into sorts select @newrecipeid,sortid,sortlabel,active,sortsize,zone1start,zone1stop,zone2start,zone2stop,
	pkgspersort,rw,ordercount,sortstamps,sortstampslabel,sortsprays,sortsprayslabel,binid,cn2frequency,trimflag,productslabel,SecProdID,SecSize
	from sorts where recipeid=@recipeid
	insert into sortproducts select @newrecipeid,sortid,prodid 
	from sortproducts where recipeid=@recipeid
	insert into sortlengths select @newrecipeid,sortid,lengthid
	from sortlengths where recipeid=@recipeid
	insert into sortproductlengths select @newrecipeid,sortid,prodid,lengthid
	from sortproductlengths where recipeid=@recipeid
	insert into sortproductschild select @newrecipeid,sortid,prodid 
	from sortproductschild where recipeid=@recipeid
	insert into sortlengthschild select @newrecipeid,sortid,lengthid
	from sortlengthschild where recipeid=@recipeid
	insert into sortproductlengthschild select @newrecipeid,sortid,prodid,lengthid
	from sortproductlengthschild where recipeid=@recipeid
	insert into gradematrix select @newrecipeid,plcgradeid,websortgradeid,gradestamps,[default]
	from gradematrix where recipeid=@recipeid
	insert into drives select @newrecipeid,driveid,command,actual,length1multiplier,length2multiplier,length3multiplier,length4multiplier,length5multiplier
	,length6multiplier,length7multiplier,length8multiplier,length9multiplier,length10multiplier
	from drives where recipeid=@recipeid
		
		
		
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
