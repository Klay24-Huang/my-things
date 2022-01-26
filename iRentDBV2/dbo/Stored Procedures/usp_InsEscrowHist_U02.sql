/***********************************************************************************************
* Serve    : sqyhi03az.database.windows.net
* Database : IRENT_V2
* 程式名稱 : usp_InsEscrowHist_U02
* 系    統 : IRENT
* 程式功能 : 履約保證紀錄
* 作    者 : eason
* 撰寫日期 : 20210527
* 修改日期 : 20220118 UPD BY AMBER REASON: 新增Input參數:UseType、MonthlyNo
Example :
***********************************************************************************************/
CREATE PROCEDURE [dbo].[usp_InsEscrowHist_U02]
(   
	@IDNO			   VARCHAR(20)       , --身分證號
	@MemberID          VARCHAR(20)                ,
	@AccountID		   VARCHAR(20)			      ,    
    @Email				    VARCHAR(200)		  ,
    @PhoneNo				VARCHAR(20)			  , 
    @Amount					INT			          ,
	@TotalAmount            INT                   ,
    @CreateDate				DATETIME			  ,    
    @LastStoreTransId		VARCHAR(50)			  , --最近一次訂單編號
    @LastTransId			VARCHAR(50)			  , --最近一次台新訂單編號	
	@EcStatus				VARCHAR(20)           ,
	@LastTransDate			DATETIME		= null,
	@UseType			    INT			          , --履保類別 0:訂閱儲值，1:使用
	@MonthlyNo			    INT			          , --履保對應編號 0:MonthlyRentId 1:OrderNo
	@PRGID			        VARCHAR(50)		      , --程式ID
	@xError                 INT              OUTPUT, 
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
)
AS
BEGIN
    SET NOCOUNT ON

	declare @spIn nvarchar(max), @SpOut nvarchar(max)--splog
	begin 		
		select @spIn = isnull((
		select @IDNO[IDNO], @MemberID[MemberID], @AccountID[AccountID], 
		@EcStatus[EcStatus], @Email[Email], @PhoneNo[PhoneNo],
		@Amount[Amount], @TotalAmount[TotalAmount],	@CreateDate[CreateDate], 
		@LastTransDate[LastTransDate], @LastStoreTransId[LastStoreTransId], @LastTransId[LastTransId],@UseType[UseType],@MonthlyNo[MonthlyNo],@PRGID[PRGID]
		FOR JSON PATH, WITHOUT_ARRAY_WRAPPER),'{}')
	end

 	SET @xError = 0
    SET	@ErrorCode  = '0000'	
    SET	@ErrorMsg   = 'SUCCESS'	
    SET	@SQLExceptionCode = ''		
    SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_InsEscrowHist_U02'
	declare @LogID bigint = 99999
	DECLARE @NowTime DATETIME=dbo.GET_TWDATE()
	SET @PRGID=ISNULL(@PRGID,'')
	SET @Email=ISNULL(@Email,'')
	SET @PhoneNo=ISNULL(@PhoneNo,'')
	

	IF @IDNO = '' or @MemberID = '' or @AccountID = '' or @Amount = 0 
	BEGIN
		SET @xError=1
		SET @ErrorCode = 'ERR257' --參數遺漏
	END

	BEGIN TRY 
		BEGIN TRAN   
            if @xError = 0 
			begin   
			    insert into TB_EscrowHist
				(IDNO, MemberID, AccountID, EcStatus, 
				Email, PhoneNo, Amount, TotalAmount,
				CreateDate, LastTransDate, 
				LastStoreTransId, LastTransId,UseType,MonthlyNo,
				A_PRGID,A_USERID,U_PRGID,U_USERID,UPDTime)
				values(
				 @IDNO,@MemberID, @AccountID, @EcStatus,
				 @Email, @PhoneNo, @Amount, @TotalAmount,
				 @CreateDate, @LastTransDate,
				 @LastStoreTransId, @LastTransId,@UseType,@MonthlyNo,
				 @PRGID,@IDNO,@PRGID,@IDNO,@NowTime)

				 declare @EscrowUser_ID bigint = 0
				 select top 1 @EscrowUser_ID = u.EscrowUser_ID from TB_EscrowUser u
				 where u.IDNO = @IDNO 

				 if @EscrowUser_ID > 0
				 begin
					UPDATE u 
					SET u.TotalAmount=@TotalAmount,
					    u.LastStoreTransId=@LastStoreTransId,
					    u.LastTransDate=@LastTransDate,
						u.LastTransId=@LastTransId,
						u.Status= case when @EcStatus <> '' then @EcStatus else u.Status end,
						u.Email=@Email,
						u.PhoneNo=@PhoneNo,
						u.U_PRGID=@PRGID,
						u.U_USERID=@IDNO,
						u.UPDTime=@NowTime
                    from TB_EscrowUser u
					WHERE u.EscrowUser_ID = @EscrowUser_ID
				 end
				 else
				 begin
				   		insert into TB_EscrowUser
						(IDNO, MemberID, AccountID, Status, 
						Email, PhoneNo, TotalAmount,
						CreateDate, LastTransDate, 
						LastStoreTransId, LastTransId,
						A_PRGID,A_USERID,U_PRGID,U_USERID,UPDTime)
						values(
						@IDNO,@MemberID, @AccountID, @EcStatus,
						@Email, @PhoneNo,@TotalAmount,
						@CreateDate, @LastTransDate,
						@LastStoreTransId, @LastTransId,
						@PRGID,@IDNO,@PRGID,@IDNO,@NowTime)
				 end
			end
		COMMIT TRAN 
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		SET @xError=-1;
		SET @ErrorCode='ERR999';
		SET @ErrorMsg='我要寫錯誤訊息';
		SET @SQLExceptionCode=ERROR_NUMBER();
		SET @SQLExceptionMsg=ERROR_MESSAGE();

        INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])
        VALUES (@FunName,@ErrorCode,@ErrorType,@SQLExceptionCode,@SQLExceptionMsg,@LogID,@IsSystem);
	END CATCH

	if @xError <> 0 --sp錯誤log
		exec dbo.usp_SpSubsLog @FunName, @spIn, null,@xError, @ErrorCode, @ErrorMsg, @SQLExceptionCode, @SQLExceptionMsg

END



