using Domain.Common;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Booking;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.Booking;
using Domain.TB;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.Input.Param;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;
using Domain.SP.Input.Subscription;
using WebAPI.Models.ComboFunc;
using WebAPI.Service;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Notification;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 預約
    /// </summary>
    public class BookingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

        CommonFunc baseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoBooking(Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var monSp = new MonSubsSp();
            HttpContext httpContext = HttpContext.Current;
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
            #region 檢查信用卡是否綁卡
            if (flag)
            {
                var result = creditAuthComm.CheckTaishinBindCard(ref flag, IDNO, ref errCode);
            }
            #endregion
            #region 檢查欠費
            if (flag)
            {
                int TAMT = 0;
                WebAPI.Models.ComboFunc.ContactComm contract = new Models.ComboFunc.ContactComm();
                flag = contract.CheckNPR330(IDNO, LogID, ref TAMT);
                if (TAMT > 0)
                {
                    flag = false;
                    errCode = "ERR233";
                }
            }
            #endregion
            #region 判斷專案限制及取得專案設定
            if (flag)
            {
                //判斷專案限制及取得專案設定
                if (ProjType == 3)
                {
                    LastPickCarTime = SDate.AddMinutes(AnyRentDefaultPickTime);
                }
                else if (ProjType == 4)
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
                    int proType = -1;
                    var isSpring = cr_com.isSpring(SDate, EDate);
                    var sp_re = new CarRentSp().sp_GetEstimate(apiInput.ProjID, CarType, LogID, ref errMsg);
                    if (sp_re != null)
                        proType = sp_re.PROJTYPE;
                    if (isSpring && carProTypes.Any(x => x == proType))
                    {
                        var xre = GetPriceBill(apiInput.ProjID, proType, apiInput.CarType, IDNO, LogID, lstHoliday, SDate, EDate, priceBase.PRICE, priceBase.PRICE_H, funName);
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
                SPInput_Booking spInput = new SPInput_Booking()
                {
                    CarNo = (string.IsNullOrWhiteSpace(apiInput.CarNo)) ? "" : apiInput.CarNo,
                    Price = price,
                    Insurance = apiInput.Insurance,
                    CarType = CarType,
                    ED = EDate,
                    StopPickTime = LastPickCarTime,
                    IDNO = IDNO,
                    InsurancePurePrice = InsurancePurePrice,
                    LogID = LogID,
                    PayMode = PayMode,
                    ProjID = apiInput.ProjID,
                    ProjType = ProjType,
                    RStationID = StationID,
                    StationID = StationID,
                    Token = Access_Token,
                    SD = SDate
                    //20211012 ADD BY ADAM REASON.增加手機定位點
                    //PhoneLat = apiInput.PhoneLat,
                    //PhoneLon = apiInput.PhoneLon
                };
                SPName = "usp_Booking";
                SQLHelper<SPInput_Booking, SPOutput_Booking> sqlHelp = new SQLHelper<SPInput_Booking, SPOutput_Booking>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion
            #region 寫入訂單對應訂閱制月租
            if (flag)
            {
                if (spOut != null && spOut.haveCar == 1
                    && spOut.OrderNum > 0 && apiInput.MonId > 0
                    && !string.IsNullOrWhiteSpace(IDNO) && LogID > 0)
                {
                    var sp_in = new SPInput_SetSubsBookingMonth()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        OrderNo = spOut.OrderNum,
                        MonthlyRentId = apiInput.MonId
                    };
                    monSp.sp_SetSubsBookingMonth(sp_in, ref errCode);
                    //不擋booking
                }
            }
            #endregion
            #region 預授權機制
            if (flag && spOut.haveCar == 1 && (ProjType == 0 || ProjType == 3))
            {
                SPOutput_OrderForPreAuth orderData = commonService.GetOrderForPreAuth(spOut.OrderNum);
                //預授權不處理專案(長租客服月結E077)
                string notHandle = new CommonRepository(connetStr).GetCodeData("PreAuth").FirstOrDefault().MapCode;
                if (!notHandle.Contains(apiInput.ProjID) && orderData.DoPreAuth ==1)
                {
                    var trace = new TraceCom();
                    trace.traceAdd("apiIn", value);
                    int preAuthAmt = 0;
                    bool canAuth = false;
                    bool authflag = false;
                    #region 計算預授權金
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
                        //同站取車前6小時後預約立即授權
                        DateTime checkDate = SDate.AddHours(-6);
                        canAuth = DateTime.Compare(DateTime.Now, checkDate) >= 0 ? true : false;
                    }
                    else if (ProjType == 3)
                    {
                        canAuth = true;
                        int triaHour = 1;  //2021/12/16 因企劃臨時要求路邊只預收1小時授權金
                        estimateData.ED = SDate.AddHours(triaHour);
                    }
                    EstimateDetail estimateDetail;
                    commonService.EstimatePreAuthAmt(estimateData, out estimateDetail);

                    //需扣掉春節訂金
                    preAuthAmt = orderData.PreAuthAmt == 0 ? estimateDetail.estimateAmt : estimateDetail.estimateAmt - orderData.PreAuthAmt;

                    trace.traceAdd("GetEsimateAuthAmt", new { canAuth, estimateData, estimateDetail, preAuthAmt });
                    trace.FlowList.Add("計算預授權金");
                    #endregion
                    #region 後續流程
                    if (preAuthAmt > 0)
                    {
                        #region 刷卡授權
                        if (canAuth)
                        {
                            var AuthInput = new IFN_CreditAuthRequest
                            {
                                CheckoutMode = 4,
                                OrderNo = spOut.OrderNum,
                                IDNO = IDNO,
                                Amount = preAuthAmt,
                                PayType = 0,
                                autoClose = 0,
                                funName = funName,
                                insUser = funName,
                                AuthType = 1
                            };

                            try
                            {
                                authflag = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
                            }
                            catch (Exception ex)
                            {
                                authflag = false; //走取消訂單
                                flag = false;
                                trace.BaseMsg = ex.Message;
                            }

                            trace.traceAdd("DoAuthV4", new { authflag, AuthInput, AuthOutput, errCode });
                            trace.FlowList.Add("刷卡授權");
                        }
                        #endregion
                        #region 寫入預授權
                        SPInput_InsOrderAuthAmount input_AuthAmount = new SPInput_InsOrderAuthAmount()
                        {
                            IDNO = IDNO,
                            LogID = LogID,
                            Token = Access_Token,
                            AuthType = 1,
                            CardType = 1,
                            final_price = preAuthAmt,
                            OrderNo = spOut.OrderNum,
                            PRGName = funName,
                            MerchantTradNo = AuthOutput == null ? "" : AuthOutput.Transaction_no,
                            BankTradeNo = AuthOutput == null ? "" : AuthOutput.BankTradeNo,
                            Status = canAuth ? 2 : 0
                        };
                        commonService.sp_InsOrderAuthAmount(input_AuthAmount, ref error);

                        trace.traceAdd("sp_InsOrderAuthAmount", new { input_AuthAmount, error });
                        trace.FlowList.Add("寫入預授權");
                        #endregion
                        #region 新增推播訊息
                        if (authflag)
                        {
                            string cardNo = AuthOutput.CardNo.Substring((AuthOutput.CardNo.Length - 4) > 0 ? AuthOutput.CardNo.Length - 4 : 0);
                            SPInput_InsPersonNotification input_Notification = new SPInput_InsPersonNotification()
                            {
                                OrderNo = Convert.ToInt32(spOut.OrderNum),
                                IDNO = IDNO,
                                LogID = LogID,
                                NType = 19,
                                STime = DateTime.Now.AddSeconds(10),
                                Title = "取授權成功通知",
                                imageurl = "",
                                url = "",
                                Message = $"已於{DateTime.Now.ToString("MM/dd HH:mm")}以末四碼{cardNo}信用卡預約取授權成功，金額 {preAuthAmt}，謝謝!"

                            };
                            commonService.sp_InsPersonNotification(input_Notification, ref error);

                            trace.traceAdd("sp_InsPersonNotification", new { input_Notification, error });
                            trace.FlowList.Add("新增推播訊息");
                        }
                        #endregion
                        #region 授權失敗取消訂單
                        if (canAuth && !authflag)
                        {
                            SPInput_BookingCancel input_BookingCancel = new SPInput_BookingCancel()
                            {
                                OrderNo = spOut.OrderNum,
                                IDNO = IDNO,
                                LogID = LogID,
                                Token = Access_Token,
                                Cancel_Status_in = 6,
                                CheckToken = 1,
                                Descript = $"預授權失敗【取消訂單】，金額{preAuthAmt}"
                            };
                            flag = commonService.sp_BookingCancel(input_BookingCancel, ref errCode);

                            trace.traceAdd("sp_BookingCancel", new { input_BookingCancel, errCode });
                            trace.FlowList.Add("授權失敗取消訂單");

                            if (flag)
                            {
                                flag = false;
                                errCode = "ERR602";
                            }
                        }
                        #endregion
                    }
                    #endregion

                    trace.traceAdd("TraceFinal", new { errCode, errMsg });
                    trace.OrderNo = spOut.OrderNum;
                    var carRepo = new CarRentRepo();
                    carRepo.AddTraceLog(34, funName, trace, flag);
                }             
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
                    LastPickTime = LastPickCarTime.ToString("yyyyMMddHHmmss")
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

        /// <summary>
        /// 春節租金-汽車
        /// </summary>
        /// <returns></returns>
        private int GetPriceBill(string ProjID, int ProjType, string CarType, string IDNO, long LogID, List<Holiday> lstHoliday, DateTime SD, DateTime ED, double Price, double PRICE_H, string funNm = "")
        {
            int re = 0;
            var cr_com = new CarRentCommon();
            var bizIn = new IBIZ_SpringInit()
            {
                ProjID = ProjID,
                ProjType = ProjType,
                CarType = CarType,
                IDNO = IDNO,
                LogID = LogID,
                lstHoliday = lstHoliday,
                SD = SD,
                ED = ED,
                ProDisPRICE = Price / 10,
                ProDisPRICE_H = PRICE_H / 10
            };
            var xre = cr_com.GetSpringInit(bizIn, connetStr, funNm);
            if (xre != null)
                re = xre.RentInPay;
            return re;
        }

    }
}