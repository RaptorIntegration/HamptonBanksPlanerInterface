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
	declare @counter smallint, @text varchar(500), @surface varchar(10), @seasoning varchar(10), @lengthfinal varchar(20)
	declare @shiftindex int, @runindex int
	declare @volume real
	
	select @shiftindex = (select MAX(shiftindex) from shifts)
	select @runindex = (select MAX(runindex) from Runs)
	select @surface= 'RGH'
	select @seasoning= 'GRN'
    
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
		update ProductionPackages set ticketprinted=0 where Packagenumber=@Packagenumber
		
		
	end
	else 
    begin
		--Select @PackageNumber = (select min(Packagenumber) from ProductionPackagesPrevious with (nolock) where ticketprinted=0 )
		--if @PackageNumber is not null
		begin
			
			select @current=0	
			update ProductionPackagesPrevious set ticketprinted=0 where Packagenumber=@Packagenumber
			
		
		end
		
		
	end
			
	
	begin
		insert into RaptorTicketLog select getdate(),'Inventory: ' + convert(varchar,@PackageNumber)	
		select @filepath = 'c:\Inventory\'
		
		truncate table #t
		if @current=1
		begin
			
			insert into #t
			select productionpackages.Packagenumber,GETDATE(),PackageSize,sum(ProductionPackagesProducts.boardcount),
			PackageLabel,max(ThickNominal),max(WidthNominal),(LengthNominal)/12,MIN(gradeid),''
			from ProductionPackages,ProductionPackagesProducts,products,lengths
			where   ProductionPackages.PackageNumber = @PackageNumber
			and ProductionPackages.PackageNumber=ProductionPackagesProducts.PackageNumber
			and products.ProdID=ProductionPackagesProducts.prodid
			and lengths.LengthID=ProductionPackagesProducts.lengthid
			group by productionpackages.PackageNumber,lengthnominal,PackageSize,ProductionPackagesProducts.boardcount,PackageLabel
		             
			select @c=8-datalength(convert(varchar,@PackageNumber))
			select @tagid=''
			while @c>0
			begin
				select @tagid=@tagid + '0'
				select @c=@c-1			end
			select @tagid = @tagid + convert(varchar,@PackageNumber)
			update #t set PackageLabel=@tagid
			update #t set GradeLink=(select gradedescription from grades where grades.gradeid=#t.gradeid)
			
			
		end
		else
		begin
			insert into #t
			select productionpackagesprevious.Packagenumber,GETDATE(),PackageSize,sum(ProductionPackagesProductsprevious.boardcount),
			PackageLabel,max(ThickNominal),max(WidthNominal),(LengthNominal)/12,MIN(gradeid),''
			from ProductionPackagesprevious,ProductionPackagesProductsprevious,products,lengths
			where   ProductionPackagesprevious.PackageNumber = @PackageNumber
			and ProductionPackagesprevious.PackageNumber=ProductionPackagesProductsprevious.PackageNumber
			and products.ProdID=ProductionPackagesProductsprevious.prodid
			and lengths.LengthID=ProductionPackagesProductsprevious.lengthid
			group by productionpackagesprevious.PackageNumber,lengthnominal,PackageSize,ProductionPackagesProductsprevious.boardcount,PackageLabel
		             
			select @c=8-datalength(convert(varchar,@PackageNumber))
			select @tagid=''
			while @c>0
			begin
				select @tagid=@tagid + '0'
				select @c=@c-1			end
			select @tagid = @tagid + convert(varchar,@PackageNumber)
			update #t set PackageLabel=@tagid
			update #t set GradeLink=(select gradedescription from grades where grades.gradeid=#t.gradeid)
			
		end
		select * from #t
		
		if (select gradelabel from grades where gradeid=(select min(gradeid) from #t)) like 'HVY'
			select @surface='RGH-HVY'
			
		if (select count(distinct [length]) from #t) =1
			select @lengthfinal=(select min([length]) from #t)
		else
			select @lengthfinal=(select convert(varchar,min(convert(int,[length]))) from #t) + '-' + (select convert(varchar,max(convert(int,[length]))) from #t)
			
		select @filename=(select @tagid)
		declare @f varchar(100)
		select @f=(select 'del /Q ' + @filepath + '"' + @filename + '.txt"')
		exec master..xp_cmdshell @f,no_output
		select @totalpieces=(select sum(packagecount) from #t)
		
		select @text=(select 'echo '  
			+ 'P,' + RIGHT('0'+convert(varchar,Thickness),2) + 'X' + RIGHT('0'+convert(varchar,Width),2)
			+ ',RWD' 
			+ ',' + GradeLink
			+ ',' + @seasoning
			+ ',' + @surface
			+ ',,'
			+ ',' + convert(varchar,Thickness)
			+ ',' + convert(varchar,Width)
			+ ',' + @lengthfinal		
			+ ',,,'
			+ ',' + convert(varchar,@totalpieces)
			+ ',,,' + char(13)
			+ '  >> ' + @filepath + '"' + @filename + '.txt"' from #t where id=1)
			
		exec master..xp_cmdshell @text,no_output
		select @counter=(select MIN(id) from #t)
		while @counter<=(select max(id) from #t)
		begin
			select @text=(select 'echo '  
			+ 'T' 
			+ ',' + GradeLink
			+ ',' + RIGHT('0'+convert(varchar,Thickness),2) + 'X' + RIGHT('0'+convert(varchar,Width),2)
			+ ',' + convert(varchar,[length])	
			+ ',' + convert(varchar,packagecount)
			+ char(13)
			+ '  >> ' + @filepath + '"' + @filename + '.txt"' from #t where id=@counter)
			
			exec master..xp_cmdshell @text,no_output
			
		
				
			select @counter=@counter+1
		end           
    end

	delete from raptorticketlog where id <(select max(id)-1000 from raptorticketlog)

return (0)
end
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
