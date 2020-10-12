using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 修改密碼
    /// </summary>
    public class ChangePWDController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 修改密碼
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doChangePWD(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"]==null)?"": httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ChangePWDController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ChangePWD apiInput = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
        
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string,ref Access_Token);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ChangePWD>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.OldPWD,apiInput.NewPWD};
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                //if (flag)
                //{
                //    //2.判斷格式
                //    flag = baseVerify.checkIDNO(apiInput.IDNO);
                //    if (false == flag)
                //    {
                //        errCode = "ERR103";
                //    }

                //}
                //if (flag)
                //{
                //    if (apiInput.APPVersion.Split('.').Count() < 3)
                //    {
                //        flag = false;
                //        errCode = "ERR104";
                //    }
                //}
                //if (flag)
                //{
                //    flag = (apiInput.APP.HasValue);
                //    if (false == flag)
                //    {
                //        errCode = "ERR900";
                //    }
                //    else
                //    {
                //        APPKind = apiInput.APP.Value;
                //        if (APPKind < 0 || APPKind > 1)
                //        {
                //            flag = false;
                //            errCode = "ERR105";
                //        }
                //    }
                //}
            }
            #endregion

            #region TB
            //Token判斷
            //if (flag)
            //{
            //    string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckToken);
            //    SPInput_CheckToken spCheckTokenInput = new SPInput_CheckToken()
            //    {
            //        APP = apiInput.APP.Value,
            //        APPVersion = apiInput.APPVersion,
            //        DeviceID = apiInput.DeviceID,
            //        IDNO = apiInput.IDNO,
            //        LogID = LogID,
            //        Token = Access_Token
            //    };
            //    SPOutput_Base spOut = new SPOutput_Base();
            //    SQLHelper<SPInput_CheckToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckToken, SPOutput_Base>(connetStr);
            //    flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
            //    baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            //}
            if (flag )
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }

            }
            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.ChangePWD);
                SPInput_ChangePWD spInput = new SPInput_ChangePWD()
                {
                    LogID = LogID,
                    IDNO = IDNO,
                    NewPWD = apiInput.NewPWD,
                    OldPWD=apiInput.OldPWD,
              
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_ChangePWD, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_ChangePWD, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag,ref spOut, ref lstError, ref errCode);

            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, CheckAccountAPI, token);
            return objOutput;
            #endregion
        }
    }
}
