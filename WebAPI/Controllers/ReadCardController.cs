using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 讀卡
    /// </summary>
    public class ReadCardController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoReadCard(Dictionary<string, object> value)
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
            string funName = "BookingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ReadCard apiInput = null;
            OAPI_ReadCard outputApi = new OAPI_ReadCard();
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ReadCard>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
                {
                    if (apiInput.OrderNo.IndexOf("H") < 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        flag = Int64.TryParse(apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                        if (flag)
                        {
                            if (tmpOrder <= 0)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }

                        }
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

            //開始做讀卡判斷
            if (flag)
            {
                SPInput_ReadCard spInput = new SPInput_ReadCard()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.ReadCard);
                SPOutput_ReadCard spOut = new SPOutput_ReadCard();
                SQLHelper<SPInput_ReadCard, SPOutput_ReadCard> sqlHelp = new SQLHelper<SPInput_ReadCard, SPOutput_ReadCard>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    #region 興聯車機要開讀卡機電源
                    if (spOut.IsCens == 1)
                    {
                        CensWebAPI censWebAPI = new CensWebAPI();
                        WSOutput_Base WsOutput = new WSOutput_Base();
                        flag = censWebAPI.NFCPower(spOut.CID, 1, LogID, ref WsOutput);
                        if (flag == false)
                        {
                            errCode = WsOutput.ErrorCode;
                            errMsg = WsOutput.ErrMsg;
                        }
                    
                    }
                    #endregion
                }
                if (flag) { 
                    int NowCount = 0;
                    DateTime NowTime = DateTime.Now.AddSeconds(-15);
                    bool ReadFlag = false;
                    
                    while (NowCount < 60)
                    {
                        Thread.Sleep(1000);
                         ReadFlag = new CarCMDRepository(connetStr).CheckHasReadCard(spOut.CID, NowTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        if (ReadFlag)
                        {
                            outputApi.HasBind = 1;
                            break;
                        }
                        NowCount++;
                    }

                    if (false == ReadFlag)
                    {
                        outputApi.HasBind = 0;
                    }
                    if (ReadFlag)
                    {
                        SPInput_BindUUCard SPBindInput = new SPInput_BindUUCard()
                        {
                            OrderNo = tmpOrder,
                            IDNO = IDNO,
                            LogID = LogID,
                            Token = Access_Token
                        };
                        SPOutput_Base SPBindOutput = new SPOutput_Base();
                        SQLHelper<SPInput_BindUUCard, SPOutput_Base> sqlBindHelp = new SQLHelper<SPInput_BindUUCard, SPOutput_Base>(connetStr);
                        flag = sqlBindHelp.ExecuteSPNonQuery(SPName, SPBindInput, ref SPBindOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
