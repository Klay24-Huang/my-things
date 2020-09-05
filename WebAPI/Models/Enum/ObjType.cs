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
            }
            return SPName;
        }
    }
}