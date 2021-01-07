using Domain.CarMachine;
using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Mochi;
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】強取強還
    /// </summary>
    public class BE_ContactSettingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】強取強還
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_ContactSetting(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_ContactSettingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_ContactSetting apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            string Contentjson = "";
            Int64 tmpOrder = 0;
            bool clearFlag = false;
            DateTime SD = DateTime.Now, ReturnDate = DateTime.Now;
            List<BE_CarScheduleTimeLog> lstOrder = null;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_ContactSetting>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.OrderNo };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    if (apiInput.type < 0 || apiInput.type > 2)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }

                if (flag)
                {
                    if (apiInput.Mode < 0 || apiInput.Mode > 2)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }

                if (flag)
                {
                    if (apiInput.type == 1 && !string.IsNullOrEmpty(apiInput.returnDate))
                    {
                        if (string.IsNullOrEmpty(apiInput.returnDate))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        else
                        {
                            flag = DateTime.TryParse(apiInput.returnDate, out ReturnDate);
                            if (flag == false)
                            {
                                errCode = "ERR900";
                            }
                        }
                    }
                }
                if (flag)
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
            #endregion

            #region TB
            if (flag)
            {
                if (apiInput.Mode == 0)
                {
                    #region 取出目前訂單狀態
                    StationAndCarRepository _repository = new StationAndCarRepository(connetStr);

                    lstOrder = _repository.GetOrderStatus(tmpOrder);
                    if (lstOrder == null)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else
                    {
                        if (lstOrder.Count <= 0)
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                    }
                    #endregion
                    if (flag)
                    {
                        if (lstOrder[0].cancel_status > 0)
                        {
                            flag = false;
                            errCode = "ERR734";
                        }
                    }
                    if (flag)
                    {
                        IDNO = lstOrder[0].IDNO;
                        if (apiInput.type == 2) //取消
                        {
                            if (lstOrder[0].car_mgt_status > 0)
                            {
                                flag = false;
                                errCode = "ERR735";
                            }
                            else
                            {
                                string spName = new ObjType().GetSPName(ObjType.SPType.BE_BookingCancel);
                                SPInput_BE_BookingCancel spInput = new SPInput_BE_BookingCancel()
                                {
                                    LogID = LogID,
                                    OrderNo = tmpOrder,
                                    UserID = apiInput.UserID
                                };
                                SPOutput_Base spOut = new SPOutput_Base();
                                SQLHelper<SPInput_BE_BookingCancel, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_BookingCancel, SPOutput_Base>(connetStr);
                                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                            }
                        }
                        else if (apiInput.type == 0)
                        {
                            //取車
                            if (lstOrder[0].car_mgt_status > 4)
                            {
                                flag = false;
                                errCode = "ERR736";
                            }
                            if (flag)
                            {
                                if (lstOrder[0].SD.AddMinutes(-30) > DateTime.Now)
                                {
                                    flag = false;
                                    errCode = "ERR739";
                                }
                                if (flag)
                                {
                                    if (lstOrder[0].CarNo.Substring(0, 1) == "E")
                                    {
                                        //機車
                                        flag = DoPickMotor(tmpOrder, lstOrder[0].IDNO, LogID, apiInput.UserID, ref errCode, ref errMsg, baseVerify);
                                    }
                                    else
                                    {
                                        //汽車
                                        flag = DoPickCar(tmpOrder, lstOrder[0].IDNO, LogID, apiInput.UserID, ref errCode, ref errMsg, baseVerify);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //還車
                            if (lstOrder[0].car_mgt_status < 4)
                            {
                                flag = false;
                                errCode = "ERR737";
                            }
                            if (flag)
                            {
                                if (lstOrder[0].car_mgt_status == 16)
                                {
                                    flag = false;
                                    errCode = "ERR738";
                                }
                            }
                            #region 強還，先判斷目前是不是相同訂單，要不要下車機cmd
                            if (flag)
                            {
                                #region 發票各類型判斷
                                if (flag)
                                {
                                    switch (apiInput.bill_option)
                                    {
                                        case "4":     // 統編
                                            if (string.IsNullOrWhiteSpace(apiInput.unified_business_no))
                                            {
                                                flag = false;
                                                errCode = "ERR190";
                                            }
                                            else
                                            {
                                                flag = baseVerify.checkUniNum(apiInput.unified_business_no);
                                                if (false == flag)
                                                {
                                                    flag = false;
                                                    errCode = "ERR191";
                                                }
                                            }
                                            break;
                                        case "5":     // 手機條碼
                                            if (string.IsNullOrWhiteSpace(apiInput.CARRIERID))
                                            {
                                                flag = false;
                                                errCode = "ERR193";
                                            }
                                            else
                                            {
                                                flag = new HiEasyRentAPI().CheckEinvBiz(apiInput.CARRIERID, ref errCode);
                                            }
                                            break;
                                        case "6":     // 自然人憑證
                                            if (string.IsNullOrWhiteSpace(apiInput.CARRIERID))
                                            {
                                                flag = false;
                                                errCode = "ERR193";
                                            }
                                            break;
                                    }
                                }
                                #endregion

                                if (apiInput.type == 1)  //還車
                                {
                                    BE_CheckHasOrder tmp = new ContactRepository(this.connetStr).CheckCanClear(tmpOrder.ToString());
                                    if (tmp != null)
                                    {
                                        clearFlag = (tmp.Flag == 0);    //true目前沒有其他訂單，要下車機cmd
                                    }
                                }
                                if (flag)
                                {
                                    if (clearFlag)
                                    {
                                        flag = new CarCommonFunc().BE_CheckReturnCar(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode);
                                    }
                                    // 20201223;不管檢核結果，強還照做
                                    flag = true;
                                    errCode = "000000";
                                }
                                if (flag)
                                {
                                    if (lstOrder[0].car_mgt_status == 15)
                                    {
                                        if (clearFlag)
                                        {
                                            bool CarFlag = new CarCommonFunc().DoBECloseRent(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode);
                                            if (CarFlag == false)
                                            {
                                                //寫入車機錯誤
                                            }
                                            errCode = "000000";
                                        }
                                        //已經付完款，直接更改狀態
                                        SPInput_BE_ContactFinish PayInput = new SPInput_BE_ContactFinish()
                                        {
                                            IDNO = IDNO,
                                            LogID = LogID,
                                            OrderNo = tmpOrder,
                                            UserID = apiInput.UserID,
                                            transaction_no = "",
                                            ReturnDate = ReturnDate,
                                            bill_option = apiInput.bill_option,
                                            NPOBAN = apiInput.NPOBAN,
                                            CARRIERID = apiInput.CARRIERID,
                                            unified_business_no = apiInput.unified_business_no,
                                            ParkingSpace = apiInput.parkingSpace
                                        };
                                        string SPName = new ObjType().GetSPName(ObjType.SPType.BE_ContactFinish);
                                        SPOutput_Base PayOutput = new SPOutput_Base();
                                        SQLHelper<SPInput_BE_ContactFinish, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_BE_ContactFinish, SPOutput_Base>(connetStr);
                                        flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                                        baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                                    }
                                    else
                                    {
                                        //重新計價
                                        flag = DoReCalRent(tmpOrder, IDNO, LogID, apiInput.UserID, apiInput.returnDate, ref errCode);
                                        if (flag)
                                        {
                                            if (clearFlag)
                                            {
                                                bool CarFlag = new CarCommonFunc().DoBECloseRent(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode);
                                                if (CarFlag == false)
                                                {
                                                    //寫入車機錯誤
                                                }
                                                errCode = "000000";
                                            }
                                            //已經付完款，直接更改狀態
                                            SPInput_BE_ContactFinish PayInput = new SPInput_BE_ContactFinish()
                                            {
                                                IDNO = IDNO,
                                                LogID = LogID,
                                                OrderNo = tmpOrder,
                                                UserID = apiInput.UserID,
                                                transaction_no = "",
                                                ReturnDate = ReturnDate,
                                                bill_option = apiInput.bill_option,
                                                NPOBAN = apiInput.NPOBAN,
                                                CARRIERID = apiInput.CARRIERID,
                                                unified_business_no = apiInput.unified_business_no,
                                                ParkingSpace = apiInput.parkingSpace
                                            };
                                            string SPName = new ObjType().GetSPName(ObjType.SPType.BE_ContactFinish);
                                            SPOutput_Base PayOutput = new SPOutput_Base();
                                            SQLHelper<SPInput_BE_ContactFinish, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_BE_ContactFinish, SPOutput_Base>(connetStr);
                                            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                                            baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
        /// <summary>
        /// 取汽車
        /// </summary>
        /// <param name="tmpOrder"></param>
        /// <param name="IDNO"></param>
        /// <param name="LogID"></param>
        /// <param name="UserID"></param>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <param name="baseVerify"></param>
        /// <returns></returns>
        private bool DoPickCar(Int64 tmpOrder, string IDNO, Int64 LogID, string UserID, ref string errCode, ref string errMsg, CommonFunc baseVerify)
        {
            bool flag = true;
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            double mil = 0;
            List<CardList> lstCardList = new List<CardList>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CarInfo info = new CarInfo();
            string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.BE_BeforeBookingStart);
            SPInput_BE_BeforeBookingStart spBeforeStart = new SPInput_BE_BeforeBookingStart()
            {
                OrderNo = tmpOrder,
                IDNO = IDNO,
                LogID = LogID,
                UserID = UserID
            };
            SPOutput_BE_BeforeBookingStart spOut = new SPOutput_BE_BeforeBookingStart();
            SQLHelper<SPInput_BE_BeforeBookingStart, SPOutput_BE_BeforeBookingStart> sqlHelp = new SQLHelper<SPInput_BE_BeforeBookingStart, SPOutput_BE_BeforeBookingStart>(connetStr);
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
                        string BookingStartName = new ObjType().GetSPName(ObjType.SPType.BE_BookingStart);
                        SPInput_BE_BookingStart SPBookingStartInput = new SPInput_BE_BookingStart()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            OrderNo = tmpOrder,
                            UserID = UserID,
                            NowMileage = Convert.ToSingle(mil),
                            StopTime = ""
                        };
                        SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                        SQLHelper<SPInput_BE_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_BookingStart, SPOutput_Base>(connetStr);
                        flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingStartName, SPBookingStartInput, ref SPBookingStartOutput, ref lstError);
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
                            string BookingStartName = new ObjType().GetSPName(ObjType.SPType.BE_BookingStart);
                            SPInput_BE_BookingStart SPBookingStartInput = new SPInput_BE_BookingStart()
                            {
                                IDNO = IDNO,
                                LogID = LogID,
                                OrderNo = tmpOrder,
                                UserID = UserID,
                                NowMileage = Convert.ToSingle(mil),
                                StopTime = ""
                            };
                            SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                            SQLHelper<SPInput_BE_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_BookingStart, SPOutput_Base>(connetStr);
                            flag = SQLBookingStartHelp.ExecuteSPNonQuery(BookingStartName, SPBookingStartInput, ref SPBookingStartOutput, ref lstError);
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
            return flag;
        }
        private bool DoPickMotor(Int64 tmpOrder, string IDNO, Int64 LogID, string UserID, ref string errCode, ref string errMsg, CommonFunc baseVerify)
        {
            bool flag = true;
            string CID = "";
            string deviceToken = "";
            int IsMotor = 0;
            int IsCens = 0;
            double mil = 0;
            List<CardList> lstCardList = new List<CardList>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MotorInfo info = new MotorInfo();
            string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.BE_BeforeBookingStart);
            SPInput_BE_BeforeBookingStart spBeforeStart = new SPInput_BE_BeforeBookingStart()
            {
                OrderNo = tmpOrder,
                IDNO = IDNO,
                LogID = LogID,
                UserID = UserID
            };
            SPOutput_BE_BeforeBookingStart spOut = new SPOutput_BE_BeforeBookingStart();
            SQLHelper<SPInput_BE_BeforeBookingStart, SPOutput_BE_BeforeBookingStart> sqlHelp = new SQLHelper<SPInput_BE_BeforeBookingStart, SPOutput_BE_BeforeBookingStart>(connetStr);
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
                string BookingStartName = new ObjType().GetSPName(ObjType.SPType.BE_BookingStart);
                SPInput_BE_BookingStart SPBookingStartInput = new SPInput_BE_BookingStart()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    OrderNo = tmpOrder,
                    UserID = UserID,
                    NowMileage = Convert.ToSingle(mil),
                    StopTime = ""
                };
                SPOutput_Base SPBookingStartOutput = new SPOutput_Base();
                SQLHelper<SPInput_BE_BookingStart, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_BookingStart, SPOutput_Base>(connetStr);
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
                    }
                }
                #endregion
            }
            return flag;
        }

        private bool DoReCalRent(Int64 tmpOrder, string IDNO, Int64 LogID, string UserID, string returnDate, ref string errCode)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            bool flag = true;
            float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

            List<Holiday> lstHoliday = null; //假日列表
            List<OrderQueryFullData> OrderDataLists = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            int ProjType = 0;
            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數

            int Discount = 0; //要折抵的點數
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime FineDate = new DateTime();
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;

            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int TotalFineInsuranceMinutes = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
            int days = 0; int hours = 0; int mins = 0; //以分計費總時數
            int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
            int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int CarRentPrice = 0; //車輛租金
            int MonthlyPoint = 0;   //月租折抵點數        20201128 ADD BY ADAM 
            int MonthlyPrice = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
            int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            bool UseMonthMode = false;
            int InsurancePerHours = 0;  //安心服務每小時價
            int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
            CarRentInfo carInfo = new CarRentInfo();//汽車資料
            int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM

            double nor_car_wDisc = 0;//只有一般時段時平日折扣
            double nor_car_hDisc = 0;//只有一般時段時價日折扣
            int nor_car_PayDisc = 0;//只有一般時段時總折扣
            int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

            int gift_point = 0;//使用時數(汽車)
            int gift_motor_point = 0;//使用時數(機車)
            int motoBaseMins = 6;//機車基本分鐘數
            int carBaseMins = 60;//汽車基本分鐘數

            #endregion

            #region 取出訂單資訊
            if (flag)
            {
                SPInput_BE_GetOrderStatusByOrderNo spInput = new SPInput_BE_GetOrderStatusByOrderNo()
                {
                    IDNO = IDNO,
                    OrderNo = tmpOrder,
                    LogID = LogID,
                    UserID = UserID
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
                SPOutput_Base spOutBase = new SPOutput_Base();
                SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                OrderDataLists = new List<OrderQueryFullData>();
                DataSet ds = new DataSet();
                flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                //判斷訂單狀態
                if (flag)
                {
                    if (OrderDataLists.Count == 0)
                    {
                        flag = false;
                        errCode = "ERR203";
                    }
                }
            }
            #endregion

            if (OrderDataLists != null && OrderDataLists.Count() > 0)
                motoBaseMins = OrderDataLists[0].BaseMinutes > 0 ? OrderDataLists[0].BaseMinutes : motoBaseMins;

            //取得專案狀態
            if (flag)
            {
                ProjType = OrderDataLists[0].ProjType;
                SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                SD = SD.AddSeconds(SD.Second * -1); //去秒數
                //機車路邊不計算預計還車時間
                if (OrderDataLists[0].ProjType == 4)
                {
                    ED = Convert.ToDateTime(returnDate);
                    ED = ED.AddSeconds(ED.Second * -1); //去秒數
                }
                else
                {
                    ED = Convert.ToDateTime(returnDate);
                    ED = ED.AddSeconds(ED.Second * -1); //去秒數
                }
                FED = Convert.ToDateTime(returnDate);
                FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                if (FED < SD)
                {
                    flag = false;
                    errCode = "ERR740";
                }
                if (flag)
                {
                    if (FED.Subtract(ED).Ticks > 0)
                    {
                        FineDate = ED;
                        hasFine = true;                    
                    }
                }
            }

            #region 汽車計費資訊 
            //note:汽車計費資訊PayDetail
            int car_payAllMins = 0; //全部計費租用分鐘
            int car_payInMins = 0;//未超時計費分鐘
            int car_payOutMins = 0;//超時分鐘-顯示用
            double car_pay_in_wMins = 0;//未超時平日計費分鐘
            double car_pay_in_hMins = 0;//未超時假日計費分鐘
            double car_pay_out_wMins = 0;//超時平日計費分鐘
            double car_pay_out_hMins = 0;//超時假日計費分鐘
            int car_inPrice = 0;//未超時費用
            int car_outPrice = 0;//超時費用
            int car_n_price = OrderDataLists[0].PRICE;
            int car_h_price = OrderDataLists[0].PRICE_H;

            if (flag)
            {
                if (ProjType != 4)
                {
                    var input = new IBIZ_CRNoMonth()
                    {
                        car_n_price = car_n_price,
                        car_h_price = car_h_price,
                        WeekdayPrice = OrderDataLists[0].WeekdayPrice,
                        HoildayPrice = OrderDataLists[0].HoildayPrice,
                        SD = SD,
                        ED = ED,
                        FED = FED,
                        hasFine = hasFine,
                        carBaseMins = carBaseMins,
                        lstHoliday = lstHoliday
                    };
                    var car_re = cr_com.CRNoMonth(input);
                    if (car_re != null)
                    {
                        flag = car_re.flag;
                        car_payAllMins = car_re.car_payAllMins;
                        car_payInMins = car_re.car_payInMins;
                        car_payOutMins = car_re.car_payOutMins;
                        car_pay_in_wMins = car_re.car_pay_in_wMins;
                        car_pay_in_hMins = car_re.car_pay_in_hMins;
                        car_inPrice = car_re.car_inPrice;
                        car_outPrice = car_re.car_outPrice;
                    }
                }
            }

            #endregion

            #region 與短租查時數
            if (flag)
            {
                var inp = new IBIZ_NPR270Query()
                {
                    IDNO = IDNO
                };
                var re270 = cr_com.NPR270Query(inp);
                if (re270 != null)
                {
                    flag = re270.flag;
                    MotorPoint = re270.MotorPoint;
                    CarPoint = re270.CarPoint;
                }

                //判斷輸入的點數有沒有超過總點數
                if (ProjType == 4)
                {
                    if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
                    {
                        //flag = false;
                        //errCode = "ERR205";
                    }
                    else
                    {
                        if (Discount > (MotorPoint + CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
                        {
                            flag = false;
                            errCode = "ERR207";
                        }
                    }

                    if (TotalRentMinutes <= 6 && Discount == 6)
                    {

                    }
                    else if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
                    {
                        flag = false;
                        errCode = "ERR303";
                    }
                }
                else
                {
                    if (Discount > 0 && Discount % 30 > 0)
                    {
                        flag = false;
                        errCode = "ERR206";
                    }
                    else
                    {
                        if (Discount > CarPoint)
                        {
                            flag = false;
                            errCode = "ERR207";
                        }
                    }
                }

            }
            #endregion
            #region 查ETAG 20201202 ADD BY ADAM
            if (flag && OrderDataLists[0].ProjType != 4)    //汽車才需要進來
            {
                var orderNo = "H" + tmpOrder.ToString().PadLeft(7, '0');

                WebAPIOutput_ETAG010 wsOutput = new WebAPIOutput_ETAG010();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                //ETAG查詢失敗也不影響流程
                var ordNo = "H" + tmpOrder.ToString().PadLeft(7, '0');
                var input = new IBIZ_ETagCk()
                {
                    OrderNo = ordNo
                };
                var etag_re = cr_com.ETagCk(input);
                if (etag_re != null)
                {
                    flag = etag_re.flag;
                    errCode = etag_re.errCode;
                    etagPrice = etag_re.etagPrice;
                }
            }
            #endregion

            #region 建空模及塞入要輸出的值
            if (flag)
            {
                outputApi.CanUseDiscount = 1;   //先暫時寫死，之後改專案設定，由專案設定引入
                outputApi.CanUseMonthRent = 1;  //先暫時寫死，之後改專案設定，由專案設定引入
                outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
                outputApi.DiscountAlertMsg = "";
                outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
                outputApi.IsMotor = (ProjType == 4) ? 1 : 0;    //是否為機車
                outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase();  //月租資訊
                outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();  //機車資訊
                outputApi.PayMode = (ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
                outputApi.ProType = ProjType;
                outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() //訂單基本資訊
                {
                    BookingEndDate = ED.ToString("yyyy-MM-dd HH:mm:ss"),
                    BookingStartDate = SD.ToString("yyyy-MM-dd HH:mm:ss"),
                    CarNo = OrderDataLists[0].CarNo,
                    RedeemingTimeCarInterval = "0",
                    RedeemingTimeMotorInterval = "0",
                    RedeemingTimeInterval = "0",
                    RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
                    RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
                };

                if (ProjType == 4)
                {
                    TotalPoint = (CarPoint + MotorPoint);
                    outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
                    {
                        BaseMinutePrice = OrderDataLists[0].BaseMinutesPrice,
                        BaseMinutes = OrderDataLists[0].BaseMinutes,
                        MinuteOfPrice = OrderDataLists[0].MinuteOfPrice
                    };
                }
                else
                {
                    TotalPoint = CarPoint;
                    outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
                    {
                        HoildayOfHourPrice = OrderDataLists[0].PRICE_H,
                        HourOfOneDay = 10,
                        WorkdayOfHourPrice = OrderDataLists[0].PRICE,
                        WorkdayPrice = OrderDataLists[0].PRICE * 10,
                        MilUnit = OrderDataLists[0].MilageUnit,
                        HoildayPrice = OrderDataLists[0].PRICE_H * 10
                    };
                }
                //20201201 ADD BY ADAM REASON.轉乘優惠
                TransferPrice = OrderDataLists[0].init_TransDiscount;
            }

            if (flag && OrderDataLists[0].ProjType != 4 && false)//20201224 add by adam 問題未確定前先關掉車麻吉
            {
                //檢查有無車麻吉停車費用
                var input = new IBIZ_CarMagi()
                {
                    LogID = LogID,
                    CarNo = OrderDataLists[0].CarNo,
                    SD = SD,
                    ED = FED.AddDays(1)
                };
                var magi_Re = cr_com.CarMagi(input);
                if (magi_Re != null)
                {
                    flag = magi_Re.flag;
                    outputApi.Rent.ParkingFee = magi_Re.ParkingFee;
                }
            }
            #endregion
            #region 月租
            //note: 月租GetPayDetail
            if (flag)
            {
                var item = OrderDataLists[0];
                var motoDayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                var input = new IBIZ_MonthRent()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    intOrderNO = tmpOrder,
                    ProjType = item.ProjType,
                    MotoDayMaxMins = motoDayMaxMinns,
                    MinuteOfPrice = item.MinuteOfPrice,
                    hasFine = hasFine,
                    SD = SD,
                    ED = ED,
                    FED = FED,
                    MotoBaseMins = motoBaseMins,
                    lstHoliday = lstHoliday,
                    Discount = Discount,
                    PRICE = item.PRICE,
                    PRICE_H = item.PRICE_H,
                    carBaseMins = 60
                };
                var mon_re = cr_com.MonthRentSave(input);
                if (mon_re != null)
                {
                    flag = mon_re.flag;
                    UseMonthMode = mon_re.UseMonthMode;
                    outputApi.IsMonthRent = mon_re.IsMonthRent;
                    carInfo = mon_re.carInfo;
                    Discount = mon_re.useDisc;                    
                    monthlyRentDatas = mon_re.monthlyRentDatas;

                    if(ProjType == 4)
                      outputApi.Rent.CarRental = mon_re.CarRental;//機車用
                    else
                      CarRentPrice += mon_re.CarRental;//汽車用
                }
            }
            #endregion
            #region 開始計價
            if (flag)
            {
                lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                if (ProjType == 4)
                {
                    if (UseMonthMode)   //true:有月租;false:無月租
                    {
                        outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                        outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                    }
                    else
                    {
                        var item = OrderDataLists[0];
                        var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                        carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, null, null, 0);
                        if (carInfo != null)
                            outputApi.Rent.CarRental = carInfo.RentInPay;
                    }

                    outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                }
                else
                {
                    #region 非月租折扣計算
                    var input = new IBIZ_CRNoMonthDisc()
                    {
                        UseMonthMode = UseMonthMode,
                        hasFine = hasFine,
                        SD = SD,
                        ED = ED,
                        FED = FED,
                        CarBaseMins = carBaseMins,
                        lstHoliday = lstHoliday,
                        car_n_price = car_n_price,
                        car_h_price = car_h_price
                    };
                    var disc_re = cr_com.CRNoMonthDisc(input);
                    if (disc_re != null)
                    {
                        nor_car_PayDisc = disc_re.nor_car_PayDisc;
                        nor_car_wDisc = disc_re.nor_car_wDisc;
                        nor_car_hDisc = disc_re.nor_car_hDisc;
                        nor_car_PayDiscPrice = disc_re.nor_car_PayDiscPrice;
                        Discount = disc_re.UseDisc;
                    }
                    #endregion

                    if (UseMonthMode)
                    {
                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
                        outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
                    }
                    else
                    {
                        CarRentPrice = car_inPrice;//未逾時租用費用
                        if (hasFine)
                            outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                    }

                    if (Discount > 0)
                    {
                        double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
                        double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

                        if (UseMonthMode)
                        {

                        }
                        else
                        {
                            //非月租折扣
                            CarRentPrice -= nor_car_PayDiscPrice;
                            CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
                        }
                    }
                    //安心服務
                    InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
                    if (InsurancePerHours > 0)
                    {
                        outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((car_payInMins / 30.0) * InsurancePerHours / 2)));

                        //逾時安心服務計算
                        if (TotalFineRentMinutes > 0)
                        {
                            outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((car_payOutMins / 30.0) * InsurancePerHours / 2)));
                        }
                    }

                    outputApi.Rent.CarRental = CarRentPrice;
                    outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                    //outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                    //里程費計算修改，遇到取不到里程數的先以0元為主
                    //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                    // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                    if (OrderDataLists[0].start_mile == 0 ||
                        OrderDataLists[0].end_mile == 0 ||
                        ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) > 1000) ||
                        ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) < 0)
                        )
                    {
                        outputApi.Rent.MileageRent = 0;
                    }
                    else
                    {
                        outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                    }
                }

                outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                //outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes).ToString();
                outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                //20201202 ADD BY ADAM REASON.ETAG費用
                outputApi.Rent.ETAGRental = etagPrice;

                var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
                xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                outputApi.Rent.TotalRental = xTotalRental;

                #region 修正輸出欄位
                //note: 修正輸出欄位PayDetail
                if (ProjType == 4)
                {
                    outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                    outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                    outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

                    //2020-12-29 所有點數改成皆可折抵
                    //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                    outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

                    outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

                    var cDisc = 0;
                    var mDisc = 0;
                    if (carInfo.useDisc > 0)
                    {
                        int lastDisc = carInfo.useDisc;
                        var useMdisc = mDisc > carInfo.useDisc ? carInfo.useDisc : mDisc;
                        lastDisc -= useMdisc;
                        gift_motor_point = useMdisc;
                        if (lastDisc > 0)
                        {
                            var useCdisc = cDisc > lastDisc ? lastDisc : cDisc;
                            lastDisc -= useCdisc;
                            gift_point = useCdisc;
                        }
                    }                
                }
                else
                {
                    if (UseMonthMode)
                    {
                        outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                        outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                        outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

                        //2020 - 12 - 29 所有點數改成皆可折抵
                        //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                        outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

                        outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
                        if (carInfo != null && carInfo.useDisc > 0)
                            gift_point = carInfo.useDisc;
                    }
                    else
                    {
                        outputApi.Rent.UseNorTimeInterval = Discount.ToString();
                        outputApi.Rent.RentalTimeInterval = car_payInMins.ToString(); //租用時數(未逾時)
                        outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_pay_in_wMins + car_pay_in_hMins).ToString();//可折抵租用時數
                        outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
                        gift_point = nor_car_PayDisc;
                    }

                    gift_motor_point = 0;
                    outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                }

                #endregion

                string SPName = new ObjType().GetSPName(ObjType.SPType.BE_CalFinalPrice);
                SPInput_BE_CalFinalPrice SPInput = new SPInput_BE_CalFinalPrice()
                {
                    IDNO = IDNO,
                    OrderNo = tmpOrder,
                    final_price = outputApi.Rent.TotalRental,
                    pure_price = outputApi.Rent.CarRental,
                    mileage_price = outputApi.Rent.MileageRent,
                    Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                    fine_price = outputApi.Rent.OvertimeRental,
                    gift_point = gift_point,
                    //gift_motor_point = gift_motor_point,

                    Etag = outputApi.Rent.ETAGRental,
                    parkingFee = outputApi.Rent.ParkingFee,
                    TransDiscount = outputApi.Rent.TransferPrice,
                    LogID = LogID,
                    UserID = UserID
                };
                SPOutput_Base SPOutput = new SPOutput_Base();
                SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base>(connetStr);
                flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
            }
            #endregion
            return flag;
        }

        #region mark-DoReCalRent
        //private bool DoReCalRent(Int64 tmpOrder, string IDNO, Int64 LogID, string UserID, string returnDate, ref string errCode)
        //{
        //    #region 初始宣告
        //    bool flag = true;
        //    float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());


        //    List<Holiday> lstHoliday = null; //假日列表
        //    List<OrderQueryFullData> OrderDataLists = null;
        //    CommonFunc baseVerify = new CommonFunc();
        //    List<ErrorInfo> lstError = new List<ErrorInfo>();
        //    OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
        //    int ProjType = 0;
        //    int TotalPoint = 0; //總點數
        //    int MotorPoint = 0; //機車點數
        //    int CarPoint = 0;   //汽車點數

        //    int Discount = 0; //要折抵的點數
        //    DateTime SD = new DateTime();
        //    DateTime ED = new DateTime();
        //    DateTime FED = new DateTime();
        //    DateTime FineDate = new DateTime();
        //    bool hasFine = false; //是否逾時
        //    DateTime NowTime = DateTime.Now;

        //    int TotalRentMinutes = 0; //總租車時數
        //    int TotalFineRentMinutes = 0; //總逾時時數
        //    int TotalFineInsuranceMinutes = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
        //    int days = 0; int hours = 0; int mins = 0; //以分計費總時數
        //    int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
        //    int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
        //    int ActualRedeemableTimePoint = 0; //實際可抵折點數
        //    int CarRentPrice = 0; //車輛租金
        //    int MonthlyPoint = 0;   //月租折抵點數        20201128 ADD BY ADAM 
        //    int MonthlyPrice = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
        //    int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
        //    MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
        //    BillCommon billCommon = new BillCommon();
        //    List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
        //    bool UseMonthMode = false;
        //    int InsurancePerHours = 0;  //安心服務每小時價
        //    int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
        //    CarRentInfo carInfo = new CarRentInfo();//汽車資料
        //    int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM

        //    double nor_car_wDisc = 0;//只有一般時段時平日折扣
        //    double nor_car_hDisc = 0;//只有一般時段時價日折扣
        //    int nor_car_PayDisc = 0;//只有一般時段時總折扣
        //    int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

        //    int gift_point = 0;//使用時數(汽車)
        //    int gift_motor_point = 0;//使用時數(機車)
        //    int motoBaseMins = 6;//機車基本分鐘數
        //    int carBaseMins = 60;//汽車基本分鐘數

        //    #endregion

        //    #region 取出訂單資訊
        //    if (flag)
        //    {
        //        SPInput_BE_GetOrderStatusByOrderNo spInput = new SPInput_BE_GetOrderStatusByOrderNo()
        //        {
        //            IDNO = IDNO,
        //            OrderNo = tmpOrder,
        //            LogID = LogID,
        //            UserID = UserID
        //        };
        //        string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
        //        SPOutput_Base spOutBase = new SPOutput_Base();
        //        SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
        //        OrderDataLists = new List<OrderQueryFullData>();
        //        DataSet ds = new DataSet();
        //        flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
        //        baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
        //        //判斷訂單狀態
        //        if (flag)
        //        {
        //            if (OrderDataLists.Count == 0)
        //            {
        //                flag = false;
        //                errCode = "ERR203";
        //            }
        //        }
        //    }
        //    #endregion

        //    if (OrderDataLists != null && OrderDataLists.Count() > 0)
        //        motoBaseMins = OrderDataLists[0].BaseMinutes > 0 ? OrderDataLists[0].BaseMinutes : motoBaseMins;

        //    //取得專案狀態
        //    if (flag)
        //    {
        //        ProjType = OrderDataLists[0].ProjType;
        //        SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
        //        SD = SD.AddSeconds(SD.Second * -1); //去秒數
        //        //機車路邊不計算預計還車時間
        //        if (OrderDataLists[0].ProjType == 4)
        //        {
        //            //ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time==""? returnDate: OrderDataLists[0].final_stop_time);
        //            ED = Convert.ToDateTime(returnDate);
        //            ED = ED.AddSeconds(ED.Second * -1); //去秒數
        //        }
        //        else
        //        {
        //            //ED = Convert.ToDateTime(OrderDataLists[0].stop_time==""? returnDate: OrderDataLists[0].stop_time);
        //            ED = Convert.ToDateTime(returnDate);
        //            ED = ED.AddSeconds(ED.Second * -1); //去秒數
        //        }
        //        FED = Convert.ToDateTime(returnDate);
        //        FED = FED.AddSeconds(FED.Second * -1);  //去秒數
        //        lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
        //        if (FED < SD)
        //        {
        //            flag = false;
        //            errCode = "ERR740";
        //        }
        //        if (flag)
        //        {
        //            if (FED.Subtract(ED).Ticks > 0)
        //            {
        //                FineDate = ED;
        //                hasFine = true;
        //                billCommon.CalDayHourMin(SD, ED, ref days, ref hours, ref mins); //未逾時的總時數
        //                TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
        //                billCommon.CalDayHourMin(ED, FED, ref FineDays, ref FineHours, ref FineMins);
        //                TotalFineRentMinutes = ((FineDays * 10) + FineHours) * 60 + FineMins; //逾時的總時數
        //                TotalFineInsuranceMinutes = ((FineDays * 6) + FineHours) * 60 + FineMins;  //逾時的安心服務總計(一日上限6小時)
        //            }
        //            else
        //            {
        //                billCommon.CalDayHourMin(SD, FED, ref days, ref hours, ref mins); //未逾時的總時數
        //                TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
        //            }
        //        }

        //    }
        //    //if (flag)
        //    //{
        //    //    if (NowTime.Subtract(FED).TotalMinutes >= 30)
        //    //    {
        //    //        flag = false;
        //    //        errCode = "ERR208";
        //    //    }
        //    //}

        //    #region 汽車計費資訊 
        //    //note:汽車計費資訊PayDetail
        //    int car_payAllMins = 0; //全部計費租用分鐘
        //    int car_payInMins = 0;//未超時計費分鐘
        //    int car_payOutMins = 0;//超時分鐘-顯示用

        //    double car_pay_in_wMins = 0;//未超時平日計費分鐘
        //    double car_pay_in_hMins = 0;//未超時假日計費分鐘
        //    double car_pay_out_wMins = 0;//超時平日計費分鐘
        //    double car_pay_out_hMins = 0;//超時假日計費分鐘

        //    int car_inPrice = 0;//未超時費用
        //    int car_outPrice = 0;//超時費用

        //    int car_n_price = OrderDataLists[0].PRICE;
        //    int car_h_price = OrderDataLists[0].PRICE_H;

        //    if (flag)
        //    {
        //        if (ProjType == 4)
        //        {

        //        }
        //        else
        //        {
        //            if (hasFine)
        //            {
        //                var reInMins = billCommon.GetCarRangeMins(SD, ED, carBaseMins, 600, lstHoliday);
        //                if (reInMins != null)
        //                {
        //                    car_payInMins = Convert.ToInt32(reInMins.Item1 + reInMins.Item2);
        //                    car_payAllMins += car_payInMins;
        //                    car_pay_in_wMins = reInMins.Item1;
        //                    car_pay_in_hMins = reInMins.Item2;
        //                }

        //                var reOutMins = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
        //                if (reOutMins != null)
        //                {
        //                    car_payOutMins = Convert.ToInt32(reOutMins.Item1 + reOutMins.Item2);
        //                    car_payAllMins += car_payOutMins;
        //                    car_pay_out_wMins = reOutMins.Item1;
        //                    car_pay_out_hMins = reOutMins.Item2;
        //                }

        //                car_inPrice = billCommon.CarRentCompute(SD, ED, car_n_price * 10, car_h_price * 10, 10, lstHoliday);
        //                car_outPrice = billCommon.CarRentCompute(ED, FED, OrderDataLists[0].WeekdayPrice, OrderDataLists[0].HoildayPrice, 6, lstHoliday, true, 0);
        //            }
        //            else
        //            {
        //                var reAllMins = billCommon.GetCarRangeMins(SD, FED, carBaseMins, 600, lstHoliday);
        //                if (reAllMins != null)
        //                {
        //                    car_payAllMins = Convert.ToInt32(reAllMins.Item1 + reAllMins.Item2);
        //                    car_payInMins = car_payAllMins;
        //                    car_pay_in_wMins = reAllMins.Item1;
        //                    car_pay_in_hMins = reAllMins.Item2;
        //                }

        //                car_inPrice = billCommon.CarRentCompute(SD, FED, car_n_price * 10, car_h_price * 10, 10, lstHoliday);
        //            }
        //        }
        //    }
        //    #endregion

        //    #region 與短租查時數
        //    if (flag)
        //    {
        //        WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
        //        HiEasyRentAPI wsAPI = new HiEasyRentAPI();
        //        flag = wsAPI.NPR270Query(IDNO, ref wsOutput);
        //        if (flag)
        //        {
        //            int giftLen = wsOutput.Data.Length;

        //            if (giftLen > 0)
        //            {
        //                for (int i = 0; i < giftLen; i++)
        //                {
        //                    DateTime tmpDate;
        //                    int tmpPoint = 0;
        //                    bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
        //                    bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
        //                    if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
        //                    {
        //                        if (wsOutput.Data[i].GIFTTYPE == "01")  //汽車
        //                        {
        //                            CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
        //                        }
        //                        else
        //                        {
        //                            MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            flag = true;
        //            errCode = "0000";
        //        }
        //        //判斷輸入的點數有沒有超過總點數
        //        if (ProjType == 4)
        //        {
        //            if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
        //            {
        //                //flag = false;
        //                //errCode = "ERR205";
        //            }
        //            else
        //            {
        //                if (Discount > (MotorPoint + CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
        //                {
        //                    flag = false;
        //                    errCode = "ERR207";
        //                }
        //            }

        //            if (TotalRentMinutes <= 6 && Discount == 6)
        //            {

        //            }
        //            else if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
        //            {
        //                flag = false;
        //                errCode = "ERR303";
        //            }

        //            if (flag)
        //            {
        //                billCommon.CalPointerToDayHourMin(MotorPoint + CarPoint, ref PDays, ref PHours, ref PMins);
        //            }
        //        }
        //        else
        //        {
        //            if (Discount > 0 && Discount % 30 > 0)
        //            {
        //                flag = false;
        //                errCode = "ERR206";
        //            }
        //            else
        //            {
        //                if (Discount > CarPoint)
        //                {
        //                    flag = false;
        //                    errCode = "ERR207";
        //                }
        //            }
        //            if (flag)
        //            {
        //                billCommon.CalPointerToDayHourMin(CarPoint, ref PDays, ref PHours, ref PMins);
        //            }
        //        }

        //    }
        //    #endregion
        //    #region 查ETAG 20201202 ADD BY ADAM
        //    if (flag && OrderDataLists[0].ProjType != 4)    //汽車才需要進來
        //    {
        //        WebAPIOutput_ETAG010 wsOutput = new WebAPIOutput_ETAG010();
        //        HiEasyRentAPI wsAPI = new HiEasyRentAPI();
        //        //ETAG查詢失敗也不影響流程
        //        var orderNo = "H" + tmpOrder.ToString().PadLeft(7, '0');
        //        flag = wsAPI.ETAG010Send(orderNo, "", ref wsOutput);
        //        if (flag)
        //        {
        //            if (wsOutput.RtnCode == "0")
        //            {
        //                //取出ETAG費用
        //                if (wsOutput.Data.Length > 0)
        //                {
        //                    etagPrice = wsOutput.Data[0].TAMT == "" ? 0 : int.Parse(wsOutput.Data[0].TAMT);
        //                }
        //            }
        //        }
        //    }
        //    #endregion


        //    #region 建空模及塞入要輸出的值
        //    if (flag)
        //    {
        //        outputApi.CanUseDiscount = 1;   //先暫時寫死，之後改專案設定，由專案設定引入
        //        outputApi.CanUseMonthRent = 1;  //先暫時寫死，之後改專案設定，由專案設定引入
        //        outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
        //        outputApi.DiscountAlertMsg = "";
        //        outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
        //        outputApi.IsMotor = (ProjType == 4) ? 1 : 0;    //是否為機車
        //        outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase();  //月租資訊
        //        outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();  //機車資訊
        //        outputApi.PayMode = (ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
        //        outputApi.ProType = ProjType;
        //        outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() //訂單基本資訊
        //        {
        //            BookingEndDate = ED.ToString("yyyy-MM-dd HH:mm:ss"),
        //            BookingStartDate = SD.ToString("yyyy-MM-dd HH:mm:ss"),
        //            CarNo = OrderDataLists[0].CarNo,
        //            RedeemingTimeCarInterval = "0",
        //            RedeemingTimeMotorInterval = "0",
        //            RedeemingTimeInterval = "0",
        //            RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
        //            RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
        //        };

        //        if (ProjType == 4)
        //        {
        //            TotalPoint = (CarPoint + MotorPoint);
        //            outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
        //            {
        //                BaseMinutePrice = OrderDataLists[0].BaseMinutesPrice,
        //                BaseMinutes = OrderDataLists[0].BaseMinutes,
        //                MinuteOfPrice = OrderDataLists[0].MinuteOfPrice
        //            };
        //        }
        //        else
        //        {
        //            TotalPoint = CarPoint;
        //            outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
        //            {
        //                HoildayOfHourPrice = OrderDataLists[0].PRICE_H,
        //                HourOfOneDay = 10,
        //                WorkdayOfHourPrice = OrderDataLists[0].PRICE,
        //                WorkdayPrice = OrderDataLists[0].PRICE * 10,
        //                MilUnit = OrderDataLists[0].MilageUnit,
        //                HoildayPrice = OrderDataLists[0].PRICE_H * 10
        //            };
        //        }
        //        //20201201 ADD BY ADAM REASON.轉乘優惠
        //        TransferPrice = OrderDataLists[0].init_TransDiscount;


        //    }

        //    if (flag && OrderDataLists[0].ProjType != 4)
        //    {
        //        //檢查有無車麻吉停車費用
        //        WebAPIOutput_QueryBillByCar mochiOutput = new WebAPIOutput_QueryBillByCar();
        //        MachiComm mochi = new MachiComm();
        //        flag = mochi.GetParkingBill(LogID, OrderDataLists[0].CarNo, SD, FED.AddDays(1), ref ParkingPrice, ref mochiOutput);
        //        if (flag)
        //        {
        //            if (outputApi.Rent != null)
        //            {
        //                outputApi.Rent.ParkingFee = ParkingPrice;
        //            }
        //        }
        //    }
        //    #endregion
        //    #region 月租
        //    //note: 月租GetPayDetail
        //    if (flag)
        //    {
        //        //1.0 先還原這個單號使用的
        //        flag = monthlyRentRepository.RestoreHistory(IDNO, tmpOrder, LogID, ref errCode);
        //        int RateType = (ProjType == 4) ? 1 : 0;
        //        if (!hasFine)
        //        {
        //            monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
        //        }
        //        else
        //        {
        //            monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
        //        }
        //        int MonthlyLen = monthlyRentDatas.Count;
        //        if (MonthlyLen > 0)
        //        {
        //            UseMonthMode = true;
        //            outputApi.IsMonthRent = 1;
        //            if (flag)
        //            {
        //                if (ProjType == 4)
        //                {
        //                    var motoMonth = objUti.Clone(monthlyRentDatas);
        //                    var item = OrderDataLists[0];
        //                    var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

        //                    int motoDisc = Discount;
        //                    carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, lstHoliday, motoMonth, 0);

        //                    if (carInfo != null)
        //                    {
        //                        outputApi.Rent.CarRental += carInfo.RentInPay;
        //                        if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
        //                            motoMonth = carInfo.mFinal;
        //                        Discount = carInfo.useDisc;
        //                    }

        //                    motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
        //                    if (motoMonth.Count > 0)
        //                    {
        //                        UseMonthMode = true;
        //                        int UseLen = motoMonth.Count;
        //                        for (int i = 0; i < UseLen; i++)
        //                        {
        //                            flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), LogID, ref errCode); //寫入記錄
        //                        }
        //                    }
        //                    else
        //                    {
        //                        UseMonthMode = false;
        //                    }
        //                }
        //                else
        //                {
        //                    List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();

        //                    UseMonthlyRent = monthlyRentDatas;

        //                    int xDiscount = Discount;//帶入月租運算的折扣
        //                    if (hasFine)
        //                    {
        //                        //CarRentPrice = billCommon.CalBillBySubScription(SD, ED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
        //                        carInfo = billCommon.CarRentInCompute(SD, ED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, carBaseMins, 10, lstHoliday, UseMonthlyRent, xDiscount);
        //                        if (carInfo != null)
        //                        {
        //                            CarRentPrice += carInfo.RentInPay;
        //                            if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
        //                                UseMonthlyRent = carInfo.mFinal;
        //                            Discount = carInfo.useDisc;
        //                        }
        //                        CarRentPrice += car_outPrice;
        //                    }
        //                    else
        //                    {
        //                        //CarRentPrice = billCommon.CalBillBySubScription(SD, FED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
        //                        carInfo = billCommon.CarRentInCompute(SD, FED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, carBaseMins, 10, lstHoliday, UseMonthlyRent, xDiscount);
        //                        if (carInfo != null)
        //                        {
        //                            CarRentPrice += carInfo.RentInPay;
        //                            if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
        //                                UseMonthlyRent = carInfo.mFinal;
        //                            Discount = carInfo.useDisc;
        //                        }
        //                    }
        //                    if (UseMonthlyRent.Count > 0)
        //                    {
        //                        UseMonthMode = true;
        //                        int UseLen = UseMonthlyRent.Count;
        //                        for (int i = 0; i < UseLen; i++)
        //                        {
        //                            //flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours), Convert.ToInt32(UseMonthlyRent[i].HolidayHours), 0, LogID, ref errCode); //寫入記錄
        //                            flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, LogID, ref errCode); //寫入記錄
        //                        }
        //                    }
        //                    else
        //                    {
        //                        UseMonthMode = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //    #region 開始計價  20201129 還沒同步到前端計算租金
        //    if (flag)
        //    {
        //        lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
        //        if (ProjType == 4)
        //        {
        //            if (UseMonthMode)   //true:有月租;false:無月租
        //            {
        //                outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
        //                outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
        //            }
        //            else
        //            {
        //                var item = OrderDataLists[0];
        //                var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

        //                carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, null, null, 0);
        //                if (carInfo != null)
        //                    outputApi.Rent.CarRental = carInfo.RentInPay;
        //            }

        //            outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
        //        }
        //        else
        //        {
        //            int BaseMinutes = 60;
        //            int tmpTotalRentMinutes = TotalRentMinutes;

        //            if (TotalRentMinutes < BaseMinutes)
        //            {
        //                TotalRentMinutes = BaseMinutes;
        //            }
        //            if (UseMonthMode)
        //            {
        //                TotalRentMinutes -= Convert.ToInt32((billCommon._scriptHolidayHour + billCommon._scriptWorkHour) * 60);
        //                if (TotalRentMinutes < 0)
        //                {
        //                    TotalRentMinutes = 0;
        //                }
        //            }
        //            if (TotalPoint >= TotalRentMinutes)
        //            {
        //                ActualRedeemableTimePoint = TotalRentMinutes;
        //            }
        //            else
        //            {
        //                if ((TotalPoint - TotalRentMinutes) < 30)
        //                {
        //                    ActualRedeemableTimePoint = TotalRentMinutes - 30;
        //                }
        //            }

        //            #region 非月租折扣計算
        //            //note: 折扣計算
        //            //double wDisc = 0;
        //            //double hDisc = 0;
        //            //int PayDisc = 0;
        //            if (!UseMonthMode)
        //            {
        //                if (hasFine)
        //                {
        //                    var xre = new BillCommon().CarDiscToPara(SD, ED, 60, 600, lstHoliday, Discount);
        //                    if (xre != null)
        //                    {
        //                        nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
        //                        nor_car_wDisc = xre.Item2;
        //                        nor_car_hDisc = xre.Item3;
        //                    }
        //                }
        //                else
        //                {
        //                    var xre = new BillCommon().CarDiscToPara(SD, FED, 60, 600, lstHoliday, Discount);
        //                    if (xre != null)
        //                    {
        //                        nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
        //                        nor_car_wDisc = xre.Item2;
        //                        nor_car_hDisc = xre.Item3;
        //                    }
        //                }

        //                var discPrice = Convert.ToDouble(car_n_price) * (nor_car_wDisc / 60) + Convert.ToDouble(car_h_price) * (nor_car_hDisc / 60);
        //                nor_car_PayDiscPrice = Convert.ToInt32(Math.Floor(discPrice));
        //                Discount = nor_car_PayDisc;
        //            }

        //            #endregion

        //            if (TotalRentMinutes > 0)
        //            {
        //                TotalRentMinutes -= Discount;
        //            }
        //            else
        //            {
        //                TotalRentMinutes = 0;
        //            }

        //            if (UseMonthMode)
        //            {
        //                outputApi.Rent.CarRental = CarRentPrice;
        //                outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
        //                outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
        //            }
        //            else
        //            {
        //                CarRentPrice = car_inPrice;//未逾時租用費用
        //                if (hasFine)
        //                    outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
        //            }

        //            if (Discount > 0)
        //            {
        //                var result = new BillCommon().GetCarRangeMins(SD, ED, 60, 10 * 60, lstHoliday);

        //                int DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * OrderDataLists[0].PRICE)));

        //                double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
        //                double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

        //                if (UseMonthMode)
        //                {

        //                }
        //                else
        //                {
        //                    //非月租折扣
        //                    DiscountPrice = Convert.ToInt32(((nor_car_wDisc / 60) * n_price) + ((nor_car_hDisc / 60) * h_price));
        //                    CarRentPrice -= DiscountPrice;
        //                    CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
        //                }
        //            }
        //            //安心服務
        //            InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
        //            if (InsurancePerHours > 0)
        //            {
        //                outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((car_payInMins / 30.0) * InsurancePerHours / 2)));

        //                //逾時安心服務計算
        //                if (TotalFineRentMinutes > 0)
        //                {
        //                    outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((car_payOutMins / 30.0) * InsurancePerHours / 2)));
        //                }
        //            }

        //            outputApi.Rent.CarRental = CarRentPrice;
        //            outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
        //            outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
        //            //outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
        //            //里程費計算修改，遇到取不到里程數的先以0元為主
        //            //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
        //            // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
        //            if (OrderDataLists[0].start_mile == 0 ||
        //                OrderDataLists[0].end_mile == 0 ||
        //                ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) > 1000) ||
        //                ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) < 0)
        //                )
        //            {
        //                outputApi.Rent.MileageRent = 0;
        //            }
        //            else
        //            {
        //                outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
        //            }
        //        }

        //        outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
        //        //outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes).ToString();
        //        outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
        //        outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
        //        //20201202 ADD BY ADAM REASON.ETAG費用
        //        outputApi.Rent.ETAGRental = etagPrice;

        //        var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
        //        xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
        //        outputApi.Rent.TotalRental = xTotalRental;

        //        #region 修正輸出欄位

        //        var tra = OrderDataLists[0].init_TransDiscount;
        //        if (xTotalRental == 0)
        //        {
        //            var carPri = outputApi.Rent.CarRental;
        //            if (carPri > 0)
        //                outputApi.Rent.TransferPrice = carPri;
        //        }

        //        //note: 修正輸出欄位PayDetail
        //        if (ProjType == 4)
        //        {
        //            outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
        //            outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
        //            outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)
        //            outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
        //            outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

        //            //var cDisc = apiInput.Discount;
        //            //var mDisc = apiInput.MotorDiscount;
        //            var cDisc = 0;
        //            var mDisc = 0;
        //            if (carInfo.useDisc > 0)
        //            {
        //                int lastDisc = carInfo.useDisc;
        //                var useMdisc = mDisc > carInfo.useDisc ? carInfo.useDisc : mDisc;
        //                lastDisc -= useMdisc;
        //                gift_motor_point = useMdisc;
        //                if (lastDisc > 0)
        //                {
        //                    var useCdisc = cDisc > lastDisc ? lastDisc : cDisc;
        //                    lastDisc -= useCdisc;
        //                    gift_point = useCdisc;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (UseMonthMode)
        //            {
        //                outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
        //                outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
        //                outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)
        //                outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
        //                outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
        //                if (carInfo != null && carInfo.useDisc > 0)
        //                    gift_point = carInfo.useDisc;
        //            }
        //            else
        //            {
        //                outputApi.Rent.UseNorTimeInterval = Discount.ToString();
        //                outputApi.Rent.RentalTimeInterval = car_payInMins.ToString(); //租用時數(未逾時)
        //                outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_pay_in_wMins + car_pay_in_hMins).ToString();//可折抵租用時數
        //                outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
        //                gift_point = nor_car_PayDisc;
        //            }

        //            gift_motor_point = 0;
        //            outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
        //        }

        //        #endregion

        //        string SPName = new ObjType().GetSPName(ObjType.SPType.BE_CalFinalPrice);
        //        SPInput_BE_CalFinalPrice SPInput = new SPInput_BE_CalFinalPrice()
        //        {
        //            IDNO = IDNO,
        //            OrderNo = tmpOrder,
        //            final_price = outputApi.Rent.TotalRental,
        //            pure_price = outputApi.Rent.CarRental,
        //            mileage_price = outputApi.Rent.MileageRent,
        //            Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
        //            fine_price = outputApi.Rent.OvertimeRental,
        //            gift_point = gift_point,
        //            //gift_motor_point = gift_motor_point,  //等等補上

        //            Etag = outputApi.Rent.ETAGRental,
        //            parkingFee = outputApi.Rent.ParkingFee,
        //            TransDiscount = outputApi.Rent.TransferPrice,
        //            LogID = LogID,
        //            UserID = UserID
        //        };
        //        SPOutput_Base SPOutput = new SPOutput_Base();
        //        SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base>(connetStr);
        //        flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
        //        baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
        //    }
        //    #endregion
        //    return flag;
        //}

        #endregion

        private bool DoReturn()
        {
            bool flag = true;

            return flag;
        }
    }
}