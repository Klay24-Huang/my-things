using Domain.CarMachine;
using Domain.Common;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.Output.CENS;
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
    /// 取車
    /// </summary>
    public class BookingStartController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoBookingStart(Dictionary<string, object> value)
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
            string funName = "BookingStartController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingStart apiInput = null;
            OAPI_BookingStart outputApi = new OAPI_BookingStart();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CarInfo info = new CarInfo();

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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingStart>(Contentjson);
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
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(apiInput.ED))
                {
                    flag = DateTime.TryParse(apiInput.ED, out StopTime);
                    if (flag == false)
                    {
                        errCode = "ERR173";
                    }
                    else
                    {
                        if (StopTime <= DateTime.Now)
                        {
                            flag = false;
                            errCode = "ERR174";
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
                    lstCardList = new CarCardCommonRepository(connetStr).GetCardListByCustom(IDNO, ref lstCarError);
                }
                //開始對車機做動作
                if (flag)
                {
                    if (IsCens == 1)
                    {
                        #region 興聯
                        CensWebAPI webAPI = new CensWebAPI();
                        //取最新狀況
                        WSOutput_GetInfo wsOutInfo = new WSOutput_GetInfo();
                        flag = webAPI.GetInfo(CID, ref wsOutInfo);
                        if (false == flag)
                        {
                            errCode = wsOutInfo.ErrorCode;
                            mil = 0;
                        }
                        else
                        {
                            if (wsOutInfo.data.CID == CID)
                            {
                                if (wsOutInfo.data.Milage > 0)
                                {
                                    mil = wsOutInfo.data.Milage;
                                }
                                else
                                {
                                    //判斷是否為0，若是0則抓取前一天內里程大於0的值
                                    //DbAssister da = new DbAssister();
                                    mil = 0;
                                }
                            }
                            else
                            {
                                flag = false;
                                errCode = "ERR468";
                            }
                        }
                        //寫入顧客卡
                        if (flag)
                        {
                            //要將卡號寫入車機
                            int count = 0;
                            int CardLen = lstCardList.Count;
                            if (CardLen > 0)
                            {
                                SendCarNoData[] CardData = new SendCarNoData[CardLen];
                                //寫入顧客卡
                                WSInput_SendCardNo wsInput = new WSInput_SendCardNo()
                                {
                                    CID = CID,
                                    mode = 1
                                };
                                for (int i = 0; i < CardLen; i++)
                                {
                                    CardData[i] = new SendCarNoData();
                                    CardData[i].CardNo = lstCardList[i].CardNO;
                                    CardData[i].CardType = (lstCardList[i].CardType == "C") ? 1 : 0;
                                    count++;
                                }
                                //  Array.Resize(ref CardData, count + 1);
                                wsInput.data = new SendCarNoData[CardLen];
                                wsInput.data = CardData;
                                WSOutput_Base wsOut = new WSOutput_Base();
                                Thread.Sleep(500);
                                flag = webAPI.SendCardNo(wsInput, ref wsOut);
                                if (false == flag)
                                {
                                    errCode = wsOut.ErrorCode;
                                }
                            }
                        }
                        //執行sp合約
                        if (flag)
                        {
                            string BookingStartName = new ObjType().GetSPName(ObjType.SPType.BookingStart);
                            Domain.SP.Input.Rent.SPInput_BookingStart SPBookingStartInput = new Domain.SP.Input.Rent.SPInput_BookingStart()
                            {
                                IDNO = IDNO,
                                LogID = LogID,
                                OrderNo = tmpOrder,
                                Token = Access_Token,
                                NowMileage = Convert.ToSingle(mil),
                                StopTime = (string.IsNullOrWhiteSpace(apiInput.ED)) ? "" : apiInput.ED,
                                Insurance = apiInput.Insurance
                            };
                            SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                            SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base>(connetStr);
                            flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingStartName, SPBookingStartInput, ref SPBookingStartOutput, ref lstError);
                            baseVerify.checkSQLResult(ref flag, ref SPBookingStartOutput, ref lstError, ref errCode);
                        }
                        if (flag)
                        {
                            string BookingControlName = new ObjType().GetSPName(ObjType.SPType.BookingControl);
                            SPInput_BookingControl SPBookingControlInput = new SPInput_BookingControl()
                            {
                                IDNO = IDNO,
                                OrderNo = tmpOrder,
                                Token = Access_Token,
                                LogID = LogID
                            };
                            SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                            SQLHelper<SPInput_BookingControl, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BookingControl, SPOutput_Base>(connetStr);
                            flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingControlName, SPBookingControlInput, ref SPBookingStartOutput, ref lstError);
                            baseVerify.checkSQLResult(ref flag, ref SPBookingStartOutput, ref lstError, ref errCode);
                        }
                        //設定租約狀態
                        if (flag)
                        {
                            WSInput_SetOrderStatus wsOrderInput = new WSInput_SetOrderStatus()
                            {
                                CID = CID,
                                OrderStatus = 1
                            };
                            WSOutput_Base wsOut = new WSOutput_Base();
                            Thread.Sleep(1000);
                            flag = webAPI.SetOrderStatus(wsOrderInput, ref wsOut);
                            if (false == flag || wsOut.Result == 1)
                            {
                                errCode = wsOut.ErrorCode;
                                errMsg = wsOut.ErrMsg;
                            }
                        }
                        //解防盜
                        if (flag)
                        {
                            WSInput_SendLock wsLockInput = new WSInput_SendLock()
                            {
                                CID = CID,
                                CMD = 4
                            };
                            WSOutput_Base wsOut = new WSOutput_Base();
                            Thread.Sleep(1500);
                            flag = webAPI.SendLock(wsLockInput, ref wsOut);
                            if (false == flag || wsOut.Result == 1)
                            {
                                errCode = wsOut.ErrorCode;
                                errMsg = wsOut.ErrMsg;
                            }
                        }
                        //開啟NFC電源
                        if (flag)
                        {
                            Thread.Sleep(1000);
                            WSOutput_Base wsOut = new WSOutput_Base();
                            flag = webAPI.NFCPower(CID, 1, LogID, ref wsOut);
                            if (false == flag || wsOut.Result == 1)
                            {
                                errCode = wsOut.ErrorCode;
                                errMsg = wsOut.ErrMsg;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 遠傳
                        //取最新狀況, 先送getlast之後從tb捉最近一筆
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
                            info = new CarStatusCommon(connetStr).GetInfoByCar(CID);
                            if (info != null)
                            {
                                mil = info.Millage;
                            }
                        }
                        //寫入顧客卡
                        if (flag)
                        {
                            if (lstCardList != null)
                            {
                                int CardLen = lstCardList.Count;
                                if (CardLen > 0)
                                {
                                    string[] CardStr = new string[CardLen];
                                    for (int i = 0; i < CardLen; i++)
                                    {
                                        CardStr[i] = lstCardList[i].CardNO;
                                    }
                                    if (CardStr.Length > 0)
                                    {
                                        CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo);
                                        CmdType = OtherService.Enum.MachineCommandType.CommandType.SetClientCardNo;
                                        WSInput_Base<ClientCardNoObj> SetCardInput = new WSInput_Base<ClientCardNoObj>()
                                        {
                                            command = true,
                                            method = CommandType,
                                            requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                            _params = new ClientCardNoObj()
                                            {
                                                ClientCardNo = CardStr
                                            }
                                        };
                                        requestId = SetCardInput.requestId;
                                        method = CommandType;
                                        flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, SetCardInput, LogID);
                                        if (flag)
                                        {
                                            flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                        }
                                    }
                                }
                            }

                            //執行sp合約
                            if (flag)
                            {
                                string BookingStartName = new ObjType().GetSPName(ObjType.SPType.BookingStart);
                                Domain.SP.Input.Rent.SPInput_BookingStart SPBookingStartInput = new Domain.SP.Input.Rent.SPInput_BookingStart()
                                {
                                    IDNO = IDNO,
                                    LogID = LogID,
                                    OrderNo = tmpOrder,
                                    Token = Access_Token,
                                    NowMileage = Convert.ToSingle(mil),
                                    StopTime = (string.IsNullOrWhiteSpace(apiInput.ED)) ? "" : apiInput.ED,
                                    Insurance = apiInput.Insurance
                                };
                                SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                                SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<Domain.SP.Input.Rent.SPInput_BookingStart, SPOutput_Base>(connetStr);
                                flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingStartName, SPBookingStartInput, ref SPBookingStartOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, ref SPBookingStartOutput, ref lstError, ref errCode);
                            }
                            if (flag)
                            {
                                string BookingControlName = new ObjType().GetSPName(ObjType.SPType.BookingControl);
                                SPInput_BookingControl SPBookingControlInput = new SPInput_BookingControl()
                                {
                                    IDNO = IDNO,
                                    OrderNo = tmpOrder,
                                    Token = Access_Token,
                                    LogID = LogID
                                };
                                SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                                SQLHelper<SPInput_BookingControl, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BookingControl, SPOutput_Base>(connetStr);
                                flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingControlName, SPBookingControlInput, ref SPBookingStartOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, ref SPBookingStartOutput, ref lstError, ref errCode);
                            }
                            //設定租約狀態
                            if (flag)
                            {
                                if (info.extDeviceStatus1 == 0) //無租約才要送設約租
                                {
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.SetVehicleRent);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.SetVehicleRent;
                                    WSInput_Base<Params> SetRentInput = new WSInput_Base<Params>()
                                    {
                                        command = true,
                                        method = CommandType,
                                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        _params = new Params()
                                    };

                                    requestId = SetRentInput.requestId;
                                    method = CommandType;
                                    flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, SetRentInput, LogID);
                                    if (flag)
                                    {
                                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                    }
                                }
                            }
                            //解防盜
                            if (flag)
                            {
                                if (info.SecurityStatus == 1) //有開防盜才要解
                                {
                                    CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.AlertOff);
                                    CmdType = OtherService.Enum.MachineCommandType.CommandType.AlertOff;
                                    WSInput_Base<Params> SetAlertOffInput = new WSInput_Base<Params>()
                                    {
                                        command = true,
                                        method = CommandType,
                                        requestId = string.Format("{0}_{1}", CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                        _params = new Params()
                                    };

                                    requestId = SetAlertOffInput.requestId;
                                    method = CommandType;
                                    flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, SetAlertOffInput, LogID);
                                    if (flag)
                                    {
                                        flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
                //送短租, 先pendding
                if (flag)
                {
                    //預約的資料，似乎走排程比較好
                    //由另外的JOB來呼叫執行，在BookingStart存檔那邊去處理狀態
                }
                #endregion
            }
            #region 寫取車照片到azure
            if (flag)
            {
                OtherRepository otherRepository = new OtherRepository(connetStr);
                List<CarPIC> lstCarPIC = otherRepository.GetCarPIC(tmpOrder, 0);
                int PICLen = lstCarPIC.Count;
                for (int i = 0; i < PICLen; i++)
                {
                    try
                    {
                        string FileName = string.Format("{0}_{1}_{2}.png", apiInput.OrderNo, (lstCarPIC[i].ImageType == 5) ? "Sign" : "PIC" + lstCarPIC[i].ImageType.ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"));

                        flag = new AzureStorageHandle().UploadFileToAzureStorage(lstCarPIC[i].Image, FileName, "carpic");
                        if (flag)
                        {
                            bool DelFlag = otherRepository.HandleTempCarPIC(tmpOrder, 0, lstCarPIC[i].ImageType, FileName); //更新為azure的檔名
                        }
                    }
                    catch (Exception ex)
                    {
                        flag = true; //先bypass，之後補傳再刪
                    }
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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