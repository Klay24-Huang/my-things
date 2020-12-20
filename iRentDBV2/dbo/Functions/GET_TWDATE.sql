/****** Object:  UserDefinedFunction [dbo].[GET_TWDATE]    Script Date: 2020/12/20 下午 04:05:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*********************************************************
  * Server   : SQYHFC06VM
  * Database : EASYRENT_WEB
  * 系    統 : 和運短租官網
  * 程式名稱 : GET_TWDATE()
  * 程式功能 : 
  * 程式作者 : ADAM
  * 修改日期 : 
  ********************************************************/
  /*
	SELECT dbo.GET_TWDATE()
  */

CREATE FUNCTION [dbo].[GET_TWDATE]()

RETURNS DATETIME  AS 

BEGIN 
	DECLARE @RTNDATE DATETIME
	SET @RTNDATE = DATEADD(hour,8,GETDATE())

	RETURN (@RTNDATE)
END
