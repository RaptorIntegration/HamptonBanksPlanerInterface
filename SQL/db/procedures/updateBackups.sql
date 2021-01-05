SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <June 15, 2010>
-- Description:	<Performs various backup tasks>
-- =============================================
CREATE PROCEDURE [dbo].[updateBackups]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		BACKUP DATABASE  RaptorWebsort  TO DISK = 'c:\raptorwebsort\websortbackup.bak' WITH INIT        
		insert into RaptorShiftMasterLog select getdate(),'Database Backed up to c:\RaptorWebSort\websortbackup.bak'
	END TRY
	BEGIN CATCH
		insert into RaptorShiftMasterLog select getdate(),'Database Backup failed'
	END CATCH
	
	--create daily backup on storage drive (not overwritten)
	/*BEGIN TRY
		declare @filename varchar(150)
		select @filename = 'd:\databasebackups\websortbackup ' + replace(CONVERT(varchar,getdate(),101),'/','-') + ' ' + DATENAME(hh,getdate()) + '.' + DATENAME(mi,getdate()) + '.bak'
		BACKUP DATABASE  RaptorWebsort  TO DISK = @filename WITH INIT        
		insert into RaptorShiftMasterLog select getdate(),'Database Backed up to ' + @filename
	END TRY
	BEGIN CATCH
		insert into RaptorShiftMasterLog select getdate(),'Database Backup failed'
	END CATCH
	
	Exec master.dbo.xp_cmdshell 'c:\raptorwebsort\filetrimmer.exe /d d:\databasebackups /a 10',no_output*/
	
	
	/*BEGIN TRY
	    
		Exec master.dbo.xp_cmdshell 'md f:\websortdatabasebackups',no_output
		BACKUP DATABASE  RaptorWebsort  TO DISK = 'f:\websortdatabasebackups\websortbackup.bak' WITH INIT
		insert into RaptorShiftMasterLog select getdate(),'Database Backed up to memory stick'
	END TRY
	BEGIN CATCH
		insert into RaptorShiftMasterLog select getdate(),'Database Backup to memory stick failed'
	END CATCH*/
	/*BEGIN TRY
		EXEC master.dbo.sp_configure 'show advanced options',1
		Reconfigure
		EXEC master.dbo.sp_configure 'xp_cmdshell',1
		Reconfigure
		
		--copy only newer files to memory stick
		Exec master.dbo.xp_cmdshell 'xcopy c:\inetpub\wwwroot\websort\*.* f:\install\inetpub\wwwroot\websort\ /D /E /Q /Y',no_output
		Exec master.dbo.xp_cmdshell 'xcopy c:\raptorwebsort\*.* f:\install\raptorwebsort\ /D /Q /Y',no_output
		insert into RaptorShiftMasterLog select getdate(),'WEBSort files Backed up to memory stick'
	END TRY
	BEGIN CATCH
		insert into RaptorShiftMasterLog select getdate(),'WEBSort files Backup to memory stick failed'
	END CATCH*/
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
