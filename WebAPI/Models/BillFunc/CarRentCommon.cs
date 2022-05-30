using Domain.Flow.CarRentCompute;
using Domain.Log;
using Domain.SP.Input.Arrears;
using Domain.SP.Input.Discount;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Mochi;
using Newtonsoft.Json;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Models.BillFunc
{
    public class CarRentCommon
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        #region 點數查詢
        /// <summary>
        /// 點數查詢
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OBIZ_NPR270Query NPR270Query(IBIZ_NPR270Query sour)
        {
            var re = new OBIZ_NPR270Query();

            WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
            re.flag = wsAPI.NPR270Query(sour.IDNO, ref wsOutput);
            if (re.flag)
            {
                int giftLen = wsOutput.Data.Length;

                if (giftLen > 0)
                {
                    for (int i = 0; i < giftLen; i++)
                    {
                        DateTime tmpDate;
                        int tmpPoint = 0;
                        bool DateFlag = DateTime.TryParse(wsOutput.Data[i].EDATE, out tmpDate);
                        bool PointFlag = int.TryParse(wsOutput.Data[i].GIFTPOINT, out tmpPoint);
                        if (DateFlag && (tmpDate >= DateTime.Now) && PointFlag)
                        {
                            if (wsOutput.Data[i].GIFTTYPE == "01")  //汽車
                            {
                                re.CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                            }
                            else
                            {
                                re.MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                            }
                        }
                    }
                }
            }
            else
            {
                re.flag = true;
                re.errCode = "0000";
            }

            return re;
        }
        #endregion

        #region ETAG010查詢費用
        public OBIZ_ETagCk ETagCk(IBIZ_ETagCk sour)
        {
            var re = new OBIZ_ETagCk();

            WebAPIOutput_ETAG010 wsOutput = new WebAPIOutput_ETAG010();
            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
            //ETAG查詢失敗也不影響流程
            re.flag = wsAPI.ETAG010Send(sour.OrderNo, "", ref wsOutput);
            if (re.flag)
            {
                if (wsOutput.RtnCode == "0")
                {
                    //取出ETAG費用
                    if (wsOutput.Data.Length > 0)
                    {
                        re.etagPrice = wsOutput.Data[0].TAMT == "" ? 0 : int.Parse(wsOutput.Data[0].TAMT);
                    }
                }
            }
            re.flag = true;
            re.errCode = "000000";

            return re;
        }
        #endregion

        #region 車麻吉
        /// <summary>
        /// 車麻吉
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OBIZ_CarMagi CarMagi(IBIZ_CarMagi sour)
        {
            var re = new OBIZ_CarMagi();

            //檢查有無車麻吉停車費用
            WebAPIOutput_QueryBillByCar mochiOutput = new WebAPIOutput_QueryBillByCar();
            MachiComm mochi = new MachiComm();
            int ParkingPrice = 0;
            //re.flag = mochi.GetParkingBill(sour.LogID, sour.CarNo, sour.SD, sour.ED.AddDays(1), ref ParkingPrice, ref mochiOutput);
            re.flag = mochi.GetParkingBill(sour.LogID, sour.OrderNo, sour.CarNo, sour.SD, sour.ED.AddDays(1), ref ParkingPrice, ref mochiOutput);

            if (re.flag)
            {
                re.ParkingFee = ParkingPrice;
            }

            return re;
        }
        #endregion

        #region 汽機車月租,不含逾時(有存檔)
        /// <summary>
        /// 汽機車月租,不含逾時(有存檔)
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OBIZ_MonthRent MonthRentSave(IBIZ_MonthRent sour)
        {
            var re = new OBIZ_MonthRent();
            var cr_com = new CarRentCommon();
            var cr_sp = new CarRentSp();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var monthlyRentRepository = new MonthlyRentRepository(connetStr);
            var monthlyRentDatas = new List<MonthlyRentData>();
            var billCommon = new BillCommon();
            var errCode = re.errCode;
            string funNm = "MonthRentSave";
            re.flag = true;
            re.UseMonthMode = false;
            re.IsMonthRent = 0;

            trace.traceAdd("fnIn", sour);

            if (sour != null && !string.IsNullOrWhiteSpace(sour.MonIds))
            {
                // 20220210 UPD BY YEH REASON:有訂閱制才還原資料
                //1.0 先還原這個單號使用的
                re.flag = monthlyRentRepository.RestoreHistory(sour.IDNO, sour.intOrderNO, sour.LogID, ref errCode);
                re.errCode = errCode;

                //monthlyRentDatas = monthlyRentRepository.GetSubscriptionRatesByMonthlyRentId(sour.IDNO, sour.MonIds);
                monthlyRentDatas = monthlyRentRepository.GetCanUseMonthly(sour.IDNO, sour.intOrderNO, sour.MonIds, sour.LogID,ref errCode);

                //假日優惠費率置換:只限汽車月租,只置換假日
                List<int> CarProTypes = new List<int>() { 0, 3 };
                if (monthlyRentDatas != null && monthlyRentDatas.Count() > 0 && CarProTypes.Any(x => x == sour.ProjType) && sour.intOrderNO > 0)
                {
                    string xErrMsg = "";
                    foreach (var m in monthlyRentDatas)
                    {
                        var pri = cr_sp.sp_GetEstimate("", "", sour.LogID, ref xErrMsg, sour.intOrderNO);
                        if (pri != null && pri.PRICE_H > 0)
                            m.HoildayRateForCar = Convert.ToSingle(pri.PRICE_H) / 10; //20210731 ADD BY ADAM REASON.補上除以10
                    }
                    trace.FlowList.Add("置換汽車假日優惠費率");
                }
            }

            if (sour.CancelMonthRent)
                monthlyRentDatas = new List<MonthlyRentData>();

            trace.traceAdd("monthlyRentDatas", monthlyRentDatas);

            //虛擬月租
            if (sour.VisMons != null && sour.VisMons.Count() > 0)
                monthlyRentDatas.Insert(0, sour.VisMons[0]);

            if (monthlyRentDatas != null && monthlyRentDatas.Count() > 0)
                re.monthlyRentDatas = monthlyRentDatas;

            int MonthlyLen = monthlyRentDatas.Count;

            if (MonthlyLen > 0)
            {
                re.UseMonthMode = true;
                re.IsMonthRent = 1;

                if (re.flag)
                {
                    try
                    {
                        if (sour.ProjType == 4)
                        {
                            var motoMonth = objUti.Clone(monthlyRentDatas);
                            int motoDisc = sour.Discount;

                            // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
                            var xre = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPriceH, sour.MotoBaseMins, 600, sour.lstHoliday, motoMonth, motoDisc, 600, sour.MaxPrice, sour.MotoBasePrice, sour.FirstFreeMins, sour.GiveMinute);
                            if (xre != null)
                            {
                                re.carInfo = xre;
                                re.CarRental = xre.RentInPay;
                                if (xre.mFinal != null && xre.mFinal.Count > 0)
                                    motoMonth = xre.mFinal;
                                else
                                    motoMonth = new List<MonthlyRentData>();
                                re.useDisc = xre.useDisc;
                            }

                            if (motoMonth != null && motoMonth.Count() > 0 && sour.VisMons != null && sour.VisMons.Count() > 0)   //虛擬月租不存
                                motoMonth = motoMonth.Where(x => !sour.VisMons.Any(y => y.MonthlyRentId == x.MonthlyRentId)).ToList();

                            if (motoMonth.Count > 0)
                            {
                                int UseLen = motoMonth.Count;
                                for (int i = 0; i < UseLen; i++)
                                {
                                    re.flag = monthlyRentRepository.InsMonthlyHistory(
                                        sour.IDNO, sour.intOrderNO, motoMonth[i].MonthlyRentId,
                                        0, 0, 0,
                                        Convert.ToInt32(motoMonth[i].MotoTotalHours),
                                        Convert.ToInt32(motoMonth[i].MotoWorkDayMins),
                                        Convert.ToInt32(motoMonth[i].MotoHolidayMins),
                                        sour.LogID, ref errCode); //寫入記錄
                                }
                            }
                        }
                        else
                        {
                            List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();

                            UseMonthlyRent = monthlyRentDatas;

                            int xDiscount = sour.Discount;//帶入月租運算的折扣
                            if (sour.hasFine)
                            {
                                re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.ED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount, sour.FirstFreeMins, sour.GiveMinute);
                                if (re.carInfo != null)
                                {
                                    re.CarRental += re.carInfo.RentInPay;
                                    if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                        UseMonthlyRent = re.carInfo.mFinal;
                                    else
                                        UseMonthlyRent = new List<MonthlyRentData>();
                                    re.useDisc = re.carInfo.useDisc;
                                }
                            }
                            else
                            {
                                re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.FED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount, sour.FirstFreeMins, sour.GiveMinute);
                                if (re.carInfo != null)
                                {
                                    re.CarRental += re.carInfo.RentInPay;
                                    if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                        UseMonthlyRent = re.carInfo.mFinal;
                                    else
                                        UseMonthlyRent = new List<MonthlyRentData>();
                                    re.useDisc = re.carInfo.useDisc;
                                }
                            }

                            if (UseMonthlyRent != null && UseMonthlyRent.Count() > 0 && //虛擬月租不存
                                sour.VisMons != null && sour.VisMons.Count() > 0)
                                UseMonthlyRent = UseMonthlyRent.Where(x => !sour.VisMons.Any(y => y.MonthlyRentId == x.MonthlyRentId)).ToList();

                            if (UseMonthlyRent.Count > 0)
                            {
                                int UseLen = UseMonthlyRent.Count;
                                for (int i = 0; i < UseLen; i++)
                                {
                                    re.flag = monthlyRentRepository.InsMonthlyHistory(
                                        sour.IDNO, sour.intOrderNO, UseMonthlyRent[i].MonthlyRentId,
                                        Convert.ToInt32(UseMonthlyRent[i].CarTotalHours * 60),
                                        Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60),
                                        Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60),
                                        0, 0, 0,
                                        sour.LogID, ref errCode); //寫入記錄
                                }
                            }
                        }
                        trace.FlowList.Add("月租計算");
                    }
                    catch (Exception ex)
                    {
                        int FunId = SiteUV.GetFunId(funNm);
                        trace.BaseMsg = ex.Message;
                        carRepo.AddTraceLog(FunId, funNm, trace, re.flag);
                        throw;
                    }
                }
            }

            re.errCode = errCode;

            return re;
        }
        #endregion

        #region 汽機車月租,不含逾時(不存檔)
        /// <summary>
        /// 汽機車月租,不含逾時(不存檔)
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OBIZ_MonthRent MonthRentNoSave(IBIZ_MonthRent sour)
        {
            var repo = new CarRentRepo(connetStr);
            var re = new OBIZ_MonthRent();
            var cr_com = new CarRentCommon();
            var monthlyRentRepository = new MonthlyRentRepository(connetStr);
            var monthlyRentDatas = new List<MonthlyRentData>();
            var monthlyHistory = new List<MonthlyRentHis>();
            var billCommon = new BillCommon();
            var errCode = re.errCode;
            re.flag = true;
            bool isSpring = cr_com.isSpring(sour.SD, sour.ED);
            //1.0 先還原這個單號使用的
            //re.flag = monthlyRentRepository.RestoreHistory(sour.IDNO, sour.intOrderNO, sour.LogID, ref errCode);
            //re.errCode = errCode;      

            //跨春節不計算一般月租
            int RateType = (sour.ProjType == 4) ? 1 : 0;
            if (sour.hasFine)
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
            else
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);

            //還原單號使用
            if (monthlyRentDatas != null && monthlyRentDatas.Count() > 0)
            {
                re.monthlyRentDatas = monthlyRentDatas;
                var temp = monthlyRentDatas.Select(x => x.MonthlyRentId.ToString()).ToList();
                string MonthlyRentIds = String.Join(",", temp);
                var monHis = repo.GetMonthlyRentHistory(MonthlyRentIds, sour.intOrderNO.ToString());
                if (monHis != null && monHis.Count() > 0)
                {
                    monthlyRentDatas.ForEach(x =>
                    {
                        x.CarTotalHours += Convert.ToSingle(monHis.Where(g => g.MonthlyRentId == x.MonthlyRentId).Select(h => h.UseCarTotalHours).Sum());
                        x.WorkDayHours += Convert.ToSingle(monHis.Where(a => a.MonthlyRentId == x.MonthlyRentId).Select(b => b.UseWorkDayHours).Sum());
                        x.HolidayHours += Convert.ToSingle(monHis.Where(c => c.MonthlyRentId == x.MonthlyRentId).Select(d => d.UseHolidayHours).Sum());
                        x.MotoTotalHours += Convert.ToSingle(monHis.Where(e => e.MonthlyRentId == x.MonthlyRentId).Select(f => f.UseMotoTotalHours).Sum());
                        x.MotoWorkDayMins += Convert.ToSingle(monHis.Where(i => i.MonthlyRentId == x.MonthlyRentId).Select(j => j.UseMotoWorkDayMins).Sum());
                        x.MotoHolidayMins += Convert.ToSingle(monHis.Where(k => k.MonthlyRentId == x.MonthlyRentId).Select(l => l.UseMotoHolidayMins).Sum());
                    });
                }
            }

            //虛擬月租
            if (sour.VisMons != null && sour.VisMons.Count() > 0)
                monthlyRentDatas.Insert(0, sour.VisMons[0]);

            if (monthlyRentDatas != null && monthlyRentDatas.Count() > 0)
                re.monthlyRentDatas = monthlyRentDatas;

            int MonthlyLen = monthlyRentDatas.Count;
            if (MonthlyLen > 0)
            {
                re.UseMonthMode = true;
                re.IsMonthRent = 1;

                if (re.flag)
                {
                    if (sour.ProjType == 4)
                    {
                        var motoMonth = objUti.Clone(monthlyRentDatas);
                        int motoDisc = sour.Discount;

                        DateTime sprSD = Convert.ToDateTime(SiteUV.strSpringSd);
                        DateTime sprED = Convert.ToDateTime(SiteUV.strSpringEd);

                        //春前
                        if (sour.ED <= sprSD)
                        {
                            var xre = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPrice, sour.MotoBaseMins, 200, sour.lstHoliday, motoMonth, motoDisc, 199, 300);
                            if (xre != null)
                            {
                                re.carInfo = xre;
                                re.CarRental = xre.RentInPay;
                                if (xre.mFinal != null && xre.mFinal.Count > 0)
                                    motoMonth = xre.mFinal;
                                else
                                    motoMonth = new List<MonthlyRentData>();
                                re.useDisc = xre.useDisc;
                            }
                        }
                        //春後
                        else
                        {
                            var xre = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPrice, sour.MotoBaseMins, 600, sour.lstHoliday, motoMonth, motoDisc, 600, 901);
                            if (xre != null)
                            {
                                re.carInfo = xre;
                                re.CarRental = xre.RentInPay;
                                if (xre.mFinal != null && xre.mFinal.Count > 0)
                                    motoMonth = xre.mFinal;
                                else
                                    motoMonth = new List<MonthlyRentData>();
                                re.useDisc = xre.useDisc;
                            }
                        }

                        if (motoMonth != null && motoMonth.Count() > 0 && //虛擬月租不存
                            sour.VisMons != null && sour.VisMons.Count() > 0)
                            motoMonth = motoMonth.Where(x => !sour.VisMons.Any(y => y.MonthlyRentId == x.MonthlyRentId)).ToList();

                        motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
                        if (motoMonth.Count > 0)
                        {

                        }
                    }
                    else
                    {
                        List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();

                        UseMonthlyRent = monthlyRentDatas;

                        int xDiscount = sour.Discount;//帶入月租運算的折扣
                        if (sour.hasFine)
                        {
                            re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.ED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount);
                            if (re.carInfo != null)
                            {
                                re.CarRental += re.carInfo.RentInPay;
                                if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                    UseMonthlyRent = re.carInfo.mFinal;
                                else
                                    UseMonthlyRent = new List<MonthlyRentData>();
                                re.useDisc = re.carInfo.useDisc;
                            }
                        }
                        else
                        {
                            re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.FED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount);
                            if (re.carInfo != null)
                            {
                                re.CarRental += re.carInfo.RentInPay;
                                if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                    UseMonthlyRent = re.carInfo.mFinal;
                                else
                                    UseMonthlyRent = new List<MonthlyRentData>();
                                re.useDisc = re.carInfo.useDisc;
                            }
                        }

                        //不進行sp存檔,以下省略...
                    }
                }
            }

            re.errCode = errCode;

            return re;
        }
        #endregion

        #region GetPayDetail InputCheck
        public OBIZ_InCheck InCheck(IBIZ_InCheck sour)
        {
            var re = new OBIZ_InCheck();
            re.flag = true;
            var baseVerify = new CommonFunc();

            if (string.IsNullOrWhiteSpace(sour.OrderNo))
            {
                re.flag = false;
                re.errCode = "ERR900";
            }
            else
            {
                if (sour.OrderNo.IndexOf("H") < 0)
                {
                    re.flag = false;
                    re.errCode = "ERR900";
                }
                if (re.flag)
                {
                    var longOrderNo = re.longOrderNo;
                    re.flag = Int64.TryParse(sour.OrderNo.Replace("H", ""), out longOrderNo);
                    re.longOrderNo = longOrderNo;
                    if (re.flag)
                    {
                        if (longOrderNo <= 0)
                        {
                            re.flag = false;
                            re.errCode = "ERR900";
                        }
                    }
                }
            }
            if (re.flag)
            {
                if (sour.Discount < 0)
                {
                    re.flag = false;
                    re.errCode = "ERR202";
                }

                if (sour.MotorDiscount < 0)
                {
                    re.flag = false;
                    re.errCode = "ERR202";
                }

                re.Discount = sour.Discount + sour.MotorDiscount;
            }

            return re;
        }
        #endregion

        #region 避免db回傳0
        /// <summary>
        /// 避免db回傳0
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OrderQueryFullData dbValeFix(OrderQueryFullData sour)
        {
            sour.MaxPrice = sour.MaxPrice == 0 ? 300 : sour.MaxPrice;
            sour.MinuteOfPrice = Convert.ToSingle(sour.MinuteOfPrice == 0 ? 1.5 : sour.MinuteOfPrice);
            sour.PRICE = sour.PRICE == 0 ? 99 : sour.PRICE;
            sour.PRICE_H = sour.PRICE_H == 0 ? 168 : sour.PRICE_H;
            return sour;
        }
        #endregion

        #region 取得虛擬月租
        public OBIZ_SpringInit GetVisualMonth(IBIZ_SpringInit sour)
        {
            var re = new OBIZ_SpringInit();
            var spRepo = new CarRentSp();
            string errMsg = "";

            var projType = sour.ProjType;
            if (projType == 0)
            {   // 同站的邏輯比較單純，因為同站只要碰到活動(春節)時專案只會剩下活動專案，因此只要去撈出非活動專案時的價格
                re.ProDisPRICE = sour.ProDisPRICE;      // 活動平日價
                re.ProDisPRICE_H = sour.ProDisPRICE_H;  // 活動假日價
                var visMon = new MonthlyRentData()
                {
                    Mode = 0,
                    MonthlyRentId = 999001,
                    StartDate = Convert.ToDateTime(SiteUV.strSpringSd),
                    EndDate = Convert.ToDateTime(SiteUV.strSpringEd),
                    WorkDayHours = 0,
                    HolidayHours = 0,
                    MotoTotalHours = 0,
                    WorkDayRateForCar = Convert.ToSingle(Math.Floor(sour.ProDisPRICE)),
                    HoildayRateForCar = Convert.ToSingle(Math.Floor(sour.ProDisPRICE_H)),
                    WorkDayRateForMoto = 0,
                    HoildayRateForMoto = 0
                };
                re.VisMons.Add(visMon);

                var ProjectList = spRepo.GetCarProject(sour.ProjID, sour.CarType, sour.OrderNo, sour.IDNO, sour.SD, sour.ED, sour.ProjType, sour.CarNo, sour.LogID, ref errMsg);
                var Normal = ProjectList.Where(x => x.Event == 0).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();
                if (Normal != null)
                {
                    re.PRICE = Normal.PRICE / 10;       // 非活動平日價
                    re.PRICE_H = Normal.PRICE_H / 10;   // 非活動假日價
                }
                else
                {   // 撈不到就給原專案的價格
                    re.PRICE = sour.ProDisPRICE;        // 非活動平日價
                    re.PRICE_H = sour.ProDisPRICE_H;    // 非活動假日價
                }
            }
            else if (projType == 3)
            {   // 路邊的邏輯會比較複雜，因為傳進來的專案不一定，因此要做判斷，將專案價格做調整
                /*
                 * 路邊會有(1)預約起迄日在活動區間 (2)預約迄日跨在活動區間 或 (3)預約起日落在活動區間 三種狀況
                 * EX: 2022春節是 2022/01/29 - 2022/02/06
                 * Case 1:預約是2022/02/01，用車至2022/01/03，此CASE就完整落在活動區間，因此專案只會有一個，非活動平日價/非活動假日價就一樣放活動平日價/活動假日價
                 * 
                 * Case 2:預約是2022/01/28，用車至2022/01/30，因此計價就要拆成兩段，2022/01/28-2022/01/29(一般專案)，2022/01/29-2022/01/30(春節專案)
                 *        這種狀況傳入的PorjID會是一般專案，就要去撈春節專案放到 活動平日價/活動假日價
                 *        
                 * Case 3:預約是2022/02/05，用車至2022/02/08，因此計價就要拆成兩段，2022/02/05-2022/02/06(春節專案)，2022/02/06-2022/02/08(一般專案)
                 *        這種狀況傳入的PorjID會是春節專案，就要去撈一般專案放到 非活動平日價/非活動假日價
                 */

                var ProjectList = spRepo.GetCarProject(sour.ProjID, sour.CarType, sour.OrderNo, sour.IDNO, sour.SD, sour.ED, sour.ProjType, sour.CarNo, sour.LogID, ref errMsg);
                var Event = ProjectList.Where(x => x.Event == 1).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();     // 活動專案
                var Normal = ProjectList.Where(x => x.Event == 0).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();    // 一般專案

                if (Event != null)
                {
                    if (Event.PROJID.Contains(sour.ProjID)) // 活動專案 包含 訂單專案，則要取非活動專案的價格
                    {
                        re.ProDisPRICE = sour.ProDisPRICE;       // 活動平日價
                        re.ProDisPRICE_H = sour.ProDisPRICE_H;   // 活動假日價
                        if (Normal != null)
                        {
                            re.PRICE = Normal.PRICE / 10;       // 非活動平日價
                            re.PRICE_H = Normal.PRICE_H / 10;   // 非活動假日價
                        }
                        else
                        {   // 撈不到就給原專案的價格
                            re.PRICE = sour.ProDisPRICE;        // 非活動平日價
                            re.PRICE_H = sour.ProDisPRICE_H;    // 非活動假日價
                        }
                    }
                    else
                    {   // 訂單專案 不是 活動專案，則要將活動專案的價格放到活動平日價/假日價，訂單專案的價格放到非活動專案的價格
                        re.ProDisPRICE = Event.PRICE / 10;      // 活動平日價
                        re.ProDisPRICE_H = Event.PRICE_H / 10;  // 活動假日價
                        re.PRICE = sour.ProDisPRICE;            // 非活動平日價
                        re.PRICE_H = sour.ProDisPRICE_H;        // 非活動假日價
                    }
                }
                else
                {
                    re.ProDisPRICE = sour.ProDisPRICE;      // 活動平日價
                    re.ProDisPRICE_H = sour.ProDisPRICE_H;  // 活動假日價
                    re.PRICE = sour.ProDisPRICE;            // 非活動平日價
                    re.PRICE_H = sour.ProDisPRICE_H;        // 非活動假日價
                }

                var visMon = new MonthlyRentData()
                {
                    Mode = 0,
                    MonthlyRentId = 999001,
                    StartDate = Convert.ToDateTime(SiteUV.strSpringSd),
                    EndDate = Convert.ToDateTime(SiteUV.strSpringEd),
                    WorkDayHours = 0,
                    HolidayHours = 0,
                    MotoTotalHours = 0,
                    WorkDayRateForCar = Convert.ToSingle(Math.Floor(re.ProDisPRICE)),
                    HoildayRateForCar = Convert.ToSingle(Math.Floor(re.ProDisPRICE_H)),
                    WorkDayRateForMoto = 0,
                    HoildayRateForMoto = 0
                };
                re.VisMons.Add(visMon);
            }
            else if (projType == 4)
            {
                re.ProDisPRICE = sour.ProDisPRICE;
                re.ProDisPRICE_H = sour.ProDisPRICE_H;
                var visMon = new MonthlyRentData()
                {
                    Mode = 1,
                    MonthlyRentId = 999002,
                    StartDate = Convert.ToDateTime(SiteUV.strSpringSd),
                    EndDate = Convert.ToDateTime(SiteUV.strSpringEd),
                    WorkDayHours = 0,
                    HolidayHours = 0,
                    MotoTotalHours = 0,
                    WorkDayRateForCar = 0,
                    HoildayRateForCar = 0,
                    WorkDayRateForMoto = Convert.ToSingle(sour.ProDisPRICE),
                    HoildayRateForMoto = Convert.ToSingle(sour.ProDisPRICE_H)
                };
                re.VisMons.Add(visMon);
                // 20211230 UPD BY YEH REASON:機車已經漲價且沒有春節專案，因此不特別撈價格
                re.PRICE = sour.ProDisPRICE;
                re.PRICE_H = sour.ProDisPRICE_H;
            }

            return re;
        }
        #endregion

        #region 春節專案
        /// <summary>
        /// 春節專案
        /// </summary>
        /// <param name="sour"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public CarRentInfo GetSpringInit(IBIZ_SpringInit sour, string conStr, string funNM = "")
        {
            var carRepo = new CarRentRepo(conStr);
            var trace = new TraceCom();
            var tlog = new TraceLogVM()
            {
                ApiId = 999002,
                ApiNm = "GetSpringInit",
                CodeVersion = trace.codeVersion,
            };
            trace.traceAdd(nameof(sour), sour);

            if (!string.IsNullOrWhiteSpace(funNM))
                trace.traceAdd(nameof(funNM), funNM);

            try
            {
                //if (sour == null || string.IsNullOrWhiteSpace(conStr))
                //    throw new Exception("sour, conStr不可為空");
                //if (sour.ProjType == -1)
                //    throw new Exception("ProjType必填");
                //if (sour.SD == null || sour.ED == null || sour.SD > sour.ED)
                //    throw new Exception("SD, ED錯誤");
                //trace.FlowList.Add("inpt驗證完成");

                var xsour = objUti.Clone(sour);
                if (sour.PRICE <= 0 || sour.PRICE_H <= 0)
                {   //一般平假日價格
                    trace.FlowList.Add("一般平假日價格為0");
                    string errMsg = "";

                    if (sour.ProjType == 0)
                    {
                        var ProjectList = new CarRentSp().GetCarProject(sour.ProjID, sour.CarType, sour.OrderNo, sour.IDNO, sour.SD, sour.ED, sour.ProjType, sour.CarNo, sour.LogID, ref errMsg);
                        var Event = ProjectList.Where(x => x.Event == 1).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();     // 活動專案
                        var Normal = ProjectList.Where(x => x.Event == 0).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();    // 一般專案

                        if (Event != null && Event.PROJID.Contains(sour.ProjID))
                        {
                            if (Normal != null)
                            {
                                xsour.PRICE = Normal.PRICE / 10;
                                xsour.PRICE_H = Normal.PRICE_H / 10;
                            }
                            else
                            {   // 撈不到就給原專案的價格
                                xsour.PRICE = sour.ProDisPRICE;
                                xsour.PRICE_H = sour.ProDisPRICE_H;
                            }
                        }
                    }
                    else if (sour.ProjType == 3)
                    {
                        var ProjectList = new CarRentSp().GetCarProject(sour.ProjID, sour.CarType, sour.OrderNo, sour.IDNO, sour.SD, sour.ED, sour.ProjType, sour.CarNo, sour.LogID, ref errMsg);
                        var Event = ProjectList.Where(x => x.Event == 1).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();     // 活動專案
                        var Normal = ProjectList.Where(x => x.Event == 0).OrderBy(x => x.PRICE).ThenBy(x => x.PRICE_H).FirstOrDefault();    // 一般專案

                        if (Event != null && Event.PROJID.Contains(sour.ProjID))
                        {   // 所選專案 是 活動專案
                            if (Normal != null)
                            {
                                xsour.PRICE = Normal.PRICE / 10;        // 非活動平日價 = 一般專案平日價
                                xsour.PRICE_H = Normal.PRICE_H / 10;    // 非活動假日價 = 一般專案假日價
                            }
                            else
                            {   // 撈不到就給原專案的價格
                                xsour.PRICE = sour.ProDisPRICE;
                                xsour.PRICE_H = sour.ProDisPRICE_H;
                            }
                        }
                        else
                        {
                            //20220129 ADD BY ADAM REASON.春節路邊專案有可能取不到Normal專案
                            if (Normal != null)
                            {
                                xsour.PRICE = Normal.PRICE / 10;
                                xsour.PRICE_H = Normal.PRICE_H / 10;
                            }
                            else
                            {
                                // 撈不到就給原專案的價格
                                xsour.PRICE = sour.ProDisPRICE;
                                xsour.PRICE_H = sour.ProDisPRICE_H;
                            }
                        }
                    }
                }
                if (sour.ProDisPRICE <= 0 || sour.ProDisPRICE_H < 0)
                {   //專案會升級春節虛擬月租
                    trace.FlowList.Add("專案平假日價格為0");
                    //春節專案平假日價格 
                    trace.FlowList.Add("春節期間");
                    if (string.IsNullOrWhiteSpace(sour.CarType))
                        throw new Exception("CarType必填");
                    if (sour.ProjType == 0)
                    {
                        var xre = carRepo.GetFirstProDisc("R320", sour.CarType);
                        if (xre != null)
                        {
                            trace.traceAdd(nameof(xre), xre);
                            xsour.ProDisPRICE = xre.PRICE / 10;
                            xsour.ProDisPRICE_H = xre.PRICE_H / 10;
                        }
                    }
                    else if (sour.ProjType == 3)
                    {
                        var xre = carRepo.GetFirstProDisc("R321", sour.CarType);
                        if (xre != null)
                        {
                            trace.traceAdd(nameof(xre), xre);
                            xsour.ProDisPRICE = xre.PRICE / 10;
                            xsour.ProDisPRICE_H = xre.PRICE_H / 10;
                        }
                    }
                }

                #region trace
                trace.traceAdd(nameof(xsour), xsour);
                trace.FlowList.Add("呼叫計算");
                tlog.ApiMsg = JsonConvert.SerializeObject(trace.getObjs());
                tlog.FlowStep = trace.FlowStep();
                tlog.TraceType = eumTraceType.fun;
                //carRepo.AddTraceLog(tlog);
                #endregion

                return xGetSpringInit(xsour, conStr, funNM);
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                tlog.ApiMsg = ex.Message;
                tlog.FlowStep = JsonConvert.SerializeObject(trace.getObjs());
                tlog.TraceType = eumTraceType.exception;
                carRepo.AddTraceLog(tlog);
                throw;
            }
        }

        /// <summary>
        /// 春節月租-汽車
        /// </summary>
        /// <param name="sour"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        private CarRentInfo xGetSpringInit(IBIZ_SpringInit sour, string conStr, string funNm = "")
        {
            var re = new CarRentInfo();
            //string funNm = "xGetSpringInit";
            var carReo = new CarRentRepo(connetStr);
            var monRents = new List<MonthlyRentData>();
            var traceLog = new TraceLogVM()
            {
                ApiId = 99901,
                ApiNm = "xGetSpringInit",
                CodeVersion = SiteUV.codeVersion
            };
            var trace = new TraceCom();
            var funInput = objUti.Clone(sour);
            trace.traceAdd(nameof(funInput), funInput);
            if (!string.IsNullOrWhiteSpace(funNm))
                trace.traceAdd(nameof(funNm), funNm);
            try
            {
                var monRepo = new MonthlyRentRepository(conStr);
                var carRentSp = new CarRentSp();
                var bill = new BillCommon();
                if (string.IsNullOrWhiteSpace(conStr))
                    throw new Exception("連線字串必填");

                if (sour == null|| sour.SD == null || sour.ED == null || sour.SD > sour.ED
                    //|| string.IsNullOrWhiteSpace(sour.IDNO)
                    )
                    throw new Exception("sour資料錯誤");
                trace.FlowList.Add("sour檢核完成");

                bool isPriceEdit = false;
                if (sour.PRICE <= 0 || sour.PRICE_H <= 0 || sour.ProDisPRICE <= 0 || sour.ProDisPRICE_H <= 0)
                {
                    trace.LogicErr.Add("一般平假日或虛擬月租價格有0");
                    isPriceEdit = true;
                }

                if (sour.PRICE <= 0) sour.PRICE = 99;
                if (sour.PRICE_H <= 0) sour.PRICE_H = 168;
                if (sour.ProDisPRICE <= 0) sour.ProDisPRICE = 99;
                if (sour.ProDisPRICE_H <= 0) sour.ProDisPRICE = 168;

                if (isPriceEdit)
                {
                    var priEdit = objUti.Clone(sour);
                    trace.traceAdd(nameof(priEdit), priEdit);
                }

                //一般月租:汽車月租不會跟春節重疊-確認過
                var month = monRepo.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), 0);
                if (month != null && month.Count() > 0)
                {
                    trace.traceAdd(nameof(month), month);
                    monRents.AddRange(month);
                    monRents.ForEach(x => { x.WorkDayHours = 0; x.HolidayHours = 0; x.MotoTotalHours = 0; });
                    trace.FlowList.Add("一般月租");
                }

                //春節期間才會加入虛擬春節月租
                var monSpring = new MonthlyRentData();
                if (isSpring(sour.SD, sour.ED))
                {
                    monSpring = new MonthlyRentData()
                    {
                        MonthlyRentId = 99999,
                        StartDate = Convert.ToDateTime(SiteUV.strSpringSd),
                        EndDate = Convert.ToDateTime(SiteUV.strSpringEd),
                        WorkDayRateForCar = Convert.ToSingle(sour.ProDisPRICE),
                        HoildayRateForCar = Convert.ToSingle(sour.ProDisPRICE_H),
                        Mode = 0
                    };
                    monRents.Insert(0, monSpring);
                    trace.FlowList.Add("加入春節月租");
                    trace.traceAdd(nameof(monSpring), monSpring);
                }

                re = bill.CarRentInCompute(sour.SD, sour.ED, sour.PRICE, sour.PRICE_H, 60, 10, sour.lstHoliday, monRents, 0);
                trace.traceAdd(nameof(re), re);
                trace.FlowList.Add("月租計算");

                #region trace
                if (re.RentInPay == 0)
                    trace.marks.Add("租金為0");
                traceLog.TraceType = eumTraceType.mark;
                traceLog.ApiMsg = JsonConvert.SerializeObject(trace.getObjs());
                //carReo.AddTraceLog(traceLog);
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                traceLog.TraceType = eumTraceType.exception;
                traceLog.ApiMsg = ex.Message;
                traceLog.FlowStep = JsonConvert.SerializeObject(trace.getObjs());
                carReo.AddTraceLog(traceLog);
                throw;
            }

            return re;
        }

        public bool isSpring(DateTime SD, DateTime ED)
        {
            if (SD == null || ED == null || SD > ED)
                throw new Exception("isSpring:SD,ED錯誤");
            DateTime vsd = Convert.ToDateTime(SiteUV.strSpringSd);
            DateTime ved = Convert.ToDateTime(SiteUV.strSpringEd);
            if (ED > vsd && ED <= ved)  // 還車時間 > 春節起日 AND 還車時間<= 春節迄日
                return true;
            else if (SD >= vsd && SD < ved) // 開始用車時間 >= 春節起日 && 開始用車時間 < 春節迄日
                return true;
            return false;
        }
        #endregion

        /// <summary>
        /// 取得針對車號取得預估租金的優惠標籤
        /// </summary>
        /// <param name="spInput"></param>
        /// <returns></returns>
        public DiscountLabel GetDiscountLabelByCar(SPInput_GetDiscountLabelByCarNo spInput)
        {
            var baseVerify = new CommonFunc();
            string SPName = "usp_GetDiscountLabelByCarNo";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetDiscountLabelByCarNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetDiscountLabelByCarNo, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            List<DiscountLabel> list = new List<DiscountLabel>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "000000";
            bool flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref list, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

            DiscountLabel result = (flag && list.Count > 0) ? list.FirstOrDefault() ?? null : null;

            return result;
        }


        /// <summary>
        /// 取得優惠標籤(AnyRent提供車號清單)
        /// </summary>
        /// <param name="spInput"></param>
        /// <returns></returns>
        public List<AnyRentDiscountLabel> GetDiscountLabelForAnyRentCars(SPInput_GetDiscountLabelForAnyRentCars spInput)
        {
            var baseVerify = new CommonFunc();
            string SPName = "usp_GetDiscountLabelForAnyRentCars";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetDiscountLabelForAnyRentCars, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetDiscountLabelForAnyRentCars, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            List<AnyRentDiscountLabel> list = new List<AnyRentDiscountLabel>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "000000";
            bool flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref list, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

            List<AnyRentDiscountLabel> result = (flag && list.Count > 0) ? list ?? null : null;

            return result;
        }

        /// <summary>
        /// 取得針對車號取得AnyRentProject優惠標籤
        /// </summary>
        /// <param name="spInput"></param>
        /// <returns></returns>
        public ProjectDiscountLabel GetDiscountLabelForAnyRentProject(SPInput_GetDiscountLabelForAnyRentProject spInput)
        {
            var baseVerify = new CommonFunc();
            string SPName = "usp_GetDiscountLabelForAnyRentProject";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetDiscountLabelForAnyRentProject, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetDiscountLabelForAnyRentProject, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            List<DiscountLabel> list = new List<DiscountLabel>();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string errCode = "000000";
            bool flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref list, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

            ProjectDiscountLabel result = (flag && list.Count > 0) 
                ? list.Select(x=>new ProjectDiscountLabel { LabelType = x.LabelType,GiveMinute = x.GiveMinute,AppDescribe = x.Describe, Describe = "", }).FirstOrDefault() 
                ?? new ProjectDiscountLabel() : new ProjectDiscountLabel();

            return result;
        }
    }

    #region repo
    public class CarRentRepo : BaseRepository
    {
        private string defConStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public CarRentRepo()
        {
            this.ConnectionString = defConStr;
        }
        public CarRentRepo(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        #region RePayDetail(重新計算租金明細)用
        public List<MonthlyRentHis> GetMonthlyRentHistory(string MonthlyRentIds, string OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var mHis = new List<MonthlyRentHis>();

            if (string.IsNullOrWhiteSpace(MonthlyRentIds) || string.IsNullOrWhiteSpace(OrderNo))
                throw new Exception("MonthlyRentIds, OrderNo必填");

            string SQL = @"
            SELECT 
            MonthlyRentId, 
            UseCarTotalHours, UseWorkDayHours, UseHolidayHours, 
            UseMotoTotalHours, UseMotoWorkDayMins, UseMotoHolidayMins 
            FROM TB_MonthlyRentHistory
            WHERE OrderNo = {0} AND MonthlyRentId IN ({1}) ";
            SQL = string.Format(SQL, OrderNo, MonthlyRentIds);
            mHis = GetObjList<MonthlyRentHis>(ref flag, ref lstError, SQL, null, "");
            return mHis;
        }
        public List<OrderQueryFullData> GetOrders(string orderNos)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var re = new List<OrderQueryFullData>();

            if (!string.IsNullOrWhiteSpace(orderNos))
            {
                string SQL = "select o.order_number[OrderNo], o.ProjType, o.start_time, o.stop_time, o.init_price from TB_OrderMain o where o.order_number in (" + orderNos + ")";
                re = GetObjList<OrderQueryFullData>(ref flag, ref lstError, SQL, null, "");
            }
            return re;
        }
        public bool SetInitPriceByOrderNo(OrderQueryFullData sour)
        {
            bool flag = true;
            string SQL = "";

            SQL = "UPDATE TB_OrderMain SET init_price= " + sour.init_price + " WHERE order_number = " + sour.OrderNo.ToString();
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        #endregion
        #region 取得短期
        /// <summary>
        /// 取得短期
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <param name="Mode"></param>
        /// <para>0:汽車</para>
        /// <para>1:機車</para>
        /// <returns></returns>
        public List<MonBase> GetMonths(string IDNO, DateTime StartDate, DateTime EndDate, int Mode = -1)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var re = new List<MonBase>();
            if (string.IsNullOrWhiteSpace(IDNO) || StartDate == null || EndDate == null || Mode == -1)
                throw new Exception("GetMonths: 輸入參數皆為必填");

            if (StartDate > EndDate)
                throw new Exception("GetMonths: 起不可大於迄");

            string strSD = StartDate.ToString("yyyy-MM-dd HH:mm");
            string strED = EndDate.ToString("yyyy-MM-dd HH:mm");

            string SQL = "SELECT DISTINCT MonthlyRentId, ProjNM FROM TB_MonthlyRent WHERE Mode = {7} AND IDNO = '{0}'";
            SQL += " AND ((EndDate > '{1}' AND EndDate <= '{2}') OR (StartDate >= '{3}' AND StartDate < '{4}') OR (StartDate <= '{5}' AND EndDate >= '{6}'))";

            SQL = string.Format(SQL, IDNO, strSD, strED, strSD, strED, strSD, strED, Mode.ToString());
            re = GetObjList<MonBase>(ref flag, ref lstError, SQL, null, "");
            return re;
        }
        #endregion
        #region 春節專案用
        public ProjectDiscountTBVM GetFirstProDisc(string ProjID, string CarTypeNm)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var re = new ProjectDiscountTBVM();

            string SQL = @"
            SELECT TOP 1
            v.CarTypeGroupCode,
            v.PRICE,
            v.PRICE_H
            FROM dbo.VW_GetFullProjectCollectionOfCarTypeGroup v WITH(NOLOCK)                 
            WHERE 1=1 ";

            if (!string.IsNullOrWhiteSpace(ProjID))
                SQL += " AND v.PROJID = '" + ProjID + "' ";
            if (!string.IsNullOrWhiteSpace(CarTypeNm))
                SQL += " AND LOWER(v.CarTypeGroupCode) = LOWER('" + CarTypeNm + "')";

            var xre = GetObjList<ProjectDiscountTBVM>(ref flag, ref lstError, SQL, null, "");
            if (xre != null && xre.Count() > 0)
                re = xre.FirstOrDefault();
            return re;
        }
        #endregion
        #region 2021春節訂金資訊
        #region 取得2021春節訂金資訊(目前已無用)
        /// <summary>
        /// 取得訂金資訊
        /// </summary>
        /// <param name="order_number"></param>
        /// <returns></returns>
        public List<NYPayList> GetNYPayList(int order_number)
        {
            var re = new List<NYPayList>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if (order_number <= 0)
                throw new Exception("order_number為必填");
            string SQL = @"select p.order_number, p.PAYDATE, p.PAYAMT, p.RETURNAMT, p.NORDNO from TB_NYPayList p WITH(NOLOCK) where p.order_number = {0} ";
            SQL = String.Format(SQL, order_number.ToString());
            re = GetObjList<NYPayList>(ref flag, ref lstError, SQL, null, "");
            return re;
        }
        #endregion
        #region 更新返還訂金
        /// <summary>
        /// 更新返還訂金
        /// </summary>
        /// <param name="orderNo">訂單編號</param>
        /// <param name="RETURNAMT">返還訂金</param>
        /// <returns></returns>
        public bool UpdNYPayList(int orderNo, int RETURNAMT)
        {
            if (orderNo <= 0)
                throw new Exception("orderNo必填");
            bool flag = true;
            string SQL = @"
                update TB_NYPayList
                set RETURNAMT = {0},
                UPDTime = DATEADD(hour,8,getdate())
                where order_number = {1}";
            SQL = String.Format(SQL, RETURNAMT.ToString(), orderNo.ToString());
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        #endregion
        #endregion
        #region GetCarTypeGroupCode
        public string GetCarTypeGroupCode(string CarNo)
        {
            string re = "";
            var sqlRe = new List<GetFullProjectVM>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if (string.IsNullOrWhiteSpace(CarNo))
                throw new Exception("CarNo 為必填");
            string SQL = @"
	            select top 1 g.CarTypeGroupCode from TB_Car c WITH(NOLOCK)
	            join TB_CarType t WITH(NOLOCK) on t.CarType = c.CarType
	            join TB_CarTypeGroupConsist gc WITH(NOLOCK) on gc.CarType = t.CarType
	            join TB_CarTypeGroup g WITH(NOLOCK) on gc.CarTypeGroupID = g.CarTypeGroupID
	            where c.CarNo = '{0}'";
            SQL = String.Format(SQL, CarNo.ToString());
            sqlRe = GetObjList<GetFullProjectVM>(ref flag, ref lstError, SQL, null, "");

            if (sqlRe != null && sqlRe.Count() > 0)
                re = sqlRe.FirstOrDefault().CarTypeGroupCode;
            return re;
        }
        #endregion
        #region TraceLog
        public bool AddTraceLog(int apiId, string funName, TraceCom trace, bool flag)
        {
            if (!string.IsNullOrWhiteSpace(trace.BaseMsg))
                return AddTraceLog(apiId, funName, eumTraceType.exception, trace);
            else
            {
                if (flag)
                    return AddTraceLog(apiId, funName, eumTraceType.mark, trace);
                else
                    return AddTraceLog(apiId, funName, eumTraceType.followErr, trace);
            }
        }
        public bool AddTraceLog(int apiId, string funName, eumTraceType traceType, TraceCom sour)
        {
            if (sour.objs == null || sour.objs.Count == 0)
                sour.objs = sour.getObjs();
            var item = new TraceLogVM()
            {
                ApiId = apiId,
                ApiMsg = JsonConvert.SerializeObject(sour),
                ApiNm = funName,
                CodeVersion = sour.codeVersion,
                FlowStep = sour.FlowStep(),
                OrderNo = sour.OrderNo,
                TraceType = traceType
            };
            return AddTraceLog(item);
        }
        public bool AddTraceLog(TraceLogVM sour)
        {
            if (sour != null)
            {
                var def = new TraceLogVM();
                if (string.IsNullOrWhiteSpace(sour.CodeVersion))
                    sour.CodeVersion = def.CodeVersion;
                else
                    sour.CodeVersion = sour.CodeVersion.Replace("'", "''");

                if (string.IsNullOrWhiteSpace(sour.ApiNm))
                    sour.ApiNm = def.ApiNm;
                else
                    sour.ApiNm = sour.ApiNm.Replace("'", "''");

                if (string.IsNullOrWhiteSpace(sour.ApiMsg))
                    sour.ApiMsg = def.ApiMsg;
                else
                    sour.ApiMsg = sour.ApiMsg.Replace("'", "''");

                if (string.IsNullOrWhiteSpace(sour.FlowStep))
                    sour.FlowStep = def.FlowStep;
                else
                    sour.FlowStep = sour.FlowStep.Replace("'", "''");

                return xAddTraceLog(sour);
            }
            return false;
        }
        private bool xAddTraceLog(TraceLogVM sour)
        {
            bool flag = true;
            string SQL = "";
            SQL = "INSERT INTO TB_TraceLog (CodeVersion, OrderNo, ApiId, ApiNm, ApiMsg, FlowStep, TraceType)";
            SQL += " VALUES ('" + sour.CodeVersion + "',"
                + sour.OrderNo.ToString() + "," + sour.ApiId.ToString() + "," +
                "'" + sour.ApiNm + "','" + sour.ApiMsg + "','" + sour.FlowStep + "','" + sour.TraceType.ToString() + "'" +
              ")";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        #endregion
    }
    #region 取得專案資訊
    public class CarRentSp
    {
        public GetFullProjectVM sp_GetEstimate(string PROJID, string CARTYPE, long LogID, ref string errMsg, Int64 OrderNo = 0)
        {
            var re = new GetFullProjectVM();

            if (!string.IsNullOrWhiteSpace(PROJID) || !string.IsNullOrWhiteSpace(CARTYPE))
            {
                if (string.IsNullOrWhiteSpace(PROJID) || string.IsNullOrWhiteSpace(CARTYPE))
                    throw new Exception("PROJID, CARTYPE 必填");
            }

            if (string.IsNullOrWhiteSpace(PROJID) && string.IsNullOrWhiteSpace(CARTYPE))
            {
                if (OrderNo == 0)
                    throw new Exception("OrderNo 必填");
            }

            List<GetFullProjectVM> GetFullProjectVMs = new List<GetFullProjectVM>();

            string SPName = new ObjType().GetSPName(ObjType.SPType.GetEstimate);

            object[] param = new object[4];
            param[0] = PROJID;
            param[1] = CARTYPE;
            param[2] = LogID;
            param[3] = OrderNo;

            DataSet ds1 = null;
            string returnMessage = "";
            string messageLevel = "";
            string messageType = "";

            ds1 = WebApiClient.SPRetB(ServerInfo.GetServerInfo(), SPName, param, ref returnMessage, ref messageLevel, ref messageType);

            if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
            {
                if (ds1.Tables.Count >= 2)
                {
                    if (ds1.Tables.Count >= 2)
                        GetFullProjectVMs = objUti.ConvertToList<GetFullProjectVM>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errMsg = re_db.ErrorMsg;
                    }
                }

                if (GetFullProjectVMs != null && GetFullProjectVMs.Count() > 0)
                    re = GetFullProjectVMs.FirstOrDefault();
            }
            else
                errMsg = returnMessage;

            return re;
        }

        /// <summary>
        /// 取得日期區間內的專案列表
        /// </summary>
        /// <param name="ProjID">專案代碼</param>
        /// <param name="CarType">車型代碼</param>
        /// <param name="OrderNo">訂單編號</param>
        /// <param name="IDNO">帳號</param>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="ProjType">專案類型</param>
        /// <param name="CarNo">車號</param>
        /// <param name="LogID"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public List<GetFullProjectVM> GetCarProject(string ProjID, string CarType, Int64 OrderNo, string IDNO, DateTime SD, DateTime ED, int ProjType,string CarNo, long LogID, ref string errMsg)
        {
            var result = new List<GetFullProjectVM>();

            string SPName = "usp_GetEstimate_Q1";

            object[] param = new object[9];
            param[0] = ProjID;
            param[1] = CarType;
            param[2] = OrderNo;
            param[3] = IDNO;
            param[4] = SD;
            param[5] = ED;
            param[6] = ProjType;
            param[7] = CarNo;
            param[8] = LogID;

            DataSet ds1 = null;
            string returnMessage = "";
            string messageLevel = "";
            string messageType = "";

            ds1 = WebApiClient.SPRetB(ServerInfo.GetServerInfo(), SPName, param, ref returnMessage, ref messageLevel, ref messageType);

            if (string.IsNullOrEmpty(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
            {
                if (ds1.Tables.Count == 2)
                {
                    result = objUti.ConvertToList<GetFullProjectVM>(ds1.Tables[0]);
                }
            }
            else
            {
                errMsg = returnMessage;
            }

            return result;
        }
    }
    #endregion
    #region 欠費查詢
    public class ArrearsSp
    {
        public List<ArrearsQueryDetail> sp_ArrearsQuery(WebAPIOutput_NPR330QueryData[] apiList, SPInput_ArrearsQuery spInput, ref string errMsg)
        {
            List<ArrearsQueryDetail> re = new List<ArrearsQueryDetail>();

            try
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetArrearsQuery);
                int apiLen = apiList.Length;
                object[] objparms = new object[apiLen == 0 ? 1 : apiLen];
                if (apiLen > 0)
                {
                    for (int i = 0; i < apiLen; i++)
                    {
                        objparms[i] = new
                        {
                            CUSTID = apiList[i].CUSTID,
                            ORDNO = apiList[i].ORDNO,
                            CNTRNO = apiList[i].CNTRNO,
                            PAYMENTTYPE = apiList[i].PAYMENTTYPE,
                            SPAYMENTTYPE = apiList[i].SPAYMENTTYPE,
                            DUEAMT = apiList[i].DUEAMT,
                            PAIDAMT = apiList[i].PAIDAMT,
                            CARNO = apiList[i].CARNO,
                            POLNO = apiList[i].POLNO,
                            PAYTYPE = apiList[i].PAYTYPE,
                            GIVEDATE = apiList[i].GIVEDATE,
                            RNTDATE = apiList[i].RNTDATE,
                            INBRNHCD = apiList[i].INBRNHCD,
                            IRENTORDNO = apiList[i].IRENTORDNO,
                            TAMT = apiList[i].TAMT
                        };
                    }
                }
                else
                {
                    objparms[0] = new
                    {
                        CARNO = "",
                        CNTRNO = "",
                        CUSTID = "",
                        DUEAMT = 0,
                        GIVEDATE = "",
                        INBRNHCD = "",
                        IRENTORDNO = "",
                        ORDNO = "",
                        PAIDAMT = 0,
                        PAYMENTTYPE = "",
                        PAYTYPE = "",
                        POLNO = "",
                        RNTDATE = "",
                        SPAYMENTTYPE = "",
                        TAMT = 0
                    };
                }

                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.IsSave,
                        spInput.LogID
                    },
                    objparms
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
                {
                    if (ds1.Tables.Count >= 2)
                        re = objUti.ConvertToList<ArrearsQueryDetail>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errMsg = re_db.ErrorMsg;
                    }
                }
                else
                    errMsg = returnMessage;

                return re;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                throw ex;
            }
        }
    }
    #endregion
    #endregion

    #region TraceVm
    public class TraceBase
    {
        public TraceBase()
        {
            codeVersion = SiteUV.codeVersion;
        }
        public string BaseMsg { get; set; }
        /// <summary>
        /// 版號,程式有修改請務必變更
        /// </summary>
        public string codeVersion { get; set; }
        public Int64 OrderNo { set; get; }
        public new List<string> marks { get; set; } = new List<string>();
        public new List<string> LogicErr { get; set; } = new List<string>();
        public new List<string> FlowList { get; set; } = new List<string>();
        public string FlowStep()
        {
            string re = "";
            if (FlowList != null && FlowList.Count() > 0)
            {
                re = string.Join("=>", FlowList);
            }
            return re;
        }
    }
    public class TraceCom : TraceBase
    {
        private Dictionary<string, object> _objs { get; set; }
        public Dictionary<string, object> objs { get; set; }
        public TraceCom()
        {
            _objs = new Dictionary<string, object>();
            objs = new Dictionary<string, object>();
        }
        /// <summary>
        /// 避免重複名稱-新增
        /// </summary>
        /// <param name="xKey"></param>
        /// <param name="xValue"></param>
        public void traceAdd(string xKey, object xValue)
        {
            if (_objs.ContainsKey(xKey))
            {
                string ikey = xKey;
                int itemLoop = 1;
                bool hasItem = true;
                while (hasItem == true && itemLoop <= 100)
                {
                    ikey = xKey + itemLoop.ToString();
                    hasItem = _objs.ContainsKey(ikey);
                    if (!hasItem)
                    {
                        _objs.Add(ikey, xValue);
                        break;
                    }
                    itemLoop += 1;
                }
            }
            else
                _objs.Add(xKey, xValue);
        }
        /// <summary>
        /// 避免重複名稱-取得
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, object> getObjs()
        {
            return _objs;
        }
    }
    #endregion
}