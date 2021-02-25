using Domain.Common;
using Domain.SP.MA.Input;
using Domain.SP.Output;
using Domain.TB.Maintain;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Maintain.Input;
using WebAPI.Models.Param.Maintain.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台及整備人員共用】新增保修清潔合約
    /// </summary>
    public class MA_InsertCleanController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoMA_InsertClean(Dictionary<string, object> value)
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
            string funName = "MA_InsertCleanController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MA_InsClean apiInput = null;
            NullOutput outputApi = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CarRepository _repository = new CarRepository(connetStr);
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            DateTime SD = DateTime.Now;
            DateTime ED = DateTime.Now;
            string ManageStation = "";
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MA_InsClean>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            }
            if (flag)
            {
                if (flag)
                {
                    if (false == DateTime.TryParseExact(apiInput.SD, "yyyyMMddHHmm00", null, System.Globalization.DateTimeStyles.None, out SD))
                    {
                        flag = false;
                        errCode = "ERR907";
                    }
                    if (false == DateTime.TryParseExact(apiInput.ED, "yyyyMMddHHmm00", null, System.Globalization.DateTimeStyles.None, out ED))
                    {
                        flag = false;
                        errCode = "ERR907";
                    }
                    if (flag)
                    {
                        DateTime nowTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                        // ED = ED.AddHours(8);
                        if (nowTime > SD)
                        {
                            flag = false;
                            errCode = "ERR154";
                        }
                        if (flag)
                        {
                            if (SD > ED || ED.Subtract(SD).TotalMinutes < 5)
                            {
                                flag = false;
                                errCode = "ERR153";
                            }
                        }
                    }
                }
            }
            #endregion
            #region TB
            if (flag)
            {
                //usp_SettingClearMemberData_202003
                string spName = "usp_MA_InsClean";

          

                SPInput_MA_InsClean spInput = new SPInput_MA_InsClean()
                {
                    manager = apiInput.manager,
                    CarNo = apiInput.CarNo,
                    SD = SD,
                    ED = ED,
                    SpecCode = apiInput.SpecCode,
                     LogID=LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();

                flag = new SQLHelper<SPInput_MA_InsClean, SPOutput_Base>(connetStr).ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }

            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
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
