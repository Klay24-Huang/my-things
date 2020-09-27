using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取車前判斷車輛狀態
    /// </summary>
    public class CheckCarStatusController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoCheckCarStatus(Dictionary<string, object> value)
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
            string funName = "CheckCarStatusController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CheckCarStatus apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MotorInfo motorinfo = new MotorInfo();
            CarInfo carInfo = new CarInfo();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            double mil = 0;
            DateTime StopTime;
            List<CardList> lstCardList = new List<CardList>();

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CheckCarStatus>(Contentjson);
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
            //取車判斷
            if (flag)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckCarStatus);
                SPInput_CheckCarStatus spBeforeStart = new SPInput_CheckCarStatus()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckCarStatus spOut = new SPOutput_CheckCarStatus();
                SQLHelper<SPInput_CheckCarStatus, SPOutput_CheckCarStatus> sqlHelp = new SQLHelper<SPInput_CheckCarStatus, SPOutput_CheckCarStatus>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spBeforeStart, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    CID = spOut.CID;
                    deviceToken = spOut.deviceToken;
                    IsCens = spOut.IsCens;
                    IsMotor = spOut.IsMotor;
                    List<ErrorInfo> lstCarError = new List<ErrorInfo>();
                    lstCardList = new CarCardCommonRepository(connetStr).GetCardListByCustom(CID.ToUpper(), ref lstCarError);
                }
                //開始對車機做動作
                if (flag)
                {
                    if (IsCens == 0)
                    {
                        #region 取最新狀況, 先送getlast之後從tb捉最近一筆
                        FETCatAPI FetAPI = new FETCatAPI();
                        string requestId = "";
                        string CommandType = "";
                        OtherService.Enum.MachineCommandType.CommandType CmdType;
                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.ReportNow);
                        CmdType = OtherService.Enum.MachineCommandType.CommandType.ReportNow;
                        WSInput_Base<Params> input = new WSInput_Base<Params>()
                        {
                            command = true,
                            method = CommandType,
                            requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                            _params = new Params()

                        };
                        requestId = input.requestId;
                        string method = CommandType;
                        flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, input, LogID);
                        if (flag)
                        {
                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        }
                        #endregion
                    }
                    else
                    {
                        #region 興聯
                        CensWebAPI webAPI = new CensWebAPI();
                        //取最新狀況
                        WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                        flag = webAPI.GetInfo(CID, ref wsOutInfo);
                        if (false == flag)
                        {
                            errCode = wsOutInfo.ErrorCode;
                        }
                      
                        #endregion
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