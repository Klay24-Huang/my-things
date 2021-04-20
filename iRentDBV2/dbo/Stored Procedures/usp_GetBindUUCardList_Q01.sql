/****** Object:  StoredProcedure [dbo].[usp_GetBindUUCardList_Q01]    Script Date: 2021/4/15 上午 09:23:52 ******/

-- =============================================
-- Author:      JET
-- Create Date: 2021-04-14
-- Description: 取得待綁定悠遊卡的資料
-- =============================================
/* EXAMPLE
exec usp_GetBindUUCardList_Q01 ''
*/
CREATE PROCEDURE [dbo].[usp_GetBindUUCardList_Q01]
	@MSG			VARCHAR(100) OUTPUT
AS
	SET @MSG = ''

	SET NOCOUNT ON
	
	SELECT [OrderNumber]
		,[IDNO]
		,[CID]
		,[DeviceToken]
		,[IsCens]
		,[OldCardNo]
		,[NewCardNo]
	FROM [dbo].[TB_BindUUCard]
	WHERE [Result]=0
	ORDER BY [UPTime],OrderNumber
GO