/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_GetMonthlyPayInvData
* 系    統 : IRENT
* 程式功能 : 訂閱制續期取月租設定產發票用
* 作    者 : AMBER
* 撰寫日期 : 20210812
* 修改日期 : 20220209 ADD BY AMBER REASON.新增判斷InvJob條件
Example :

***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMonthlyPayInvData]
(   @MonthlyRentId      　　INT      　　　　　　 ,
	@IdNo               　　VARCHAR(10)           ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
 	SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''	
	DECLARE @LogID INT = 0
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_GetMonthlyPayInvData'
	DECLARE @Error INT;
	/*初始設定*/
	SET @Error=0

BEGIN TRY
	  BEGIN
			IF @MonthlyRentId=0 or @IdNo=''
			BEGIN
			   SET @Error = 1
			   SET @ErrorCode='ERR900'		   
			END

			IF @Error = 0
			BEGIN	
				SELECT 
				u.ProjID as MonProjID,　
				u.MonProPeriod,
				u.ShortDays,
				a.NowPeriod,
				a.final_price　as PreiodPrice,
				convert(varchar(8),u.StartDate,112) as SDate,
				convert(varchar(8),u.EndDate,112) as EDate,
				a.IDNO,
				m.MEMCNAME,
				m.MEMEMAIL,
				case when m.MEMSENDCD IN (0,7) then 2 else m.MEMSENDCD end as InvoiceType,
				m.CARRIERID,
				m.UNIMNO,
				m.NPOBAN,
				s.IsMoto,
				p.PayTypeId,
				p.InvoTypeId,
				a.transaction_no as MerchantTradeNo,
				a.TaishinTradeNo,
				a.CardNumber,
				a.AuthIdResp as AuthCode --20210819 ADD BY AMBER REASON.授權碼改抓此欄位
				FROM TB_OrderAuthMonthly a WITH(NOLOCK) 
				JOIN TB_MonthlyPay p WITH(NOLOCK) ON a.MonthlyRentId=p.MonthlyRentId
				JOIN TB_MonthlyRentUse u WITH(NOLOCK) ON p.MonthlyRentId=u.MonthlyRentId
				JOIN TB_MonthlyRentSet s WITH(NOLOCK) ON u.ProjID=s.MonProjID AND u.MonProPeriod=s.MonProPeriod AND u.ShortDays=s.ShortDays
				JOIN TB_MemberData m  WITH(NOLOCK) ON a.IDNO=m.MEMIDNO
				WHERE p.ActualPay=1 AND a.TaishinTradeNo <>''
				AND a.MonthlyRentId=@MonthlyRentId AND a.IDNO=@IdNo AND a.InvJob=0 AND a.AuthFlg=1;   --20220209 ADD BY AMBER REASON.新增判斷InvJob條件

				IF @@ROWCOUNT=0
				BEGIN
					SET @Error = 1
					SET @ErrorCode='ERR911'	
				END			
			END				
	  END

	--寫入錯誤訊息
	IF @Error=1
	BEGIN
		INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
		VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END

END TRY
BEGIN CATCH
		SET @Error=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();
		SET @IsSystem=1;
	　　SET @ErrorType=4;

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
END CATCH

RETURN @Error
EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMonthlyPayInvData';

