using Domain.Common;
using Domain.SP.Input.MonthlyRent;
using Domain.SP.Output;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Models.Enum;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    public class MonthlyPayJobController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoMonthlyPayJob(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MonthlyPayJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPInput_InsMonthlyPayData spInput = new SPInput_InsMonthlyPayData();
            SPOutput_Base spOut = new SPOutput_Base();
            NullOutput outputApi = null;
            #endregion

            #region TB
            if (flag)
            {
                string spName = "usp_InsMonthlyPayData";
                flag = new SQLHelper<SPInput_InsMonthlyPayData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}