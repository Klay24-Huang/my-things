using Domain.TB.SubScript;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using WebCommon;

namespace WebModel
{
    public class iRentNUser : IValidatableObject
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "帳號為必填")]
        public string Account { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required(ErrorMessage = "密碼為必填")]
        public string Password { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ReturnUrl { get; set; }

        public string UserName { get; set; }

        public iRentNUser()
        {
        }

        public string sha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            iRentNUser userName = JsonConvert.DeserializeObject<iRentNUser>(JsonConvert.SerializeObject(validationContext.ObjectInstance));
            iRentNUser _iRentNUser = new iRentNUser()
            {
                Account = userName.Account,
                Password = userName.Password
            };
            iRentNUser mEMCNAME = _iRentNUser;
            List<ErrorInfo> errorInfos = new List<ErrorInfo>();
            WSInput_MemberLogin wSInputMemberLogin = new WSInput_MemberLogin()
            {
                MEMIDNO = mEMCNAME.Account,
                MEMPWD = mEMCNAME.Password
            };
            WSInput_MemberLogin wSInputMemberLogin1 = wSInputMemberLogin;
            //if (mEMCNAME.Password.Length > 10)
            //{
            //    wSInputMemberLogin1.MEMPWD = mEMCNAME.Password.Substring(0, 10);
            //}
            WSOutput_MemberLoginNew wSOutputMemberLoginNew = new WSOutput_MemberLoginNew();
            bool flag = false;
            //SHA256 sha256 = new SHA256CryptoServiceProvider();
            //string shaPWD = Convert.ToBase64String(sha256.ComputeHash(Encoding.Default.GetBytes(wSInputMemberLogin.MEMPWD)));
            string shaPWD = "0X"+sha256(wSInputMemberLogin.MEMPWD);
            List<MemberAuth> lstQuery = null;
            string dataPWD = "";

            try
            {
                //flag = (new WebServiceFactory()).Create(WebServiceFactory.WebServiceType.hfcEasyRent).ExecuteAPI<WSInput_MemberLogin, WSOutput_MemberLoginNew>(ref wSInputMemberLogin1, ref wSOutputMemberLoginNew, APIType.ServiceFunType.MemberLogin201902, ref errorInfos);

                SubScriptionRepository subScriptionRepository = new SubScriptionRepository(this.connetStr);

                lstQuery = subScriptionRepository.GetMemberData(wSInputMemberLogin.MEMIDNO);
                if (lstQuery.Count > 0)
                {
                    flag = true;
                    dataPWD = lstQuery[0].MEMPWD.ToUpper();
                }
                else
                {
                    flag = false;
                    errorInfos.Add(new ErrorInfo()
                    {
                        ErrorMsg = "帳號或密碼輸入錯誤"
                    });
                }
                if (flag)
                {
                    if (dataPWD != shaPWD)
                    {
                        flag = false;
                        errorInfos.Add(new ErrorInfo()
                        {
                            ErrorMsg = "帳號或密碼輸入錯誤"
                        });
                    }
                    else
                    {
                        dataPWD = shaPWD;
                    }
                }
            }
            catch (Exception exception)
            {
                errorInfos.Add(new ErrorInfo()
                {
                    ErrorMsg = "網路發生錯誤，請稍候再試"
                });
            }
            if (flag)
            {
                dataPWD = shaPWD;
            }
            else
            {
                string errorMsg = errorInfos[0].ErrorMsg;
                yield return new ValidationResult(errorMsg, new string[] { "Account" });
            }
        }
    }
}