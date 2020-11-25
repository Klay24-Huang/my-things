-- =============================================
-- Author:      ADAM
-- Create Date: 2020-11-22
-- Description: 短租轉送查詢
-- =============================================
CREATE PROCEDURE [dbo].[usp_HiEasyRetry_Q]
	@MSG			VARCHAR(100) OUTPUT,
	@NPRCD			INT		--(NPR060:0,NPR125:1,NPR130:2)
AS

	SET @MSG = ''
	SET NOCOUNT ON

	IF @NPRCD = 0
	BEGIN
		SELECT TOP 10 OrderNo=order_number
		FROM TB_BookingControl WITH(NOLOCK)
		WHERE isRetry=0
		AND RetryTimes < 3
		ORDER BY UPDTime DESC
	END
	ELSE IF @NPRCD=1
	BEGIN
		SELECT TOP 10 OrderNo=IRENTORDNO
		FROM TB_lendCarControl WITH(NOLOCK)
		WHERE isRetry=0
		AND RetryTimes < 3
		ORDER BY UPDTime DESC
	END
	ELSE
	BEGIN
		SELECT TOP 10 OrderNo=IRENTORDNO
		FROM TB_ReturnCarControl WITH(NOLOCK)
		WHERE isRetry=0
		AND RetryTimes < 3
		ORDER BY UPDTime DESC
	END