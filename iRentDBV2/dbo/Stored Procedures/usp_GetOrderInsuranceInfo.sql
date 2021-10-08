/********************************************************************************************************
* Server   :  SQYHI03AZ
* Database :  IRENT_V2
* 系    統 :  IRENT後台
* 程式名稱 :  exec usp_GetOrderInsuranceInfo '12162' 
* 程式功能 :  訂單安心服務資格及價格查詢
* 程式作者 :  20210826 ADD BY PO YU
* 程式修改 :  20210830 UPDATE BY PO YU 新增防呆判斷
*			  20210902 UPDATE BY PO YU 新增判斷Insurance=2(TB_OrderMain Insurance=1)
*			  20210903 UPDATE BY PO YU @JointInsurancePerHours數值抓取由TB_InsuranceInfoOther改為TB_CarTypeGroup
*			  20210903 UPDATE BY PO YU 新增ISNULL防呆
*			  20210907 UPDATE BY PO YU 修正共同承租提示訊息改為獨立判斷
* 測試資料 :  8886961 ProjType=4
H12487275
SELECT ProjType FROM TB_OrderMain with(nolock) WHERE  order_number='12487275'

SELECT top 1 B.InsuranceLevel,A.CarTypeGroupCode FROM VW_GetOrderData A with(nolock)
LEFT JOIN TB_BookingInsuranceOfUser B with(nolock) on A.IDNO= B.IDNO WHERE A.order_number='12208162' AND B.IDNO IS NOT NULL
SELECT top 1 * TB_BookingInsuranceOfUser B with(nolock) 

SELECT top 1 * FROM VW_GetOrderData A with(nolock) WHERE A.order_number='12208165'


SELECT InsurancePerHours 
FROM TB_InsuranceInfo with(nolock)
WHERE InsuranceLevel='1' AND CarTypeGroupCode=NULL

SELECT * FROM TB_TogetherPassenger with(nolock) WHERE Order_number='12208165'
********************************************************************************************************/
CREATE PROCEDURE [dbo].[usp_GetOrderInsuranceInfo]
@OrderNo  varchar(20)
AS

DECLARE 
@Insurance int,
@MainInsurancePerHour int,
@JointInsurancePerHours int,
@JointAlertMessage varchar(100)


BEGIN
	DECLARE @ProjType int;
	IF EXISTS(SELECT TOP 1 1 FROM TB_OrderMain with(nolock) WHERE  order_number=@OrderNo)
	BEGIN
		SELECT @ProjType=ProjType FROM TB_OrderMain with(nolock) WHERE  order_number=@OrderNo
		IF  @ProjType=4
			BEGIN
				SET @Insurance=0;
				SET @MainInsurancePerHour=0;
				SET @JointInsurancePerHours=0;
			END
		ELSE
			BEGIN
				--判斷可否使用安心服務
				DECLARE 
				@MainInsuLV int,
				@CarTypeGroupCode VARCHAR(20)
				SELECT top 1 @MainInsuLV=B.InsuranceLevel,@CarTypeGroupCode=A.CarTypeGroupCode FROM VW_GetOrderData A with(nolock)
				LEFT JOIN TB_BookingInsuranceOfUser B with(nolock) on A.IDNO= B.IDNO WHERE A.order_number=@OrderNo

				IF @MainInsuLV>=4 OR @MainInsuLV IS NULL
					BEGIN
						SET @Insurance=0;
						SET @MainInsurancePerHour=0;
						SET @JointInsurancePerHours=0;
					END
				ELSE
					BEGIN
						IF EXISTS(SELECT TOP 1 1 FROM TB_TogetherPassenger A with(nolock) 
									Left join TB_BookingInsuranceOfUser B with(nolock) on A.MEMIDNO=B.IDNO
									WHERE A.Order_number=@OrderNo AND InsuranceLevel>=4)
							BEGIN
								SET @Insurance=0;
								SET @MainInsurancePerHour=0;
								SET @JointInsurancePerHours=0;
							END
						ELSE
							BEGIN
								SET @Insurance=1;
								DECLARE  @IsMainInsu int;
								SELECT @IsMainInsu=Insurance FROM TB_OrderMain WITH(NOLOCK) WHERE order_number=@OrderNo
								IF(@IsMainInsu=1)
									BEGIN
										SET @Insurance=2;
									END
								--主承租人每小時安心服務價格
								IF @MainInsuLV IS NULL
									BEGIN
										SET @MainInsuLV=3;
									END
								IF @MainInsuLV>=4
									BEGIN
										SET @MainInsurancePerHour=0;
									END
								ELSE
									BEGIN
										SELECT @MainInsurancePerHour=InsurancePerHours 
										FROM TB_InsuranceInfo with(nolock)
										WHERE InsuranceLevel=@MainInsuLV AND CarTypeGroupCode=@CarTypeGroupCode
									END

								--單一共同承租人每小時安心服務價格
								IF NOT EXISTS(SELECT top 1 1 FROM TB_TogetherPassenger with(nolock) WHERE Order_number=@OrderNo)
									BEGIN
										SET @JointInsurancePerHours=0;
									END
								ELSE
									BEGIN
										SELECT @JointInsurancePerHours=InsurancePerHours 
										FROM TB_CarTypeGroup WIth(NOlock)
										WHERE CarTypeGroupCode=@CarTypeGroupCode
									END
							END
					END
			
			END
		END
		ELSE
			BEGIN
				SET @Insurance=0;
				SET @MainInsurancePerHour=0;
				SET @JointInsurancePerHours=0;
			END
		--共同承租提示訊息
		DECLARE @IsInvited varchar(1);
		SELECT @IsInvited=ChkType FROM TB_TogetherPassenger with(nolock) WHERE Order_number=@OrderNo
		IF(@IsInvited='S')
			BEGIN
				SET @JointAlertMessage='還有人沒有回覆邀請喔!快通知對方開啟通知中心確認';
			END
		ELSE
			BEGIN
				SET @JointAlertMessage='';
			END
	SELECT 
	ISNULL(@Insurance,0) AS Insurance,
	ISNULL(@MainInsurancePerHour,0) AS MainInsurancePerHour,
	ISNULL(@JointInsurancePerHours,0) AS JointInsurancePerHours,
	ISNULL(@JointAlertMessage,'') AS JointAlertMessage
END