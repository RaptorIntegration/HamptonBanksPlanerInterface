SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
CREATE        procedure [dbo].[selectTicket]
@Packagenumber int
  
as begin
    declare  @c int, @tagid varchar(8), @current int
    declare @totalpieces int, @totalvolume real
	declare @filepath varchar(50),@filename varchar(50), @packwidth varchar(50), @packsize int
	declare @counter smallint, @text varchar(500), @surface varchar(10), @seasoning varchar(10), @lengthfinal varchar(20), @species varchar(10)
	declare @shiftindex int, @runindex int
	declare @volume real, @gradelabel varchar(20), @singlelength smallint, @dimensionsuffix varchar(1)
	
	select @shiftindex = (select MAX(shiftindex) from shifts)
	select @runindex = (select MAX(runindex) from Runs)
	select @species = 'DF'
	
	select @seasoning= 'GRN'
	
	select @surface= 'S4S'
	
	select @singlelength=0
	select @dimensionsuffix=''
    
    create table #t(
    id bigint identity,
    Packagenumber int,
    TimeStampReset datetime,
    PackageSize int,
    PackageCount int,
    PackageLabel varchar(50),
    Thickness int,
    Width int,
    [Length] int,
    GradeID int,
    GradeLink varchar(50),
    
    
    )
    
    
    
	If @PackageNumber in  (select Packagenumber from ProductionPackages with (nolock) )
	--if @PackageNumber is not null
	begin
		
		select @current=1
		update ProductionPackages set ticketprinted=1,printed=1 where Packagenumber=@Packagenumber
		
		
	end
	else 
    begin
		--Select @PackageNumber = (select min(Packagenumber) from ProductionPackagesPrevious with (nolock) where ticketprinted=0 )
		--if @PackageNumber is not null
		begin
			
			select @current=0	
			update ProductionPackagesPrevious set ticketprinted=1,printed=1 where Packagenumber=@Packagenumber
			
		
		end
		
		
	end
			
	
	
	insert into RaptorTicketLog select getdate(),'Inventory: ' + convert(varchar,@PackageNumber)	
	select @filepath = 'c:\Inventory\'
	
	truncate table #t
	if @current=1
	begin
		
		insert into #t
		select productionpackages.Packagenumber,GETDATE(),PackageSize,sum(ProductionPackagesProducts.boardcount),
		PackageLabel,max(ThickNominal),max(WidthNominal),(LengthNominal)/12,0,''
		from ProductionPackages,ProductionPackagesProducts,products,lengths
		where   ProductionPackages.PackageNumber = @PackageNumber
		and ProductionPackages.PackageNumber=ProductionPackagesProducts.PackageNumber
		and products.ProdID=ProductionPackagesProducts.prodid
		and lengths.LengthID=ProductionPackagesProducts.lengthid
		group by productionpackages.PackageNumber,lengthnominal,PackageSize,ProductionPackagesProducts.boardcount,PackageLabel
	             
	    select @gradelabel = (select gradelabel from Grades where GradeLabel like '%' + (select SUBSTRING(packagelabel,1,CHARINDEX(' ',packagelabel,1)-1) from ProductionPackages where PackageNumber=@Packagenumber) + '%')
		--select @gradelabel
		update #t set GradeLink=(select gradedescription from grades where grades.gradelabel=@gradelabel)
	
		            
		select @c=8-datalength(convert(varchar,@PackageNumber))
		select @tagid=''
		while @c>0
		begin
			select @tagid=@tagid + '0'
			select @c=@c-1		end
		select @tagid = @tagid + convert(varchar,@PackageNumber)
		update #t set PackageLabel=@tagid
		
		if (select recipelabel from Recipes where RecipeID = (select recipeid from runs where runindex=(select runindex from productionpackages where packagenumber=@Packagenumber))) like '%RGH%'
		begin
			select @surface = 'RGH'
			select @dimensionsuffix = 'R'
		end
		
	end
	else
	begin
		insert into #t
		select productionpackagesprevious.Packagenumber,GETDATE(),PackageSize,sum(ProductionPackagesProductsprevious.boardcount),
		PackageLabel,max(ThickNominal),max(WidthNominal),(LengthNominal)/12,0,''
		from ProductionPackagesprevious,ProductionPackagesProductsprevious,products,lengths
		where   ProductionPackagesprevious.PackageNumber = @PackageNumber
		and ProductionPackagesprevious.PackageNumber=ProductionPackagesProductsprevious.PackageNumber
		and products.ProdID=ProductionPackagesProductsprevious.prodid
		and lengths.LengthID=ProductionPackagesProductsprevious.lengthid
		group by productionpackagesprevious.PackageNumber,lengthnominal,PackageSize,ProductionPackagesProductsprevious.boardcount,PackageLabel
	             
	    select @gradelabel = (select gradelabel from Grades where GradeLabel like '%' + (select SUBSTRING(packagelabel,1,CHARINDEX(' ',packagelabel,1)-1) from ProductionPackagesprevious where PackageNumber=@Packagenumber) + '%')
		--select @gradelabel
		update #t set GradeLink=(select gradedescription from grades where grades.gradelabel=@gradelabel)
		
		select @c=8-datalength(convert(varchar,@PackageNumber))
		select @tagid=''
		while @c>0
		begin
			select @tagid=@tagid + '0'
			select @c=@c-1		end
		select @tagid = @tagid + convert(varchar,@PackageNumber)
		update #t set PackageLabel=@tagid
		
		if (select recipelabel from Recipes where RecipeID = (select recipeid from runs where runindex=(select runindex from productionpackagesprevious where packagenumber=@Packagenumber))) like '%RGH%'
		begin
			select @surface = 'RGH'
			select @dimensionsuffix = 'R'
		end
		
	end
	select * from #t
	
	if (select count(distinct [length]) from #t) =1
	begin
		select @lengthfinal=(select RIGHT('0'+convert(varchar,min([length])),2) from #t)
		select @singlelength=1
	end
	else
		select @lengthfinal=(select RIGHT('0'+convert(varchar,min([length])),2) from #t) + '-' + (select RIGHT('0'+convert(varchar,max([length])),2) from #t)
		
	select @filename=(select @tagid)
	declare @f varchar(100)
	select @f=(select 'del /Q ' + @filepath + '"' + @filename + '.txt"')
	exec master..xp_cmdshell @f,no_output
	select @totalpieces=(select sum(packagecount) from #t)
	
		
	select @text=(select 'echo ' 
		+ 'P'  
		+ ',' + @species 
		+ ',' + @seasoning
		+ ',' + replace(GradeLink,'&','^&')
		+ ',' + @surface
		+ ',' + (convert(varchar,Thickness) + @dimensionsuffix + 'X' + convert(varchar,Width) + @dimensionsuffix)
		+ ',,'
		+ ',' + convert(varchar,Thickness)   --RIGHT('0'+convert(varchar,Thickness),2)
		+ ',' + convert(varchar,Width)   --RIGHT('0'+convert(varchar,Width),2)
		+ ',' + @lengthfinal		
		+ ',' + @tagid 
		+ ',BAN' 
		+ ',' 
		+ ',' + convert(varchar,@totalpieces)
		+ ',' 
		+ ',BANDER'
		+ ','
		+ char(13)
		+ '  >> ' + @filepath + '"' + @filename + '.txt"' from #t where id=1)
		
	exec master..xp_cmdshell @text,no_output
	if @singlelength = 0
	begin
		select @counter=(select MIN(id) from #t)
		while @counter<=(select max(id) from #t)
		begin
			select @text=(select 'echo '  
			+ 'T' 
			+ ',' + replace(GradeLink,'&','^&')
			+ ',' + convert(varchar,Thickness) + 'X' + convert(varchar,Width)   --RIGHT('0'+convert(varchar,Thickness),2) + 'X' + RIGHT('0'+convert(varchar,Width),2)
			+ ',' + RIGHT('0' + convert(varchar,[length]),2)	
			+ ',' + convert(varchar,packagecount)
			+ char(13)
			+ '  >> ' + @filepath + '"' + @filename + '.txt"' from #t where id=@counter)
			
			--select @text
			exec master..xp_cmdshell @text,no_output
			
		
				
			select @counter=@counter+1
		end           
    end

	delete from raptorticketlog where id <(select max(id)-1000 from raptorticketlog)
	--copy files to network drive
	--Exec master.dbo.xp_cmdshell 'net use z: \delete /Y',no_output
	--Exec master.dbo.xp_cmdshell 'net use z: \\10.1.203.201\import /user:cl-bigchain\smbigchain password /persistent:Yes',no_output
	--Exec master.dbo.xp_cmdshell 'c:\raptorwebsort\movefiles.bat',no_output
return (0)
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
