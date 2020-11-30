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
using Domain.WebAPI.Output.CENS;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
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
            //string[] headers=httpContext.Request.Headers.AllKeys;
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
            Int16 APPKind = 2;
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
                    if (apiInput.type == 1 && string.IsNullOrEmpty(apiInput.returnDate))
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
                        { //取車
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
                                        flag = DoPickMotor(tmpOrder, lstOrder[0].IDNO, LogID, apiInput.UserID, ref errCode, ref errMsg, baseVerify);
                                        //機車
                                    }
                                    else
                                    {
                                        flag = DoPickCar(tmpOrder, lstOrder[0].IDNO, LogID, apiInput.UserID, ref errCode, ref errMsg, baseVerify);
                                        //汽車
                                    }
                                }
                            }
                        }
                        else
                        {
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
                                            unified_business_no = apiInput.unified_business_no
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
                                                unified_business_no = apiInput.unified_business_no
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
            bool flag = true;
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            List<Holiday> lstHoliday = null; //假日列表
            List<OrderQueryFullData> OrderDataLists = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime FineDate = new DateTime();
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;
            int ProjType = 0;
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
            bool UseMonthMode = false;
            float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
            int InsurancePerHours = 0;  //安心服務每小時價
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
            //取得專案狀態
            if (flag)
            {
                ProjType = OrderDataLists[0].ProjType;
                SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                SD = SD.AddSeconds(SD.Second * -1); //去秒數
                //機車路邊不計算預計還車時間
                if (OrderDataLists[0].ProjType == 4)
                {
                    ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time==""? returnDate: OrderDataLists[0].final_stop_time);
                    ED = ED.AddSeconds(ED.Second * -1); //去秒數
                }
                else
                {
                    ED = Convert.ToDateTime(OrderDataLists[0].stop_time==""? returnDate: OrderDataLists[0].stop_time);
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
                        billCommon.CalDayHourMin(SD, ED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                        billCommon.CalDayHourMin(ED, FED, ref FineDays, ref FineHours, ref FineMins);
                        TotalFineRentMinutes = ((FineDays * 10) + FineHours) * 60 + FineMins; //逾時的總時數
                    }
                    else
                    {
                        billCommon.CalDayHourMin(SD, FED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                    }
                }

            }
            //if (flag)
            //{
            //    if (NowTime.Subtract(FED).TotalMinutes >= 30)
            //    {
            //        flag = false;
            //        errCode = "ERR208";
            //    }
            //}
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

                    outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
                    {
                        BaseMinutePrice = OrderDataLists[0].BaseMinutesPrice,
                        BaseMinutes = OrderDataLists[0].BaseMinutes,
                        MinuteOfPrice = OrderDataLists[0].MinuteOfPrice
                    };
                }
                else
                {

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
            }
            #endregion
            #region 月租
            if (flag)
            {
                //1.0 先還原這個單號使用的
                flag = monthlyRentRepository.RestoreHistory(IDNO, tmpOrder, LogID, ref errCode);
                int RateType = (ProjType == 4) ? 1 : 0;
                if (!hasFine)
                {
                    monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                }
                else
                {
                    monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                }
                int MonthlyLen = monthlyRentDatas.Count;
                if (MonthlyLen > 0)
                {
                    UseMonthMode = true;
                    outputApi.IsMonthRent = 1;
                    if (flag)
                    {
                        if (ProjType == 4)
                        {
                            //機車沒有分平假日，直接送即可
                            for (int i = 0; i < MonthlyLen; i++)
                            {
                                int MotoTotalMinutes = Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);
                                if (MotoTotalMinutes >= TotalRentMinutes) //全部扣光
                                {
                                    MonthlyPoint += TotalRentMinutes;    //20201128 ADD BY ADAM REASON.月租折抵點數計算
                                    flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, TotalRentMinutes, LogID, ref errCode);//寫入記錄
                                    TotalRentMinutes = 0;
                                    break;
                                }
                                else
                                {
                                    //折抵不能全折時，基本分鐘數會擺在最後折，且要一次折抵掉
                                    //一般時數會先折抵基本分鐘數，所以月租必須先折非基本分鐘，否則兩邊會有牴觸
                                    if (TotalRentMinutes - MotoTotalMinutes >= OrderDataLists[0].BaseMinutes) //扣完有超過基本費
                                    {
                                        MonthlyPoint += MotoTotalMinutes;        //20201128 ADD BY ADAM REASON.月租折抵點數計算
                                        TotalRentMinutes -= MotoTotalMinutes;
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, MotoTotalMinutes, LogID, ref errCode); //寫入記錄
                                    }
                                    else
                                    {
                                        //折抵時數不夠扣基本費 只能折  租用時數-基本分鐘數
                                        int tmpMonthlyPoint = TotalRentMinutes - OrderDataLists[0].BaseMinutes;
                                        MonthlyPoint += tmpMonthlyPoint;
                                        TotalRentMinutes -= tmpMonthlyPoint;
                                        //MotoTotalHours += TotalRentMinutes - MotoTotalHours - OrderDataLists[0].BaseMinutes;
                                        //TotalRentMinutes -= MotoTotalHours;
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, tmpMonthlyPoint, LogID, ref errCode); //寫入記錄
                                    }
                                }

                                outputApi.Rent.RemainMonthlyTimeInterval = MonthlyPoint.ToString();
                            }
                        }
                        else
                        {
                            List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();
                            if (hasFine)
                            {
                                CarRentPrice = billCommon.CalBillBySubScription(SD, ED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
                            }
                            else
                            {
                                CarRentPrice = billCommon.CalBillBySubScription(SD, FED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
                            }
                            if (UseMonthlyRent.Count > 0)
                            {
                                UseMonthMode = true;
                                int UseLen = UseMonthlyRent.Count;
                                for (int i = 0; i < UseLen; i++)
                                {
                                    flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, LogID, ref errCode); //寫入記錄
                                }
                            }
                            else
                            {
                                UseMonthMode = false;
                            }
                        }
                    }
                }
            }
            #endregion
            #region 開始計價  20201129 還沒同步到前端計算租金
            if (flag)
            {
                lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                if (ProjType == 4)
                {


                    if (UseMonthMode)   //true:有月租;false:無月租
                    {
                        billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, monthlyRentDatas[0].WorkDayRateForMoto, monthlyRentDatas[0].HoildayRateForMoto, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                        outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                        outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                    }
                    else
                    {
                        billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                    }

                    outputApi.Rent.CarRental = CarRentPrice;
                    outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                }
                else
                {
                    int BaseMinutes = 60;
                    int tmpTotalRentMinutes = TotalRentMinutes;

                    if (TotalRentMinutes < BaseMinutes)
                    {
                        TotalRentMinutes = BaseMinutes;
                    }
                    if (UseMonthMode)
                    {
                        TotalRentMinutes -= Convert.ToInt32((billCommon._scriptHolidayHour + billCommon._scriptWorkHour) * 60);
                        if (TotalRentMinutes < 0)
                        {
                            TotalRentMinutes = 0;
                        }
                    }


                    if (UseMonthMode)
                    {
                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
                        outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
                    }
                    else
                    {
                        if (hasFine)
                        {
                            CarRentPrice = Convert.ToInt32(new BillCommon().CalSpread(SD, ED, Convert.ToInt32(OrderDataLists[0].PRICE * 10), Convert.ToInt32(OrderDataLists[0].PRICE_H * 10), lstHoliday));
                        }
                        else
                        {
                            CarRentPrice = Convert.ToInt32(new BillCommon().CalSpread(SD, FED, Convert.ToInt32(OrderDataLists[0].PRICE * 10), Convert.ToInt32(OrderDataLists[0].PRICE_H * 10), lstHoliday));
                        }
                    }

                    outputApi.Rent.CarRental = CarRentPrice;
                    outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                    outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                }

                outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes).ToString();
                outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                outputApi.Rent.TotalRental = (outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice < 0) ? 0 : outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice;
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
                    gift_point = 0,
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
        private bool DoReturn()
        {
            bool flag = true;

            return flag;
        }
    }
}