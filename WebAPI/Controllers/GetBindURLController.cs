using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// 取得台新綁卡網址
    /// </summary>
    public class GetBindURLController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();

        private string BindResultURL= ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string BindSuccessURL = ConfigurationManager.AppSettings["BindSuccessURL"].ToString();
        private string BindFailURL = ConfigurationManager.AppSettings["BindFailURL"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doBindCreditCard(Dictionary<string, object> value)
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
            string funName = "GetBindURLController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetBindURL apiInput = null;
            OAPI_GetBindURL outputAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetBindURL>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                string[] checkList = { apiInput.IDNO};
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    //2.判斷格式
                    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    if (false == flag)
                    {
                        errCode = "ERR103";
                    }
                }
           
            }

            #endregion
            #region 不支援訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR150";
                }
            }
            #endregion
            #region TB
            //Token判斷
            if (flag && isGuest == false)
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

            #endregion
            #region 台新綁卡
            if(flag && IDNO == apiInput.IDNO)
            {
                TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                WebAPIInput_Base wsInput = new WebAPIInput_Base()
                {
                    ApiVer = "1.0.2",
                    ApposId = TaishinAPPOS,
                    RequestParams = new RequestParamsData()
                    {
                        FailUrl = HttpUtility.UrlEncode(BindFailURL),
                        SuccessUrl = HttpUtility.UrlEncode(BindSuccessURL),
                        ResultUrl = HttpUtility.UrlEncode(BindResultURL),
                        MemberId = IDNO,
                        OrderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                        PaymentType = "04"
                        //   PaymentType = "04"

                    },
                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString()

                };
                WebAPIOutput_Base wsOutput = new WebAPIOutput_Base();
                logger.Trace(IDNO + "Call:" + JsonConvert.SerializeObject(wsInput));
                flag = WebAPI.DoBind(wsInput, ref errCode, ref wsOutput);
                logger.Trace(IDNO + "Response:" + errCode + "," + JsonConvert.SerializeObject(wsOutput));
                if (flag)
                {
                    if (wsOutput.RtnCode == "1000")
                    {
                        outputAPI = new OAPI_GetBindURL()
                        {
                            CardAuthURL = wsOutput.ResponseParams.ResultData.CardAuthUrl,
                            SuccessURL = BindSuccessURL,
                            FailURL = BindFailURL
                        };
                       


                    }
                    else
                    {
                        flag = false;
                        errMsg = wsOutput.ResponseParams.ResultMessage;
                        errCode = "ERR600";
                    }
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputAPI, token);
            return objOutput;
            #endregion
        }
    }
}
