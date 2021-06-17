using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public class ObjType
    {
        /// <summary>
        /// 列舉sp
        /// <para>InsError:寫入錯誤資訊進資料庫</para>
        /// <para>InsAPILog:寫入API呼叫LOG進資料庫</para>
        /// </summary>
        public enum SPType
        {
            /// <summary>
            /// 寫入錯誤資訊進資料庫
            /// </summary>
            InsError,
            /// <summary>
            /// 寫入API呼叫LOG進資料庫
            /// </summary>
            InsAPILog,
            /// <summary>
            /// 會員登入
            /// </summary>
            MemberLogin,
            #region 會員註冊、會員中心相關
            /// <summary>
            /// 判斷身份證/居留證是否已存在
            /// </summary>
            CheckAccount,
            /// <summary>
            /// 註冊第一步
            /// </summary>
            Register_Step1,
            /// <summary>
            /// 確認簡訊驗證碼
            /// </summary>
            CheckVerifyCode,
            /// <summary>
            /// 註冊重發簡訊驗證碼
            /// </summary>
            RegisterReSendSMS,
            /// <summary>
            /// 註冊時設定密碼
            /// </summary>
            Register_Step2,
            /// <summary>
            /// 忘記密碼
            /// </summary>
            ForgetPWD,
            /// <summary>
            /// 修改密碼
            /// </summary>
            ChangePWD,
            /// <summary>
            /// 註冊會員基本資料
            /// </summary>
            RegisterMemberData,
            /// <summary>
            /// 上傳證件照
            /// </summary>
            UploadCredentials,
            /// <summary>
            /// 重新發送EMail
            /// </summary>
            ReSendEmail,
            /// <summary>
            /// 驗證EMail
            /// </summary>
            VerifyEMail,
            /// <summary>
            /// 重設密碼後，設定密碼
            /// </summary>
            SetPWD,
            /// <summary>
            /// 設定預設支付方式
            /// </summary>
            SetDefPayMode,
            /// <summary>
            /// 修改會員資料
            /// </summary>
            SetMemberData,
            /// <summary>
            /// 檢查手機號碼
            /// </summary>
            CheckMobile,
            #endregion
            /// <summary>
            /// Token Check
            /// </summary>
            CheckToken,
            /// <summary>
            /// 更新Token
            /// </summary>
            RefrashToken,
            /// <summary>
            /// 判斷Token是否合法
            /// </summary>
            CheckTokenOnlyToken,
            /// <summary>
            /// 判斷Token後回傳該IDNO
            /// </summary>
            CheckTokenReturnID,
            /// <summary>
            /// 設定常用站點
            /// </summary>
            SetFavoriteStation,
            /// <summary>
            /// 取里程設定
            /// </summary>
            GetMilageSetting,
            /// <summary>
            /// 預約
            /// </summary>
            Booking,
            /// <summary>
            /// 取消預約
            /// </summary>
            BookingCancel,
            /// <summary>
            /// 讀卡
            /// </summary>
            ReadCard,
            /// <summary>
            /// 讀到卡號綁定
            /// </summary>
            BindUUCard,
            /// <summary>
            /// 取車
            /// </summary>
            BookingStart,
            /// <summary>
            /// 後台強取
            /// </summary>
            BE_BookingStart,
            /// <summary>
            /// 取車前判斷(用於執行usp_BookingStart與usp_BookingStartMotor前)
            /// </summary>
            BeforeBookingStart,
            /// <summary>
            /// 後台強取前判斷(用於執行usp_BE_BookingStart與usp_BE_BookingStartMotor前)
            /// </summary>
            BE_BeforeBookingStart,
            /// <summary>
            /// 取車前判斷車況
            /// </summary>
            CheckCarStatus,
            /// <summary>
            /// 車機專用，用來取得CID、Token…等
            /// </summary>
            GetCarMachineInfoCommon,
            /// <summary>
            /// 延長用車前先取得原始用車時間
            /// </summary>
            GetBookingStartTime,
            /// <summary>
            /// 延長用車
            /// </summary>
            BookingExtend,
            /// <summary>
            /// 刪除訂單
            /// </summary>
            BookingDelete,
            /// <summary>
            /// 還車前檢查車況
            /// </summary>
            CheckCarStatusByReturn,
            /// <summary>
            /// 後台強還，還車前檢查車況
            /// </summary>
            BE_CheckCarStatusByReturn,
            /// <summary>
            /// 後台強還，寫入bypass記錄
            /// </summary>
            BE_InsCarReturnError,
            /// <summary>
            /// 通過檢查，寫入還車時間
            /// </summary>
            ReturnCar,
            /// <summary>
            /// 取得未完成的訂單列表
            /// </summary>
            GetOrderList,
            /// <summary>
            /// 取得已取消的訂單列表
            /// </summary>
            GetCancelOrder,
            /// <summary>
            /// 取得已完成的訂單列表
            /// </summary>
            GetFinishOrder,
            /// <summary>
            /// 已完成的訂單明細
            /// </summary>
            OrderDetail,
            /// <summary>
            /// 設定發票（會員及訂單）
            /// </summary>
            SettingInvoice,
            /// <summary>
            /// 上傳出還車照
            /// </summary>
            UploadCarImage,
            /// <summary>
            /// 上傳取車回饋照
            /// </summary>
            UploadFeedBackImage,
            /// <summary>
            /// 
            /// </summary>
            InsFeedBack,
            /// <summary>
            /// 上傳及設定停車格位置
            /// </summary>
            SettingParkingSpce,
            /// <summary>
            /// 使用訂單編號取出該訂單基本資訊
            /// </summary>
            GetOrderStatusByOrderNo,
            /// <summary>
            /// 後台強還，使用訂單編號取出該訂單基本資訊
            /// </summary>
            BE_GetOrderStatusByOrderNo,
            /// <summary>
            /// 計算及寫入最終租金
            /// </summary>
            CalFinalPrice,
            /// <summary>
            /// 後台強還計算及寫入最終租金
            /// </summary>
            BE_CalFinalPrice,
            /// <summary>
            /// 完成付款
            /// </summary>
            DonePayRentBill,
            /// <summary>
            /// 後台強還設定狀態
            /// </summary>
            BE_ContactFinish,
            /// <summary>
            /// 完成補繳
            /// </summary>
            DonePayBack,
            /// <summary>
            /// 電子柵欄查詢
            /// </summary>
            PolygonListQuery,
            /// <summary>
            /// 判斷是否可以申請一次性開門
            /// </summary>
            CheckCanOpenDoor,
            /// <summary>
            /// 寫入一次性開門的驗證碼
            /// </summary>
            InsOpenDoorCode,
            /// <summary>
            /// 判斷一次性開門驗證碼
            /// </summary>
            CheckOpenDoorCode,
            /// <summary>
            /// 完成一次性開門前檢查車況
            /// </summary>
            GetCarStatusBeforeOpenDoorFinish,
            /// <summary>
            /// 完成一次性開門
            /// </summary>
            FinishOpenDoor,
            /// <summary>
            /// 取得會員資訊
            /// </summary>
            GetMemberInfo,
            /// <summary>
            /// 同站以據點取出車型
            /// </summary>
            GetStationCarType,
            /// <summary>
            /// 取得多據點專案
            /// </summary>
            GetStationCarTypeOfMutiStation,
            /// <summary>
            /// 取得會員登入後狀態 20201016 ADD BY ADAM 
            /// </summary>
            GetMemberStatus,
            /// <summary>
            /// 取得會員資料 20201022 ADD BY ADAM
            /// </summary>
            GetMemberData,
            /// <summary>
            /// 取得車輛狀態
            /// </summary>
            GetMotorStatus,
            /// <summary>
            /// 取得汽車狀態
            /// </summary>
            GetCarStatus,
            /// <summary>
            /// 欠費查詢
            /// </summary>
            GetArrearsQuery,
            /// <summary>
            /// 強還(For測試)
            /// </summary>
            EnforceReturnCar,
            /// <summary>
            /// 月租訂閱
            /// </summary>
            MonthlySubscription,
            /// <summary>
            /// 更新車輛狀態
            /// 20201101 ADD BY ADAM
            /// </summary>
            SetMotorStatus,
            /// <summary>
            /// 取得安心服務價格
            /// 20201103 ADD BY ADAM
            /// </summary>
            GetInsurancePrice,
            /// <summary>
            /// 
            /// </summary>
            BookingControl,
            /// <summary>
            /// 取得專案基本價格
            /// 20201110 ADD BY ADAM
            /// </summary>
            GetProjectPriceBase,
            /// <summary>
            /// 更新解綁資料
            /// 20201122 ADD BY Jerry
            /// </summary>
            UnBindCard,
            /// <summary>
            /// 取得解綁資料
            /// 20201122 ADD BY Jerry
            /// </summary>
            GetUnBindLog,
            /// <summary>
            /// 儲存換電獎勵結果
            /// 20201201 ADD BY ADAM
            /// </summary>
            SaveNPR380Result,
            /// <summary>
            /// 檢查iButton
            /// </summary>
            CheckCarIButton,
            /// <summary>
            /// 判斷Token跟DeviceID後回傳該IDNO
            /// </summary>
            CheckTokenDeviceReturnID,
            /// <summary>
            /// 取得路邊租還車輛
            /// </summary>
            GetAnyRentCar,
            /// <summary>
            /// 欠款查詢ByNPR330ID
            /// 20201213 ADD BY ADAM
            /// </summary>
            ArrearsQueryByNPR330ID,
            /// <summary>
            /// 變更悠遊卡
            /// </summary>
            ReadUUCard,
            /// <summary>
            /// 更新簽名檔
            /// </summary>
            SignatureUpdate,
            /// <summary>
            /// 取得簡訊驗證碼
            /// </summary>
            GetVerifyCode,
            /// <summary>
            /// 取得會員手機號碼
            /// </summary>
            GetMemberMobile,
            /// <summary>
            /// 取得重新計價資料
            /// </summary>
            GetRePayList,
            /// <summary>
            /// 重新計價
            /// </summary>
            CalFinalPrice_Re,
            /// <summary>
            /// 取得還車里程
            /// </summary>
            GetCarReturnMillage,
            /// <summary>
            /// 取得廣告資訊
            /// </summary>
            GetBanner,
            /// <summary>
            /// 重新授權
            /// </summary>
            GetOrderAuthRetryList,
            CheckAppVersion,
            /// <summary>
            /// 變更悠遊卡 20210415 ADD
            /// </summary>
            ChangeUUCard,
            /// <summary>
            /// 綁定悠遊卡 20210415 ADD
            /// </summary>
            BindUUCardJob,
            GetCarTypeGroupList,
            /// <summary>
            /// 寫入電量LOG 20210516 ADD BY ADAM
            /// </summary>
            InsMotorBattLog,
            /// <summary>
            /// 取得會員積分 20210519 ADD BY YEH
            /// </summary>
            GetMemberScore,
            /// <summary>
            /// 修改會員積分明細 20210519 ADD BY YEH
            /// </summary>
            SetMemberScoreDetail,
            /// <summary>
            /// 取得會員徽章 20210521 ADD BY YEH
            /// </summary>
            GetMemberMedal,
            /// <summary>
            /// 取得會員徽章 20210521 ADD BY YEH
            /// </summary>
            GetMapMedal,
            /// <summary>
            /// 取得會員積分攻略標題 20210526 ADD BY FRANK
            /// </summary>
            GetMemberScoreItem,
            /// <summary>
            /// 取得路邊汽車專案 20210616 ADD BY YEH
            /// </summary>
            GetAnyRentProject,
            /// <summary>
            /// 取得路邊機車專案 20210617 ADD BY YEH
            /// </summary>
            GetMotorRentProject,

            #region 渣渣
            /// <summary>
            /// 個人訊息
            /// </summary>
            PersonNotice,
            /// <summary>
            /// 活動訊息
            /// </summary>
            News,
            #endregion
            #region 拓連
            /// <summary>
            /// 更新交換站點資訊
            /// </summary>
            UPD_SW_DATA,
            GetMaintainKey,
            #endregion
            #region 台新錢包
            /// <summary>
            /// 取得錢包基本資訊（開戶）
            /// </summary>
            GetWalletInfo,
            GetWalletInfoByTrans,
            SaveRecieveTSAC,
            /// <summary>
            /// 開戶+儲值
            /// </summary>
            HandleWallet,
            #endregion
            #region 車麻吉
            /// <summary>
            /// 取得車麻吉token
            /// </summary>
            GetMochiToken,
            /// <summary>
            /// 新增或修改車麻吉token
            /// </summary>
            MaintainMachiToken,
            /// <summary>
            /// 同步車麻吉停車場
            /// </summary>
            MochiParkHandle,
            /// <summary>
            /// 停用車麻吉停車場
            /// </summary>
            DisabledMachiPark,
            /// <summary>
            /// 寫入停車資料
            /// </summary>
            InsMachiParkData,
            #endregion,
            #region 短租沖銷
            HandleNPR340Save,
            HandleNPR340SaveU1,
            HandleNPR340SaveU2,

            #endregion

            #region BackEnd
            /// <summary>
            /// 更改密碼
            /// </summary>
            BE_ChangePWD,
            /// <summary>
            /// 新增及修改據點
            /// </summary>
            BE_HandleStation,
            /// <summary>
            /// 新增及修改據點（包括同步進專案)
            /// </summary>
            BE_HandleStationNew,
            /// <summary>
            /// 特約停車場處理
            /// </summary>
            BE_HandleTransParking,
            /// <summary>
            /// 保有車輛處理
            /// </summary>
            BE_HandleCarSetting,
            /// <summary>
            /// 車輛設定上下線
            /// </summary>
            BE_CarDataSettingSetOnline,
            /// <summary>
            /// 車輛設定備註
            /// </summary>
            BE_HandleCarDataSettingMemo,
            /// <summary>
            /// 停車便利付修改
            /// </summary>
            BE_HandleChargeParkingData,
            /// <summary>
            /// 修改車機綁定
            /// </summary>
            BE_HandleCarMachineData,
            /// <summary>
            /// 修改萬用卡
            /// </summary>
            BE_HandleMasterCardData,
            /// <summary>
            /// 確認訂單並取出車機、卡號
            /// </summary>
            BE_GetCarMachineAndCheckOrder,
            /// <summary>
            /// 確認訂單並取出車機、卡號、身份證
            /// </summary>
            BE_GetCarMachineAndCheckOrderNoIDNO,
            /// <summary>
            /// 更新會員卡號
            /// </summary>
            BE_UpdCardNo,
            /// <summary>
            /// 強制延長用車
            /// </summary>
            BE_HandleExtendCar,
            /// <summary>
            /// 強制取消
            /// </summary>
            BE_BookingCancel,
            /// <summary>
            /// 強制取消(新版)
            /// </summary>
            BE_BookingCancelNew,
            /// <summary>
            /// 【整備人員】取消訂單
            /// </summary>
            BE_CancelCleanOrder,
            /// <summary>
            /// 換車
            /// </summary>
            BE_ChangeCar,
            /// <summary>
            /// 判斷加盟業者統編是否存在
            /// </summary>
            BE_CheckOperator,
            /// <summary>
            /// 更新加盟業者
            /// </summary>
            BE_UPDOperator,
            /// <summary>
            /// 更新功能群組
            /// </summary>
            BE_UPDFuncGroup,
            /// <summary>
            /// 判斷功能群組編號是否存在
            /// </summary>
            BE_CheckFuncGroup,
            /// <summary>
            /// 新增/修改功能權限
            /// </summary>
            BE_HandleFunc,
            /// <summary>
            /// 判斷使用者群組編號是否存在
            /// </summary>
            BE_CheckUserGroup,
            /// <summary>
            /// 更新使用者群組
            /// </summary>
            BE_UPDUserGroup,
            /// <summary>
            /// 新增/修改使用者
            /// </summary>
            BE_HandleUserMaintain,
            /// <summary>
            /// 處理審核照片
            /// </summary>
            BE_HandleAuditImage,
            /// <summary>
            /// 處理審核資料
            /// </summary>
            BE_HandleAudit,
            /// <summary>
            /// 設定電子柵欄
            /// </summary>
            BE_HandlePolygon,
            /// <summary>
            /// 取得合約修改前資料
            /// </summary>
            BE_GetOrderInfoBeforeModify,
            /// <summary>
            /// 取得合約修改前資料(2021年新版，已不使用)
            /// </summary>
            BE_GetOrderInfoBeforeModifyNew,
            /// <summary>
            /// 取得合約修改前資料(2021年新版修正, 20210129廢棄)
            /// </summary>
            BE_GetOrderInfoBeforeModifyV2,
            /// <summary>
            ///  取得合約修改前資料(2021年新版修正)
            /// </summary>
            BE_GetOrderInfoBeforeModifyV3,
            /// <summary>
            /// 短租補傳
            /// </summary>
            BE_HandleHiEasyRentRetry,
            /// <summary>
            /// 儲存060執行結果
            /// </summary>
            BE_BookingControlSuccess,
            /// <summary>
            /// 儲存125執行結果
            /// </summary>            
            BE_LandControlSuccess,
            /// <summary>
            /// 儲存130執行結果
            /// </summary>
            BE_ReturnControlSuccess,
            /// <summary>
            /// 儲存執行136結果
            /// </summary>
            BE_NPR136Success,
            /// <summary>
            /// 合約修改（點數）
            /// </summary>
            BE_HandleOrderModifyByDiscount,
            /// <summary>
            /// 合約修改（汽機車整合共用）
            /// </summary>
            BE_HandleOrderModify,
            /// <summary>
            /// 推播訊息處理
            /// </summary>
            BE_HandleNews,
            /// <summary>
            /// 使用者回饋處理
            /// </summary>
            BE_FeedBackHandle,
            /// <summary>
            /// 取得還車合約
            /// </summary>
            BE_GetReturnCarControl,

            GetBindingCard,
            /// <summary>
            /// 匯入車機車輛綁定資料
            /// </summary>
            BE_ImportCarBindData,
            /// <summary>
            /// 更新遠傳DeviceId與DeveiceToken
            /// </summary>
            BE_UpdCATDeviceToken,
            /// <summary>
            /// 解除綁定信用卡     // 20210511 ADD BY YEH REASON.後台解綁要將DB壓失效
            /// </summary>
            BE_UnBindCreditCard,
            /// <summary>
            /// 處理機車調度停車場 20210602 ADD BY FRANK
            /// </summary>
            BE_HandleTransParking_Moto,
            /// <summary>
            /// 獲取車機當前狀態資料 20210608 ADD BY FRANK
            /// </summary>
            BE_GetCarCurrentStatus,
            #endregion
            #region 整備人員
            MA_CheckCarStatusByReturn,
            #endregion

            GetCityParkingFee,    //20210429 ADD BY ADAM REASON.增加CityPark停車費綁定

            GetEstimate,
            GetOrderAuthList,
            UpdateOrderAuthList,
            GetOrderAuthReturnList,
            UpdateOrderAuthReturnList,
            BE_Banner,//20210316唐加
            BE_InsertChargeParkingData,//20210511唐加
        }
        /// <summary>
        /// 取出SPName
        /// </summary>
        /// <param name="type">對應ObjType.SPType</param>
        /// <returns></returns>
        public string GetSPName(ObjType.SPType type)
        {
            string SPName = "";
            switch (type)
            {
                case SPType.InsAPILog:  //寫入API呼叫LOG進資料庫
                    SPName = "usp_InsAPILog";
                    break;
                case SPType.InsError:   //寫入錯誤資訊進資料庫
                    SPName = "usp_InsErrorLog";
                    break;
                case SPType.MemberLogin://會員登入
                    SPName = "usp_MemberLogin";
                    break;
                #region 會員註冊、會員中心相關
                case SPType.CheckAccount://判斷身份證/居留證是否已存在
                    SPName = "usp_CheckAccount";
                    break;
                case SPType.Register_Step1://註冊第一步
                    SPName = "usp_Register_step1";
                    break;
                case SPType.CheckVerifyCode://確認簡訊驗證碼
                    SPName = "usp_CheckVerifyCode";
                    break;
                case SPType.RegisterReSendSMS://註冊重發簡訊驗證碼
                    SPName = "usp_Register_ReSendSMS";
                    break;
                case SPType.Register_Step2: //註冊時設定密碼
                    SPName = "usp_Register_Step2";
                    break;
                case SPType.ForgetPWD://忘記密碼
                    SPName = "usp_ForgetPWD";
                    break;
                case SPType.ChangePWD://修改密碼
                    SPName = "usp_ChangePWD";
                    break;
                case SPType.RegisterMemberData: //註冊會員基本資料
                    SPName = "usp_RegisterMemberData";
                    break;
                case SPType.UploadCredentials: //上傳證件照
                    //SPName = "usp_UploadCredentials";
                    SPName = "usp_UploadCredentialsNew";
                    //SPName = "usp_UploadCredentialsNew_20210220_Tang";  //20210220唐暫時改，用於將1.0照片拋去azure
                    break;
                case SPType.ReSendEmail: //重發EMail
                    SPName = "usp_ReSendEmail";
                    break;
                case SPType.VerifyEMail: //驗證EMAIL
                    SPName = "usp_VerifyEMail";
                    break;
                case SPType.SetPWD: //重置密碼後，設定密碼
                    SPName = "usp_SetPWD";
                    break;
                case SPType.SetDefPayMode:
                    SPName = "usp_SetDefPayMode";
                    break;
                case SPType.SetMemberData:  //修改會員資料
                    SPName = "usp_SetMemberData";
                    break;
                case SPType.CheckMobile:    //檢查手機號碼
                    SPName = "usp_CheckMobile";
                    break;
                #endregion
                case SPType.CheckToken: //Check Token
                    SPName = "usp_CheckToken";
                    break;
                case SPType.RefrashToken: //Refrash Token
                    SPName = "usp_RefrashToken";
                    break;
                case SPType.CheckTokenOnlyToken:
                    SPName = "usp_CheckTokenOnlyToken";
                    break;
                case SPType.CheckTokenReturnID:
                    SPName = "usp_CheckTokenReturnID";
                    break;
                case SPType.SetFavoriteStation:
                    SPName = "usp_InsFavoriteStation";
                    break;
                case SPType.GetMilageSetting:
                    SPName = "usp_GetMilageSetting";
                    break;
                case SPType.Booking: //預約
                    SPName = "usp_Booking";     // 20210611 UPD BY YEH REASON:SP已同步，指回原版本
                    break;
                case SPType.BookingCancel: //取消訂單
                    SPName = "usp_BookingCancel";
                    break;
                case SPType.ReadCard: //讀卡
                    SPName = "usp_ReadCard";
                    break;
                case SPType.BindUUCard:
                    SPName = "usp_BindUUCard";
                    break;
                case SPType.BookingStart:
                    SPName = "usp_BookingStart";
                    break;
                case SPType.BE_BookingStart:
                    SPName = "usp_BE_BookingStart";
                    break;
                case SPType.BeforeBookingStart:
                    SPName = "usp_BeforeBookingStart";
                    break;
                case SPType.BE_BeforeBookingStart:
                    SPName = "usp_BE_BeforeBookingStart";
                    break;
                case SPType.CheckCarStatus:
                    SPName = "usp_CheckCarStatus";
                    break;
                case SPType.GetCarMachineInfoCommon:
                    SPName = "usp_GetCarMachineInfoCommon";
                    break;
                case SPType.GetBookingStartTime:
                    SPName = "usp_GetBookingStartTime";
                    break;
                case SPType.BookingExtend:
                    SPName = "usp_BookingExtend";
                    break;
                case SPType.BookingDelete: //刪除訂單
                    SPName = "usp_BookingDelete";
                    break;
                case SPType.GetOrderList:
                    SPName = "usp_OrderListQuery_20210524"; //20210524 ADD BY ADAM REASON.增加儀表板電量
                    break;
                case SPType.GetCancelOrder:
                    SPName = "usp_GetCancelOrderList";
                    break;
                case SPType.GetFinishOrder:
                    SPName = "usp_GetFinishOrderList";
                    break;
                case SPType.OrderDetail:
                    SPName = "usp_GetOrderDetail_20210517";     //20210517 ADD BY ADAM REASON.新換電獎勵需求
                    break;
                case SPType.CheckCarStatusByReturn:
                    SPName = "usp_CheckCarStatusByReturn";
                    break;
                case SPType.BE_CheckCarStatusByReturn:
                    SPName = "usp_BE_CheckCarStatusByReturn";
                    break;
                case SPType.BE_InsCarReturnError:
                    SPName = "usp_BE_InsCarReturnError";
                    break;
                case SPType.ReturnCar:
                    SPName = "usp_ReturnCar";
                    break;
                case SPType.SettingInvoice:
                    SPName = "usp_SettingInvoice";
                    break;
                case SPType.UploadCarImage:
                    //SPName = "usp_InsTmpCarImage";
                    SPName = "usp_InsTmpCarImageBatch";
                    break;
                case SPType.UploadFeedBackImage:
                    //SPName = "usp_INSTmpFeedBackPIC";
                    SPName = "usp_INSTmpFeedBackPICNew_20210517";   //20210517 ADD BY ADAM 
                    break;
                case SPType.InsFeedBack:
                    SPName = "usp_InsFeedBack";
                    break;
                case SPType.SettingParkingSpce:
                    //SPName = "usp_InsParkingSpace";
                    SPName = "usp_InsParkingSpaceNew";
                    break;
                case SPType.GetOrderStatusByOrderNo:
                    SPName = "usp_GetOrderStatusByOrderNo";
                    break;
                case SPType.BE_GetOrderStatusByOrderNo:
                    SPName = "usp_BE_GetOrderStatusByOrderNo";
                    break;
                case SPType.CalFinalPrice:
                    SPName = "usp_CalFinalPrice";   // 20210611 UPD BY YEH REASON:SP已同步，指回原版本
                    break;
                case SPType.BE_CalFinalPrice:
                    SPName = "usp_BE_CalFinalPrice";
                    break;
                case SPType.DonePayRentBill:
                    SPName = "usp_DonePayRentBillNew_20210517";     //20210523 ADD BY ADAM REASON.
                    break;
                case SPType.BE_ContactFinish:
                    SPName = "usp_BE_ContactFinish_ForTest";    // 20210609 UPD BY YEH 強還積分測試，先指向測試SP
                    break;
                case SPType.DonePayBack:
                    SPName = "usp_DonePayBack_V2";
                    break;
                case SPType.PolygonListQuery:
                    SPName = "usp_PolygonListQuery";
                    break;
                case SPType.CheckCanOpenDoor:
                    SPName = "usp_CheckCanOpenDoor";
                    break;
                case SPType.InsOpenDoorCode:
                    SPName = "usp_InsOpenDoorCode";
                    break;
                case SPType.CheckOpenDoorCode:
                    SPName = "usp_CheckOpenDoorCode";
                    break;
                case SPType.GetCarStatusBeforeOpenDoorFinish:
                    SPName = "usp_GetCarStatusBeforeOpenDoorFinish";
                    break;
                case SPType.FinishOpenDoor:
                    SPName = "usp_FinishOpenDoor";
                    break;
                case SPType.GetMemberInfo:
                    SPName = "usp_GetMemberInfo";
                    break;
                case SPType.GetStationCarType:
                    SPName = "usp_GetStationCarType"; //20201023 Eason 轉sp  // 20210611 UPD BY YEH REASON:SP已同步，指回原版本
                    break;
                case SPType.GetStationCarTypeOfMutiStation:
                    SPName = "usp_GetStationCarTypeOfMutiStation_ForTest";  // 20210611 UPD BY YEH REASON:SP已同步，指回原版本   // 20210615 UPD BY YEH REASON:測試積分<60只剩定價專案
                    break;
                case SPType.GetMemberStatus:    //20201016 ADD BY ADAM REASON.增加會員狀態(登入後狀態)
                    //SPName = "usp_GetMemberStatus";
                    SPName = "usp_GetMemberStatus_ForTest";     // 20210521 ADD BY YEH FOR TEST
                    break;
                case SPType.GetMemberData:      //20201022 ADD BY ADAM REASON.改寫為sp
                    SPName = "usp_GetMemberData";
                    break;
                case SPType.GetMotorStatus:     //取得車輛狀態
                    SPName = "usp_GetMotorStatus";
                    break;
                case SPType.GetCarStatus:      //取得汽車狀態
                    SPName = "usp_GetCarStatus";
                    break;
                case SPType.GetArrearsQuery:
                    SPName = "usp_ArrearsQuery_U1";//欠費查詢
                    break;
                case SPType.EnforceReturnCar:   //強還(For測試)
                    SPName = "usp_EnforceReturnCar";
                    break;
                case SPType.MonthlySubscription:
                    SPName = "usp_MonthRent_I01";
                    break;
                case SPType.SetMotorStatus:   //20201101 ADD BY ADAM REASON.增加直接寫入車機狀態
                    SPName = "usp_SetMotorStatus";
                    break;
                case SPType.GetInsurancePrice:  //20201103 ADD BY ADAM REASON.
                    SPName = "usp_GetInsurancePrice";
                    break;
                case SPType.GetProjectPriceBase:    //20201110 ADD BY ADAM 
                    SPName = "usp_GetProjectPriceBase";
                    break;
                case SPType.UnBindCard:    //20201122 ADD BY Jerry 
                    SPName = "usp_UnBindCard";
                    break;
                case SPType.GetUnBindLog:    //20201122 ADD BY Jerry 
                    SPName = "usp_GetUnBindLog";
                    break;
                case SPType.BookingControl:
                    SPName = "usp_BookingControl";
                    break;
                case SPType.SaveNPR380Result:    //20201201 ADD BY ADAM
                    SPName = "usp_SaveNPR380Result";
                    break;
                case SPType.CheckTokenDeviceReturnID:
                    SPName = "usp_CheckTokenDeviceReturnID";
                    break;
                case SPType.CheckCarIButton:    //檢查iButton
                    SPName = "usp_CheckCarIButton";
                    break;
                case SPType.GetAnyRentCar:      //取得路邊租還車輛
                    SPName = "usp_GetAnyRentCar";
                    break;
                case SPType.ArrearsQueryByNPR330ID: //20201213 ADD BY ADAM
                    SPName = "usp_ArrearsQuery_Q1";
                    break;
                case SPType.ReadUUCard:     //變更悠遊卡
                    SPName = "usp_ReadUUCard";
                    break;
                case SPType.SignatureUpdate:     //更新簽名檔
                    SPName = "usp_SignatureUpdate";
                    break;
                case SPType.GetVerifyCode:  //取得簡訊驗證碼
                    SPName = "usp_GetVerifyCode";
                    break;
                case SPType.GetMemberMobile:    //取得會員手機號碼
                    SPName = "usp_GetMemberMobile";
                    break;
                case SPType.GetRePayList:
                    SPName = "usp_GetRePayList_Q01";
                    break;
                case SPType.CalFinalPrice_Re:   //重新計算租金 20201224 ADD BY ADAM 
                    SPName = "usp_CalFinalPrice_Re";
                    break;
                case SPType.GetOrderAuthList:   //取得批次授權明細 20210108 ADD BY JERRY 
                    SPName = "usp_GetOrderAuthList";
                    break;
                case SPType.UpdateOrderAuthList:   //更新批次授權明細 20210108 ADD BY JERRY 
                    SPName = "usp_UpdateOrderAuthList";
                    break;
                case SPType.GetOrderAuthReturnList:   //取得批次授權明細 20210108 ADD BY JERRY 
                    SPName = "usp_GetOrderAuthReturnList";
                    break;
                case SPType.UpdateOrderAuthReturnList:   //更新批次授權明細 20210108 ADD BY JERRY 
                    SPName = "usp_UpdateOrderAuthReturnList";
                    break;
                case SPType.GetEstimate:
                    SPName = "usp_GetEstimate_Q1";
                    break;
                case SPType.GetCarReturnMillage:    // 取得還車里程 20210219 ADD
                    SPName = "usp_GetCarReturnMillage";
                    break;
                case SPType.GetBanner:  // 取得廣告資訊 20210316 ADD
                    SPName = "usp_GetBanner";
                    break;
                case SPType.GetCarTypeGroupList:
                    SPName = "usp_GetCarTypeGroupList_Q1";
                    break;
                case SPType.CheckAppVersion:    //檢查APP版本
                    SPName = "usp_CheckAppVersion";
                    break;
                case SPType.ChangeUUCard:    //變更悠遊卡 20210415 ADD
                    SPName = "usp_ChangeUUCard";
                    break;
                case SPType.BindUUCardJob:    //綁定悠遊卡 20210415 ADD
                    SPName = "usp_BindUUCardJob";
                    break;
                case SPType.GetOrderAuthRetryList:
                    SPName = "usp_GetOrderAuthRetryList";
                    break;
                case SPType.InsMotorBattLog:    //寫入機車電量 20210516 ADD BY ADAM
                    SPName = "usp_InsMotorBattLog";
                    break;
                case SPType.GetMemberScore:     //取得會員積分 20210519 ADD BY YEH
                    SPName = "usp_GetMemberScore_Q1";
                    break;
                case SPType.SetMemberScoreDetail:     //修改會員積分明細 20210519 ADD BY YEH
                    SPName = "usp_SetMemberScoreDetail";
                    break;
                case SPType.GetMemberMedal:     //取得會員徽章 20210521 ADD BY YEH
                    SPName = "usp_GetMemberMedal";
                    break;
                case SPType.GetMapMedal:     //取得地圖徽章 20210521 ADD BY YEH
                    SPName = "usp_GetMapMedal";
                    break;
                case SPType.GetMemberScoreItem:  //取得會員積分攻略標題 20210526 ADD BY FRANK
                    SPName = "usp_GetMemberScoreItem";
                    break;
                case SPType.GetAnyRentProject:  // 取得路邊汽車專案 20210616 ADD BY YEH
                    SPName = "usp_GetAnyRentProject";
                    break;
                case SPType.GetMotorRentProject:  // 取得路邊機車專案 20210617 ADD BY YEH
                    SPName = "usp_GetMotorRentProject";
                    break;

                #region 渣渣
                case SPType.PersonNotice:
                    SPName = "usp_GetNotificationList";
                    break;
                case SPType.News:
                    SPName = "usp_GetNews";
                    break;
                case SPType.BE_Banner:  //20210316唐加
                    SPName = "usp_BE_Banner_I";
                    break;
                #endregion
                #region 拓連
                /// <summary>
                /// 更新交換站點資訊
                /// </summary>
                case SPType.UPD_SW_DATA:
                    SPName = "usp_UPD_SW_DATA";
                    break;
                case SPType.GetMaintainKey:
                    SPName = "usp_GetKey";
                    break;
                #endregion
                #region 台新錢包
                case SPType.GetWalletInfo:
                    SPName = "usp_GetWalletInfo";
                    break;
                case SPType.GetWalletInfoByTrans:
                    SPName = "usp_GetWalletInfoByTrans";
                    break;
                case SPType.HandleWallet:
                    SPName = "usp_HandleWallet";
                    break;
                case SPType.SaveRecieveTSAC:
                    SPName = "usp_SaveRecieveTSAC";
                    break;
                #endregion
                #region 車麻吉
                case SPType.GetMochiToken:
                    SPName = "usp_GetMachiToken";
                    break;
                case SPType.MaintainMachiToken:
                    SPName = "usp_MaintainMachiToken";
                    break;
                case SPType.MochiParkHandle:
                    SPName = "usp_MochiParkHandle";
                    break;
                case SPType.DisabledMachiPark:
                    SPName = "usp_disabledMachiPark";
                    break;
                case SPType.InsMachiParkData:
                    SPName = "usp_InsParkingFeeData";
                    break;
                #endregion
                #region 短租沖銷
                case SPType.HandleNPR340Save:
                    SPName = "usp_HandleNPR340Save";
                    break;
                case SPType.HandleNPR340SaveU1:
                    SPName = "usp_HandleNPR340SaveU1";
                    break;
                case SPType.HandleNPR340SaveU2:
                    SPName = "usp_HandleNPR340SaveU2";
                    break;
                #endregion
                #region BackEnd
                case SPType.BE_ChangePWD:
                    SPName = "usp_BE_ChangePWD";
                    break;
                case SPType.BE_HandleStation:
                    SPName = "usp_BE_HandleStationNew";
                    break;
                case SPType.BE_HandleStationNew:
                    SPName = "usp_BE_HandleStationNew";
                    break;
                case SPType.BE_HandleTransParking:
                    SPName = "usp_BE_HandleTransParking";
                    break;
                case SPType.BE_HandleCarSetting:
                    SPName = "usp_BE_Handle_CarSetting";
                    break;
                case SPType.BE_CarDataSettingSetOnline:
                    SPName = "usp_BE_CarDataSettingSetOnline";
                    break;
                case SPType.BE_HandleCarDataSettingMemo:
                    SPName = "usp_BE_HandleCarDataSettingMemo";
                    break;
                case SPType.BE_HandleChargeParkingData:
                    //SPName = "usp_BE_HandleTransParking";
                    SPName = "usp_BE_HandleChargeParkingData";
                    break;
                case SPType.BE_HandleCarMachineData:
                    SPName = "usp_BE_HandleCarBindData";
                    break;
                case SPType.BE_HandleMasterCardData:
                    SPName = "usp_BE_HandleMasterCard";
                    break;
                case SPType.BE_GetCarMachineAndCheckOrder:
                    SPName = "usp_BE_GetCarMachineAndCheckOrder";
                    break;
                case SPType.BE_GetCarMachineAndCheckOrderNoIDNO:
                    SPName = "usp_BE_GetCarMachineAndCheckOrderNoIDNO";
                    break;
                case SPType.BE_UpdCardNo:
                    SPName = "usp_BE_UPD_CardNo";
                    break;
                case SPType.BE_HandleExtendCar:
                    SPName = "usp_BE_HandleExtendCar";
                    break;
                case SPType.BE_BookingCancel:
                    SPName = "usp_BE_BookingCancel";
                    break;
                case SPType.BE_BookingCancelNew:
                    SPName = "usp_BE_BookingCancelNew";
                    break;
                case SPType.BE_ChangeCar:
                    SPName = "usp_BE_ChangeCar";
                    break;
                case SPType.BE_CheckOperator:
                    SPName = "usp_BE_CheckOperator";
                    break;
                case SPType.BE_UPDOperator:
                    SPName = "usp_BE_UPDOperator";
                    break;
                case SPType.BE_UPDFuncGroup:
                    SPName = "usp_BE_UPDFuncGroup";
                    break;
                case SPType.BE_HandleFunc:
                    SPName = "usp_BE_HandleFunc";
                    break;
                case SPType.BE_UPDUserGroup:
                    SPName = "usp_BE_UPDUserGroup";
                    break;
                case SPType.BE_HandleUserMaintain:
                    SPName = "usp_BE_HandleUserMaintain";
                    break;
                case SPType.BE_HandleAuditImage:
                    SPName = "usp_BE_HandleAuditImage";
                    break;
                case SPType.BE_HandleAudit:
                    SPName = "usp_BE_HandleAudit";
                    break;
                case SPType.BE_HandlePolygon:
                    SPName = "usp_BE_HandlePolygon";
                    break;
                case SPType.BE_GetOrderInfoBeforeModify:
                    SPName = "usp_BE_GetOrderInfoBeforeModify";
                    break;
                case SPType.BE_GetOrderInfoBeforeModifyNew:
                    SPName = "usp_BE_GetOrderInfoBeforeModifyNew";
                    break;
                case SPType.BE_GetOrderInfoBeforeModifyV2:
                    SPName = "usp_BE_GetOrderInfoBeforeModifyV2";
                    break;
                case SPType.BE_GetOrderInfoBeforeModifyV3:
                    SPName = "usp_BE_GetOrderInfoBeforeModifyV3";
                    break;
                case SPType.BE_HandleOrderModify:
                    SPName = "usp_BE_HandleOrderModify";
                    break;
                case SPType.BE_HandleHiEasyRentRetry:
                    SPName = "usp_BE_HandleHiEasyRentRetry";
                    break;
                case SPType.BE_BookingControlSuccess:
                    SPName = "usp_BE_BookingControlSuccess";
                    break;
                case SPType.BE_LandControlSuccess:
                    SPName = "usp_BE_LandControlSuccess";
                    break;
                case SPType.BE_ReturnControlSuccess:
                    SPName = "usp_BE_ReturnControlSuccess";
                    break;
                case SPType.BE_CheckFuncGroup:
                    SPName = "usp_BE_CheckFuncGroup";
                    break;
                case SPType.BE_CheckUserGroup:
                    SPName = "usp_BE_CheckUserGroup";
                    break;
                case SPType.BE_HandleOrderModifyByDiscount:
                    SPName = "usp_BE_HandleOrderModifyByDiscount";
                    break;
                case SPType.BE_NPR136Success:
                    SPName = "usp_BE_NPR136Success";
                    break;
                case SPType.BE_HandleNews:
                    SPName = "usp_BE_HandleNews";
                    break;
                case SPType.BE_FeedBackHandle:
                    SPName = "usp_BE_FeedBackHandle";
                    break;
                case SPType.BE_GetReturnCarControl:     //取得還車合約資訊
                    SPName = "usp_BE_GetReturnCarControl";
                    break;
                case SPType.GetBindingCard:
                    SPName = "usp_GetBindingCard";
                    break;
                case SPType.GetCityParkingFee:      //20210429 ADD BY ADAM REASON.增加CityPark停車費綁定
                    SPName = "usp_GetCityParkingFee";
                    break;
                case SPType.BE_ImportCarBindData:
                    SPName = "usp_BE_ImportCarBindData";
                    break;
                case SPType.BE_UpdCATDeviceToken:
                    SPName = "usp_BE_UpdCATDeviceToken";
                    break;
                case SPType.BE_InsertChargeParkingData:
                    //SPName = "usp_BE_HandleTransParking";
                    SPName = "usp_BE_InsertChargeParkingData";
                    break;
                case SPType.BE_UnBindCreditCard:      // 20210511 ADD BY YEH REASON.後台解綁要將DB壓失效
                    SPName = "usp_BE_UnBindCreditCard";
                    break;
                case SPType.BE_HandleTransParking_Moto:  // 處理機車調度停車場 20210602 ADD BY FRANK
                    SPName = "usp_BE_InsTransParking_Moto";
                    break;
                case SPType.BE_GetCarCurrentStatus:        // 獲取當前車機狀態資料 20210608 ADD BY FRANK
                    SPName = "usp_BE_GetCarCurrentStatus";
                    break;
                #endregion
                #region 整備人員
                case SPType.MA_CheckCarStatusByReturn:
                    SPName = "usp_MA_CheckCarStatusByReturn";
                    break;
                case SPType.BE_CancelCleanOrder:
                    SPName = "usp_BE_CancelCleanOrder";
                    break;
                    #endregion
            }
            return SPName;
        }
    }
}