SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 27, 2010>
-- Description:	<Deletes recipe based on gui selection>
-- =============================================
CREATE PROCEDURE [dbo].[createRecipe]
	@RecipeLabel varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @sortcount smallint, @recipeid int,  @i smallint, @drivecount smallint
	select @sortcount = 175
	select @i=1

	update Recipes set editing=0
    insert into recipes select @RecipeLabel,0,0,0,0,0,1
	select @recipeid=(select max(recipeid) from recipes)
	while @i<=@sortcount
	begin
		delete from sorts where recipeid=@recipeid and sortid=@i
		insert into sorts select @recipeid,@i,'Sort ' + convert(varchar,@i),1,0,1,118,1,118,1,0,0,0,'',0,'',0,1,''
		
		select @i=@i+1
	end
	
	/*select @drivecount = (select count(*) from drivesettings)
	select @i=1
	while @i<=@drivecount
	begin
		delete from drives where recipeid=@recipeid and driveid=@i
		insert into drives select @recipeid,@i,0,0,1,1,1,1,1,1,1,1,1,1
		
		select @i=@i+1
	end*/
	
	select @i=1
	while @i<=32
	begin
		delete from gradematrix where recipeid=@recipeid and plcgradeid=@i
		insert into gradematrix select @recipeid,@i,@i,0,0
		
		select @i=@i+1
	end
	
	
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
