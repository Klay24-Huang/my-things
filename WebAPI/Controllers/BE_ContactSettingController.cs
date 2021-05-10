using Domain.CarMachine;
using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Car;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Car;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Rent;
using Domain.TB;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.CENS;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.Output.CENS;
using Newtonsoft.Json;
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
                            //if (lstOrder[0].car_mgt_status > 0)
                            //{
                            //    flag = false;
                            //    errCode = "ERR735";
                            //}
                            //else
                            //{
                            //    string spName = new ObjType().GetSPName(ObjType.SPType.BE_BookingCancel);
                            //    SPInput_BE_BookingCancel spInput = new SPInput_BE_BookingCancel()
                            //    {
                            //        LogID = LogID,
                            //        OrderNo = tmpOrder,
                            //        UserID = apiInput.UserID
                            //    };
                            //    SPOutput_Base spOut = new SPOutput_Base();
                            //    SQLHelper<SPInput_BE_BookingCancel, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BE_BookingCancel, SPOutput_Base>(connetStr);
                            //    flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                            //    baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                            //}
                            if (lstOrder[0].car_mgt_status < 4)
                            {
                                flag = false;
                                errCode = "ERR773";
                            }
                            else if (lstOrder[0].car_mgt_status >= 15)
                            {
                                flag = false;
                                errCode = "ERR774";
                            }
                            else
                            {
                                string spName = new ObjType().GetSPName(ObjType.SPType.BE_BookingCancelNew);
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
                                /*2021.01.19 ↓下方已抽離成一支api，於前端若是選擇前強已先處理，不需重覆做*/
                                //if (flag)
                                //{
                                //    if (clearFlag)
                                //    {
                                //        flag = new CarCommonFunc().BE_CheckReturnCar(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode);
                                //    }
                                //    // 20201223;不管檢核結果，強還照做

                                //    flag = true;
                                //    errCode = "000000";
                                //}
                                if (flag)
                                {
                                    if (lstOrder[0].car_mgt_status == 15)
                                    {
                                        if (clearFlag)
                                        {
                                            bool CarFlag = new CarCommonFunc().DoBECloseRent(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode, apiInput.ByPass);
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
                                                bool CarFlag = new CarCommonFunc().DoBECloseRent(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode, apiInput.ByPass);
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
                else if (apiInput.Mode == 1 && apiInput.type == 2)
                {
                    string spName = new ObjType().GetSPName(ObjType.SPType.BE_CancelCleanOrder);
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
            var cr_sp = new CarRentSp();
            bool flag = true;
            float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
            var carRepo = new CarRentRepo(connetStr);
            var trace = new TraceCom();
            string funName = "DoReCalRent";
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
            DateTime? FineDate = null;
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;

            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int CarRentPrice = 0; //車輛租金
            int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            bool UseMonthMode = false;
            int InsurancePerHours = 0;  //安心服務每小時價
            int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
            CarRentInfo carInfo = new CarRentInfo();//汽車資料
            int CityParkingPrice = 0;   //城市車旅停車費 20210507 ADD BY YEH 

            double nor_car_wDisc = 0;//只有一般時段時平日折扣
            double nor_car_hDisc = 0;//只有一般時段時價日折扣
            int nor_car_PayDisc = 0;//只有一般時段時總折扣
            int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

            int gift_point = 0;//使用時數(汽車)
            int gift_motor_point = 0;//使用時數(機車)
            int motoBaseMins = 6;//機車基本分鐘數
            int carBaseMins = 60;//汽車基本分鐘數
            int motoMaxMins = 200;//機車單日最大分鐘數

            int End_Mile = 0;   //還車里程

            bool isSpring = false;//是否為春節時段
            var visMons = new List<MonthlyRentData>();//虛擬月租
            DateTime sprSD = Convert.ToDateTime(SiteUV.strSpringSd);
            DateTime sprED = Convert.ToDateTime(SiteUV.strSpringEd);
            var neverHasFine = new List<int>() { 3, 4 };//路邊,機車不會逾時
            string errMsg = "";

            int UseOrderPrice = 0;//使用訂金(4捨5入)
            int OrderPrice = 0;//原始訂金

            string ProjID = "";
            #endregion
            #region trace-in
            trace.OrderNo = tmpOrder;
            var funInput = new
            {
                tmpOrder = tmpOrder,
                IDNO = IDNO,
                LogID = LogID,
                UserID = UserID,
                returnDate = returnDate,
                errCode = errCode
            };
            trace.objs.Add(nameof(funInput), funInput);
            #endregion
            try
            {
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
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        OrderDataLists = objUti.ConvertToList<OrderQueryFullData>(ds.Tables[0]);
                        trace.objs.Add(nameof(OrderDataLists), OrderDataLists);
                    }
                    trace.FlowList.Add("取出訂單資訊");
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
                {
                    var item = OrderDataLists[0];
                    motoBaseMins = item.BaseMinutes > 0 ? item.BaseMinutes : motoBaseMins;
                    ProjType = item.ProjType;
                    UseOrderPrice = item.UseOrderPrice;
                    OrderPrice = item.OrderPrice;
                    ProjID = item.ProjID;
                }
                //取得專案狀態
                if (flag)
                {
                    ProjType = OrderDataLists[0].ProjType;
                    SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                    SD = SD.AddSeconds(SD.Second * -1); //去秒數
                                                        //機車路邊不計算預計還車時間

                    if (!string.IsNullOrWhiteSpace(OrderDataLists[0].fine_Time) && Convert.ToDateTime(OrderDataLists[0].fine_Time) > Convert.ToDateTime("1911 -01-01 00:00:00"))
                    {
                        FineDate = Convert.ToDateTime(OrderDataLists[0].fine_Time);
                        FineDate = FineDate.Value.AddSeconds(ED.Second * -1); //去秒數
                    }

                    if (OrderDataLists[0].ProjType == 3 || OrderDataLists[0].ProjType == 4)
                    {
                        ED = Convert.ToDateTime(returnDate);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                        FineDate = null;
                    }
                    else
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    }
                    FED = Convert.ToDateTime(returnDate);
                    FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));

                    if (FineDate != null)
                    {
                        if (FED > FineDate)
                        {
                            hasFine = true;
                            ED = FineDate.Value;
                        }
                    }
                    else
                    {
                        if (FED.Subtract(ED).Ticks > 0)
                            hasFine = true;
                    }

                    if (FED < SD)
                    {
                        flag = false;
                        errCode = "ERR740";
                    }

                    #region trace
                    var timeMark = new
                    {
                        SD = SD,
                        ED = ED,
                        FineDate = FineDate,
                        hasFine = hasFine
                    };
                    trace.objs.Add(nameof(timeMark), timeMark);
                    trace.FlowList.Add("SD,ED,FD計算");
                    #endregion
                }

                #region 取還車里程
                if (flag)
                {
                    //bool CarFlag = new CarCommonFunc().BE_GetReturnCarMilage(tmpOrder, IDNO, LogID, UserID, ref errCode, ref End_Mile);
                    // 20210219;修改還車里程取得規則
                    if (OrderDataLists[0].car_mgt_status >= 11) 
                    {
                        //已還車
                        //保險起見，再判斷一次是否有還車里程，以防程式崩潰
                        if (OrderDataLists[0].end_mile > 0)
                        {
                            //用已記錄的還車里程
                            End_Mile = Convert.ToInt32(OrderDataLists[0].end_mile);
                        }
                        else
                        {
                            SPInput_GetCarMillage SPInput = new SPInput_GetCarMillage
                            {
                                IDNO = IDNO,
                                OrderNo = tmpOrder
                            };
                            string SPName = new ObjType().GetSPName(ObjType.SPType.GetCarReturnMillage);
                            SPOutput_GetCarMillage SPOut = new SPOutput_GetCarMillage();
                            SQLHelper<SPInput_GetCarMillage, SPOutput_GetCarMillage> sqlHelp = new SQLHelper<SPInput_GetCarMillage, SPOutput_GetCarMillage>(connetStr);
                            flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOut, ref lstError);
                            baseVerify.checkSQLResult(ref flag, SPOut.Error, SPOut.ErrorCode, ref lstError, ref errCode);
                            if (flag)
                            {
                                End_Mile = SPOut.Millage;
                            }
                        }
                    }
                    else
                    {
                        //未還車
                        SPInput_GetCarMillage SPInput = new SPInput_GetCarMillage
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder
                        };
                        string SPName = new ObjType().GetSPName(ObjType.SPType.GetCarReturnMillage);
                        SPOutput_GetCarMillage SPOut = new SPOutput_GetCarMillage();
                        SQLHelper<SPInput_GetCarMillage, SPOutput_GetCarMillage> sqlHelp = new SQLHelper<SPInput_GetCarMillage, SPOutput_GetCarMillage>(connetStr);
                        flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOut, ref lstError);
                        baseVerify.checkSQLResult(ref flag, SPOut.Error, SPOut.ErrorCode, ref lstError, ref errCode);
                        if (flag)
                        {
                            End_Mile = SPOut.Millage;
                        }
                    }
                }
                #endregion

                #region 春節時間判定
                var vsd = new DateTime();
                var ved = new DateTime();
                if (neverHasFine.Contains(ProjType))
                {
                    isSpring = cr_com.isSpring(SD, ED);
                    vsd = SD;
                    ved = ED;
                }
                else
                {
                    isSpring = cr_com.isSpring(SD, FED);
                    vsd = SD;
                    ved = FED;
                }
                #endregion

                #region 路邊,機車春前特殊處理

                if (ProjType == 0 && !isSpring)
                {
                    var item = OrderDataLists[0];
                    if (!string.IsNullOrWhiteSpace(item.ProjID) && item.ProjID.ToLower() == "r129" && !isSpring)
                    {
                        DateTime sprSd = Convert.ToDateTime(SiteUV.strSpringSd);
                        bool befSpring = false;
                        if (hasFine)
                            befSpring = sprSd >= ED;
                        else
                            befSpring = sprSd >= FED;
                        if (befSpring)
                        {
                            var xre = cr_sp.sp_GetEstimate("P735", item.CarTypeGroupCode, LogID, ref errMsg);
                            if (xre != null)
                            {
                                OrderDataLists[0].PRICE = Convert.ToInt32(Math.Floor(xre.PRICE / 10));
                                OrderDataLists[0].PRICE_H = Convert.ToInt32(Math.Floor(xre.PRICE_H / 10));
                            }
                        }
                    }
                }

                if (isSpring && neverHasFine.Any(x => x == ProjType))
                {
                    if (ProjType == 3)
                    {
                        var xre = cr_sp.sp_GetEstimate("R139", OrderDataLists[0].CarTypeGroupCode, LogID, ref errMsg);
                        if (xre != null)
                        {
                            OrderDataLists[0].PRICE = Convert.ToInt32(Math.Floor(xre.PRICE / 10));
                            OrderDataLists[0].PRICE_H = Convert.ToInt32(Math.Floor(xre.PRICE_H / 10));
                        }
                    }
                    else if (ProjType == 4)
                    {
                        var xre = cr_sp.sp_GetEstimate("R140", OrderDataLists[0].CarTypeGroupCode, LogID, ref errMsg);
                        if (xre != null)
                        {
                            //機車目前不分平假日
                            OrderDataLists[0].MinuteOfPrice = Convert.ToSingle(xre.PRICE);
                        }
                    }
                }

                #endregion

                #region 計算非逾時及逾時時間

                if (ProjType == 4)
                {
                    //春前
                    if (ED <= sprSD)
                    {
                        var xre = billCommon.GetMotoRangeMins(SD, ED, 6, 200, lstHoliday);
                        TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                        var xDays = ED.Subtract(SD).TotalDays;
                        if (xDays > 1)
                            TotalRentMinutes -= 1;
                    }
                    else
                    {
                        var xre = billCommon.GetMotoRangeMins(SD, ED, 6, 600, lstHoliday);
                        TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                    }
                }
                else
                {
                    if (hasFine)
                    {
                        var xre = billCommon.GetCarRangeMins(SD, ED, 60, 600, lstHoliday);
                        if (xre != null)
                            TotalRentMinutes += Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                        var ov_re = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                        if (ov_re != null)
                            TotalRentMinutes += Convert.ToInt32(Math.Floor(ov_re.Item1 + ov_re.Item2));
                    }
                    else
                    {
                        var xre = billCommon.GetCarRangeMins(SD, FED, 60, 600, lstHoliday);
                        if (xre != null)
                            TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                    }
                }
                TotalRentMinutes = TotalRentMinutes > 0 ? TotalRentMinutes : 0;

                #endregion

                #region 取得虛擬月租
                //dev:取得虛擬月租

                if (OrderDataLists != null && OrderDataLists.Count() > 0)
                {
                    var item = OrderDataLists[0];
                    //春節專案才產生虛擬月租
                    if (isSpring)
                    {
                        var ibiz_vMon = new IBIZ_SpringInit()
                        {
                            SD = vsd,
                            ED = ved,
                            //ProjID = item.ProjID,
                            ProjType = item.ProjType,
                            CarType = item.CarTypeGroupCode,
                            ProDisPRICE = ProjType == 4 ? item.MinuteOfPrice : item.PRICE,
                            ProDisPRICE_H = ProjType == 4 ? item.MinuteOfPrice : item.PRICE_H
                        };
                        var vmonRe = cr_com.GetVisualMonth(ibiz_vMon);
                        if (vmonRe != null)
                        {
                            if (vmonRe.VisMons != null && vmonRe.VisMons.Count() > 0)
                            {
                                visMons = vmonRe.VisMons;
                                if (ProjType == 4)
                                {
                                    //機車目前不分平假日 ,GetVisualMonth有分
                                    OrderDataLists[0].MinuteOfPrice = Convert.ToSingle(vmonRe.PRICE);
                                }
                                else
                                {
                                    OrderDataLists[0].PRICE = Convert.ToInt32(Math.Floor(vmonRe.PRICE));
                                    OrderDataLists[0].PRICE_H = Convert.ToInt32(Math.Floor(vmonRe.PRICE_H));
                                }

                                trace.objs.Add(nameof(vmonRe), vmonRe);
                                trace.FlowList.Add("新增虛擬月租");
                            }
                        }
                    }
                }
                #endregion

                #region 汽車計費資訊
                int car_payAllMins = 0; //全部計費租用分鐘
                int car_payInMins = 0;//未超時計費分鐘
                int car_payOutMins = 0;//超時分鐘-顯示用
                int car_inPrice = 0;//未超時費用
                int car_outPrice = 0;//超時費用
                int car_n_price = OrderDataLists[0].PRICE;
                int car_h_price = OrderDataLists[0].PRICE_H;

                if (flag)
                {
                    if (ProjType != 4)
                    {
                        if (hasFine)
                        {
                            var ord = OrderDataLists[0];
                            var xre = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                            if (xre != null)
                                car_payOutMins = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                            car_outPrice = billCommon.CarRentCompute(ED, FED, ord.WeekdayPrice, ord.HoildayPrice, 6, lstHoliday, true, 0);
                            car_payAllMins += car_payOutMins;

                            var car_re = billCommon.CarRentInCompute(SD, ED, car_n_price, car_h_price, 60, 10, lstHoliday, new List<MonthlyRentData>(), Discount);
                            if (car_re != null)
                            {
                                trace.traceAdd(nameof(car_re), car_re);
                                car_payAllMins += car_re.RentInMins;
                                car_payInMins = car_re.RentInMins;
                                car_inPrice = car_re.RentInPay;
                                nor_car_PayDisc = car_re.useDisc;
                            }
                        }
                        else
                        {
                            var car_re = billCommon.CarRentInCompute(SD, FED, car_n_price, car_h_price, 60, 10, lstHoliday, new List<MonthlyRentData>(), Discount);
                            if (car_re != null)
                            {
                                trace.traceAdd(nameof(car_re), car_re);

                                car_payAllMins += car_re.RentInMins;
                                car_payInMins = car_re.RentInMins;
                                car_inPrice = car_re.RentInPay;
                                nor_car_PayDisc = car_re.useDisc;
                            }
                        }

                        trace.FlowList.Add("汽車計費資訊(非月租)");
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
                        trace.objs.Add(nameof(re270), re270);
                        flag = re270.flag;
                        MotorPoint = re270.MotorPoint;
                        CarPoint = re270.CarPoint;
                    }
                    trace.FlowList.Add("與短租查時數");

                    #region mark-判斷輸入的點數有沒有超過總點數
                    //判斷輸入的點數有沒有超過總點數
                    //if (ProjType == 4)
                    //{
                    //    if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
                    //    {
                    //        //flag = false;
                    //        //errCode = "ERR205";
                    //    }
                    //    else
                    //    {
                    //        if (Discount > (MotorPoint + CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
                    //        {
                    //            flag = false;
                    //            errCode = "ERR207";
                    //        }
                    //    }

                    //    if (TotalRentMinutes <= 6 && Discount == 6)
                    //    {

                    //    }
                    //    else if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
                    //    {
                    //        flag = false;
                    //        errCode = "ERR303";
                    //    }
                    //}
                    //else
                    //{
                    //    if (Discount > 0 && Discount % 30 > 0)
                    //    {
                    //        flag = false;
                    //        errCode = "ERR206";
                    //    }
                    //    else
                    //    {
                    //        if (Discount > CarPoint)
                    //        {
                    //            flag = false;
                    //            errCode = "ERR207";
                    //        }
                    //    }
                    //}
                    #endregion
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
                        trace.objs.Add(nameof(etag_re), etag_re);
                        flag = etag_re.flag;
                        errCode = etag_re.errCode;
                        etagPrice = etag_re.etagPrice;
                    }
                    trace.FlowList.Add("查ETAG");
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
                    trace.FlowList.Add("建空模");
                }

                if (flag && OrderDataLists[0].ProjType != 4)
                {
                    //檢查有無車麻吉停車費用
                    var input = new IBIZ_CarMagi()
                    {
                        LogID = LogID,
                        CarNo = OrderDataLists[0].CarNo,
                        SD = SD,
                        ED = FED.AddDays(1),
                        OrderNo = tmpOrder
                    };
                    var magi_Re = cr_com.CarMagi(input);
                    if (magi_Re != null)
                    {
                        trace.objs.Add(nameof(magi_Re), magi_Re);
                        flag = magi_Re.flag;
                        outputApi.Rent.ParkingFee = magi_Re.ParkingFee;
                    }
                    trace.FlowList.Add("車麻吉");
                }

                //20210507 ADD BY YEH REASON.串接CityParking停車場
                if (flag && OrderDataLists[0].ProjType != 4)
                {
                    string SPName = new ObjType().GetSPName(ObjType.SPType.GetCityParkingFee);
                    SPInput_CalCityParkingFee SPInput = new SPInput_CalCityParkingFee()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        SD = SD,
                        ED = FED,
                        LogID = LogID,
                    };

                    SPOutput_CalCityParkingFee SPOutput = new SPOutput_CalCityParkingFee();
                    SQLHelper<SPInput_CalCityParkingFee, SPOutput_CalCityParkingFee> SQLBookingStartHelp = new SQLHelper<SPInput_CalCityParkingFee, SPOutput_CalCityParkingFee>(connetStr);
                    flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    flag = !(SPOutput.Error == 1 || SPOutput.ErrorCode != "0000");
                    if (SPOutput.ErrorCode == "0000" && lstError.Count > 0)
                    {
                        SPOutput.ErrorCode = lstError[0].ErrorCode;
                    }
                    if (flag)
                    {
                        CityParkingPrice = SPOutput.ParkingFee;
                        if (CityParkingPrice > 0)
                        {
                            outputApi.Rent.ParkingFee += CityParkingPrice;
                        }
                    }
                    trace.FlowList.Add("CityParkingFee");
                }
                #endregion
                #region 月租
                //note: 月租GetPayDetail
                if (flag)
                {
                    var item = OrderDataLists[0];
                    item = cr_com.dbValeFix(item);
                    var motoDayMaxMinns = 200;
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
                        carBaseMins = 60,
                        CancelMonthRent = (ProjID == "R024")
                    };

                    if (visMons != null && visMons.Count() > 0)
                        input.VisMons = visMons;

                    var mon_re = cr_com.MonthRentSave(input);
                    if (mon_re != null)
                    {
                        trace.objs.Add(nameof(mon_re), mon_re);
                        flag = mon_re.flag;
                        UseMonthMode = mon_re.UseMonthMode;
                        outputApi.IsMonthRent = mon_re.IsMonthRent;
                        if (UseMonthMode)
                        {
                            carInfo = mon_re.carInfo;
                            Discount = mon_re.useDisc;
                            monthlyRentDatas = mon_re.monthlyRentDatas;

                            if (ProjType == 4)
                                outputApi.Rent.CarRental = mon_re.CarRental;//機車用
                            else
                                CarRentPrice += mon_re.CarRental;//汽車用
                        }
                    }
                    trace.FlowList.Add("月租");
                }
                #endregion
                #region 開始計價
                if (flag)
                {
                    trace.FlowList.Add("開始計價");
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

                            //春前
                            if (ED <= sprSD)
                            {
                                var xre = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, 6, 200, lstHoliday, new List<MonthlyRentData>(), Discount, 199, 300);
                                if (xre != null)
                                {
                                    carInfo = xre;
                                    outputApi.Rent.CarRental = xre.RentInPay;
                                    Discount = xre.useDisc;
                                }
                            }
                            //春後
                            else
                            {
                                var xre = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, 6, 600, lstHoliday, new List<MonthlyRentData>(), Discount, 600, 901);
                                if (xre != null)
                                {
                                    carInfo = xre;
                                    outputApi.Rent.CarRental = xre.RentInPay;
                                    carInfo.useDisc = xre.useDisc;
                                }
                            }

                            if (carInfo != null)
                                outputApi.Rent.CarRental = carInfo.RentInPay;

                            trace.FlowList.Add("機車非月租租金計算");
                        }

                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    }
                    else
                    {
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
                            trace.FlowList.Add("汽車非月租金額給值");
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
                                //CarRentPrice -= nor_car_PayDiscPrice;
                                CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
                                trace.FlowList.Add("汽車非月租折扣扣除");
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
                        trace.FlowList.Add("安心服務");

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                        outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                        // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                        // 20210121;里程數>1000公里的判斷移除
                        if (OrderDataLists[0].start_mile == 0 || End_Mile == 0 || ((End_Mile - OrderDataLists[0].start_mile) < 0))
                        {
                            outputApi.Rent.MileageRent = 0;
                        }
                        else
                        {
                            outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (End_Mile - OrderDataLists[0].start_mile));
                        }
                        trace.FlowList.Add("里程費計算");
                    }

                    outputApi.Rent.OvertimeRental = car_outPrice;
                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                    outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                    //20201202 ADD BY ADAM REASON.ETAG費用
                    outputApi.Rent.ETAGRental = etagPrice;

                    var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
                    xTotalRental -= UseOrderPrice;//使用訂金
                    outputApi.UseOrderPrice = UseOrderPrice;
                    outputApi.FineOrderPrice = OrderPrice - UseOrderPrice;//沒收訂金                      
                    if (xTotalRental < 0)
                    {
                        outputApi.ReturnOrderPrice = (-1) * xTotalRental;
                        int orderNo = Convert.ToInt32(OrderDataLists[0].OrderNo);
                        carRepo.UpdNYPayList(orderNo, outputApi.ReturnOrderPrice);
                    }

                    xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                    outputApi.Rent.TotalRental = xTotalRental;
                    trace.FlowList.Add("總價計算");

                    #region 修正輸出欄位
                    //note: 修正輸出欄位PayDetail
                    outputApi.UseOrderPrice = UseOrderPrice;
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
                            outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_payInMins).ToString();//可折抵租用時數
                            outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
                            gift_point = nor_car_PayDisc;
                        }

                        gift_motor_point = 0;
                        outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                    }
                    trace.FlowList.Add("修正輸出欄位");
                    #endregion

                    string SPName = new ObjType().GetSPName(ObjType.SPType.BE_CalFinalPrice);
                    SPInput_BE_CalFinalPrice SPInput = new SPInput_BE_CalFinalPrice()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        UserID = UserID,
                        final_price = outputApi.Rent.TotalRental,
                        pure_price = outputApi.Rent.CarRental,
                        mileage_price = outputApi.Rent.MileageRent,
                        Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                        fine_price = outputApi.Rent.OvertimeRental,
                        gift_point = gift_point,
                        Etag = outputApi.Rent.ETAGRental,
                        parkingFee = outputApi.Rent.ParkingFee,
                        TransDiscount = outputApi.Rent.TransferPrice,
                        EndMile = End_Mile,
                        LogID = LogID
                    };

                    #region trace

                    trace.objs.Add(nameof(TotalRentMinutes), TotalRentMinutes);
                    trace.objs.Add(nameof(Discount), Discount);
                    trace.objs.Add(nameof(CarPoint), CarPoint);
                    trace.objs.Add(nameof(MotorPoint), MotorPoint);
                    trace.objs.Add(nameof(SPInput), SPInput);
                    trace.objs.Add(nameof(outputApi), outputApi);
                    trace.objs.Add(nameof(carInfo), carInfo);

                    #endregion

                    SPOutput_Base SPOutput = new SPOutput_Base();
                    SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base>(connetStr);
                    flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
                    trace.FlowList.Add("sp存檔");
                }
                #endregion

                #region 寫入錯誤Log
                bool mark = true;
                if (!flag || mark)
                {
                    trace.objs.Add(nameof(errCode), errCode);
                    trace.objs.Add(nameof(TotalPoint), TotalPoint);
                    trace.objs.Add(nameof(TransferPrice), TransferPrice);

                    string traceMsg = JsonConvert.SerializeObject(trace);
                    var errItem = new TraceLogVM()
                    {
                        ApiId = 127,
                        ApiMsg = traceMsg,
                        ApiNm = funName,
                        CodeVersion = trace.codeVersion,
                        FlowStep = trace.FlowStep(),
                        OrderNo = trace.OrderNo,
                        TraceType = !flag ? eumTraceType.followErr : eumTraceType.mark
                    };
                    carRepo.AddTraceLog(errItem);
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                var errItem = new TraceLogVM()
                {
                    ApiId = 127,
                    ApiMsg = JsonConvert.SerializeObject(trace),
                    ApiNm = funName,
                    CodeVersion = trace.codeVersion,
                    FlowStep = JsonConvert.SerializeObject(trace.FlowStep()),
                    OrderNo = trace.OrderNo,
                    TraceType = eumTraceType.exception
                };
                carRepo.AddTraceLog(errItem);
                throw;
            }

            return flag;
        }
    }
}