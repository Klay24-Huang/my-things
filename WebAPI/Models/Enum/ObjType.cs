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
            /// 還車前檢查
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
            #region 拓連
            /// <summary>
            /// 更新交換站點資訊
            /// </summary>
            UPD_SW_DATA,
            GetMaintainKey,
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
                case SPType.ReturnCar:
                    SPName = "usp_ReturnCar";
                    break;
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
            }
            return SPName;
        }
    }
}