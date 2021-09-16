using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetPayInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetPayInfo(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetPayInfoController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetPayInfo apiInput = null;
            OAPI_GetPayInfo apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);

            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Access_Token, ClientIP, funName, ref errCode, ref LogID);
            }
            //不開放訪客
            if (isGuest)
            {
                flag = false;
                errCode = "ERR150";
            }
            #endregion
            #region TB
            //Token判斷
            if (flag)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }

            if(flag)
            {
                string SPName = "usp_GetPayInfo_Q1";
                SPInput_GetPayInfo spCheckTokenInput = new SPInput_GetPayInfo()
                {
                    LogID = LogID,
                    Token = Access_Token,
                    IDNO = IDNO
                };

                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetPayInfo, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetPayInfo, SPOutput_Base>(connetStr);
                List<SPOutput_GetPayInfo> PayMode = new List<SPOutput_GetPayInfo>();

                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, spCheckTokenInput, ref spOut, ref PayMode, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag && PayMode.Count > 0)
                {
                    apiOutput = PayMode
                        .Select(t => new OAPI_GetPayInfo
                        {
                            DefPayMode = t.DefPayMode,
                            PayModeBindCount = t.PayModeBindCount,
                            PayModeList = JsonSerializer.Deserialize<List<PayModeObj>>(PayMode[0].PayModeList)
                        }).FirstOrDefault();
                }
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