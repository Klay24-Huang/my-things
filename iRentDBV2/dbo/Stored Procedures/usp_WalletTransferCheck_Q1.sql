/****** Object:  StoredProcedure [dbo].[usp_WalletTransferCheck_Q1]    Script Date: 2021/9/28 上午 10:30:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_WalletTransferCheck_Q1
* 系    統 : IRENT
* 程式功能 : 電子錢包轉贈前確認
* 作    者 : eason
* 撰寫日期 : 20210827
* 修改日期 : 20210827 - 新增輸出欄位IDNO - eason
			2021/09/23 UPD BY YANKEY 修正無下WHERE條件BUG
			2021/09/24 UPD BY YANKEY 修正當月金流判斷日期區間從當月最後一天0時到次月首日0時
Example :

EXEC usp_WalletTransferCheck_Q1 '','','1','0939583114',''
EXEC usp_WalletTransferCheck_Q1 '','F128697972','1','',''

***********************************************************************************************/

CREATE PROCEDURE [dbo].[usp_WalletTransferCheck_Q1]
(   
	@MSG			VARCHAR(10) OUTPUT,
	@IDNO			VARCHAR(20), --身分證號
    @LogID			BIGINT,
	@PhoneNo        VARCHAR(20), --手機號碼
	@SetNow         DATETIME = null 
)
AS
BEGIN
    SET NOCOUNT ON

	drop table if exists #Tmp_Member
	drop table if exists #Tmp_UserWallet
	drop table if exists #Tmp_TradeHist
	
	DECLARE @SeachKey VARCHAR(20)
	DECLARE @Error INT = 0
    DECLARE	@ErrorCode VARCHAR(6) = '0000'	
    DECLARE	@ErrorMsg  		   NVARCHAR(100) = 'SUCCESS'	
    DECLARE	@SQLExceptionCode  VARCHAR(10) = ''		
    DECLARE	@SQLExceptionMsg   NVARCHAR(1000) = ''	
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_WalletTransferCheck_Q1'
	DECLARE @NOW DATETIME = iif(@SetNow is null, DATEADD(HOUR, 8, GETDATE()),@SetNow)

	BEGIN TRY
	    set @LogID = isnull(@LogID,'')
	    set @IDNO = isnull(@IDNO,'')
		set @PhoneNo = isnull(@PhoneNo,'')
		

		IF @LogID = ''
		BEGIN
			SET @Error=1
			set @ErrorMsg = 'LogID必填'
			SET @ErrorCode = 'ERR254'
		END

		SET @SeachKey = IIF(@IDNO = '',@PhoneNo,@IDNO)
		if @Error = 0
		begin
			IF @IDNO = '' and @PhoneNo = ''
			BEGIN
				SET @Error=1
				set @ErrorMsg = 'IDNO, PhoneNo 至少填一項'
				SET @ErrorCode = 'ERR256'
			END
		end

		if @Error = 0 --Phone轉IDNO
		begin
		   if @IDNO = '' and @PhoneNo <> ''
		   begin
		      --存在同電話已審+電話驗證通過,目前取後申請者
		      select top 1 @IDNO = m.MEMIDNO 
			  from TB_MemberData m WITH(NOLOCK)
			  where m.Audit = 1 and m.HasCheckMobile = 1 
			  AND MEMTEL = @PhoneNo --20210923 UPD BY YANKEY 修正無下WHERE條件BUG
			  order by m.APPLYDT desc --取後申請的
			  if @IDNO = ''
			  begin
			     set @Error=1
				 set @ErrorMsg = 'PhoneNo無對應IDNO'
				 set @ErrorCode = 'ERR256'
			  end
		   end
		end

		IF @Error = 0
		BEGIN

		    declare @MemCount int = 0, @WalletCount int = 0, @HistCount int =0

			if @Error = 0 --是否為審核通過會員
				begin
           		select * 
				into #Tmp_Member
				from TB_MemberData m with(nolock) 
				where m.MEMIDNO=@IDNO and m.Audit = 1
				select @MemCount = count(*) from #Tmp_Member
				if @MemCount = 0
				begin
				   set @Error = 1
				   set @ErrorMsg = '非審核通過會員'
				   set @ErrorCode = 'ERR278'
				end
			end

			declare @WalletAmount int =0 --錢包剩餘金額
			if @Error = 0 --錢包會員
			begin
			--2021/09/24 UPD BY YANKEY 調整撈取欄位，讀取總金額
				SELECT top 1 WalletBalance 
				into #Tmp_UserWallet 
				FROM TB_UserWallet WITH(NOLOCK) 
				WHERE IDNO=@IDNO and Status = 2 --狀態:1=未啟用, 2=啟用, 3=凍結, 4=註記刪除
				select @WalletCount = count(1) from #Tmp_UserWallet
				select @WalletAmount = t.WalletBalance from #Tmp_UserWallet t
				/*-- 2021/09/24 UPD BY YANKEY 對方未開通錢包亦可轉贈 
				if @WalletCount = 0
				begin
				   set @Error = 1
				   set @ErrorMsg = '對方未開通錢包'
				   set @ErrorCode = 'ERR279'
				end
				*/
			end			
			declare @GoldFlowIn decimal = 0, @GoldFlowOut decimal = 0
			if @Error = 0 --當月金流
			begin
				select distinct
				tm.*,c.Code0, c.Negative 
				into #Tmp_TradeHist
				from TB_WalletTradeMain tm with(nolock) 
		        join TB_WalletCodeTable c with(nolock) on c.CodeGroup = 'TradeType' and c.Code0 = tm.TradeType
				where tm.IDNO = @IDNO
				and tm.TradeDate >= dateadd(m, datediff(m,0,@NOW),0) --當月1號
				--and tm.TradeDate <= dateadd(day ,-1, dateadd(m, datediff(m,0,@NOW)+1,0))  --當月月底
				--2021/09/24 UPD BY YANKEY 修正當月金流判斷日期區間從當月最後一天0時到次月首日0時
				and tm.TradeDate < dateadd(m, datediff(m,0,@NOW)+1,0)  --次月1號
				select @HistCount = count(*) from #Tmp_TradeHist
				if @HistCount > 0
				begin
				   select @GoldFlowIn = SUM(h.TradeAMT) 
				   from #Tmp_TradeHist h
				   where h.Negative = 0 --正向

				   select @GoldFlowOut = SUM(h.TradeAMT)
				   from #Tmp_TradeHist h  
				   where h.Negative = 1 --負向

				   if @GoldFlowIn > 300000 
				   begin
				      set @Error = 1
					  set @ErrorMsg = '金流超過上限'
					  set @ErrorCode = 'ERR280'
				   end
				end
			end

			select
			CkResult = case when @Error = 0 then 1 else 0 end --驗證結果:1成功,0失敗 
			,IDNO = (select top 1 m0.MEMIDNO from #Tmp_Member m0)
			,ShowName = (select top 1 dbo.FN_BlockName(m.MEMCNAME,'O') from #Tmp_Member m)
			,ShowValue =  (select top 1 dbo.FN_BlockStr(@SeachKey,'X',5,3) from #Tmp_Member m2)
			,@WalletAmount[WalletAmount] --錢包剩餘金額(可轉金額上限)
			,@GoldFlowIn[MonTransIn] --當月入賬總金額
			,@GoldFlowOut[MonTransOut] --當月轉出總金額(目前用不到)
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

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	--輸出系統訊息
	SELECT @ErrorCode[ErrorCode], @ErrorMsg[ErrorMsg], @SQLExceptionCode[SQLExceptionCode], @SQLExceptionMsg[SQLExceptionMsg], @Error[Error]

	drop table if exists #Tmp_Member
	drop table if exists #Tmp_UserWallet
	drop table if exists #Tmp_TradeHist

END

GO


