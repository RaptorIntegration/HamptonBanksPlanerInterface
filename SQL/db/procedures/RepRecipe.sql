SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <July 4, 2010>
-- Description:	<Retrieves data for the Recipe Crystal report>
-- =============================================
CREATE PROCEDURE [dbo].[RepRecipe]
as	
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @recipeid int
	select @recipeid=(select printrecipeid from reportheader)

	create table #temprecipe(
	SortID int,
	SortLabel varchar(100),
	Active bit,
	SortSize int,
	Zone1 varchar(50),
	Zone2 varchar(50),
	PkgsPerSort smallint,
	RW bit,
	OrderCount smallint,
	SortStamps varchar(100),
	SortSprays varchar(100),
	binid int,
	TrimFlag bit,
	ProductsLabel varchar(1000))

	insert into #temprecipe select SortID,SortLabel,Active,SortSize,convert(varchar,Zone1start) + '-' + convert(varchar,Zone1stop),
	convert(varchar,Zone2start) + '-' + convert(varchar,Zone2stop),
	PkgsPerSort,RW,OrderCount,SortStamps,SortSprays,binid,TrimFlag,	ProductsLabel
	from sorts where recipeid=@recipeid

	declare @i smallint, @j smallint, @stamps int, @stampsfinal varchar(100), @sprays int, @spraysfinal varchar(100)
	select @i=1
	while @i<=(select max(sortid) from sorts where recipeid=@recipeid)
	begin
		select @j=1
		select @stampsfinal=''
		select @stamps = (select convert(int,sortstamps) from #temprecipe where sortid=@i)
		while @j<=(select max(stampid) from stamps)
		begin
			if (power(2,@j-1) & @stamps) = power(2,@j-1)
			begin
				select @stampsfinal = @stampsfinal + convert(varchar,@j)
				select @stampsfinal=@stampsfinal + ','
			end
			select @j=@j+1
		end
		if @stampsfinal <> ''
			update #temprecipe set sortstamps = substring(@stampsfinal,1,len(@stampsfinal)-1) where sortid=@i
		select @spraysfinal=''
		select @j=1
		select @sprays = (select convert(int,sortsprays) from #temprecipe where sortid=@i)
		while @j<=(select max(sprayid) from sprays)
		begin
			if (power(2,@j-1) & @sprays) = power(2,@j-1)
			begin
				select @spraysfinal = @spraysfinal + convert(varchar,@j)
				select @spraysfinal=@spraysfinal + ','
			end
			select @j=@j+1
		end
		if @spraysfinal <> ''
			update #temprecipe set sortsprays = substring(@spraysfinal,1,len(@spraysfinal)-1) where sortid=@i
		select @i=@i+1
	end

	update #temprecipe set binid = bins.binID from bins where #temprecipe.SortID=bins.sortid
	select * from #temprecipe
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
