using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Rent;
using Domain.TB;
using Newtonsoft.Json;
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
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得租金明細
    /// </summary>
    public class GetPayDetailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        [HttpPost]
        public Dictionary<string, object> DoGetPayDetail(Dictionary<string, object> value)
        {
            #region 初始宣告
            #region API共用
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"];    //Bearer 
            var objOutput = new Dictionary<string, object>();   //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "GetPayDetailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            IAPI_GetPayDetail apiInput = null;
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            #endregion

            #region 計算用
            var cr_com = new CarRentCommon();
            var cr_sp = new CarRentSp();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo(connetStr);
            var monSp = new MonSubsSp();
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            CarRentInfo carInfo = new CarRentInfo();    //車資料
            #endregion

            #region 一堆參數
            Int64 tmpOrder = -1;

            List<OrderQueryFullData> OrderDataLists = null;
            OrderQueryFullData item = new OrderQueryFullData();

            int ProjType = 0;   //專案類型
            string ProjID = ""; //專案代碼

            int motoBaseMins = 0;   //機車基本分鐘數
            int carBaseMins = 60;   //汽車基本分鐘數
            int DayMaxMinute = 600; //單日分鐘數上限

            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數
            int Discount = 0;   //要折抵的點數
            int gift_point = 0;         //使用時數(汽車)
            int gift_motor_point = 0;   //使用時數(機車)
            int ActualRedeemableTimePoint = 0;  //實際可抵折點數

            List<Holiday> lstHoliday = null; //假日列表
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>();   //月租列表
            var visMons = new List<MonthlyRentData>();//虛擬月租

            DateTime NowTime = DateTime.Now;
            DateTime SD = new DateTime();   // 實際取車時間
            DateTime ED = new DateTime();   // 預計還車時間(逾時為原始還車時間)
            DateTime FED = new DateTime();  // 實際還車時間
            DateTime? FineDate = null;
            bool hasFine = false;   //是否逾時
            DateTime sprSD = Convert.ToDateTime(SiteUV.strSpringSd);    // 春節起日
            DateTime sprED = Convert.ToDateTime(SiteUV.strSpringEd);    // 春節迄日
            bool isSpring = false;  //是否為春節時段

            int TotalRentMinutes = 0;           //總租車時數
            int TotalFineRentMinutes = 0;       //總逾時時數
            int CarRentPrice = 0;               //車輛租金
            int TransferPrice = 0;              //轉乘優惠折抵金額  20201201 ADD BY ADAM

            bool UseMonthMode = false;  //false:無月租;true:有月租
            int InsurancePerHours = 0;  //安心服務每小時價
            int etagPrice = 0;          //ETAG費用 20201202 ADD BY ADAM
            int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM
            int CityParkingPrice = 0;   //城市車旅停車費 20210429 ADD BY ADAM
            int PreAmount = 0;          // 預授權金額 20211026 ADD BY YEH
            int DiffAmount = 0;         // 差額 20211026 ADD BY YEH
            float MillageUnit = 0;      // 每公里里程費

            int nor_car_PayDisc = 0;    //只有一般時段時總折扣

            var neverHasFine = new List<int>() { 3, 4 };    //路邊,機車不會逾時

            int OrderPrice = 0;     //原始訂金
            int UseOrderPrice = 0;  //使用訂金(4捨5入)
            string MonIds = "";     //短期月租Id可多筆
            #endregion
            #endregion
            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                if (flag)
                {
                    apiInput = JsonConvert.DeserializeObject<IAPI_GetPayDetail>(Contentjson);
                    trace.traceAdd(nameof(apiInput), apiInput);
                    trace.FlowList.Add("防呆");
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    trace.FlowList.Add("寫入API Log");
                    var input = new IBIZ_InCheck()
                    {
                        OrderNo = apiInput.OrderNo,
                        Discount = apiInput.Discount,
                        MotorDiscount = apiInput.MotorDiscount,
                        isGuest = isGuest
                    };
                    var inck_re = cr_com.InCheck(input);
                    if (inck_re != null)
                    {
                        trace.traceAdd(nameof(inck_re), inck_re);
                        flag = inck_re.flag;
                        errCode = inck_re.errCode;
                        Discount = inck_re.Discount;
                        tmpOrder = inck_re.longOrderNo;
                    }
                    trace.FlowList.Add("input檢查");

                    //不開放訪客
                    if (flag)
                    {
                        if (isGuest)
                        {
                            flag = false;
                            errCode = "ERR101";
                        }
                    }
                }
                #endregion
                #region 取出基本資料
                #region Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    trace.FlowList.Add("Token判斷");
                }
                #endregion
                #region 取出訂單資訊
                if (flag)
                {
                    SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        LogID = LogID,
                        Token = Access_Token
                    };
                    CommonService commonService = new CommonService();
                    OrderDataLists = commonService.GetOrderStatusByOrderNo(spInput, ref flag, ref errCode);

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
                #endregion
                #region 訂單檢查
                if (flag)
                {
                    item = OrderDataLists[0];
                    trace.traceAdd(nameof(OrderDataLists), OrderDataLists);
                    trace.OrderNo = item.OrderNo;
                    motoBaseMins = item.BaseMinutes;
                    ProjType = item.ProjType;
                    UseOrderPrice = item.UseOrderPrice;
                    OrderPrice = item.OrderPrice;
                    ProjID = item.ProjID;
                    PreAmount = item.PreAmount;

                    if (ProjType == 4)  // 機車
                    {
                        if (item.BaseMinutes == 0)
                        {
                            flag = false;
                            errMsg = "訂單資訊中BaseMinutes(機車基本分鐘數)不可為0";
                            errCode = "ERR914";
                        }

                        //if (flag)
                        //{
                        //    if (item.BaseMinutesPrice == 0)
                        //    {
                        //        flag = false;
                        //        errMsg = "訂單資訊中BaseMinutesPrice(機車基消)不可為0";
                        //        errCode = "ERR914";
                        //    }
                        //}
                    }

                    if (ProjType != 4)  // 非機車專案
                    {
                        Discount = apiInput.Discount;
                    }
                }
                #endregion
                #endregion

                #region 第二階段判斷及計價
                if (flag)
                {
                    #region 訂單判斷狀態
                    if (item.car_mgt_status == 16 || item.car_mgt_status < 11 || item.cancel_status > 0)
                    {
                        flag = false;
                        errCode = "ERR204";
                    }
                    #endregion

                    #region 日期判斷
                    //取得專案狀態
                    if (flag)
                    {
                        SD = Convert.ToDateTime(item.final_start_time);
                        SD = SD.AddSeconds(SD.Second * -1); //去秒數

                        if (!string.IsNullOrWhiteSpace(item.fine_Time) && Convert.ToDateTime(item.fine_Time) > Convert.ToDateTime("1911-01-01 00:00:00"))
                        {
                            FineDate = Convert.ToDateTime(item.fine_Time);
                            FineDate = FineDate.Value.AddSeconds(ED.Second * -1);   //去秒數
                        }

                        if (neverHasFine.Contains(ProjType))
                        {
                            ED = Convert.ToDateTime(item.final_stop_time);
                            ED = ED.AddSeconds(ED.Second * -1); //去秒數
                            FineDate = null;
                        }
                        else
                        {
                            ED = Convert.ToDateTime(item.stop_time);
                            ED = ED.AddSeconds(ED.Second * -1); //去秒數
                        }
                        FED = Convert.ToDateTime(item.final_stop_time);
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
                        trace.traceAdd(nameof(hasFine), hasFine);
                        trace.FlowList.Add("SD,ED,FD計算");

                        #region 春節不能使用折扣點數
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
                        if (isSpring)
                        {
                            Discount = 0;
                            apiInput.Discount = 0;
                            apiInput.MotorDiscount = 0;
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
                    }
                    #endregion

                    #region 取得使用中訂閱制月租
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
                            }
                        }
                    }
                    #endregion

                    #region 檢查點下還車鍵是否已超過三十分鐘
                    if (flag)
                    {
                        if (NowTime.Subtract(FED).TotalMinutes >= 30)
                        {
                            flag = false;
                            errCode = "ERR208";
                        }
                    }
                    #endregion

                    #region 與短租查時數 - 春節不執行
                    if (flag && !isSpring)
                    {
                        var inp = new IBIZ_NPR270Query()
                        {
                            IDNO = IDNO
                        };
                        var re270 = cr_com.NPR270Query(inp);
                        if (re270 != null)
                        {
                            trace.traceAdd(nameof(re270), re270);
                            flag = re270.flag;
                            MotorPoint = re270.MotorPoint;
                            CarPoint = re270.CarPoint;
                        }
                        trace.FlowList.Add("與短租查時數");

                        //判斷輸入的點數有沒有超過總點數
                        if (ProjType == 4)
                        {
                            if (Discount > 0 && Discount < item.BaseMinutes)   // 折抵點數 < 基本分鐘數
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
                            trace.FlowList.Add("機車一般點數檢查");
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
                            trace.FlowList.Add("汽車一般點數檢查");
                        }
                    }
                    #endregion

                    #region 汽車計費資訊
                    int car_payAllMins = 0;     //全部計費租用分鐘
                    int car_payInMins = 0;      //未超時計費分鐘
                    int car_payOutMins = 0;     //逾時分鐘-顯示用
                    int car_inPrice = 0;        //未超時費用
                    int car_outPrice = 0;       //逾時費用
                    int car_n_price = item.PRICE;      // 平日價格
                    int car_h_price = item.PRICE_H;    // 假日價格

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

                                var car_re = billCommon.CarRentInCompute(SD, ED, car_n_price, car_h_price, carBaseMins, 10, lstHoliday, new List<MonthlyRentData>(), Discount);
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
                                var car_re = billCommon.CarRentInCompute(SD, FED, car_n_price, car_h_price, carBaseMins, 10, lstHoliday, new List<MonthlyRentData>(), Discount);
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

                    #region 查ETAG 20201202 ADD BY ADAM
                    if (flag && ProjType != 4)    //汽車才需要進來
                    {
                        var input = new IBIZ_ETagCk()
                        {
                            OrderNo = apiInput.OrderNo
                        };
                        trace.traceAdd("etag_in", input);
                        try
                        {
                            var etag_re = cr_com.ETagCk(input);
                            if (etag_re != null)
                            {
                                trace.traceAdd(nameof(etag_re), etag_re);
                                //flag = etag_re.flag;
                                errCode = etag_re.errCode;
                                etagPrice = etag_re.etagPrice;
                            }
                        }
                        catch (Exception ex)
                        {
                            trace.BaseMsg += "etag_err:" + ex.Message;
                        }
                        trace.FlowList.Add("查ETAG");
                    }
                    #endregion

                    #region 建空模及塞入要輸出的值
                    if (flag)
                    {
                        int Mode = ProjType == 4 ? 1 : 0;
                        outputApi.MonBase = carRepo.GetMonths(IDNO, SD, FED, Mode); //短期下拉選項
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
                            CarNo = item.CarNo,
                            RedeemingTimeCarInterval = CarPoint.ToString(),
                            RedeemingTimeMotorInterval = MotorPoint.ToString(),
                            RedeemingTimeInterval = (ProjType == 4) ? (CarPoint + MotorPoint).ToString() : CarPoint.ToString(),
                            RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
                            RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
                        };

                        if (ProjType == 4)
                        {
                            TotalPoint = (CarPoint + MotorPoint);
                            outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
                            {
                                BaseMinutePrice = item.BaseMinutesPrice,
                                BaseMinutes = item.BaseMinutes,
                                MinuteOfPrice = item.MinuteOfPrice
                            };
                        }
                        else
                        {
                            TotalPoint = CarPoint;
                            outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
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
                    if (flag && ProjType != 4) //20201224 add by adam 問題未確定前先關掉車麻吉
                    {
                        var input = new IBIZ_CarMagi()
                        {
                            LogID = LogID,
                            CarNo = item.CarNo,
                            SD = SD,
                            ED = FED.AddDays(1),
                            OrderNo = tmpOrder
                        };
                        trace.traceAdd("magi_in", input);
                        try
                        {
                            var magi_Re = cr_com.CarMagi(input);
                            if (magi_Re != null)
                            {
                                trace.traceAdd(nameof(magi_Re), magi_Re);
                                //flag = magi_Re.flag;
                                outputApi.Rent.ParkingFee = magi_Re.ParkingFee;
                            }
                        }
                        catch (Exception ex)
                        {
                            trace.BaseMsg += "magi_err:" + ex.Message;
                        }
                        trace.FlowList.Add("車麻吉");
                    }
                    //20210428 ADD BY ADAM REASON.串接停車場
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
                        //baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
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
                            MonIds = MonIds
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
                                var xre = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPriceH, motoBaseMins, DayMaxMinute, lstHoliday, new List<MonthlyRentData>(), Discount, DayMaxMinute, item.MaxPrice, item.BaseMinutesPrice, item.FirstFreeMins);
                                if (xre != null)
                                {
                                    carInfo = xre;
                                    outputApi.Rent.CarRental = xre.RentInPay;
                                    carInfo.useDisc = xre.useDisc;
                                }

                                if (carInfo != null)
                                    outputApi.Rent.CarRental = carInfo.RentInPay;

                                trace.FlowList.Add("機車非月租租金計算");
                            }

                            outputApi.Rent.RentBasicPrice = item.BaseMinutesPrice;
                        }
                        else
                        {
                            if (UseMonthMode)   //true:有月租;false:無月租
                            {
                                outputApi.Rent.CarRental = CarRentPrice;
                                outputApi.MonthRent.HoildayRate = monthlyRentDatas == null ? 0 : monthlyRentDatas[0].HoildayRateForCar;
                                outputApi.MonthRent.WorkdayRate = monthlyRentDatas == null ? 0 : monthlyRentDatas[0].WorkDayRateForCar;
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
                            // 20210903 UPD BY YEH REASON:使用安心服務，安心服務每小時金額 = 主承租人每小時價格 + 副承租人每小時費率總和
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
                            //里程費計算修改，遇到取不到里程數的先以0元為主
                            // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                            // 20210119 里程數>1000公里的判斷移除
                            if (item.start_mile == 0 || item.end_mile == 0 || ((item.end_mile - item.start_mile) < 0))
                            {
                                outputApi.Rent.MileageRent = 0;
                            }
                            else
                            {
                                outputApi.Rent.MileageRent = Convert.ToInt32(MillageUnit * (item.end_mile - item.start_mile));
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

                        #region 2021春節訂金已不使用
                        // 20211026 UPD BY YEH REASON:2021春節訂金已不使用
                        //xTotalRental -= UseOrderPrice;  //預繳定金扣抵

                        //outputApi.UseOrderPrice = UseOrderPrice;
                        //20220126 ADD BY ADAM REASON.春節訂金改用預授權金額帶入
                        outputApi.UseOrderPrice = PreAmount;
                        outputApi.FineOrderPrice = OrderPrice - UseOrderPrice;//沒收訂金
                        
                        //如果有春節訂金就要把罰金加上去
                        xTotalRental += OrderPrice > 0 ? UseOrderPrice : 0;

                        //if (xTotalRental < 0)
                        if (UseOrderPrice > 0)
                        {
                            //outputApi.ReturnOrderPrice = (-1) * xTotalRental;
                            int orderNo = Convert.ToInt32(item.OrderNo);
                            //carRepo.UpdNYPayList(orderNo, outputApi.ReturnOrderPrice);
                            carRepo.UpdNYPayList(orderNo, UseOrderPrice);

                            //不含退還訂金
                            //OrderPrice = OrderPrice - outputApi.ReturnOrderPrice;
                            //OrderPrice = OrderPrice > 0 ? OrderPrice : 0;
                            //outputApi.UseOrderPrice = OrderPrice;
                        }
                        #endregion

                        DiffAmount = xTotalRental - PreAmount;  // 差額 = 訂單總價 - 預授權金額

                        //xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                        outputApi.Rent.TotalRental = xTotalRental;

                        // 20211229 UPD BY YEH REASON:output增加預授權金額、差額
                        outputApi.Rent.PreAmount = PreAmount;
                        outputApi.Rent.DiffAmount = DiffAmount;
                        trace.FlowList.Add("總價計算");
                        #endregion

                        #region 修正輸出欄位                 
                        if (ProjType == 4)
                        {
                            outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                            outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();

                            // 20211209 UPD BY YEH REASON:給前端顯示的租用時數改用可折抵時數
                            //outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)
                            outputApi.Rent.RentalTimeInterval = carInfo.DiscRentInMins.ToString();  // 可折抵時數

                            //2020-12-29 所有點數改成皆可折抵
                            //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                            outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

                            outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

                            var cDisc = apiInput.Discount;
                            var mDisc = apiInput.MotorDiscount;
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

                        #region 儲存使用月租時數
                        if (!string.IsNullOrWhiteSpace(IDNO) && tmpOrder > 0 && LogID > 0 && !string.IsNullOrWhiteSpace(MonIds) && carInfo != null && (carInfo.useMonthDiscW > 0 || carInfo.useMonthDiscH > 0))
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

                        #region SP存檔
                        string SPName = "usp_CalFinalPrice_U01";
                        SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            final_price = outputApi.Rent.TotalRental,
                            pure_price = outputApi.Rent.CarRental,
                            mileage_price = outputApi.Rent.MileageRent,
                            Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                            fine_price = outputApi.Rent.OvertimeRental,
                            gift_point = gift_point,
                            gift_motor_point = gift_motor_point,
                            //20210720 ADD BY ADAM REASON.訂閱制儲存沿用舊格式汽車採小時計
                            monthly_workday = carInfo.useMonthDiscW / 60,
                            monthly_holiday = carInfo.useMonthDiscH / 60,
                            Etag = outputApi.Rent.ETAGRental,
                            parkingFee = outputApi.Rent.ParkingFee,
                            TransDiscount = outputApi.Rent.TransferPrice,
                            Token = Access_Token,
                            LogID = LogID,
                            DiffAmount = DiffAmount,
                            APIName = funName
                        };
                        SPOutput_Base SPOutput = new SPOutput_Base();
                        SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
                        flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);

                        #region trace
                        trace.traceAdd(nameof(TotalRentMinutes), TotalRentMinutes);
                        trace.traceAdd(nameof(Discount), Discount);
                        trace.traceAdd(nameof(CarPoint), CarPoint);
                        trace.traceAdd(nameof(MotorPoint), MotorPoint);
                        trace.traceAdd(nameof(SPInput), SPInput);
                        trace.traceAdd(nameof(outputApi), outputApi);
                        trace.traceAdd(nameof(carInfo), carInfo);
                        #endregion

                        trace.FlowList.Add("sp存檔");
                        #endregion
                    }
                    #endregion
                }
                #endregion

                #region 寫入錯誤Log
                if (!flag)
                {
                    trace.traceAdd(nameof(errCode), errCode);
                    trace.traceAdd(nameof(TotalPoint), TotalPoint);
                    trace.traceAdd(nameof(TransferPrice), TransferPrice);
                    trace.objs = trace.getObjs();
                    var errItem = new TraceLogVM()
                    {
                        ApiId = 73,
                        ApiMsg = JsonConvert.SerializeObject(trace),
                        ApiNm = funName,
                        CodeVersion = trace.codeVersion,
                        FlowStep = trace.FlowStep(),
                        OrderNo = trace.OrderNo,
                        TraceType = string.IsNullOrWhiteSpace(trace.BaseMsg) ? (flag ? eumTraceType.mark : eumTraceType.followErr) : eumTraceType.exception
                    };
                    carRepo.AddTraceLog(errItem);
                }
                if (flag == false && isWriteError == false)
                {
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                trace.objs = trace.getObjs();
                var errItem = new TraceLogVM()
                {
                    ApiId = 73,
                    ApiMsg = JsonConvert.SerializeObject(trace),
                    ApiNm = funName,
                    CodeVersion = trace.codeVersion,
                    FlowStep = trace.FlowStep(),
                    OrderNo = trace.OrderNo,
                    TraceType = eumTraceType.exception
                };
                carRepo.AddTraceLog(errItem);
                errCode = "ERR902";
                //errMsg = "";
                //throw;
            }
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}