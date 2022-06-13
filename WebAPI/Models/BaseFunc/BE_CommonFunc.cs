using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.TB;
using Newtonsoft.Json;
using NLog;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Models.BaseFunc
{
    /// <summary>
    /// 後台API用 共用模組
    /// </summary>
    public class BE_CommonFunc
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool InsEDMLog(string IDNO, string EMAIL, string USERID, Int64 LogID)
        {
            bool flag = true;
            try
            {
                SPInput_BE_InsHotaipayEdmLog_I01 spInput = new SPInput_BE_InsHotaipayEdmLog_I01()
                {
                    IDNO = IDNO,
                    EMAIL = EMAIL,
                    USERID = USERID,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                List<ErrorInfo> lstError = new List<ErrorInfo>();
                string spName = "usp_BE_InsHotaipayEdmLog_I01";
                SQLHelper<SPInput_BE_InsHotaipayEdmLog_I01, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_InsHotaipayEdmLog_I01, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                flag = !(spOut.Error == 1 || spOut.ErrorCode != "0000");
                if (spOut.ErrorCode == "0000" && lstError.Count > 0)
                {
                    spOut.ErrorCode = lstError[0].ErrorCode;
                }
            }
            catch (Exception ex)
            {

            }

            return flag;
        }
    }
}