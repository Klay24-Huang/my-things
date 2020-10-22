using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
    /// 欠費查詢
    /// </summary>
    public class ArrearsQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost()]
        public Dictionary<string, object> DoArrearsQuery([FromBody] Dictionary<string, object> value)
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
            string funName = "ArrearsQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_ArrearsQuery apiInput = null;
            OAPI_ArrearsQuery outputApi =null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ArrearsQuery>(Contentjson);
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

            //開始送短租判斷
            if (flag)
            {
                HiEasyRentAPI api = new HiEasyRentAPI();
                WebAPIOutput_ArrearQuery WebAPIOutput = null;
                flag = api.NPR330Query(apiInput.IDNO, ref WebAPIOutput);
                if (flag)
                {
                    if (WebAPIOutput.Result)
                    {
                        if (WebAPIOutput.RtnCode == "0")
                        {
                            List<iRentStationBaseInfo> lstStation = new StationAndCarRepository(connetStr).GetAlliRentStationBaseData();
                            bool hasData = true;
                            hasData = (lstStation != null);
                            if (hasData == true)
                            {
                                hasData = (lstStation.Count > 0);
                            }
                            int DataLen = WebAPIOutput.Data.Count();
                           if (DataLen > 0)
                            {
                                outputApi = new OAPI_ArrearsQuery();
                                outputApi.ArrearsInfos = new List<ArrearsQueryDetail>();
                                for(int i = 0; i < DataLen; i++)
                                {
                                    ArrearsQueryDetail obj = new ArrearsQueryDetail()
                                    {
                                        Amount = Convert.ToInt32(WebAPIOutput.Data[i].TAMT),
                                        ArrearsKind = WebAPIOutput.Data[i].PAYMENTTYPE,
                                        EndDate = WebAPIOutput.Data[i].RNTDATE,
                                        StartDate = WebAPIOutput.Data[i].GIVEDATE,
                                        OrderNo = (string.IsNullOrEmpty(WebAPIOutput.Data[i].IRENTORDNO)) ? "-" : WebAPIOutput.Data[i].IRENTORDNO,
                                        ShortOrderNo = WebAPIOutput.Data[i].CNTRNO
                                       
                                    };
                                    if (hasData)
                                    {
                                        int Index = lstStation.FindIndex(delegate (iRentStationBaseInfo station)
                                          {
                                              return WebAPIOutput.Data[i].INBRNHCD == station.StationID;
  
                                          });
                                        if (Index > -1)
                                        {
                                            obj.StationName = lstStation[Index].StationName;
                                        }
                                        else
                                        {
                                            obj.StationName = "-";
                                        }
                                    }
                                    outputApi.ArrearsInfos.Add(obj);
                                }

                                outputApi.TradeOrderNo = outputApi.TradeOrderNo ?? "";                            
                                if(outputApi.ArrearsInfos != null && outputApi.ArrearsInfos.Count()>0)
                                    outputApi.TotalAmount = outputApi.ArrearsInfos.Select(x => x.Amount).Sum();                            
                            }
                        }
                        else
                        {
                            errCode = "ERR";
                            errMsg = WebAPIOutput.Message;
                            flag = false;
                        }
                    }
                    else
                    {
                        errCode = "ERR";
                        errMsg = WebAPIOutput.Message;
                        flag = false;
                    }
                }
                else
                {
                    errCode = "ERR301"; //贈送查詢失敗，請稍候再試
                }

            }

            #endregion
            #region 寫入金流資料表及欠費資料表
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
