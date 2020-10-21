using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
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
    /// 機車取車
    /// </summary>
    public class BookingStartMotorController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBookingStartMotor(Dictionary<string, object> value)
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
            string funName = "BookingStartMotorController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingStartMotor apiInput = null;
            OAPI_BookingStartMotor outputApi = new OAPI_BookingStartMotor();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MotorInfo info = new MotorInfo();

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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingStartMotor>(Contentjson);
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
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.BeforeBookingStart);
                SPInput_BeforeBookingStart spBeforeStart = new SPInput_BeforeBookingStart()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_BeforeBookingStart spOut = new SPOutput_BeforeBookingStart();
                SQLHelper<SPInput_BeforeBookingStart, SPOutput_BeforeBookingStart> sqlHelp = new SQLHelper<SPInput_BeforeBookingStart, SPOutput_BeforeBookingStart>(connetStr);
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
                    
                    if (flag)
                    {
                        info = new CarStatusCommon(connetStr).GetInfoByMotor(CID);
                        if (info != null)
                        {
                            mil = info.Millage;
                        }
                    }
                    #endregion
                    #region 執行sp合約
                    string BookingStartName = new ObjType().GetSPName(ObjType.SPType.BookingStart);
                    Domain.SP.Input.Rent.SPInput_BookingStart SPBookingStartInput = new Domain.SP.Input.Rent.SPInput_BookingStart()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        OrderNo = tmpOrder,
                        Token = Access_Token,
                        NowMileage = Convert.ToSingle(mil),
                        StopTime = ""
                    };
                    SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                    SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base>(connetStr);
                    flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingStartName, SPBookingStartInput, ref SPBookingStartOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref SPBookingStartOutput, ref lstError, ref errCode);
                    #endregion
                    #region 設定租約
                    if (flag)
                    {
                        if (info.extDeviceStatus1 == 0)
                        {
                            CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetMotorcycleRent);
                            CmdType = OtherService.Enum.MachineCommandType.CommandType.SetMotorcycleRent;
                            WSInput_Base<BLECode> RentInput = new WSInput_Base<BLECode>()
                            {
                                command = true,
                                method = CommandType,
                                requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                _params = new BLECode()

                            };
                            RentInput._params.BLE_Code = IDNO.Substring(0, 9);
                            requestId = RentInput.requestId;
                            method = CommandType;
                            flag = FetAPI.DoSendCmd(deviceToken, CID, CmdType, RentInput, LogID);
                            if (flag)
                            {
                                flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);

                            }

                            if (flag)
                            {
                                Thread.Sleep(1000);
                                //查ble
                                BLEInfo ble = new CarCMDRepository(connetStr).GetBLEInfo(CID);
                                if (ble != null)
                                {
                                    outputApi.BLEDEVICEID = ble.BLE_Device;
                                    outputApi.BLEDEVICEPWD = ble.BLE_PWD;
                                }

                            }
                        }
                      
                    }
                    #endregion
                    
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
