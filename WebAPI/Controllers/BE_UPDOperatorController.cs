using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Output;
using Domain.WebAPI.Input.CENS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】修改加盟業者
    /// </summary>
    public class BE_UPDOperatorController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】取得車輛列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_UPDOperator(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_UPDOperatorController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_UPDOperator apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            DateTime StartDate=DateTime.Now, EndDate=DateTime.Now;
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            string FileName = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_UPDOperator>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OperatorAccount, apiInput.OperatorName };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);

                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkUniNum(apiInput.OperatorAccount);
                    if (false == flag)
                    {
                        errCode = "ERR191";
                    }
                }
                if (flag)
                {
                    if (false == DateTime.TryParseExact(apiInput.StartDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out StartDate))
                    {
                        flag = false;
                        errCode = "ERR241";
                    }
                    if (false == DateTime.TryParseExact(apiInput.EndDate, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out EndDate))
                    {
                        flag = false;
                        errCode = "ERR243";
                    }
                    if (flag)
                    {
                        if (StartDate > EndDate)
                        {
                            flag = false;
                            errCode = "ERR267";
                        }
                    }
                }
            }
            #endregion
            #region 判斷有無更新icon
            if (flag)
            {
                if (false == string.IsNullOrEmpty(apiInput.OperatorICon))
                {
                    apiInput.OperatorICon = apiInput.OperatorICon.Replace("⊙", "");
                    FileName = string.Format("{0}_{1}", apiInput.OperatorAccount, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.OperatorICon, FileName, "operatoricon");
                  
                }
            }
            #endregion
            #region TB

            if (flag)
            {
                
                string spName = new ObjType().GetSPName(ObjType.SPType.BE_UPDOperator);
                SPInput_BE_UPDOperator spInput = new SPInput_BE_UPDOperator()
                {
                    LogID = LogID,
                    UserID = apiInput.UserID,
                    EndDate = EndDate,
                    StartDate = StartDate,
                    OperatorName = apiInput.OperatorName,
                    OperatorAccount = apiInput.OperatorAccount,
                    OperatorICon = (apiInput.OperatorICon == "") ? "" : FileName,
                    OperatorID = apiInput.OperatorID

                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BE_UPDOperator, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_UPDOperator, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
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
