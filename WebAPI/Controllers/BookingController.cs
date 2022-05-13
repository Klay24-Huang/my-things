using Domain.Common;
using Domain.Flow.CarRentCompute;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Notification;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Bill;
using Domain.SP.Output.Booking;
using Domain.SP.Output.Wallet;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 預約
    /// </summary>
    public class BookingController : ApiController
    {
        private readonly string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private readonly string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();
        private readonly string NPR330Flg = ConfigurationManager.AppSettings["NPR330Flg"].ToString();       //20220413 ADD BY ADAM REASON.欠費查詢開關

        CommonFunc baseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoBooking(Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var monSp = new MonSubsSp();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = httpContext.Request.Headers["Authorization"] ?? ""; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BookingController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_Booking apiInput = null;
            OAPI_Booking outputApi = null;
            ProjectPriceBase priceBase = null;
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository = new StationAndCarRepository(connetStr);
            ProjectRepository projectRepository = new ProjectRepository(connetStr);
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            DateTime LastPickCarTime = SDate.AddMinutes(15);
            string CarType = "", StationID = "";
            Int16 PayMode = 0;
            Int16 ProjType = 5;
            int NormalRentDefaultPickTime = 15;
            int AnyRentDefaultPickTime = 30;
            int MotorRentDefaultPickTime = 30;
            int price = 0, InsurancePurePrice = 0;
            int InsurancePerHours = 0;
            string IDNO = "";
            string CarTypeCode = "";
            string SPName = "";
            string error = "";

            List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
            TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
            OFN_CreditAuthResult AuthOutput = new OFN_CreditAuthResult();
            SPOutput_Booking spOut = new SPOutput_Booking();
            BillCommon billCommon = new BillCommon();
            CommonService commonService = new CommonService();
            CreditAuthComm creditAuthComm = new CreditAuthComm();
            WalletSp walletSp = new WalletSp();
            bool CreditFlag = true;     // 信用卡綁卡
            string CreditErrCode = "";  // 信用卡綁卡錯誤訊息
            bool WalletFlag = false;    // 綁定錢包
            int WalletNotice = 0;       // 錢包餘額不足通知 (0:不顯示 1:顯示)
            int WalletAmout = 0;        // 錢包餘額
            int motoPreAmt = 0;         // 機車預扣金額

            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_Booking>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.ProjID))
                {
                    flag = false;
                    errCode = "ERR900";
                }

                SDate = apiInput.SDate == "" ? DateTime.Now : Convert.ToDateTime(apiInput.SDate);
                EDate = apiInput.EDate == "" ? DateTime.Now.AddHours(1) : Convert.ToDateTime(apiInput.EDate);
                lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
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
            #region 防呆第二段
            if (flag)
            {
                ProjectInfo obj = projectRepository.GetProjectInfo(apiInput.ProjID);
                if (obj == null)
                {
                    flag = false;
                    errCode = "ERR164";
                }
                ProjType = Convert.ToInt16(obj.PROJTYPE);
                PayMode = Convert.ToInt16(obj.PayMode);
                CarTypeCode = apiInput.CarType;
                if (ProjType > 0)
                {
                    if (string.IsNullOrWhiteSpace(apiInput.CarNo)) //路邊及機車
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        CarData CarObj = _repository.GetCarData(apiInput.CarNo);
                        if (CarObj == null)
                        {
                            flag = false;
                            errCode = "ERR165";
                        }
                        else
                        {
                            CarType = CarObj.CarType;
                            StationID = CarObj.StationID;
                        }
                    }
                    if (flag)
                    {
                        if (ProjType == 3 || ProjType == 4)
                        {
                            //路邊調整取車時間APP僅有十進位顯示故去除後面尾數，以免影響預授權判斷
                            int diff = SDate.Minute % 10;
                            string timeString = diff > 0 ? SDate.AddMinutes(10 - diff).ToString("yyyy/MM/dd HH:mm") : SDate.ToString("yyyy/MM/dd HH:mm");
                            DateTime.TryParse(timeString, out SDate);
                            //20201212 ADD BY ADAM REASON.路邊改預設一天
                            EDate = SDate.AddDays(1);
                        }
                        else
                        {
                            EDate = SDate.AddDays(7);
                        }
                    }
                }
                else
                {
                    flag = baseVerify.CheckDate(apiInput.SDate, apiInput.EDate, ref errCode, ref SDate, ref EDate);//同站

                    if (flag)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.StationID) || string.IsNullOrWhiteSpace(apiInput.CarType))
                        {
                            flag = false;
                            errCode = "ERR900";
                        }
                        if (flag)
                        {
                            CarType = apiInput.CarType;
                            StationID = apiInput.StationID;
                        }
                    }
                }
            }
            #endregion
            #region TB
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion
            #region 取得安心服務每小時價格
            if (flag)
            {
                var result = commonService.GetInsurancePrice(IDNO, CarTypeCode, LogID, ref errCode);
                flag = result.checkFlag;
                InsurancePerHours = result.InsurancePerHours;
            }
            #endregion
            #region 檢查信用卡是否綁卡、錢包是否開通
            if (flag)
            {
                #region 檢查錢包是否開通
                string spName = "usp_CreditAndWalletQuery_Q01";
                SPInput_CreditAndWalletQuery spWalletInput = new SPInput_CreditAndWalletQuery
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOut_CreditAndWalletQuery spWalletOut = new SPOut_CreditAndWalletQuery();
                SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery> sqlHelp = new SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery>(connetStr);
                bool WalletCheck = sqlHelp.ExecuteSPNonQuery(spName, spWalletInput, ref spWalletOut, ref lstError);
                baseVerify.checkSQLResult(ref WalletCheck, spWalletOut.Error, spWalletOut.ErrorCode, ref lstError, ref error);
                if (WalletCheck)
                {
                    WalletFlag = spWalletOut.WalletStatus == "2";
                    WalletAmout = spWalletOut.WalletAmout;
                }
                #endregion

                #region 檢查信用卡是否綁卡
                var result = creditAuthComm.CheckTaishinBindCard(ref CreditFlag, IDNO, ref CreditErrCode);
                #endregion

                if (ProjType == 4)  // 4:機車
                {
                    int.TryParse(new CommonRepository(connetStr).GetCodeData("MotorPreAmt").FirstOrDefault().MapCode, out motoPreAmt);  //從TB_CODE抓取機車預扣金額

                    if (!CreditFlag && !WalletFlag) // 沒綁信用卡 也 沒開通錢包，就回錯誤訊息
                    {
                        flag = false;
                        errCode = "ERR292";
                    }
                    else if (!CreditFlag && WalletFlag) // 沒綁信用卡 但 有開通錢包，要給APP值顯示通知
                    {

                        WalletNotice = 1;
                        if (WalletAmout < motoPreAmt)   // 錢包餘額 < 50元 不給預約
                        {
                            flag = false;
                            errCode = "ERR934";
                        }
                    }
                }
                else //0:同站 3:路邊
                {
                    flag = CreditFlag;
                    errCode = CreditErrCode;
                }
            }
            #endregion
            #region 檢查欠費
            if (flag && NPR330Flg == "Y")   //20220413 ADD BY ADAM REASON.欠費查詢開關
            {
                int TAMT = 0;
                ContactComm contract = new ContactComm();
                flag = contract.CheckNPR330(IDNO, LogID, ref TAMT);
                if (flag && TAMT > 0)
                {
                    flag = false;
                    errCode = "ERR233";
                }
                else
                {
                    errCode = "ERR161";
                }
            }
            #endregion
            #region 取得最晚取車時間
            if (flag)
            {
                if (ProjType == 3)  // 路邊汽車
                {
                    LastPickCarTime = SDate.AddMinutes(AnyRentDefaultPickTime);
                }
                else if (ProjType == 4) // 路邊機車
                {
                    LastPickCarTime = SDate.AddMinutes(MotorRentDefaultPickTime);
                }
                else
                {
                    LastPickCarTime = SDate.AddMinutes(NormalRentDefaultPickTime);
                }
            }
            #endregion
            #region 計算安心服務價格&預估租金
            if (flag)
            {
                //20201103 ADD BY SS ADAM REASON.計算安心服務價格
                InsurancePurePrice = (apiInput.Insurance == 1) ? Convert.ToInt32(billCommon.CalSpread(SDate, EDate, InsurancePerHours * 10, InsurancePerHours * 10, lstHoliday)) : 0;
                //計算預估租金
                if (ProjType < 4)
                {
                    if (ProjType == 0)
                    {
                        priceBase = projectRepository.GetProjectPriceBase(apiInput.ProjID, CarType, ProjType);
                    }
                    else
                    {
                        priceBase = projectRepository.GetProjectPriceBase(apiInput.ProjID, apiInput.CarNo, ProjType);
                    }

                    #region 春節汽車
                    List<int> carProTypes = new List<int>() { 0, 3 };
                    var isSpring = cr_com.isSpring(SDate, EDate);
                    if (isSpring && carProTypes.Any(x => x == ProjType))
                    {
                        var xre = GetPriceBill(apiInput.ProjID, ProjType, apiInput.CarType, IDNO, LogID, lstHoliday, SDate, EDate, priceBase.PRICE, priceBase.PRICE_H, funName);
                        price = xre;
                    }
                    else
                        price = billCommon.CarRentCompute(SDate, EDate, priceBase.PRICE, priceBase.PRICE_H, 10, lstHoliday);
                    #endregion
                }
                else
                {
                    ProjectPriceOfMinuteBase projectPriceOfMinute = projectRepository.GetProjectPriceBaseByMinute(apiInput.ProjID, apiInput.CarNo);
                    price = Convert.ToInt32(projectPriceOfMinute.Price);
                }
            }
            #endregion

            #region 預約
            if (flag)
            {
                SPName = "usp_Booking";
                SPInput_Booking spInput = new SPInput_Booking()
                {
                    IDNO = IDNO,
                    ProjID = apiInput.ProjID,
                    ProjType = ProjType,
                    StationID = StationID,
                    CarType = CarType,
                    RStationID = StationID,
                    SD = SDate,
                    ED = EDate,
                    StopPickTime = LastPickCarTime,
                    Price = price,
                    CarNo = (string.IsNullOrWhiteSpace(apiInput.CarNo)) ? "" : apiInput.CarNo,
                    Token = Access_Token,
                    Insurance = apiInput.Insurance,
                    InsurancePurePrice = InsurancePurePrice,
                    PayMode = PayMode,
                    LogID = LogID,
                    PhoneLat = apiInput.PhoneLat,
                    PhoneLon = apiInput.PhoneLon
                };
                SQLHelper<SPInput_Booking, SPOutput_Booking> sqlHelp = new SQLHelper<SPInput_Booking, SPOutput_Booking>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

            #region 寫入訂單對應訂閱制月租
            if (flag && spOut.haveCar == 1 && apiInput.MonId > 0)
            {
                var sp_in = new SPInput_SetSubsBookingMonth()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    OrderNo = spOut.OrderNum,
                    MonthlyRentId = apiInput.MonId
                };
                monSp.sp_SetSubsBookingMonth(sp_in, ref errCode);
            }
            #endregion
            #region 預扣機制
            if (flag && spOut.haveCar == 1)
            {
                var trace = new TraceCom();
                trace.traceAdd("apiIn", value);
                int defaultPayMode = 0;       //預設支付方式(0:信用卡 1:錢包 4:和泰PAY)
                int preAuthAmt = 0;           //預扣金額
                bool payFlag = false;         //扣款結果
                bool authNow = false;         //是否立即刷卡
                int actualPayMode = -1;       //實際支付方式(0:信用卡 1:錢包 4:和泰PAY)
                bool walletEnough = false;    //錢包餘額是否足夠
                bool bookNDaysAgo = false;    //同站取車前6小時之前預約單(走排程授權)

                #region 取得預設支付方式
                SPInput_GetPayInfo getPayInfo = new SPInput_GetPayInfo()
                {
                    LogID = LogID,
                    IDNO = IDNO,
                    InputSource = 1,
                    Token = Access_Token
                };
                var wallet = walletSp.sp_GetPayInfo(getPayInfo, ref errCode);
                defaultPayMode = wallet.PayMode[0].DefPayMode;
                trace.traceAdd("sp_GetPayInfo", new { getPayInfo, defaultPayMode });
                trace.FlowList.Add("預設支付方式");
                #endregion
                #region 計算預扣款金額
                if (ProjType == 0 || ProjType == 3)
                {
                    SPOutput_OrderForPreAuth orderData = commonService.GetOrderForPreAuth(spOut.OrderNum);
                    string notHandle = new CommonRepository(connetStr).GetCodeData("PreAuth").FirstOrDefault().MapCode;//預授權不處理專案
                    if (!notHandle.Contains(apiInput.ProjID) && orderData.DoPreAuth == 1)
                    {
                        EstimateData estimateData = new EstimateData()
                        {
                            ProjID = orderData.ProjID,
                            SD = orderData.SD,
                            ED = orderData.ED,
                            CarNo = orderData.CarNo,
                            CarTypeGroupCode = orderData.CarTypeGroupCode,
                            WeekdayPrice = orderData.PRICE,
                            HoildayPrice = orderData.PRICE_H,
                            Insurance = apiInput.Insurance,
                            InsurancePerHours = orderData.InsurancePerHours,
                            ProjType = orderData.ProjType
                        };

                        if (ProjType == 0)
                        {
                            //同站取車前6小時前預約走排程授權
                            DateTime checkDate = SDate.AddHours(-6);
                            bookNDaysAgo = DateTime.Compare(DateTime.Now, checkDate) < 0;
                        }
                        else if (ProjType == 3)
                        {
                            int triaHour = 1;  //2021/12/16 企劃要求路邊只預收1小時授權金
                            estimateData.ED = SDate.AddHours(triaHour);
                        }

                        commonService.EstimatePreAuthAmt(estimateData, out EstimateDetail estimateDetail);
                        //需扣掉春節訂金
                        preAuthAmt = orderData.PreAuthAmt == 0 ? estimateDetail.estimateAmt : estimateDetail.estimateAmt - orderData.PreAuthAmt;
                        trace.traceAdd("GetEsimateAuthAmt", new { estimateData, estimateDetail, preAuthAmt, wallet.PayMode });
                        trace.FlowList.Add("計算預授權金");
                    }
                }
                string TradeType = ProjType == 4 ? "PreAuth_Motor" : "PreAuth_Car";
                preAuthAmt = ProjType == 4 ? motoPreAmt : preAuthAmt;
                #endregion
                #region 扣款流程
                if (preAuthAmt > 0)
                {
                    if (defaultPayMode == 1 && WalletAmout >= preAuthAmt) //預設錢包 餘額足
                    {
                        payFlag = DoPreAuth(defaultPayMode, spOut.OrderNum, IDNO, preAuthAmt, funName, LogID, TradeType, ProjType, Access_Token, ref error, ref AuthOutput);
                        actualPayMode = AuthOutput?.CheckoutMode ?? -1;
                        walletEnough = payFlag;

                        OFN_CreditAuthResult walletPayOut = AuthOutput;
                        trace.traceAdd("DoWalletPay", new { defaultPayMode, payFlag, walletPayOut, actualPayMode, error });
                        trace.FlowList.Add("錢包預扣款");
                    }

                    if (!walletEnough) //走信用卡支付or回餘額不足
                    {
                        if (ProjType == 4 && defaultPayMode != 1) //機車預設不為錢包，不取信用卡預授權
                        {
                            flag = true;
                            preAuthAmt = 0;
                        }
                        else if (ProjType == 4 && defaultPayMode == 1 && !CreditFlag) //機車有錢包沒綁卡 回餘額不足
                        {
                            payFlag = false; //走取消訂單
                        }
                        else if (ProjType == 4 && defaultPayMode == 1 || ProjType == 3) //路邊&機車&同站6小時前訂單  支付方式為錢包且餘額不足，轉信用卡支付
                        {
                            authNow = true;
                        }
                        else if (ProjType == 0) //同站6小時後訂單 支付方式為錢包且餘額不足，寫入排程取信用卡預授權
                        {
                            authNow = !bookNDaysAgo;
                        }
                    }
                }
                #endregion
                #region 立即刷卡
                if (authNow && preAuthAmt > 0)
                {
                    payFlag = DoPreAuth(4, spOut.OrderNum, IDNO, preAuthAmt, funName, LogID, TradeType, ProjType, Access_Token, ref errCode, ref AuthOutput); //和泰PAY為優先
                    actualPayMode = AuthOutput?.CheckoutMode ?? -1;

                    trace.traceAdd("DoPreAuth", new { payFlag, actualPayMode, AuthOutput, errCode });
                    trace.FlowList.Add("刷卡授權");
                }
                #endregion            
                #region 更新資料
                if (preAuthAmt > 0)
                {
                    #region 寫入預授權
                    SPInput_InsOrderAuthAmount input_AuthAmt = new SPInput_InsOrderAuthAmount()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        Token = Access_Token,
                        AuthType = 1,
                        final_price = preAuthAmt,
                        OrderNo = spOut.OrderNum,
                        PRGName = funName,
                        Status = payFlag ? 2 : bookNDaysAgo ? 0 : 2
                    };

                    actualPayMode = bookNDaysAgo ? -1 : actualPayMode;
                    switch (actualPayMode)
                    {
                        case 0:
                            input_AuthAmt.CardType = 1;
                            break;
                        case 1:
                            input_AuthAmt.CardType = 2;
                            break;
                        case 4:
                            input_AuthAmt.CardType = 0;
                            break;
                        default:
                            input_AuthAmt.CardType = -1;
                            break;
                    }

                    input_AuthAmt.BankTradeNo = AuthOutput?.BankTradeNo ?? "";
                    input_AuthAmt.MerchantTradNo = AuthOutput?.Transaction_no ?? "";
                    commonService.sp_InsOrderAuthAmount(input_AuthAmt, ref error);

                    trace.traceAdd("sp_InsOrderAuthAmount", new { input_AuthAmt, error });
                    trace.FlowList.Add("寫入預授權");
                    #endregion

                    #region 寫入訂單備註
                    SPInput_OrderExtInfo spInput = new SPInput_OrderExtInfo()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        PRGID = funName,
                        OrderNo = spOut.OrderNum,
                        CheckoutMode = -1,
                        PreAuthMode = defaultPayMode  //預設支付方式
                    };
                    commonService.InsertOrderExtInfo(spInput, ref error, ref lstError);
                    trace.traceAdd("InsertOrderExtInfo", new { spInput, error });
                    trace.FlowList.Add("寫入訂單備註");
                    #endregion
                }
                #endregion
                #region 新增推播
                if (payFlag)
                {
                    string interSecion;
                    if (actualPayMode == 1)
                    {
                        interSecion = $"以錢包支付";
                    }
                    else
                    {
                        string cardNo = AuthOutput.CardNo.Substring((AuthOutput.CardNo.Length - 4) > 0 ? AuthOutput.CardNo.Length - 4 : 0);
                        interSecion = $"以末四碼{cardNo}信用卡";
                    }

                    SPInput_InsPersonNotification input_Notification = new SPInput_InsPersonNotification()
                    {
                        OrderNo = Convert.ToInt32(spOut.OrderNum),
                        IDNO = IDNO,
                        LogID = LogID,
                        NType = 19,
                        STime = DateTime.Now.AddSeconds(10),
                        Title = "預扣款成功通知",
                        imageurl = "",
                        url = "",
                        Message = $"已於{DateTime.Now:MM/dd HH:mm}{interSecion}預約預扣款成功，金額 {preAuthAmt}，謝謝!"

                    };
                    commonService.sp_InsPersonNotification(input_Notification, ref error);

                    trace.traceAdd("sp_InsPersonNotification", new { input_Notification, error });
                    trace.FlowList.Add("新增推播訊息");
                }
                #endregion
                #region 扣款失敗取消訂單
                if (!bookNDaysAgo && !payFlag && preAuthAmt > 0)
                {
                    SPInput_BookingCancel input_BookingCancel = new SPInput_BookingCancel()
                    {
                        OrderNo = spOut.OrderNum,
                        IDNO = IDNO,
                        LogID = LogID,
                        Token = Access_Token,
                        Cancel_Status_in = 6,
                        CheckToken = 1,
                        Descript = $"預扣款失敗【取消訂單】，金額{preAuthAmt}"
                    };
                    flag = commonService.sp_BookingCancel(input_BookingCancel, ref errCode);

                    trace.traceAdd("sp_BookingCancel", new { input_BookingCancel, errCode });
                    trace.FlowList.Add("預扣款失敗取消訂單");

                    if (flag)
                    {
                        flag = false;
                        errCode = actualPayMode == 1 ? "ERR934" : "ERR602"; //錢包餘額不足:因取授權失敗未完成預約，請檢查卡片餘額或是重新綁卡
                    }
                }
                #endregion

                trace.traceAdd("TraceFinal", new { errCode, errMsg });
                trace.OrderNo = spOut.OrderNum;
                var carRepo = new CarRentRepo();
                carRepo.AddTraceLog(34, funName, trace, flag);
            }
            #endregion
            //預約成功
            if (flag && spOut.haveCar == 1)
            {
                #region 機車先送report now
                //車機指令改善 機車先送report now
                if (ProjType == 4)
                {
                    if (isDebug == "0") // isDebug = 1，不送車機指令
                    {
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
                        //20210325 ADD BY ADAM REASON.車機指令優化取消REPORT NOW
                        flag = FetAPI.DoSendCmd(spOut.deviceToken, spOut.CID, CmdType, input, LogID);
                        //20210326 ADD BY ADAM REASON.先不等report回覆
                        //if (flag)
                        //{
                        //    flag = FetAPI.DoWaitReceive(requestId, method, ref errCode);
                        //}
                    }
                }
                #endregion

                outputApi = new OAPI_Booking()
                {
                    //20210427 ADD BY ADAM REASON.針對編碼調整
                    OrderNo = "H" + spOut.OrderNum.ToString().PadLeft(spOut.OrderNum.ToString().Length, '0'),
                    LastPickTime = LastPickCarTime.ToString("yyyyMMddHHmmss"),
                    WalletNotice = WalletNotice
                };
            }
            else
            {
                flag = false;
                //20210113 ADD BY ADAM REASON.修正預約顯示錯誤
                if (errCode == "000000")
                {
                    errCode = "ERR161";
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

        #region 春節租金-汽車
        /// <summary>
        /// 春節租金-汽車
        /// </summary>
        /// <param name="ProjID">專案代碼</param>
        /// <param name="ProjType">專案類型</param>
        /// <param name="CarType">車型代碼</param>
        /// <param name="IDNO">帳號</param>
        /// <param name="LogID"></param>
        /// <param name="lstHoliday">假日清單</param>
        /// <param name="SD">預約起日</param>
        /// <param name="ED">預約迄日</param>
        /// <param name="Price">平日價</param>
        /// <param name="PRICE_H">假日價</param>
        /// <param name="funNm"></param>
        /// <returns></returns>
        private int GetPriceBill(string ProjID, int ProjType, string CarType, string IDNO, long LogID, List<Holiday> lstHoliday, DateTime SD, DateTime ED, double Price, double PRICE_H, string funNm = "")
        {
            int re = 0;
            var cr_com = new CarRentCommon();
            var bizIn = new IBIZ_SpringInit()
            {
                IDNO = IDNO,
                ProjID = ProjID,
                ProjType = ProjType,
                CarType = CarType,
                SD = SD,
                ED = ED,
                ProDisPRICE = Price / 10,
                ProDisPRICE_H = PRICE_H / 10,
                lstHoliday = lstHoliday,
                LogID = LogID
            };
            var xre = cr_com.GetSpringInit(bizIn, connetStr, funNm);
            if (xre != null)
                re = xre.RentInPay;
            return re;
        }
        #endregion

        #region 預扣
        /// <summary>
        /// 預扣
        /// </summary>
        /// <param name="CheckoutMode">預設支付方式</param>
        /// <param name="OrderNo">訂單號碼</param>
        /// <param name="IDNO">會員帳號</param>
        /// <param name="Amount">預扣金額</param>
        /// <param name="funName">程式名稱</param>
        /// <param name="errCode">回傳錯誤</param>
        /// <param name="errCode">回傳訊息</param>
        /// <param name="PreAuthResult">回傳物件</param>
        /// <returns></returns>
        bool DoPreAuth(int CheckoutMode, long OrderNo, string IDNO, int Amount, string funName, long LogID, string TradeType, int ProjType, string accessToken, ref string errCode, ref OFN_CreditAuthResult PreAuthResult)
        {
            CreditAuthComm creditAuthComm = new CreditAuthComm();
            var AuthInput = new IFN_CreditAuthRequest
            {
                CheckoutMode = CheckoutMode,
                OrderNo = OrderNo,
                IDNO = IDNO,
                Amount = Amount,
                PayType = 0,
                autoClose = 0,
                funName = funName,
                insUser = funName,
                AuthType = 1,
                InputSource = 1,
                Token = accessToken,
                LogID = LogID,
                TradeType = TradeType,
                ProjType = ProjType,
                PayUp = 1,

            };

            bool flag;
            try
            {
                flag = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref PreAuthResult);
            }
            catch (Exception ex)
            {
                flag = false;
                PreAuthResult.AuthMessage = ex.ToString();
            }

            return flag;
        }
        #endregion
    }
}