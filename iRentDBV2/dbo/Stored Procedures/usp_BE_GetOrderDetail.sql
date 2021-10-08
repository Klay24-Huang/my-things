/***********************************************************************************************
* Server   : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_BE_GetOrderDetail
* 系    統 : IRENT
* 程式功能 : 取得後台訂單明細
* 作    者 : YEH
* 撰寫日期 : 20210930
* 修改日期 : 

* Example  : 
***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_BE_GetOrderDetail]
	@OrderNo	INT,		-- 訂單編號
	@IDNO		VARCHAR(10)	-- 帳號
AS
BEGIN
	-- 判斷是否為共同承租人，是的話將@IDNO置換成主承租人的帳號
	IF EXISTS(SELECT * FROM TB_SavePassenger WITH(NOLOCK) WHERE Order_number=@OrderNo AND MEMIDNO=@IDNO)
	BEGIN
		SELECT @IDNO=IDNO FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo;
	END

	SELECT *,AesEncode='' 
	FROM VW_BE_GetOrderFullDetail_Contact WITH(NOLOCK)
	WHERE OrderNo=@OrderNo
	AND (IDNO=@IDNO OR CAST(MEMRFNBR AS VARCHAR)=@IDNO)
	ORDER BY OrderNo ASC;
END

GO