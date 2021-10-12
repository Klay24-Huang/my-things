using Domain.Common;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Booking;
using Domain.SP.Input.Subscription;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
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
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 預約
    /// </summary>
    public class BookingController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

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
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
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

            List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
            BillCommon billCommon = new BillCommon();

            bool CreditFlag = true;     // 信用卡綁卡
            bool WalletFlag = false;    // 綁定錢包
            int WalletNotice = 0;       // 錢包餘額不足通知 (0:不顯示 1:顯示)
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
                EDate = apiInput.SDate == "" ? DateTime.Now.AddHours(1) : Convert.ToDateTime(apiInput.EDate);
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
                        if (ProjType == 3)
                        {
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
                    flag = baseVerify.CheckDate(apiInput.SDate, apiInput.EDate, ref errCode, ref SDate, ref EDate); //同站

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
            //20201103 ADD BY ADAM REASON.取得安心服務每小時價格
            if (flag)
            {
                string GetInsurancePriceName = "usp_GetInsurancePrice";
                SPInput_GetInsurancePrice spGetInsurancePrice = new SPInput_GetInsurancePrice()
                {
                    IDNO = IDNO,
                    CarType = CarTypeCode,
                    LogID = LogID
                };
                List<SPOutput_GetInsurancePrice> re = new List<SPOutput_GetInsurancePrice>();
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetInsurancePrice, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetInsurancePrice, SPOutput_Base>(connetStr);
                DataSet ds = new DataSet();

                flag = sqlHelp.ExeuteSP(GetInsurancePriceName, spGetInsurancePrice, ref spOut, ref re, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag && re.Count > 0)
                {
                    InsurancePerHours = int.Parse(re[0].InsurancePerHours.ToString());
                }
            }
            #endregion

            #region 檢查信用卡是否綁卡、錢包是否開通
            if (flag)
            {
                if (ProjType == 4)  // 4:機車
                {
                    #region 檢查信用卡是否綁卡
                    DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errMsg);
                    if (ds.Tables.Count == 0)
                    {
                        CreditFlag = false;
                    }
                    else if (ds.Tables[0].Rows.Count == 0)
                    {
                        CreditFlag = false;
                    }
                    ds.Dispose();
                    #endregion

                    #region 檢查錢包是否開通
                    string SPName = "usp_CreditAndWalletQuery_Q01";
                    SPInput_CreditAndWalletQuery spInput = new SPInput_CreditAndWalletQuery
                    {
                        IDNO = IDNO,
                        Token = Access_Token,
                        LogID = LogID
                    };
                    SPOut_CreditAndWalletQuery spOut = new SPOut_CreditAndWalletQuery();
                    SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery> sqlHelp = new SQLHelper<SPInput_CreditAndWalletQuery, SPOut_CreditAndWalletQuery>(connetStr);
                    flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                    if (flag)
                    {
                        WalletFlag = spOut.WalletStatus == "2" ? true : false;
                    }
                    #endregion

                    if (!CreditFlag && !WalletFlag) // 沒綁信用卡 也 沒開通錢包，就回錯誤訊息
                    {
                        flag = false;
                        errCode = "ERR730";
                    }
                    else if (!CreditFlag && WalletFlag) // 沒綁信用卡 但 有開通錢包，要給APP值顯示通知
                    {
                        WalletNotice = 1;
                    }
                }
                else
                {   // 0:同站汽車 3:路邊汽車
                    #region 檢查信用卡是否綁卡
                    // 汽車只要沒綁信用卡就回錯誤
                    //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                    DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errMsg);
                    if (ds.Tables.Count == 0)
                    {
                        flag = false;
                        errCode = "ERR730";
                    }
                    else if (ds.Tables[0].Rows.Count == 0)
                    {
                        flag = false;
                        errCode = "ERR730";
                    }
                    ds.Dispose();
                    #endregion
                }
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

            #region 計算預估租金
            if (flag)
            {
                //20201103 ADD BY SS ADAM REASON.計算安心服務價格
                InsurancePurePrice = (apiInput.Insurance == 1) ? Convert.ToInt32(billCommon.CalSpread(SDate, EDate, InsurancePerHours * 10, InsurancePerHours * 10, lstHoliday)) : 0;
                //計算預估租金
                if (ProjType < 4)
                {
                    ProjectPriceBase priceBase = null;
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
                    ProjectPriceOfMinuteBase priceBase = projectRepository.GetProjectPriceBaseByMinute(apiInput.ProjID, apiInput.CarNo);
                    price = Convert.ToInt32(priceBase.Price);
                }
            }
            #endregion

            #region 預約
            //開始做預約
            if (flag)
            {
                string SPName = "usp_Booking";
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
                };
                SPOutput_Booking spOut = new SPOutput_Booking();
                SQLHelper<SPInput_Booking, SPOutput_Booking> sqlHelp = new SQLHelper<SPInput_Booking, SPOutput_Booking>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag && spOut.haveCar == 1)
                {
                    #region 機車先送report now
                    //車機指令改善 機車先送report now
                    if (ProjType == 4)
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
                    #endregion

                    #region 寫入訂單對應訂閱制月租
                    if (spOut.OrderNum > 0 && apiInput.MonId > 0 && !string.IsNullOrWhiteSpace(IDNO) && LogID > 0)
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
                        errCode = "ERR161";
                }
            }
            #endregion
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
        #endregion
    }
}