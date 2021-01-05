SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
create PROCEDURE [dbo].[selectMobileProduction]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	create table #t
	(
	id int identity,
	category varchar(50),
	data int)
	
	insert into #t select 'VOLUME', convert(int,currentvolume) from CurrentState
	insert into #t select 'VOLUME/HR', convert(int,CurrentVolumePerHour) from CurrentState
	insert into #t select 'PIECES', convert(int,currentpieces) from CurrentState
	insert into #t select 'PIECES/HR', convert(int,CurrentPiecesPerHour) from CurrentState
	insert into #t select 'SHIFT LUG FILL', convert(int,CurrentShiftLugFill) from CurrentState
	select * from #t

    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
