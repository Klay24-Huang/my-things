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
            /// 取車前判斷(用於執行usp_BookingStart與usp_BookingStartMotor前)
            /// </summary>
            BeforeBookingStart,
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
            /// 計算及寫入最終租金
            /// </summary>
            CalFinalPrice,
            /// <summary>
            /// 完成付款
            /// </summary>
            DonePayRentBill,
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
            #endregion,
            #region BackEnd
                /// <summary>
                /// 更改密碼
                /// </summary>
                BE_ChangePWD,
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
            BE_HandleExtendCar
            #endregion

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
                case SPType.RegisterMemberData://註冊會員基本資料
                    SPName = "usp_RegisterMemberData";
                    break;
                case SPType.UploadCredentials: //上傳證件照
                    SPName = "usp_UploadCredentials";
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
                    SPName = "usp_Booking";
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
                case SPType.BeforeBookingStart:
                    SPName = "usp_BeforeBookingStart";
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
                    SPName = "usp_OrderListQuery";
                    break;
                case SPType.GetCancelOrder:
                    SPName = "usp_GetCancelOrderList";
                    break;
                case SPType.GetFinishOrder:
                    SPName = "usp_GetFinishOrderList";
                    break;
                case SPType.OrderDetail:
                    SPName = "usp_GetOrderDetail";
                    break;
                case SPType.CheckCarStatusByReturn:
                    SPName = "usp_CheckCarStatusByReturn";
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
                    SPName = "usp_INSTmpFeedBackPIC";
                    break;
                case SPType.InsFeedBack:
                    SPName = "usp_InsFeedBack";
                    break;
                case SPType.SettingParkingSpce:
                    SPName = "usp_InsParkingSpace";
                    break;
                case SPType.GetOrderStatusByOrderNo:
                    SPName = "usp_GetOrderStatusByOrderNo";
                    break;
                case SPType.CalFinalPrice:
                    SPName = "usp_CalFinalPrice";
                    break;
                case SPType.DonePayRentBill:
                    SPName = "usp_DonePayRentBill";
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
                    SPName = "usp_GetStationCarType"; //20201023 Eason 轉sp
                    break;
                case SPType.GetStationCarTypeOfMutiStation:
                    SPName = "usp_GetStationCarTypeOfMutiStation";
                    break;
                case SPType.GetMemberStatus:    //20201016 ADD BY ADAM REASON.增加會員狀態(登入後狀態)
                    SPName = "usp_GetMemberStatus";
                    break;
                case SPType.GetMemberData:      //20201022 ADD BY ADAM REASON.改寫為sp
                    SPName = "usp_GetMemberData";
                    break;
                #region 渣渣
                case SPType.PersonNotice:
                    SPName = "usp_GetNotificationList";
                    break;
                case SPType.News:
                    SPName = "usp_GetNews";
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
                #endregion
                #region BackEnd
                case SPType.BE_ChangePWD:
                    SPName = "usp_BE_ChangePWD";
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
                    SPName = "usp_BE_HandleTransParking";
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
                    #endregion
            }
            return SPName;
        }
    }
}