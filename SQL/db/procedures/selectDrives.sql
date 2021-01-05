SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO
-- =============================================
-- Author:		<Kevin Bushell>
-- Create date: <September 13, 2010>
-- Description:	<This procedure selects the data for the WEBSort Drives Screen>
-- =============================================
CREATE PROCEDURE [dbo].[selectDrives]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @sqlstring nvarchar(800), @NumLengths smallint
	declare @length1label varchar(20),@length2label varchar(20),@length3label varchar(20),@length4label varchar(20),@length5label varchar(20)
	declare @length6label varchar(20),@length7label varchar(20),@length8label varchar(20),@length9label varchar(20),@length10label varchar(20)
	
	select @NumLengths = NumLengths from WEBSortSetup
	select @length1label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=1)
	if @length1label is null select @length1label=' '
	select @length2label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=2)
	if @length2label is null select @length2label=' '
	select @length3label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=3)
	if @length3label is null select @length3label=' '
	select @length4label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=4)
	if @length4label is null select @length4label=' '
	select @length5label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=5)
	if @length5label is null select @length5label=' '
	select @length6label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=6)
	if @length6label is null select @length6label=' '
	select @length7label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=7)
	if @length7label is null select @length7label=' '
	select @length8label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=8)
	if @length8label is null select @length8label=' '
	select @length9label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=9)
	if @length9label is null select @length9label=' '
	select @length10label = (select replace(lengthlabel,'''','''''') from lengths where lengthid=10)
	if @length10label is null select @length10label=' '
	
	select @sqlstring=''
	/*select @sqlstring = @sqlstring + 'select drives.DriveID,DriveLabel,Type,Command,Result,''' + @length1label + '''=Length1Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length2label + '''=Length2Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length3label + '''=Length3Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length4label + '''=Length4Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length5label + '''=Length5Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length6label + '''=Length6Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length7label + '''=Length7Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length8label + '''=Length8Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length9label + '''=Length9Multiplier,'
	select @sqlstring = @sqlstring + '''' + @length10label + '''=Length10Multiplier'
	select @sqlstring = @sqlstring + ' from drives,drivesettings where '
	select @sqlstring = @sqlstring + ' drives.driveid=drivesettings.driveid and recipeid=(select recipeid from recipes where editing=1)'

	EXECUTE sp_executesql @sqlstring*/

	
	select @sqlstring = @sqlstring +  'select drives.DriveID,DriveLabel,Type,Command,Actual,L1=Length1Multiplier,'
	select @sqlstring = @sqlstring + 'L2=Length2Multiplier,'
	select @sqlstring = @sqlstring + 'L3=Length3Multiplier,'
	select @sqlstring = @sqlstring + 'L4=Length4Multiplier,'
	select @sqlstring = @sqlstring + 'L5=Length5Multiplier,'
	select @sqlstring = @sqlstring + 'L6=Length6Multiplier,'
	select @sqlstring = @sqlstring + 'L7=Length7Multiplier,'
	select @sqlstring = @sqlstring + 'L8=Length8Multiplier,'
	select @sqlstring = @sqlstring + 'L9=Length9Multiplier,'
	select @sqlstring = @sqlstring + 'L10=Length10Multiplier'
	select @sqlstring = @sqlstring + ' from drives,drivesettings where '
	select @sqlstring = @sqlstring + ' drives.driveid=drivesettings.driveid '


	EXECUTE sp_executesql @sqlstring
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

GO
