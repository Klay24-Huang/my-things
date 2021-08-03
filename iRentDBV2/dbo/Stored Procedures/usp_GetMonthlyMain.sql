﻿/********************************************************************
功能：報表/月租總表查詢
新增人員：胡湘梅(Umeko)
新增期間：2021/07/12
修改歷程：

***********************************************************************/
CREATE PROCEDURE [dbo].[usp_GetMonthlyMain]
	@IDNO varchar(20) = null,
	@SD datetime = null,
	@ED datetime = null,
	@hasPointer int = 2,
	@LogID                  BIGINT                ,
	@ErrorCode 				VARCHAR(6)		OUTPUT,	--回傳錯誤代碼
	@ErrorMsg  				NVARCHAR(100)	OUTPUT,	--回傳錯誤訊息
	@SQLExceptionCode		VARCHAR(10)		OUTPUT,	--回傳sqlException代碼
	@SQLExceptionMsg		NVARCHAR(1000)	OUTPUT	--回傳sqlException訊息
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--SET NOCOUNT ON;

	SET	@ErrorCode  = '0000'	
	SET	@ErrorMsg   = 'SUCCESS'	
	SET	@SQLExceptionCode = ''		
	SET	@SQLExceptionMsg = ''		
	DECLARE @IsSystem TINYINT = 1
	DECLARE @ErrorType TINYINT = 4
	DECLARE @FunName VARCHAR(50) = 'usp_GetMonthlyMain'

	DECLARE @NowTime datetime = dbo.Get_TWDATE()

	DECLARE @Error INT;
	/*初始設定*/
	SET @Error=0;

	BEGIN TRY
   
	IF @hasPointer > 2
	Begin
		Set @Error= 1
		Set @ErrorCode = 'ERR101'
		Set @ErrorMsg = '@hasPointer outer of range'
	End

	IF @Error = 0
	Begin
		IF  @SD is null And @ED is null
		Begin
			Set @ED = Convert(varchar(10),dbo.GET_TWDATE(),111)
			Set @SD = DATEADD(month,-1,@ED)
		End

		IF @SD is not null And @ED is not null
		Begin
			If @hasPointer = 0
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @ED 
					And (Main.WorkDayHours=0 AND Main.HolidayHours=0 AND Main.MotoTotalHours=0) 
				ORDER BY Main.IDNO ASC
			End
			Else if  @hasPointer = 1
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @ED 
					And (Main.WorkDayHours>0 AND Main.HolidayHours>0 AND Main.MotoTotalHours>0) 
				ORDER BY Main.IDNO ASC
			End
			Else
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @ED 
				ORDER BY Main.IDNO ASC
			End
		End
		Else if @SD is not null And @ED is null 
		Begin
			If @hasPointer = 0
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @SD 
					And (Main.WorkDayHours=0 AND Main.HolidayHours=0 AND Main.MotoTotalHours=0) 
				ORDER BY Main.IDNO ASC
			End
			Else if  @hasPointer = 1
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @SD 
					And (Main.WorkDayHours>0 AND Main.HolidayHours>0 AND Main.MotoTotalHours>0) 
				ORDER BY Main.IDNO ASC
			End
			Else
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @SD 
				ORDER BY Main.IDNO ASC
			End
		End
		Else IF @SD is null And @ED is not null 
		Begin
			If @hasPointer = 0
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @SD 
					And (Main.WorkDayHours=0 AND Main.HolidayHours=0 AND Main.MotoTotalHours=0) 
				ORDER BY Main.IDNO ASC
			End
			Else if  @hasPointer = 1
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @SD 
					And (Main.WorkDayHours>0 AND Main.HolidayHours>0 AND Main.MotoTotalHours>0) 
				ORDER BY Main.IDNO ASC
			End
			Else
			Begin
				Select Main.IDNO,Main.WorkDayHours,Main.HolidayHours,Main.MotoTotalHours,Main.StartDate,Main.EndDate,ISNULL(Main.SEQNO,0) AS SEQNO,ISNULL(Main.[ProjID],'''') AS ProjID,ISNULL(Main.[ProjNM],'''') AS ProjNM 
					,Main.MonProPeriod,'Y' IsTiedUp
					,Case When NxtMonSetID > 0 Then 'Y' Else 'N' End AutomaticRenewal
				From SYN_MonthlyRent AS Main 
				Left Join dbo.TB_SubsNxt AS SubsNxt On Main.MonthlyRentId = SubsNxt.NowMonthlyRentId
				Where Main.IDNO = Case When @IDNO is null Then Main.IDNO Else @IDNO End
					And Main.EndDate >= @SD And Main.StartDate <= @SD 
				ORDER BY Main.IDNO ASC
			End
		End
	End 

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

EXECUTE sp_addextendedproperty @name = N'Platform', @value = N'API', @level0type = N'SCHEMA', @level0name = N'dbo', @level1type = N'PROCEDURE', @level1name = N'usp_GetMonthlyMain';