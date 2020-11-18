using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
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
    /// 點數歷程查詢
    /// </summary>
    public class BonusHistoryQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost()]
        public Dictionary<string, object> DoBonusHistoryQuery([FromBody] Dictionary<string, object> value)
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
            string funName = "BonusHistoryQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BonusHistoryQuery apiInput = null;
            //List<OAPI_BonusQuery> outputApi = new List<OAPI_BonusQuery>();
            OAPI_BonusHistoryQuery outputApi = new OAPI_BonusHistoryQuery();
            Int64 tmpOrder = -1;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BonusHistoryQuery>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.IDNO))
                {
                    flag = false;
                    errCode = "ERR900";
                }
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
            //不開放訪客
            if (flag)
            {
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
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
            if (flag)
            {
                if (IDNO != apiInput.IDNO)
                {
                    flag = false;
                    errCode = "ERR101";
                }
            }
            //開始送短租查詢
            if (flag)
            {
                WebAPIOutput_NPR271Query wsOutput = new WebAPIOutput_NPR271Query();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                flag = wsAPI.NPR271Query(apiInput.IDNO, apiInput.SEQNO, ref wsOutput);


                if (flag)
                {
                    int giftLen = wsOutput.Data.Length;

                    if (giftLen > 0)
                    {
                        outputApi.BonusObj = new List<BonusHistoryData>();
                        int TotalGiftPoint = 0;
                        int TotalUsePoint = 0;
                        int TotalLastPoint = 0;

                        for (int i = 0; i < giftLen; i++)
                        {
                            DateTime tmpDate;
                            bool DateFlag = DateTime.TryParse(wsOutput.Data[i].PROCDT, out tmpDate);

                            if (DateFlag)
                            {


                                BonusHistoryData objPoint = new BonusHistoryData()
                                {
                                    GIFTPOINT = wsOutput.Data[i].GIFTPOINT.ToString(),
                                    USEDATE = Convert.ToDateTime(wsOutput.Data[i].PROCDT).ToString("yyyy-MM-dd HH:mm:ss"),
                                    MEMO = wsOutput.Data[i].MEMO

                                };

                                if (wsOutput.Data[i].LOGTYPE == "01" || wsOutput.Data[i].LOGTYPE == "03")
                                {
                                    TotalGiftPoint += wsOutput.Data[i].GIFTPOINT;
                                }
                                else if (wsOutput.Data[i].LOGTYPE == "02")
                                {
                                    TotalUsePoint += wsOutput.Data[i].GIFTPOINT;
                                    objPoint.MEMO = "用車折抵";
                                    outputApi.BonusObj.Add(objPoint);
                                }
                                else
                                {
                                    TotalUsePoint += wsOutput.Data[i].GIFTPOINT;
                                    outputApi.BonusObj.Add(objPoint);
                                }

                            }
                        }
                        TotalLastPoint = TotalGiftPoint - TotalUsePoint;
                        outputApi.TotalGIFTPOINT = TotalGiftPoint;
                        outputApi.TotalLASTPOINT = TotalLastPoint;
                    }
                }
                else
                {
                    errCode = "ERR";
                    errMsg = wsOutput.Message;
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}