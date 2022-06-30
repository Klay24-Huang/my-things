using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Rent;
using Domain.TB;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;
using System.IO;
using System.Text;
using WebAPI.Utils;
using System.Net;
using NLog;
using System.Globalization;
using Domain.SP.Input;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 城市車旅Ftp資料比對
    /// </summary>
    public class FtpCityParkDataCheckController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoFtpCityParkDataCheck(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "FtpCityParkDataCheckController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            OAPI_FtpCityParkDataCheck apiOutput = null;
            #endregion

            #region 防呆
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("no Input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion

            #region TB

            if (flag)
            {
                SPInput_FtpCityParkDataCheck spInput = new SPInput_FtpCityParkDataCheck()
                {
                    LogID = LogID,
                    APIName = funName

                };
                string SPName = "usp_FtpCityParkDataCheck";
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_FtpCityParkDataCheck, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_FtpCityParkDataCheck, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }

            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }




    }
}