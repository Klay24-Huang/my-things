/*******************************************************
* Server     : SQYHI03AZ                               *
* Database   : IRENT_V2                                *	
* 程式名稱   : FN_CalRntdays                           *
* 系    統   : IRENT                                   *
* 程式功能   : 計算租用日                              *
* 作      者 : Adam                                    *
*													   *
*******************************************************/
CREATE FUNCTION [dbo].[FN_CalRntdays]
(
	@GIVEDATE DATETIME,
	@RNTDATE DATETIME
)
RETURNS NUMERIC(4,1)
AS
BEGIN
	DECLARE  @RNTDAYS	NUMERIC(4,1)
			,@DAYS		INT
			,@HOURS		INT
			,@DAYMAXHOUR	INT=1440
	SELECT   @DAYS = ISNULL(DATEDIFF(MINUTE,@GIVEDATE,@RNTDATE) / @DAYMAXHOUR,0)
			,@HOURS	= ISNULL(DATEDIFF(MINUTE,@GIVEDATE,@RNTDATE) % @DAYMAXHOUR,0)
	IF @HOURS >= 600
		SET @HOURS=600
	RETURN IIF(@HOURS=0,0,@DAYS + (@HOURS / 600.0))
END