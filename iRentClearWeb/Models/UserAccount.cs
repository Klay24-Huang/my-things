using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using WebCommon;

namespace iRentClearWeb.Models
{
    public class UserAccount : IValidatableObject
    {
        /// <summary>
        /// 要導回的網址
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReturnUrl { get; set; }
        /// <summary>
		/// 帳號
		/// </summary>
		[DisplayFormat(ConvertEmptyStringToNull = false)]

        [Required(ErrorMessage = "帳號為必填")]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; }
        /// <summary>
        /// 使用者姓名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 群組代碼
        /// </summary>
        public string AUTHGPNO { set; get; }
        /// <summary>
        /// 群組名稱
        /// </summary>
        public string AUTHGPNM { set; get; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            string happyAccount = String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("happyAccount")) ? "" : ConfigurationManager.AppSettings.Get("happyAccount").ToString();
            string happyPWD = String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("happyPWD")) ? "" : ConfigurationManager.AppSettings.Get("happyPWD").ToString();
            string happyUserName = String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("happyUserName")) ? "" : ConfigurationManager.AppSettings.Get("happyUserName").ToString();
            UserAccount tmpUser = new UserAccount()
            {
                Account = Account,
                Password = Password
            };
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            bool flag = false;
            if (happyAccount != "" && happyPWD != "" && happyUserName != "")
            {
                if (Account == happyAccount && Password == happyPWD)
                {
                    flag = true;
                    tmpUser.AUTHGPNM = "anypass";
                    tmpUser.AUTHGPNO = "anypass";
                    tmpUser.UserName = happyUserName;
                }
                else
                {
                    //flag = new WebServiceFactory().Create(WebServiceFactory.WebServiceType.easyRent).ExecuteAPI(ref tmpUser, ref lstError);
                    //要加入短租員工登入
                }
            }
            else
            {
                // flag = new WebServiceFactory().Create(WebServiceFactory.WebServiceType.easyRent).ExecuteAPI(ref tmpUser, ref lstError);
                //要加入短租員工登入
            }

            if (false == flag)
            {
                yield return new ValidationResult(lstError[0].ErrorMsg, new string[] { "Account" });
            }
            else
            {
                AUTHGPNM = tmpUser.AUTHGPNM;
                AUTHGPNO = tmpUser.AUTHGPNO;
                UserName = tmpUser.UserName;
                //UserName = oMember.MEMCNAME;

            }

        }
    }
}
