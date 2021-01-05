SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 23, 2010>
-- Description:	<Retrieves data for Crystal Package Summary Report>
-- =============================================
CREATE PROCEDURE [dbo].[RepPackageSummary]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    declare @shiftstart int, @shiftend int, @runstart int, @runend int, @recipeid int, @recipeidstart int, @recipeidend int
	declare @maxshiftid int, @count int, @countprevious int, @pkgcount int, @pkgcountprevious int, @grandtotalpkgs int
	declare @trimlossfactor real
	
	select @trimlossfactor = (select trimlossfactor/100 from WEBSortSetup)

	select @maxshiftid = (select max(shiftindex) from shifts)

	select @shiftstart = (select shiftindexstart from reportheader)
	select @shiftend = (select shiftindexend from reportheader)
	select @runstart = (select runindexstart from reportheader)
	select @runend = (select runindexend from reportheader)
	select @recipeid = (select recipeid from reportheader)
	if @recipeid = 0
	begin
		select @recipeidstart = 1
		select @recipeidend = 32767
	end
	else
	begin
		select @recipeidstart = @recipeid
		select @recipeidend = @recipeid
	end
	create table #package(
	packagenumber int,
	[Package Type Count] int,
	packagelabel varchar(100),
	packagesize int,
	packagecount int,
	thicknominal real,
	widthnominal real,
	gradelabel varchar(50),
	moisturelabel varchar(50),
	lengthnominal real,
	lengthlabel varchar(50),
	boardcount int,
	totalvolume real,
	grandtotalpkgs int)

	if @shiftstart = @maxshiftid --current shift only
	begin
		if (select count(*) from ProductionPackages, runs
		where runs.runindex = ProductionPackages.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend) = 0
			insert into #package
			select 0,0,'No Data',0,0,0,0,'No Data',0,'No Data',0,0,0
		else
		begin
			select @grandtotalpkgs = (select count(packagenumber) from ProductionPackages)
			insert into #package
			select ProductionPackages.packagenumber,0,packagelabel,packagesize,packagecount,thicknominal,widthnominal,gradelabel,moisturelabel,lengthnominal,lengthlabel,boardcount=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)*@trimlossfactor,grandtotalpkgs=@grandtotalpkgs
			from ProductionPackages,ProductionPackagesProducts,products,lengths,grades,moistures,runs
			where products.gradeid=grades.gradeid
			and products.moistureid=moistures.moistureid
			and products.prodid = ProductionPackagesProducts.prodid
			and lengths.lengthid = ProductionPackagesProducts.lengthid
			and runs.runindex = ProductionPackages.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionPackages.shiftindex = @maxshiftid
			and ProductionPackages.PackageNumber = ProductionPackagesProducts.PackageNumber
			group by ProductionPackages.packagenumber,thicknominal,widthnominal,packagelabel,gradelabel,moisturelabel,lengthnominal,lengthlabel,packagesize,packagecount
		end
	end
	
	else --previous shifts and/or current shift
	begin
		select @count = (select count(*) from ProductionPackages,runs 
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionPackages.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @count is null select @count = 0
		
		select @countprevious = (select count(*) from ProductionPackagesPrevious,runs
		where shiftindex between @shiftstart and @shiftend
		and runs.runindex = ProductionPackagesPrevious.runindex
		and runs.RecipeID between @recipeidstart and @recipeidend)
		if @countprevious is null select @countprevious = 0
		select @count = @count + @countprevious
		
		if @count = 0
			insert into #package
			select 0,0,'No Data',0,0,0,0,'No Data',0,'No Data',0,0,0
		else
		begin
			select @pkgcount = (select count(Packagenumber) from ProductionPackages)
			select @pkgcountprevious = (select count(Packagenumber) from ProductionPackagesPrevious)
			if @pkgcount is null select @pkgcount = 0
			if @pkgcountprevious is null select @pkgcountprevious = 0
			select @grandtotalpkgs = @pkgcount + @pkgcountprevious
			insert into #package
			select ProductionPackages.packagenumber,0,packagelabel,packagesize,packagecount,thicknominal,widthnominal,gradelabel,moisturelabel,lengthnominal,lengthlabel,boardcount=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)*@trimlossfactor,grandtotalpkgs=@grandtotalpkgs
			from ProductionPackages,ProductionPackagesProducts,products,lengths,grades,moistures,runs
			where products.gradeid=grades.gradeid
			and products.moistureid=moistures.moistureid
			and products.prodid = ProductionPackagesProducts.prodid
			and lengths.lengthid = ProductionPackagesProducts.lengthid
			and runs.runindex = ProductionPackages.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionPackages.shiftindex between @shiftstart and @shiftend
			and ProductionPackages.PackageNumber = ProductionPackagesProducts.PackageNumber
			group by ProductionPackages.packagenumber,thicknominal,widthnominal,packagelabel,productslabel,gradelabel,moisturelabel,lengthnominal,lengthlabel,packagesize,packagecount
			union
			select ProductionPackagesPrevious.packagenumber,0,packagelabel,packagesize,packagecount,thicknominal,widthnominal,gradelabel,moisturelabel,lengthnominal,lengthlabel,boardcount=sum(boardcount),totalvolume=sum(thicknominal*widthnominal*lengthnominal*boardcount/144)*@trimlossfactor,grandtotalpkgs=@grandtotalpkgs
			from ProductionPackagesPrevious,ProductionPackagesProductsPrevious,products,lengths,grades,moistures,runs
			where products.gradeid=grades.gradeid
			and products.moistureid=moistures.moistureid
			and products.prodid = ProductionPackagesProductsPrevious.prodid
			and lengths.lengthid = ProductionPackagesProductsPrevious.lengthid
			and runs.runindex = ProductionPackagesPrevious.runindex
			and runs.RecipeID between @recipeidstart and @recipeidend
			and ProductionPackagesPrevious.shiftindex between @shiftstart and @shiftend
			and ProductionPackagesPrevious.PackageNumber = ProductionPackagesProductsPrevious.PackageNumber
			group by ProductionPackagesPrevious.packagenumber,thicknominal,widthnominal,packagelabel,productslabel,gradelabel,moisturelabel,lengthnominal,lengthlabel,packagesize,packagecount
		end
	end

	declare @i int, @j int,@id int, @counter int
	select @i = (select min(packagenumber) from #package)
	while @i <= (select max(packagenumber) from #package)
	begin
		select @counter = (select count(distinct packagenumber) from #package where packagelabel = (select distinct packagelabel from #package where packagenumber=@i))
		update #package set [package type count]=@counter where packagenumber=@i
		select @i=(select min(packagenumber) from #package where packagenumber>@i)
	end	

	/*select ([package type count]) as 'Package Type Count',packagelabel,packagesize,packagecount,thicknominal,widthnominal,gradelabel,lengthnominal,
	lengthlabel,boardcount,totalvolume,grandtotalpkgs  from #package
	group by [package type count],thicknominal,widthnominal,packagelabel,gradelabel,lengthnominal,lengthlabel,packagesize,packagecount,boardcount,totalvolume,grandtotalpkgs
*/
	select * from #package
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
