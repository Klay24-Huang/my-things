using Domain.CarMachine;
using Domain.Common;
using Domain.Flow.CarRentCompute;
using Domain.Log;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Car;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
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
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
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
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

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
            int FinalPrice = 0; // 20211108 ADD BY YEH REASON:總價
            CommonService commonService = new CommonService();
            PreAmountData PreAmount = new PreAmountData();
            List<TradeCloseList> TradeCloseLists = new List<TradeCloseList>();
            int RewardPoint = 0;    // 20211109 ADD BY YEH REASON:換電獎勵
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_BE_ContactSetting>(Contentjson);
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
                    if (apiInput.Mode < 0 || apiInput.Mode > 7)
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
                if (apiInput.Mode == 0 || apiInput.Mode == 3 || apiInput.Mode == 4 || apiInput.Mode == 5 || apiInput.Mode == 6 || apiInput.Mode == 7)
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
                            #region 強制取消
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
                                string spName = "usp_BE_BookingCancelNew";
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
                            #endregion
                        }
                        else if (apiInput.type == 0)
                        {
                            //取車
                            #region 強制取車
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
                            #endregion
                        }
                        else
                        {
                            //還車
                            #region 強制還車
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
                                    if (lstOrder[0].car_mgt_status == 15)   //基本上不會跑進這個判斷中，訂單是從11跳16
                                    {
                                        #region 無用
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
                                        string SPName = "usp_BE_ContactFinish";
                                        SPOutput_Base PayOutput = new SPOutput_Base();
                                        SQLHelper<SPInput_BE_ContactFinish, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_BE_ContactFinish, SPOutput_Base>(connetStr);
                                        flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                                        baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                                        #endregion
                                    }
                                    else
                                    {
                                        //重新計價
                                        flag = DoReCalRent(tmpOrder, IDNO, LogID, apiInput.UserID, apiInput.returnDate, ref errCode, ref FinalPrice);
                                        if (flag)
                                        {
                                            if (clearFlag)
                                            {
                                                #region 車機指令
                                                if (isDebug == "0") // isDebug = 1，不送車機指令
                                                {
                                                    bool CarFlag = new CarCommonFunc().DoBECloseRent(tmpOrder, IDNO, LogID, apiInput.UserID, ref errCode, apiInput.ByPass);
                                                    if (CarFlag == false)
                                                    {
                                                        //寫入車機錯誤
                                                    }
                                                    errCode = "000000";
                                                }
                                                #endregion
                                            }

                                            #region 取得預授權金額
                                            if (flag)
                                            {
                                                PreAmount = commonService.GetPreAmount(IDNO, Access_Token, tmpOrder, "N", LogID, ref flag, ref errCode);
                                            }
                                            #endregion

                                            #region 訂單預授權判斷
                                            if (flag)
                                            {
                                                TradeCloseLists = commonService.DoPreAmount(PreAmount, FinalPrice);
                                            }
                                            #endregion

                                            #region SP存檔
                                            if (flag)
                                            {
                                                string spName = "usp_BE_ContactSetting_U01";

                                                object[] objparms = new object[TradeCloseLists.Count == 0 ? 1 : TradeCloseLists.Count];

                                                if (TradeCloseLists.Count > 0)
                                                {
                                                    for (int i = 0; i < TradeCloseLists.Count; i++)
                                                    {
                                                        objparms[i] = new
                                                        {
                                                            CloseID = TradeCloseLists[i].CloseID,
                                                            CardType = TradeCloseLists[i].CardType,
                                                            AuthType = TradeCloseLists[i].AuthType,
                                                            ChkClose = TradeCloseLists[i].ChkClose,
                                                            CloseAmout = TradeCloseLists[i].CloseAmout,
                                                            RefundAmount = TradeCloseLists[i].RefundAmount
                                                        };
                                                    }
                                                }
                                                else
                                                {
                                                    objparms[0] = new
                                                    {
                                                        CloseID = 0,
                                                        CardType = 0,
                                                        AuthType = 0,
                                                        ChkClose = 0,
                                                        CloseAmout = 0,
                                                        RefundAmount = 0
                                                    };
                                                }

                                                object[][] parms1 = {
                                                    new object[] {
                                                        IDNO,
                                                        tmpOrder,
                                                        apiInput.UserID,
                                                        ReturnDate,
                                                        apiInput.bill_option,
                                                        apiInput.unified_business_no,
                                                        apiInput.CARRIERID,
                                                        apiInput.NPOBAN,
                                                        apiInput.parkingSpace,
                                                        apiInput.Mode,
                                                        LogID
                                                    },
                                                    objparms
                                                };

                                                DataSet ds1 = new DataSet();
                                                string returnMessage = "";
                                                string messageLevel = "";
                                                string messageType = "";

                                                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), spName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                                                if (ds1.Tables.Count == 0)
                                                {
                                                    flag = false;
                                                    errCode = "ERR999";
                                                    errMsg = returnMessage;
                                                }
                                                else
                                                {
                                                    baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[1].Rows[0]["Error"]), ds1.Tables[1].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                                                    if (flag)
                                                    {
                                                        if (ds1.Tables[0].Rows.Count > 0)
                                                        {
                                                            RewardPoint = Convert.ToInt32(ds1.Tables[0].Rows[0]["Reward"]);
                                                            if (RewardPoint > 0)
                                                            {
                                                                flag = DoNPR380(IDNO, RewardPoint, tmpOrder, LogID,ref errCode);
                                                            }
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
                            #endregion
                        }
                    }
                }
                else if (apiInput.Mode == 1 && apiInput.type == 2)
                {
                    #region 整備取消
                    string spName = "usp_BE_CancelCleanOrder";
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
                    #endregion
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        #region 取車-汽車
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
            string CheckTokenName = "usp_BE_BeforeBookingStart";
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
                    #region 最新狀況
                    if (isDebug == "0") // isDebug = 1，不送車機指令
                    {
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
                    }
                    #endregion
                    #region 執行sp合約
                    if (flag)
                    {
                        string BookingStartName = "usp_BE_BookingStart";
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
                    #endregion
                    #region 車機指令(設定租約、解防盜、寫入顧客卡)
                    if (isDebug == "0") // isDebug = 1，不送車機指令
                    {
                        if (flag && webAPI.IsSupportCombineCmd(CID))
                        {
                            WSInput_CombineCmdGetCar wsInput = new WSInput_CombineCmdGetCar()
                            {
                                CID = CID,
                                data = new WSInput_CombineCmdGetCar.SendCarNoData[] { }
                            };
                            //要將卡號寫入車機
                            int count = 0;
                            int CardLen = lstCardList.Count;
                            if (CardLen > 0)
                            {
                                WSInput_CombineCmdGetCar.SendCarNoData[] CardData = new WSInput_CombineCmdGetCar.SendCarNoData[CardLen];
                                //寫入顧客卡
                                var CardNo = string.Empty;
                                for (int i = 0; i < CardLen; i++)
                                {
                                    CardData[i] = new WSInput_CombineCmdGetCar.SendCarNoData();
                                    CardData[i].CardNo = lstCardList[i].CardNO;
                                    CardNo += lstCardList[i].CardNO;
                                    count++;
                                }

                                if (!string.IsNullOrEmpty(CardNo))  // 有卡號才呼叫車機
                                {
                                    wsInput.data = CardData;
                                }
                            }
                            WSOutput_Base wsOut = new WSOutput_Base();
                            Thread.Sleep(1000);
                            flag = webAPI.CombineCmdGetCar(wsInput, ref wsOut);
                            if (false == flag || wsOut.Result == 1)
                            {
                                errCode = wsOut.ErrorCode;
                                errMsg = wsOut.ErrMsg;
                            }
                        }
                        else
                        {
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
                            //寫入顧客卡 20210316 ADD BY ADAM REASON.開啟租約就可以直接寫入顧客卡
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
                            //開啟NFC電源 20210316 ADD BY ADAM REASON.開啟租約就可以直接寫入顧客卡就不用開啟電源了
                            //if (flag)
                            //{
                            //    Thread.Sleep(1000);
                            //    WSOutput_Base wsOut = new WSOutput_Base();
                            //    flag = webAPI.NFCPower(CID, 1, LogID, ref wsOut);
                            //    if (false == flag || wsOut.Result == 1)
                            //    {
                            //        errCode = wsOut.ErrorCode;
                            //        errMsg = wsOut.ErrMsg;
                            //    }
                            //}
                        }
                    }
                    #endregion
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
                    string method = "";
                    #region ReportNow
                    if (isDebug == "0") // isDebug = 1，不送車機指令
                    {
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
                        method = CommandType;
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
                    }
                    #endregion
                    if (flag)
                    {
                        #region 執行sp合約
                        if (flag)
                        {
                            string BookingStartName = "usp_BE_BookingStart";
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
                        #endregion
                        #region 車機指令(設定租約、解防盜、寫入顧客卡)
                        if (isDebug == "0") // isDebug = 1，不送車機指令
                        {
                            if (flag && FetAPI.IsSupportCombineCmd(CID))
                            {
                                CommandType = new OtherService.Enum.MachineCommandType().GetCommandName(OtherService.Enum.MachineCommandType.CommandType.VehicleRentCombo);
                                CmdType = OtherService.Enum.MachineCommandType.CommandType.VehicleRentCombo;
                                WSInput_Base<ClientCardNoObj> SetCardInput = new WSInput_Base<ClientCardNoObj>()
                                {
                                    command = true,
                                    method = CommandType,
                                    requestId = string.Format("{0}_{1}", spOut.CID, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                                    _params = new ClientCardNoObj()
                                    {
                                        ClientCardNo = new string[] { }
                                    }
                                };
                                //寫入顧客卡
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
                                            SetCardInput._params.ClientCardNo = CardStr;
                                        }
                                    }
                                }
                                //組合指令顧客卡必輸入，若沒有則帶隨機值
                                if (SetCardInput._params.ClientCardNo.Length == 0)
                                {
                                    SetCardInput._params.ClientCardNo = new string[] { (new Random()).Next(10000000, 99999999).ToString().PadLeft(10, 'X') };
                                }
                                requestId = SetCardInput.requestId;
                                method = CommandType;
                                flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, SetCardInput, LogID);
                                if (flag)
                                {
                                    flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                                }
                            }
                            else
                            {
                                //寫入顧客卡
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
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            return flag;
        }
        #endregion

        #region 取車-機車
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
            string CheckTokenName = "usp_BE_BeforeBookingStart";
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
                string BookingStartName = "usp_BE_BookingStart";
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
        #endregion

        #region 重新計價
        private bool DoReCalRent(Int64 tmpOrder, string IDNO, Int64 LogID, string UserID, string returnDate, ref string errCode, ref int FinalPrice)
        {
            #region 初始宣告
            #region API共用
            bool flag = true;
            string funName = "BE_ContactSettingController";
            float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            string errMsg = "";
            #endregion
            #region 計算用
            var cr_com = new CarRentCommon();
            var cr_sp = new CarRentSp();
            var carRepo = new CarRentRepo(connetStr);
            var monSp = new MonSubsSp();
            var trace = new TraceCom();
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            CarRentInfo carInfo = new CarRentInfo();//汽車資料
            #endregion
            #region 一堆參數
            List<OrderQueryFullData> OrderDataLists = null;
            OrderQueryFullData item = new OrderQueryFullData();
            List<Holiday> lstHoliday = null; //假日列表
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            var visMons = new List<MonthlyRentData>();//虛擬月租

            int ProjType = 0;   //專案類型
            string ProjID = ""; //專案代碼

            int motoBaseMins = 6;   //機車基本分鐘數
            int carBaseMins = 60;   //汽車基本分鐘數
            int DayMaxMinute = 600; //單日分鐘數上限

            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數
            int Discount = 0;   //要折抵的點數
            int gift_point = 0;         //使用時數(汽車)
            int gift_motor_point = 0;   //使用時數(機車)
            int ActualRedeemableTimePoint = 0; //實際可抵折點數

            DateTime NowTime = DateTime.Now;
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime? FineDate = null;
            bool hasFine = false;   //是否逾時
            DateTime sprSD = Convert.ToDateTime(SiteUV.strSpringSd);
            DateTime sprED = Convert.ToDateTime(SiteUV.strSpringEd);
            bool isSpring = false;  //是否為春節時段

            int TotalRentMinutes = 0;       //總租車時數
            int TotalFineRentMinutes = 0;   //總逾時時數
            int CarRentPrice = 0;       //車輛租金
            int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
            bool UseMonthMode = false;  //false:無月租;true:有月租
            int InsurancePerHours = 0;  //安心服務每小時價
            int etagPrice = 0;          //ETAG費用 20201202 ADD BY ADAM
            int CityParkingPrice = 0;   //城市車旅停車費 20210507 ADD BY YEH
            int End_Mile = 0;           //還車里程
            int PreAmount = 0;          // 預授權金額 20211108 ADD BY YEH
            int DiffAmount = 0;         // 差額 20211108 ADD BY YEH
            float MillageUnit = 0;      // 每公里里程費

            string MonIds = "";//短期月租Id可多筆
            var neverHasFine = new List<int>() { 3, 4 };//路邊,機車不會逾時

            int nor_car_PayDisc = 0;//只有一般時段時總折扣
            int UseOrderPrice = 0;//使用訂金(4捨5入)
            int OrderPrice = 0;//原始訂金

            int GiveMinute = 0;     // 優惠分鐘數
            #endregion
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
            trace.traceAdd(nameof(funInput), funInput);
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
                    string SPName = "usp_BE_GetOrderStatusByOrderNo";
                    SPOutput_Base spOutBase = new SPOutput_Base();
                    SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                    OrderDataLists = new List<OrderQueryFullData>();
                    DataSet ds = new DataSet();
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

                    trace.FlowList.Add("取出訂單資訊");
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
                
                if (flag)
                {
                    trace.traceAdd(nameof(OrderDataLists), OrderDataLists);

                    item = OrderDataLists[0];
                    motoBaseMins = item.BaseMinutes;
                    ProjType = item.ProjType;
                    UseOrderPrice = item.UseOrderPrice;
                    OrderPrice = item.OrderPrice;
                    ProjID = item.ProjID;
                    PreAmount = item.PreAmount;
                    GiveMinute = item.GiveMinute;
                }
                #endregion

                #region 日期判斷
                if (flag)
                {
                    SD = Convert.ToDateTime(item.final_start_time);
                    SD = SD.AddSeconds(SD.Second * -1); //去秒數

                    if (!string.IsNullOrWhiteSpace(item.fine_Time) && Convert.ToDateTime(item.fine_Time) > Convert.ToDateTime("1911-01-01 00:00:00"))
                    {
                        FineDate = Convert.ToDateTime(item.fine_Time);
                        FineDate = FineDate.Value.AddSeconds(ED.Second * -1); //去秒數
                    }

                    if (item.ProjType == 3 || item.ProjType == 4)
                    {
                        ED = Convert.ToDateTime(returnDate);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                        FineDate = null;
                    }
                    else
                    {
                        ED = Convert.ToDateTime(item.stop_time);
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
                        FED = FED,
                        FineDate = FineDate,
                        hasFine = hasFine,
                        lstHoliday = lstHoliday
                    };
                    trace.traceAdd(nameof(timeMark), timeMark);
                    trace.FlowList.Add("SD,ED,FD判斷");
                    #endregion
                }
                #endregion
                
                #region 取得使用中訂閱制月租
                //取得使用中訂閱制月租
                if (flag)
                {
                    List<int> CarCodes = new List<int>() { 0, 3 };

                    int isMoto = -1;
                    if (ProjType == 4)
                        isMoto = 1;
                    else if (CarCodes.Any(x => x == ProjType))
                        isMoto = 0;

                    if (isMoto != -1 && tmpOrder > 0)
                    {
                        var sp_list = monSp.sp_GetSubsBookingMonth(tmpOrder, ref errCode);
                        if (sp_list != null && sp_list.Count() > 0)
                        {
                            List<string> mIds = sp_list.Select(x => x.MonthlyRentId.ToString()).ToList();
                            MonIds = string.Join(",", mIds);
                            trace.traceAdd("UseMonthList", sp_list);
                            trace.FlowList.Add("取得使用中訂閱制月租");
                        }
                    }
                }
                #endregion

                #region 取還車里程
                if (flag)
                {
                    //bool CarFlag = new CarCommonFunc().BE_GetReturnCarMilage(tmpOrder, IDNO, LogID, UserID, ref errCode, ref End_Mile);
                    // 20210219;修改還車里程取得規則
                    if (item.car_mgt_status >= 11)
                    {
                        //已還車
                        //保險起見，再判斷一次是否有還車里程，以防程式崩潰
                        if (item.end_mile > 0)
                        {
                            //用已記錄的還車里程
                            End_Mile = Convert.ToInt32(item.end_mile);
                        }
                        else
                        {
                            SPInput_GetCarMillage SPInput = new SPInput_GetCarMillage
                            {
                                IDNO = IDNO,
                                OrderNo = tmpOrder
                            };
                            string SPName = "usp_GetCarReturnMillage";
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
                        string SPName = "usp_GetCarReturnMillage";
                        SPOutput_GetCarMillage SPOut = new SPOutput_GetCarMillage();
                        SQLHelper<SPInput_GetCarMillage, SPOutput_GetCarMillage> sqlHelp = new SQLHelper<SPInput_GetCarMillage, SPOutput_GetCarMillage>(connetStr);
                        flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOut, ref lstError);
                        baseVerify.checkSQLResult(ref flag, SPOut.Error, SPOut.ErrorCode, ref lstError, ref errCode);
                        if (flag)
                        {
                            End_Mile = SPOut.Millage;
                        }
                    }
                    trace.FlowList.Add("取還車里程");
                    var TraceObject = new
                    {
                        flag = flag,
                        errCode = errCode,
                        End_Mile = End_Mile
                    };
                    trace.traceAdd("End_Mile", TraceObject);
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

                #region 同站專案價格置換
                if (ProjType == 0 && !isSpring) // 同站 AND 非春節期間
                {   // 這段在做：預約是春節專案，但實際用車時間沒有落在春節期間，專案價格要取非春節專案的
                    var ChineseNewYearList = new CommonRepository(connetStr).GetCodeData("ChineseNewYear"); // 春節專案列表
                    if (ChineseNewYearList.Select(x => x.MapCode).Contains(ProjID)) // 訂單的專案為春節專案才進後續判斷
                    {
                        if ((sprSD >= FED) || (sprSD >= ED))    // 春節起日 >= 實際還車時間 OR 春節起日 >= 原始還車時間
                        {
                            var ProjectList = cr_sp.GetCarProject(ProjID, item.CarTypeGroupCode, tmpOrder, IDNO, SD, FED, ProjType, item.CarNo, LogID, ref errMsg);

                            if (ProjectList != null && ProjectList.Count > 0)
                            {
                                var NormalPrice = ProjectList.Where(x => x.PROJID != ProjID).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).ToList().FirstOrDefault();
                                if (NormalPrice != null)
                                {
                                    item.PRICE = Convert.ToInt32(NormalPrice.PRICE / 10);
                                    item.PRICE_H = Convert.ToInt32(NormalPrice.PRICE_H / 10);
                                }
                            }
                        }
                    }
                }
                #endregion

                #region 計算非逾時及逾時時間
                if (ProjType == 4)
                {
                    var xre = billCommon.GetMotoRangeMins(SD, ED, motoBaseMins, DayMaxMinute, lstHoliday);
                    TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                }
                else
                {
                    if (hasFine)
                    {
                        var xre = billCommon.GetCarRangeMins(SD, ED, carBaseMins, DayMaxMinute, lstHoliday);
                        if (xre != null)
                            TotalRentMinutes += Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                        var ov_re = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                        if (ov_re != null)
                        {
                            TotalRentMinutes += Convert.ToInt32(Math.Floor(ov_re.Item1 + ov_re.Item2));
                            //20210913 ADD BY ADAM REASON.補上全部逾時分鐘數加總
                            TotalFineRentMinutes = Convert.ToInt32(Math.Floor(ov_re.Item1 + ov_re.Item2));
                        }
                    }
                    else
                    {
                        var xre = billCommon.GetCarRangeMins(SD, FED, carBaseMins, DayMaxMinute, lstHoliday);
                        if (xre != null)
                            TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                    }
                }
                TotalRentMinutes = TotalRentMinutes > 0 ? TotalRentMinutes : 0;
                #endregion

                #region 取得虛擬月租
                if (isSpring)   //春節專案才產生虛擬月租
                {
                    var ibiz_vMon = new IBIZ_SpringInit()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        ProjID = ProjID,
                        ProjType = ProjType,
                        CarType = item.CarTypeGroupCode,
                        SD = vsd,
                        ED = ved,
                        ProDisPRICE = ProjType == 4 ? item.MinuteOfPrice : item.PRICE,
                        ProDisPRICE_H = ProjType == 4 ? item.MinuteOfPrice : item.PRICE_H,
                        LogID = LogID
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
                                item.MinuteOfPrice = Convert.ToSingle(vmonRe.PRICE);
                            }
                            else
                            {
                                item.PRICE = Convert.ToInt32(Math.Floor(vmonRe.PRICE));
                                item.PRICE_H = Convert.ToInt32(Math.Floor(vmonRe.PRICE_H));
                            }

                            trace.traceAdd(nameof(vmonRe), vmonRe);
                            trace.FlowList.Add("新增虛擬月租");
                        }
                    }
                }
                #endregion

                #region 汽車計費資訊
                int car_payAllMins = 0; //全部計費租用分鐘
                int car_payInMins = 0;  //未超時計費分鐘
                int car_payOutMins = 0; //超時分鐘-顯示用
                int car_inPrice = 0;    //未超時費用
                int car_outPrice = 0;   //超時費用
                int UseGiveMinute = 0;  // 使用標籤優惠分鐘數

                if (flag)
                {
                    if (ProjType != 4)
                    {
                        if (hasFine)    // 逾時
                        {
                            var xre = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                            if (xre != null)
                                car_payOutMins = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));    //逾時分鐘
                            //逾時費用
                            car_outPrice = billCommon.CarRentCompute(ED, FED, item.WeekdayPrice, item.HoildayPrice, 6, lstHoliday, true, 0);
                            car_payAllMins += car_payOutMins;

                            var car_re = billCommon.CarRentInCompute(SD, ED, item.PRICE, item.PRICE_H, carBaseMins, 10, lstHoliday, new List<MonthlyRentData>(), Discount, item.FirstFreeMins, item.GiveMinute);
                            if (car_re != null)
                            {
                                trace.traceAdd(nameof(car_re), car_re);
                                car_payAllMins += car_re.RentInMins;
                                car_payInMins = car_re.RentInMins;
                                car_inPrice = car_re.RentInPay;
                                nor_car_PayDisc = car_re.useDisc;
                                UseGiveMinute = car_re.UseGiveMinute;
                            }
                        }
                        else
                        {
                            var car_re = billCommon.CarRentInCompute(SD, FED, item.PRICE, item.PRICE_H, carBaseMins, 10, lstHoliday, new List<MonthlyRentData>(), Discount, item.FirstFreeMins, item.GiveMinute);
                            if (car_re != null)
                            {
                                trace.traceAdd(nameof(car_re), car_re);
                                car_payAllMins += car_re.RentInMins;
                                car_payInMins = car_re.RentInMins;
                                car_inPrice = car_re.RentInPay;
                                nor_car_PayDisc = car_re.useDisc;
                                UseGiveMinute = car_re.UseGiveMinute;
                            }
                        }
                        trace.FlowList.Add("汽車計費資訊(非月租)");
                    }
                }
                #endregion                

                #region 與短租查時數
                if (flag && !isSpring)
                {
                    var inp = new IBIZ_NPR270Query()
                    {
                        IDNO = IDNO
                    };
                    var re270 = cr_com.NPR270Query(inp);
                    if (re270 != null)
                    {
                        trace.traceAdd("NPR270", re270);
                        flag = re270.flag;
                        MotorPoint = re270.MotorPoint;
                        CarPoint = re270.CarPoint;
                    }
                    trace.FlowList.Add("與短租查時數");

                    #region mark-判斷輸入的點數有沒有超過總點數
                    //判斷輸入的點數有沒有超過總點數
                    //if (ProjType == 4)
                    //{
                    //    if (Discount > 0 && Discount < item.BaseMinutes)   // 折抵點數 < 基本分鐘數
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
                if (flag && ProjType != 4)    //汽車才需要進來
                {
                    //ETAG查詢失敗也不影響流程
                    var orderNo = "H" + tmpOrder.ToString().PadLeft(tmpOrder.ToString().Length, '0');
                    var input = new IBIZ_ETagCk()
                    {
                        OrderNo = orderNo
                    };
                    var etag_re = cr_com.ETagCk(input);
                    if (etag_re != null)
                    {
                        trace.traceAdd(nameof(etag_re), etag_re);
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
                    outputApi.CarRent = new CarRentBase();
                    outputApi.DiscountAlertMsg = "";
                    outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
                    outputApi.IsMotor = (ProjType == 4) ? 1 : 0;    //是否為機車
                    outputApi.MonthRent = new MonthRentBase();  //月租資訊
                    outputApi.MotorRent = new MotorRentBase();  //機車資訊
                    outputApi.PayMode = (ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
                    outputApi.ProType = ProjType;
                    outputApi.Rent = new RentBase() //訂單基本資訊
                    {
                        BookingEndDate = ED.ToString("yyyy-MM-dd HH:mm:ss"),
                        BookingStartDate = SD.ToString("yyyy-MM-dd HH:mm:ss"),
                        CarNo = item.CarNo,
                        RedeemingTimeCarInterval = "0",
                        RedeemingTimeMotorInterval = "0",
                        RedeemingTimeInterval = "0",
                        RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
                        RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
                    };

                    if (ProjType == 4)
                    {
                        TotalPoint = (CarPoint + MotorPoint);
                        outputApi.MotorRent = new MotorRentBase()
                        {
                            BaseMinutePrice = item.BaseMinutesPrice,
                            BaseMinutes = item.BaseMinutes,
                            MinuteOfPrice = item.MinuteOfPrice
                        };
                    }
                    else
                    {
                        TotalPoint = CarPoint;
                        outputApi.CarRent = new CarRentBase()
                        {
                            HoildayOfHourPrice = item.PRICE_H,
                            HourOfOneDay = 10,
                            WorkdayOfHourPrice = item.PRICE,
                            WorkdayPrice = item.PRICE * 10,
                            MilUnit = item.MilageUnit,
                            HoildayPrice = item.PRICE_H * 10
                        };
                    }
                    //20201201 ADD BY ADAM REASON.轉乘優惠
                    TransferPrice = item.init_TransDiscount;
                    trace.FlowList.Add("建空模");
                }
                #endregion

                #region 停車費
                // 20210510 UPD BY YEH REASON.把車麻吉停車費打開
                if (flag && ProjType != 4)
                {
                    //檢查有無車麻吉停車費用
                    var input = new IBIZ_CarMagi()
                    {
                        LogID = LogID,
                        CarNo = item.CarNo,
                        SD = SD,
                        ED = FED.AddDays(1),
                        OrderNo = tmpOrder
                    };
                    try
                    {
                        var magi_Re = cr_com.CarMagi(input);
                        if (magi_Re != null)
                        {
                            trace.traceAdd(nameof(magi_Re), magi_Re);
                            flag = magi_Re.flag;
                            outputApi.Rent.ParkingFee = magi_Re.ParkingFee;
                        }
                    }
                    catch (Exception ex)
                    {
                        trace.BaseMsg += "CarMachii_Error:" + ex.Message;
                    }
                    trace.FlowList.Add("車麻吉");
                }

                //20210510 ADD BY YEH REASON.串接CityParking停車場
                if (flag && ProjType != 4)
                {
                    string SPName = "usp_GetCityParkingFee";
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
                if (flag)
                {
                    var input = new IBIZ_MonthRent()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        intOrderNO = tmpOrder,
                        ProjType = item.ProjType,
                        MotoBasePrice = item.BaseMinutesPrice,
                        MotoDayMaxMins = DayMaxMinute,  //資料庫缺欄位先給預設值
                        MinuteOfPrice = item.MinuteOfPrice,
                        MinuteOfPriceH = item.MinuteOfPriceH,
                        hasFine = hasFine,
                        SD = SD,
                        ED = ED,
                        FED = FED,
                        MotoBaseMins = motoBaseMins,
                        lstHoliday = lstHoliday,
                        Discount = Discount,
                        PRICE = item.PRICE,
                        PRICE_H = item.PRICE_H,
                        carBaseMins = carBaseMins,
                        MaxPrice = item.MaxPrice,    // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
                        FirstFreeMins = item.FirstFreeMins,
                        MonIds = MonIds,
                        GiveMinute = item.GiveMinute
                    };

                    if (visMons != null && visMons.Count() > 0)
                        input.VisMons = visMons;

                    trace.traceAdd("monIn", input);

                    var mon_re = cr_com.MonthRentSave(input);
                    if (mon_re != null)
                    {
                        trace.traceAdd(nameof(mon_re), mon_re);
                        flag = mon_re.flag;
                        UseMonthMode = mon_re.UseMonthMode;
                        outputApi.IsMonthRent = mon_re.IsMonthRent;
                        if (UseMonthMode)
                        {
                            carInfo = mon_re.carInfo;
                            Discount = mon_re.useDisc;
                            monthlyRentDatas = mon_re.monthlyRentDatas;
                            UseGiveMinute = carInfo.UseGiveMinute;

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
                    if (ProjType == 4)
                    {
                        if (UseMonthMode)   //true:有月租;false:無月租
                        {
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                        }
                        else
                        {
                            // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
                            var xre = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPriceH, motoBaseMins, DayMaxMinute, lstHoliday, new List<MonthlyRentData>(), Discount, DayMaxMinute, item.MaxPrice, item.BaseMinutesPrice, item.FirstFreeMins, item.GiveMinute);
                            if (xre != null)
                            {
                                carInfo = xre;
                                outputApi.Rent.CarRental = xre.RentInPay;
                                carInfo.useDisc = xre.useDisc;
                                UseGiveMinute = carInfo.UseGiveMinute;
                            }
                            trace.FlowList.Add("機車非月租租金計算");
                        }

                        outputApi.Rent.RentBasicPrice = item.BaseMinutesPrice;
                    }
                    else
                    {
                        if (UseMonthMode)   //true:有月租;false:無月租
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
                            if (UseMonthMode)   //true:有月租;false:無月租
                            {

                            }
                            else
                            {
                                //非月租折扣
                                CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
                                trace.FlowList.Add("汽車非月租折扣扣除");
                            }
                        }

                        #region 安心服務
                        // 20210908 UPD BY YEH REASON:使用安心服務，安心服務每小時金額 = 主承租人每小時價格 + 副承租人每小時費率總和
                        InsurancePerHours = item.Insurance == 1 ? (Convert.ToInt32(item.InsurancePerHours) + item.JointInsurancePerHour) : 0;
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
                        #endregion

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = item.BaseMinutesPrice;

                        #region 里程費
                        MillageUnit = (item.MilageUnit <= 0) ? Mildef : item.MilageUnit;
                        outputApi.CarRent.MilUnit = MillageUnit;
                        // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                        // 20210121;里程數>1000公里的判斷移除
                        if (item.start_mile == 0 || End_Mile == 0 || ((End_Mile - item.start_mile) < 0))
                        {
                            outputApi.Rent.MileageRent = 0;
                        }
                        else
                        {
                            outputApi.Rent.MileageRent = Convert.ToInt32(MillageUnit * (End_Mile - item.start_mile));
                        }
                        trace.FlowList.Add("里程費計算");
                        #endregion
                    }

                    outputApi.Rent.OvertimeRental = car_outPrice;
                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                    outputApi.Rent.TransferPrice = (item.init_TransDiscount > 0) ? item.init_TransDiscount : 0;
                    //20201202 ADD BY ADAM REASON.ETAG費用
                    outputApi.Rent.ETAGRental = etagPrice;

                    #region 轉乘優惠只能抵租金
                    int xCarRental = outputApi.Rent.CarRental;
                    int xTransferPrice = outputApi.Rent.TransferPrice;
                    int FinalTransferPrice = (xCarRental - xTransferPrice) > 0 ? xTransferPrice : xCarRental;
                    outputApi.Rent.TransferPrice = FinalTransferPrice;
                    xCarRental = (xCarRental - FinalTransferPrice);
                    #endregion

                    #region 總價計算
                    // 總價 = 車輛租金 + 停車費 + 里程費 + 逾時費用 + 安心服務費用 + 安心服務費用(逾時) + ETAG費用
                    var xTotalRental = xCarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice + outputApi.Rent.ETAGRental;

                    #region 春節訂金
                    //20220126 ADD BY ADAM REASON.春節訂金改用預授權金額帶入
                    outputApi.UseOrderPrice = PreAmount;
                    outputApi.FineOrderPrice = UseOrderPrice;//改罰金

                    //如果有春節訂金就要把罰金加上去
                    xTotalRental += ((OrderPrice > 0) ? UseOrderPrice : 0);

                    //if (xTotalRental < 0)
                    if (UseOrderPrice > 0)
                    {
                        int orderNo = Convert.ToInt32(item.OrderNo);
                        carRepo.UpdNYPayList(orderNo, UseOrderPrice);
                    }
                    #endregion

                    FinalPrice = xTotalRental;
                    DiffAmount = xTotalRental - PreAmount;  // 差額 = 訂單總價 - 預授權金額

                    xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                    outputApi.Rent.TotalRental = xTotalRental;
                    outputApi.Rent.PreAmount = PreAmount;
                    outputApi.Rent.DiffAmount = DiffAmount;
                    trace.FlowList.Add("總價計算");
                    #endregion

                    #region 修正輸出欄位
                    if (ProjType == 4)
                    {
                        outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();
                        // 20211209 UPD BY YEH REASON:給前端顯示的租用時數改用可折抵時數
                        outputApi.Rent.RentalTimeInterval = carInfo.RentInMins.ToString();//租用時數(未逾時)
                        //outputApi.Rent.RentalTimeInterval = carInfo.DiscRentInMins.ToString();  // 可折抵時數
                        outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                        outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                        outputApi.Rent.UseGiveMinute = carInfo.UseGiveMinute;
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
                            outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();
                            outputApi.Rent.RentalTimeInterval = carInfo.RentInMins.ToString();//租用時數(未逾時)
                            outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                            outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                            outputApi.Rent.UseGiveMinute = carInfo.UseGiveMinute;
                            outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
                            gift_point = carInfo.useDisc;
                        }
                        else
                        {
                            outputApi.Rent.ActualRedeemableTimeInterval = (car_payInMins - UseGiveMinute).ToString(); //可折抵租用時數
                            outputApi.Rent.RentalTimeInterval = car_payInMins.ToString(); //租用時數(未逾時)
                            outputApi.Rent.UseNorTimeInterval = Discount.ToString();
                            outputApi.Rent.UseGiveMinute = UseGiveMinute;
                            outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount - UseGiveMinute).ToString();    //未逾時折抵後的租用時數
                            gift_point = nor_car_PayDisc;
                        }

                        gift_motor_point = 0;
                        outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                    }
                    trace.FlowList.Add("修正輸出欄位");
                    #endregion

                    #region 儲存使用月租時數
                    if (!string.IsNullOrWhiteSpace(MonIds) && carInfo != null && (carInfo.useMonthDiscW > 0 || carInfo.useMonthDiscH > 0))
                    {
                        string sp_errCode = "";
                        var monthId = MonIds.Split(',').Select(x => Convert.ToInt64(x)).FirstOrDefault();
                        var spin = new SPInput_SetSubsBookingMonth()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            OrderNo = tmpOrder,
                            MonthlyRentId = monthId
                        };
                        if (ProjType == 4)
                            spin.UseMotoTotalMins = carInfo.useMonthDiscW + carInfo.useMonthDiscH;
                        else
                        {
                            spin.UseCarWDHours = carInfo.useMonthDiscW;
                            spin.UseCarHDHours = carInfo.useMonthDiscH;
                        }
                        monSp.sp_SetSubsBookingMonth(spin, ref sp_errCode);
                        trace.traceAdd("SetSubsBookingMonth", new { spin, sp_errCode });
                    }
                    #endregion

                    #region sp存檔
                    string SPName = "usp_BE_CalFinalPrice_U02";
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
                        Etag = outputApi.Rent.ETAGRental,
                        parkingFee = outputApi.Rent.ParkingFee,
                        TransDiscount = outputApi.Rent.TransferPrice,
                        gift_point = gift_point,
                        gift_motor_point = gift_motor_point,
                        monthly_workday = carInfo.useMonthDiscW / 60,
                        monthly_holiday = carInfo.useMonthDiscH / 60,
                        EndMile = End_Mile,
                        DiffAmount = DiffAmount,
                        APIName = funName,
                        UseGiveMinute = UseGiveMinute,
                        LogID = LogID
                    };

                    SPOutput_Base SPOutput = new SPOutput_Base();
                    SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base>(connetStr);
                    flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);

                    #region trace
                    trace.traceAdd(nameof(TotalRentMinutes), TotalRentMinutes);
                    trace.traceAdd(nameof(Discount), Discount);
                    trace.traceAdd(nameof(SPInput), SPInput);
                    trace.traceAdd(nameof(carInfo), carInfo);
                    trace.traceAdd(nameof(outputApi), outputApi);
                    #endregion

                    trace.FlowList.Add("sp存檔");
                    #endregion
                }
                #endregion

                #region 寫入錯誤Log
                if (!flag)
                {
                    trace.objs.Add(nameof(errCode), errCode);
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                flag = false;
                errCode = "ERR902";
            }
            finally
            {
                trace.objs = trace.getObjs();
                var errItem = new TraceLogVM()
                {
                    ApiId = 127,
                    ApiMsg = JsonConvert.SerializeObject(trace),
                    ApiNm = funName,
                    CodeVersion = trace.codeVersion,
                    FlowStep = trace.FlowStep(),
                    OrderNo = trace.OrderNo,
                    TraceType = string.IsNullOrWhiteSpace(trace.BaseMsg) ? (flag ? eumTraceType.mark : eumTraceType.followErr) : eumTraceType.exception
                };
                carRepo.AddTraceLog(errItem);
            }

            return flag;
        }
        #endregion

        #region 換電獎勵
        public bool DoNPR380(string IDNO, int Reward, long OrderNo, long LogID, ref string errCode)
        {
            bool flag = false;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            WebAPIOutput_NPR380Save wsOutput = new WebAPIOutput_NPR380Save();
            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
            flag = wsAPI.NPR380Save(IDNO, Reward.ToString(), "H" + OrderNo.ToString().PadLeft(OrderNo.ToString().Length, '0'), ref wsOutput);

            //存檔
            string SPName = "usp_SaveNPR380Result";
            SPOutput_Base NPR380Output = new SPOutput_Base();
            SPInput_SetRewardResult NPR380Input = new SPInput_SetRewardResult()
            {
                OrderNo = OrderNo,
                Result = flag == true ? 1 : 0,
                LogID = LogID
            };
            SQLHelper<SPInput_SetRewardResult, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_SetRewardResult, SPOutput_Base>(connetStr);
            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, NPR380Input, ref NPR380Output, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref NPR380Output, ref lstError, ref errCode);

            return flag;
        }
        #endregion
    }
}