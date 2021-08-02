using Domain.SP.BE.Input;
using Domain.SP.Input.Arrears;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
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
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Models.BillFunc
{
    public class CarRentCommon
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        public OBIZ_TokenCk TokenCk(IBIZ_TokenCk sour)
        {
            var re = new OBIZ_TokenCk();
            re.flag = false;

            if (sour.LogID > 0 && !string.IsNullOrWhiteSpace(sour.Access_Token))
            {
                var baseVerify = new CommonFunc();
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = sour.LogID,
                    Token = sour.Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                var lstError = re.lstError;
                re.flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                re.lstError = lstError;
                bool flag = re.flag;
                string errCode = re.errCode;
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                re.flag = flag;
                re.errCode = errCode;
                if (re.flag)
                {
                    re.IDNO = spOut.IDNO;
                }
            }
            return re;
        }

        public OBIZ_CRNoMonth CRNoMonth(IBIZ_CRNoMonth sour)
        {
            var re = new OBIZ_CRNoMonth();
            var billCommon = new BillCommon();
            if (sour.hasFine)
            {
                var reInMins = billCommon.GetCarRangeMins(sour.SD, sour.ED, sour.carBaseMins, 600, sour.lstHoliday);
                if (reInMins != null)
                {
                    re.car_payInMins = Convert.ToInt32(reInMins.Item1 + reInMins.Item2);
                    re.car_payAllMins += re.car_payInMins;
                    re.car_pay_in_wMins = reInMins.Item1;
                    re.car_pay_in_hMins = reInMins.Item2;
                }

                var reOutMins = billCommon.GetCarOutComputeMins(sour.ED, sour.FED, 0, 360, sour.lstHoliday);
                if (reOutMins != null)
                {
                    re.car_payOutMins = Convert.ToInt32(reOutMins.Item1 + reOutMins.Item2);
                    re.car_payAllMins += re.car_payOutMins;
                    re.car_pay_out_wMins = reOutMins.Item1;
                    re.car_pay_out_hMins = reOutMins.Item2;
                }

                re.car_inPrice = billCommon.CarRentCompute(sour.SD, sour.ED, sour.car_n_price * 10, sour.car_h_price * 10, 10, sour.lstHoliday);
                re.car_outPrice = billCommon.CarRentCompute(sour.ED, sour.FED, sour.WeekdayPrice, sour.HoildayPrice, 6, sour.lstHoliday, true, 0);
            }
            else
            {
                var reAllMins = billCommon.GetCarRangeMins(sour.SD, sour.FED, sour.carBaseMins, 600, sour.lstHoliday);
                if (reAllMins != null)
                {
                    re.car_payAllMins = Convert.ToInt32(reAllMins.Item1 + reAllMins.Item2);
                    re.car_payInMins = re.car_payAllMins;
                    re.car_pay_in_wMins = reAllMins.Item1;
                    re.car_pay_in_hMins = reAllMins.Item2;
                }

                re.car_inPrice = billCommon.CarRentCompute(sour.SD, sour.FED, sour.car_n_price * 10, sour.car_h_price * 10, 10, sour.lstHoliday);
            }
            re.flag = true;
            return re;
        }

        /// <summary>
        /// 非月租折扣計算
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OBIZ_CRNoMonthDisc CRNoMonthDisc(IBIZ_CRNoMonthDisc sour)
        {
            var re = new OBIZ_CRNoMonthDisc();
            if (!sour.UseMonthMode)
            {
                if (sour.hasFine)
                {
                    var xre = new BillCommon().CarDiscToPara(sour.SD, sour.ED, sour.CarBaseMins, 600, sour.lstHoliday, sour.Discount);
                    if (xre != null)
                    {
                        re.nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
                        re.nor_car_wDisc = xre.Item2;
                        re.nor_car_hDisc = xre.Item3;
                    }
                }
                else
                {
                    var xre = new BillCommon().CarDiscToPara(sour.SD, sour.FED, sour.CarBaseMins, 600, sour.lstHoliday, sour.Discount);
                    if (xre != null)
                    {
                        re.nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
                        re.nor_car_wDisc = xre.Item2;
                        re.nor_car_hDisc = xre.Item3;
                    }
                }

                var discPrice = Convert.ToDouble(sour.car_n_price) * (re.nor_car_wDisc / 60) + Convert.ToDouble(sour.car_h_price) * (re.nor_car_hDisc / 60);
                //re.nor_car_PayDiscPrice = Convert.ToInt32(Math.Floor(discPrice));
                re.nor_car_PayDiscPrice = Convert.ToInt32(Math.Round(discPrice, 0, MidpointRounding.AwayFromZero));
                re.UseDisc = re.nor_car_PayDisc;
            }

            return re;
        }
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

            bool isSpring = cr_com.isSpring(sour.SD, sour.ED);

            //1.0 先還原這個單號使用的
            re.flag = monthlyRentRepository.RestoreHistory(sour.IDNO, sour.intOrderNO, sour.LogID, ref errCode);

            re.errCode = errCode;
            int RateType = (sour.ProjType == 4) ? 1 : 0;
            //if (sour.hasFine)
            //    monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType, sour.ShortTermIds);
            //else
            //    monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType, sour.ShortTermIds);

            if (sour != null && !string.IsNullOrWhiteSpace(sour.MonIds))
            {
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRatesByMonthlyRentId(sour.IDNO, sour.MonIds);

                //假日優惠費率置換:只限汽車月租,只置換假日
                List<int> CarProTypes = new List<int>() { 0, 3 };
                if (monthlyRentDatas != null && monthlyRentDatas.Count() > 0 && CarProTypes.Any(x=>x == sour.ProjType) && sour.intOrderNO > 0)
                {
                    string xErrMsg = "";
                    foreach (var m in monthlyRentDatas)
                    {
                        var pri = cr_sp.sp_GetEstimate("", "", sour.LogID, ref xErrMsg, sour.intOrderNO);
                        if (pri != null && pri.PRICE_H > 0)
                            m.HoildayRateForCar = Convert.ToSingle(pri.PRICE_H/10);
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

                            DateTime sprSD = Convert.ToDateTime(SiteUV.strSpringSd);
                            DateTime sprED = Convert.ToDateTime(SiteUV.strSpringEd);

                            //春前
                            if (sour.ED <= sprSD)
                            {
                                // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
                                var xre = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPrice, sour.MotoBaseMins, 200, sour.lstHoliday, motoMonth, motoDisc, 199, sour.MaxPrice, sour.MotoBasePrice, sour.FirstFreeMins);
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
                                // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
                                var xre = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPriceH, sour.MotoBaseMins, 600, sour.lstHoliday, motoMonth, motoDisc, 600, sour.MaxPrice, sour.MotoBasePrice, sour.FirstFreeMins);
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

                            //motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0 || x.MotoWorkDayMins >0 || x.MotoHolidayMins > 0).ToList();
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
                                re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.ED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount, sour.FirstFreeMins);
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
                                re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.FED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount, sour.FirstFreeMins);
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
                    catch(Exception ex) 
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

        public OBIZ_SpringInit GetVisualMonth(IBIZ_SpringInit sour)
        {
            var re = new OBIZ_SpringInit();
            var spRepo = new CarRentSp();
            string errMsg = "";

            re.PRICE = sour.PRICE;
            re.PRICE_H = sour.PRICE_H;

            if (sour == null
                || sour.ProjType == -1
                // || string.IsNullOrWhiteSpace(sour.ProjID)
                || string.IsNullOrWhiteSpace(sour.CarType)
                || sour.SD == null || sour.ED == null || sour.SD > sour.ED)
                throw new Exception("GetVisualMonth 輸入資料錯誤");

            var projType = sour.ProjType;
            //var projID = sour.ProjID;
            var carType = sour.CarType;
            if (projType == 0)
            {
                re.ProDisPRICE = sour.ProDisPRICE;
                re.ProDisPRICE_H = sour.ProDisPRICE_H;
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
                var xre = spRepo.sp_GetEstimate("P735", carType, 999999, ref errMsg);
                if (xre != null)
                {
                    re.PRICE = xre.PRICE / 10;
                    re.PRICE_H = xre.PRICE_H / 10;
                }
            }
            else if (projType == 3)
            {
                re.ProDisPRICE = sour.ProDisPRICE;
                re.ProDisPRICE_H = sour.ProDisPRICE_H;
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
                var xre = spRepo.sp_GetEstimate("P621", carType, 999999, ref errMsg);
                if (xre != null)
                {
                    re.PRICE = xre.PRICE / 10;
                    re.PRICE_H = xre.PRICE_H / 10;
                }
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
                var xre = spRepo.sp_GetEstimate("P686", carType, 999999, ref errMsg);
                if (xre != null)
                {
                    re.PRICE = xre.PRICE;
                    re.PRICE_H = xre.PRICE_H;
                }
            }

            return re;
        }

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

            bool isSpr = false;//是否為春節
            try
            {
                if (sour == null || string.IsNullOrWhiteSpace(conStr))
                    throw new Exception("sour, conStr不可為空");
                if (sour.ProjType == -1)
                    throw new Exception("ProjType必填");
                if (sour.SD == null || sour.ED == null || sour.SD > sour.ED)
                    throw new Exception("SD, ED錯誤");

                trace.FlowList.Add("inpt驗證完成");
                isSpr = isSpring(sour.SD, sour.ED);
                trace.traceAdd(nameof(isSpr), isSpr);

                var xsour = objUti.Clone(sour);
                if (sour.PRICE <= 0 || sour.PRICE_H <= 0)
                {//一般平假日價格
                    trace.FlowList.Add("一般平假日價格為0");
                    string errMsg = "";

                    if (isSpr)
                    {
                        if (sour.ProjType == 0)
                        {
                            //P735暫時寫死
                            var norPri = new CarRentSp().sp_GetEstimate("P735", sour.CarType, sour.LogID, ref errMsg);
                            if (norPri != null)
                            {
                                trace.traceAdd(nameof(norPri), norPri);
                                xsour.PRICE = norPri.PRICE / 10;
                                xsour.PRICE_H = norPri.PRICE_H / 10;
                            }
                        }
                        else if (sour.ProjType == 3)
                        {
                            //P621暫時寫死
                            var norPri = new CarRentSp().sp_GetEstimate("P621", sour.CarType, sour.LogID, ref errMsg);
                            if (norPri != null)
                            {
                                trace.traceAdd(nameof(norPri), norPri);
                                xsour.PRICE = norPri.PRICE / 10;
                                xsour.PRICE_H = norPri.PRICE_H / 10;
                            }
                        }
                    }
                }
                if (sour.ProDisPRICE <= 0 || sour.ProDisPRICE_H < 0)
                {//專案會升級春節虛擬月租
                    trace.FlowList.Add("專案平假日價格為0");
                    if (isSpr)
                    {//春節專案平假日價格 
                        trace.FlowList.Add("春節期間");
                        if (string.IsNullOrWhiteSpace(sour.CarType))
                            throw new Exception("CarType必填");
                        if (sour.ProjType == 0)
                        {
                            var xre = carRepo.GetFirstProDisc("R129", sour.CarType);
                            if (xre != null)
                            {
                                trace.traceAdd(nameof(xre), xre);
                                xsour.ProDisPRICE = xre.PRICE / 10;
                                xsour.ProDisPRICE_H = xre.PRICE_H / 10;
                            }
                        }
                        else if (sour.ProjType == 3)
                        {
                            var xre = carRepo.GetFirstProDisc("R139", sour.CarType);
                            if (xre != null)
                            {
                                trace.traceAdd(nameof(xre), xre);
                                xsour.ProDisPRICE = xre.PRICE / 10;
                                xsour.ProDisPRICE_H = xre.PRICE_H / 10;
                            }
                        }
                    }
                }

                #region trace
                trace.traceAdd(nameof(xsour), xsour);
                trace.FlowList.Add("呼叫計算");
                tlog.ApiMsg = JsonConvert.SerializeObject(trace.getObjs());
                tlog.FlowStep = trace.FlowStep();
                tlog.TraceType = eumTraceType.fun;
                carRepo.AddTraceLog(tlog);
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

                if (sour == null
                    || sour.SD == null || sour.ED == null || sour.SD > sour.ED
                    || string.IsNullOrWhiteSpace(sour.IDNO)
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
                bool mark = true;
                if (mark)
                {
                    traceLog.TraceType = eumTraceType.mark;
                    traceLog.ApiMsg = JsonConvert.SerializeObject(trace.getObjs());
                    carReo.AddTraceLog(traceLog);
                }
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
            if (ED > vsd && ED <= ved)
                return true;
            else if (SD >= vsd && SD < ved)
                return true;
            return false;
        }

    }

    #region repo
    //note: repo
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
            SQL +=  " AND ((EndDate > '{1}' AND EndDate <= '{2}') OR (StartDate >= '{3}' AND StartDate < '{4}') OR (StartDate <= '{5}' AND EndDate >= '{6}'))";
            
            SQL = string.Format(SQL, IDNO, strSD, strED, strSD, strED, strSD, strED, Mode.ToString());
            re = GetObjList<MonBase>(ref flag, ref lstError, SQL, null, "");
            return re;
        }
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
        /// <summary>
        /// 取得里程費
        /// </summary>
        /// <param name="ProjID"></param>
        /// <param name="BkTime"></param>
        /// <returns></returns>
        public double GetMilageBase(string ProjID, DateTime BkTime)
        {
            double re = 0;
            var xre = GetMilageSetting(ProjID, BkTime);
            if (xre != null && xre.Count() > 0)
            {
                var fItem = xre.FirstOrDefault();
                re = fItem.MilageBase;
            }

            return re;
        }
        /// <summary>
        /// 取得里程資全部資訊
        /// </summary>
        /// <param name="ProjID"></param>
        /// <param name="BkTime"></param>
        /// <returns></returns>
        public List<MilageSettingTBVM> GetMilageSetting(string ProjID, DateTime BkTime)
        {
            var re = new List<MilageSettingTBVM>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if (string.IsNullOrWhiteSpace(ProjID) || BkTime == null)
                throw new Exception("ProjID, BkTime為必填");
            string SQL = @"
            select ProjID, CarType ,SDate ,EDate ,MilageBase ,use_flag from TB_MilageSetting m
            where m.use_flag = 1  
            AND '{0}' BETWEEN m.SDate AND m.EDate
            and UPPER(m.ProjID) = UPPER('{1}') ";
            string strBkTime = BkTime.ToString("yyyy-MM-dd HH:mm:ss");
            SQL = String.Format(SQL, strBkTime, ProjID);
            re = GetObjList<MilageSettingTBVM>(ref flag, ref lstError, SQL, null, "");
            return re;
        }
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
            string SQL = @"
	            select p.order_number, p.PAYDATE, p.PAYAMT, p.RETURNAMT, p.NORDNO from TB_NYPayList p
	            where p.order_number = {0} ";
            SQL = String.Format(SQL, order_number.ToString());
            re = GetObjList<NYPayList>(ref flag, ref lstError, SQL, null, "");
            return re;
        }

        public List<TraceLogTBVM> GetTraceLog(int OrderNo, string ApiMsg, string TraceType, string OrderNos = "")
        {
            var re = new List<TraceLogTBVM>();
            var sour = new TraceLogTBVM()
            {
                OrderNo = OrderNo,
                ApiMsg = ApiMsg,
                TraceType = TraceType
            };
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string SQL = @"
                select top 100 t.ApiId,t.ApiMsg,t.ApiNm,t.CodeVersion,t.FlowStep,t.OrderNo,t.traceId,t.TraceType 
                from Tb_TraceLog t  where 1=1 ";

            if (string.IsNullOrWhiteSpace(OrderNos))
            {
                if (sour.OrderNo > 0)
                    SQL += " and t.OrderNo = " + sour.OrderNo;
            }
            else
                SQL += " and t.OrderNo in(" + OrderNos + ") ";

            if (!string.IsNullOrWhiteSpace(sour.ApiMsg))
                SQL += " and t.ApiMsg like '%" + sour.ApiMsg + "%' ";
            if (!string.IsNullOrWhiteSpace(sour.TraceType))
                SQL += " and t.TraceType like '%" + sour.TraceType + "%' ";
            re = GetObjList<TraceLogTBVM>(ref flag, ref lstError, SQL, null, "");
            return re;
        }

        public string GetCarTypeGroupCode(string CarNo)
        {
            string re = "";
            var sqlRe = new List<GetFullProjectVM>();
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            if (string.IsNullOrWhiteSpace(CarNo))
                throw new Exception("CarNo 為必填");
            string SQL = @"
	            select top 1 g.CarTypeGroupCode from TB_Car c
	            join TB_CarType t on t.CarType = c.CarType
	            join TB_CarTypeGroupConsist gc on gc.CarType = t.CarType
	            join TB_CarTypeGroup g on gc.CarTypeGroupID = g.CarTypeGroupID
	            where c.CarNo = '{0}'";
            SQL = String.Format(SQL, CarNo.ToString());
            sqlRe = GetObjList<GetFullProjectVM>(ref flag, ref lstError, SQL, null, "");

            if (sqlRe != null && sqlRe.Count() > 0)
                re = sqlRe.FirstOrDefault().CarTypeGroupCode;
            return re;
        }

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
        public bool UpdOrderMainByOrderNo(int orderNo, double init_price, double InsPrice)
        {
            if (orderNo == 0)
                throw new Exception("orderNo必填");

            bool flag = true;
            string SQL = @"
                UPDATE o
                SET o.init_price= {0},
                o.InsurancePurePrice = {1}
                from TB_OrderMain o WHERE 
                o.order_number in ({2})";
            SQL = String.Format(SQL, init_price.ToString(), InsPrice.ToString(), orderNo.ToString());
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        public bool SetInitPriceByOrderNo(OrderQueryFullData sour)
        {
            bool flag = true;
            string SQL = "";

            SQL = "UPDATE TB_OrderMain SET init_price= " + sour.init_price + " WHERE order_number = " + sour.OrderNo.ToString();
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        public bool AddErrLog(string FunName, string Msg, string ErrorCode = "")
        {
            return AddErrLog(FunName, ErrorCode, 0, 999, Msg);
        }
        public bool AddErrLog(string FunName, string ErrorCode, int ErrType, int SQLErrorCode, string SQLErrorDesc)
        {
            if (string.IsNullOrWhiteSpace(FunName))
                FunName = "x";
            if (string.IsNullOrWhiteSpace(ErrorCode))
                FunName = "x";
            if (string.IsNullOrWhiteSpace(SQLErrorDesc))
                FunName = "x";

            bool flag = true;
            string SQL = "";
            SQL = "INSERT INTO TB_ErrorLog([FunName],[ErrorCode],[ErrType],[SQLErrorCode],[SQLErrorDesc],[LogID],[IsSystem])";
            SQL += " VALUES ('" + FunName + "'," +
                "'" + ErrorCode + "'," + ErrType.ToString() + "," + SQLErrorCode.ToString() + "," +
                "'" + SQLErrorDesc + "',9999,1)";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
        public bool AddApiLog(int APIID, string Msg, string OrderNo = "")
        {
            return AddApiLog(APIID, "99999", Msg, OrderNo);
        }
        public bool AddApiLog(int APIID, string CLIENTIP, string APIInput, string ORDNO)
        {
            if (string.IsNullOrWhiteSpace(CLIENTIP))
                CLIENTIP = "x";
            if (string.IsNullOrWhiteSpace(APIInput))
                APIInput = "x";
            if (string.IsNullOrWhiteSpace(ORDNO))
                ORDNO = "x";

            bool flag = true;
            string SQL = "";
            SQL = "INSERT INTO TB_APILog(APIID,CLIENTIP,APIInput,ORDNO)";
            SQL += " VALUES (" + APIID + "," +
                "'" + CLIENTIP + "','" + APIInput + "','" + ORDNO + "'" +
              ")";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }    
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
        public bool AddGoldFlowLog(GoldFlowLogVM sour)
        {
            if (sour != null)
            {
                var def = new GoldFlowLogVM();
                if (string.IsNullOrWhiteSpace(sour.CodeVersion))
                    sour.CodeVersion = def.CodeVersion;
                if (string.IsNullOrWhiteSpace(sour.ApiNm))
                    sour.ApiNm = def.ApiNm;
                if (string.IsNullOrWhiteSpace(sour.ApiMsg))
                    sour.ApiMsg = def.ApiMsg;
                if (string.IsNullOrWhiteSpace(sour.FlowStep))
                    sour.FlowStep = def.FlowStep;
                if (string.IsNullOrWhiteSpace(sour.FlowType))
                    sour.FlowType = def.FlowType;

                return xAddGoldFlowLog(sour);
            }
            return false;
        }
        private bool xAddGoldFlowLog(GoldFlowLogVM sour)
        {
            bool flag = true;
            string SQL = "";
            SQL = "INSERT INTO Tb_GoldFlowLog (CodeVersion, OrderNo, ApiId, ApiNm, ApiMsg, FlowStep, FlowType)";
            SQL += " VALUES ('" + sour.CodeVersion + "',"
                + sour.OrderNo.ToString() + "," + sour.ApiId.ToString() + "," +
                "'" + sour.ApiNm + "','" + sour.ApiMsg + "','" + sour.FlowStep + "','" + sour.FlowType.ToString() + "'" +
              ")";
            ExecNonResponse(ref flag, SQL);
            return flag;
        }
    }

    public class CarRentSp
    {
        public GetFullProjectVM sp_GetEstimate(string PROJID, string CARTYPE, long LogID, ref string errMsg, Int64 OrderNo=0)
        {
            var re = new GetFullProjectVM();

            if(!string.IsNullOrWhiteSpace(PROJID) || !string.IsNullOrWhiteSpace(CARTYPE))
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
    }

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

    #region VM
    //note: vm

    #region 春節月租

    public class IBIZ_SpringInit
    {
        public string IDNO { get; set; }
        public long LogID { set; get; }
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { set; get; }
        public int ProjType { get; set; } = -1;
        /// <summary>
        /// 車型
        /// </summary>
        public string CarType { set; get; }
        /// <summary>
        /// 預計取車時間
        /// </summary>
        public DateTime SD { set; get; }
        /// <summary>
        /// 預計還車時間
        /// </summary>
        public DateTime ED { set; get; }
        /// <summary>
        /// 平日價-小時
        /// </summary>
        public double PRICE { set; get; }
        /// <summary>
        /// 假日價-小時
        /// </summary>
        public double PRICE_H { set; get; }
        /// <summary>
        /// 專案平日價-小時
        /// </summary>
        public double ProDisPRICE { set; get; }
        /// <summary>
        /// 專案假日價-小時
        /// </summary>
        public double ProDisPRICE_H { set; get; }
        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; } = new List<Holiday>();
    }
    public class OBIZ_SpringInit : IBIZ_SpringInit
    {
        /// <summary>
        /// 虛擬月租
        /// </summary>
        public List<MonthlyRentData> VisMons { get; set; } = new List<MonthlyRentData>();
    }

    //public class MonRentDataVM: MonthlyRentData
    //{
    //    public string CarType { get; set; }
    //}

    #endregion

    public class IBIZ_GetPayDetail
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 汽車使用的點數
        /// </summary>
        public int Discount { set; get; }

        /// <summary>
        /// 機車使用的點數
        /// </summary>
        public int MotorDiscount { set; get; }

        public string UserID { get; set; }//呼叫BE_ContactSettingController需要
        public Int64 LogID { get; set; }//呼叫BE_ContactSettingController需要
        public string returnDate { get; set; }//呼叫BE_ContactSettingController需要

        public string funName { get; set; }

        public bool isGuest { get; set; } = true;

        public string Access_Token { get; set; }

        public string ClientIP { get; set; }

        public string errMsg { get; set; }

        public string errCode { get; set; }

        //取出訂單資訊
        public eumOrderData OrderData { get; set; }

        /// <summary>
        /// 若Ck_Token=false時必填
        /// </summary>
        public string xIDNO { set; get; }

        /// <summary>
        /// 防呆驗證
        /// </summary>
        public bool InCheck { get; set; } = false;

        /// <summary>
        /// 是否存入使用月租使用紀錄表
        /// </summary>
        public bool db_InsMonthlyHistory { get; set; } = false;

        /// <summary>
        /// 次否執行usp_CalFinalPrice存檔
        /// </summary>
        public bool db_CalFinalPrice { get; set; } = false;

        /// <summary>
        /// 是否驗證token
        /// </summary>
        public bool Ck_Token { get; set; } = false;

        /// <summary>
        /// 與短租查時數
        /// </summary>
        public bool Call_NPR270Query { get; set; } = false;

        /// <summary>
        /// 查ETAG
        /// </summary>
        public bool Call_ETAG { get; set; } = false;

        /// <summary>
        /// 檢查有無車麻吉停車費用
        /// </summary>
        public bool Call_CarMagi { get; set; } = false;
    }
    public class OBIZ_GetPayDetail : OAPI_GetPayDetail
    {
        public bool flag { get; set; } = false;
        public Dictionary<string, object> objOutput { get; set; }//輸出
    }
    public class BIZ_CRBase
    {
        public bool flag { get; set; }
        public string errMsg { get; set; }
        public string errCode { get; set; }
        public List<ErrorInfo> lstError { get; set; }

        //20210109 ADD BY ADAM REASON.增加constructor
        public BIZ_CRBase()
        {
            flag = false;
            errMsg = "";
            errCode = "000000";
            lstError = new List<ErrorInfo>();
        }
    }
    public class IBIZ_TokenCk
    {
        public Int64 LogID { get; set; }
        public string Access_Token { get; set; }
    }
    public class OBIZ_TokenCk : BIZ_CRBase
    {
        public string IDNO { set; get; }
    }
    /// <summary>
    /// 非月租租金計算in
    /// </summary>
    public class IBIZ_CRNoMonth
    {
        /// <summary>
        /// 汽車平日價-未逾時
        /// </summary>
        public int car_n_price { get; set; }
        /// <summary>
        /// 汽車假日價-未逾時
        /// </summary>
        public int car_h_price { get; set; }
        /// <summary>
        /// 汽車平日價-逾時
        /// </summary>
        public int WeekdayPrice { get; set; }
        /// <summary>
        /// 汽車假日價-逾時
        /// </summary>
        public int HoildayPrice { get; set; }
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public DateTime FED { get; set; }
        /// <summary>
        /// 是否逾時
        /// </summary>
        public bool hasFine { get; set; }
        /// <summary>
        /// 汽車基本分鐘數
        /// </summary>
        public int carBaseMins { get; set; }
        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; }
    }
    /// <summary>
    /// 非月租租金計算out
    /// </summary>
    public class OBIZ_CRNoMonth : BIZ_CRBase
    {
        /// <summary>
        /// 全部計費租用分鐘
        /// </summary>
        public int car_payAllMins { get; set; }
        /// <summary>
        /// 未超時計費分鐘
        /// </summary>
        public int car_payInMins { get; set; }
        /// <summary>
        /// 超時分鐘
        /// </summary>
        public int car_payOutMins { get; set; }
        /// <summary>
        /// 未超時平日計費分鐘
        /// </summary>
        public double car_pay_in_wMins { get; set; }
        /// <summary>
        /// 未超時假日計費分鐘
        /// </summary>
        public double car_pay_in_hMins { get; set; }
        /// <summary>
        /// 超時平日計費分鐘
        /// </summary>
        public double car_pay_out_wMins { get; set; }
        /// <summary>
        /// 超時假日計費分鐘
        /// </summary>
        public double car_pay_out_hMins { get; set; }
        /// <summary>
        /// 未超時費用
        /// </summary>
        public int car_inPrice { get; set; }
        /// <summary>
        /// 超時費用
        /// </summary>
        public int car_outPrice { get; set; }
    }
    public class IBIZ_CRNoMonthDisc
    {
        /// <summary>
        /// false:無月租;true:有月租
        /// </summary>
        public bool UseMonthMode { get; set; }
        /// <summary>
        /// 是否逾時
        /// </summary>
        public bool hasFine { get; set; }
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public DateTime FED { get; set; }
        /// <summary>
        /// 汽車基本分鐘數
        /// </summary>
        public double CarBaseMins { get; set; }
        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; }
        public int Discount { get; set; }
        public int car_n_price { get; set; } = 0;//汽車平日價
        public int car_h_price { get; set; } = 0;//汽車假日價
    }
    public class OBIZ_CRNoMonthDisc : BIZ_CRBase
    {
        /// <summary>
        /// 只有一般時段時平日折扣
        /// </summary>
        public double nor_car_wDisc { get; set; }
        /// <summary>
        /// 只有一般時段時價日折扣
        /// </summary>
        public double nor_car_hDisc { get; set; }
        /// <summary>
        /// 只有一般時段時總折扣
        /// </summary>
        public int nor_car_PayDisc { get; set; }
        /// <summary>
        /// 只有一般時段時總折扣金額
        /// </summary>
        public int nor_car_PayDiscPrice { get; set; }
        public int UseDisc { get; set; }
    }
    public class IBIZ_NPR270Query
    {
        public string IDNO { get; set; }
    }
    public class OBIZ_NPR270Query : BIZ_CRBase
    {
        /// <summary>
        /// 機車點數
        /// </summary>
        public int MotorPoint { get; set; }
        /// <summary>
        /// 汽車點數
        /// </summary>
        public int CarPoint { get; set; }
    }
    public class IBIZ_ETagCk
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
    }
    public class OBIZ_ETagCk : BIZ_CRBase
    {
        /// <summary>
        /// ETAG費用
        /// </summary>
        public int etagPrice { get; set; }
    }
    public class IBIZ_CarMagi
    {
        public Int64 LogID { get; set; }
        /// <summary>
        /// 車號
        /// </summary>
        public string CarNo { set; get; }
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public Int64 OrderNo { get; set; }
    }
    public class OBIZ_CarMagi : BIZ_CRBase
    {
        /// <summary>
        /// 車麻吉費用
        /// </summary>
        public int ParkingFee { set; get; }
    }
    public class IBIZ_MonthRent
    {
        /// <summary>
        /// 取消所有月租
        /// </summary>
        public bool CancelMonthRent { get; set; } = false;
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public Int64 intOrderNO { get; set; }
        public int ProjType { get; set; }
        /// <summary>
        /// 機車基消
        /// </summary>
        public double MotoBasePrice { get; set; }
        /// <summary>
        /// 單日計費最大分鐘數
        /// </summary>
        public double MotoDayMaxMins { get; set; }
        /// <summary>
        /// 每分鐘多少-機車平日
        /// </summary>
        public double MinuteOfPrice { set; get; }
        /// <summary>
        /// 每分鐘多少-機車假日
        /// </summary>
        public float MinuteOfPriceH { get; set; }
        /// <summary>
        /// 是否逾時
        /// </summary>
        public bool hasFine { get; set; }
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public DateTime FED { get; set; }
        /// <summary>
        /// 機車基本分鐘數
        /// </summary>
        public int MotoBaseMins { get; set; }
        /// <summary>
        /// 假日列表
        /// </summary>
        public List<Holiday> lstHoliday { get; set; }
        /// <summary>
        /// 要折抵的點數
        /// </summary>
        public int Discount { get; set; }
        /// <summary>
        /// 平日每小時-汽車
        /// </summary>
        public int PRICE { set; get; }
        /// <summary>
        /// 假日每小時-汽車
        /// </summary>
        public int PRICE_H { set; get; }
        /// <summary>
        /// 汽車基本分鐘數
        /// </summary>
        public int carBaseMins { get; set; }
        /// <summary>
        /// 前n分鐘0元
        /// </summary>
        public double FirstFreeMins { get; set; }
        /// <summary>
        /// 月租Id(可多筆)
        /// </summary>
        public string MonIds { get; set; }
        /// <summary>
        /// 每日上限金額      // 20210709 UPD BY YEH REASON:每日上限從資料庫取得
        /// </summary>
        public int MaxPrice { get; set; }
        public List<MonthlyRentData> VisMons { get; set; }//虛擬月租
    }
    public class OBIZ_MonthRent : BIZ_CRBase
    {
        /// <summary>
        /// false:無月租;true:有月租
        /// </summary>
        public bool UseMonthMode { get; set; }
        /// <summary>
        /// 月租資訊
        /// </summary>
        public List<MonthlyRentData> monthlyRentDatas { get; set; }
        /// <summary>
        /// 是否為月租
        /// <para>0:否</para>
        /// <para>1:是</para>
        /// </summary>
        public int IsMonthRent { set; get; }
        /// <summary>
        /// 車資料
        /// </summary>
        public CarRentInfo carInfo { get; set; }
        /// <summary>
        /// 實際使用使用的折抵點數
        /// </summary>
        public int useDisc { get; set; }
        /// <summary>
        /// 車輛租金
        /// </summary>
        public int CarRental { set; get; }
    }
    public class IBIZ_InCheck
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string OrderNo { set; get; }
        /// <summary>
        /// 汽車使用的點數
        /// </summary>
        public int Discount { set; get; }

        /// <summary>
        /// 機車使用的點數
        /// </summary>
        public int MotorDiscount { set; get; }
        /// <summary>
        /// 是否為訪客
        /// </summary>
        public bool isGuest { get; set; }
        /// <summary>
        /// 月租Id(可多筆)
        /// </summary>
        public string MonIds { get; set; }
    }
    public class OBIZ_InCheck : BIZ_CRBase
    {
        public long longOrderNo { get; set; }
        public int Discount { set; get; }
    }
    public class MonthlyRentHis
    {
        public int MonthlyRentId { get; set; }
        public double UseCarTotalHours { get; set; }
        public double UseWorkDayHours { get; set; }
        public double UseHolidayHours { get; set; }
        public double UseMotoTotalHours { get; set; }
        public double UseMotoWorkDayMins { get; set; }
        public double UseMotoHolidayMins { get; set; }
    }
    public class TraceLogVM
    {
        public string CodeVersion { get; set; } = "x";
        public long OrderNo { get; set; } = 0;
        public int ApiId { get; set; } = 0;
        public string ApiNm { get; set; } = "x";
        public string ApiMsg { get; set; } = "x";
        public string FlowStep { get; set; } = "x";
        public eumTraceType TraceType { get; set; } = eumTraceType.none;
    }
    public class GoldFlowLogVM
    {
        public string CodeVersion { get; set; } = "x";
        public long OrderNo { get; set; } = 0;
        public int ApiId { get; set; } = 0;
        public string ApiNm { get; set; } = "x";
        public string ApiMsg { get; set; } = "x";
        public string FlowStep { get; set; } = "x";
        public string FlowType { get; set; } = "x";
    }

    #endregion

    #region TBVM
    //note: tbvm

    public class ProjectDiscountTBVM
    {
        public string ProjID { get; set; }
        public string CARTYPE { get; set; }
        public string CUSTOMIZE { get; set; }
        public string CUSDAY { get; set; }
        public int DISTYPE { get; set; }//短整數
        public double DISRATE { get; set; }
        public double PRICE { get; set; }
        public double PRICE_H { get; set; }
        public double DISCOUNT { get; set; }
        public double PHOURS { get; set; }
    }

    public class MilageSettingTBVM
    {
        /// <summary>
        /// 專案代碼
        /// </summary>
        public string ProjID { get; set; }
        /// <summary>
        /// 車型
        /// </summary>
        public string CarType { get; set; }
        /// <summary>
        /// 起始日期
        /// </summary>
        public DateTime SDate { get; set; }
        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime EDate { get; set; }
        /// <summary>
        /// 每公里費用
        /// </summary>
        public double MilageBase { get; set; }
        /// <summary>
        /// 是否啟用(0:否;1:是;2:待上線)
        /// </summary>
        public Int16 use_flag { get; set; }
    }

    public class NYPayList
    {
        public Int64 order_number { get; set; }
        public string PAYDATE { get; set; }
        public int PAYAMT { get; set; }
        public int RETURNAMT { get; set; }
        public string NORDNO { get; set; }
    }

    public class TraceLogTBVM
    {
        public int traceId { get; set; }
        public string CodeVersion { get; set; }
        public Int64 OrderNo { get; set; }
        public int ApiId { get; set; }
        public string ApiNm { get; set; }
        public string ApiMsg { get; set; }
        public string FlowStep { get; set; }
        public string TraceType { get; set; }
    }

    #endregion

    #region eunm
    //note: eunm
    public enum eumOrderData
    {
        GetPayDetail,
        ContactSetting
    }
    public enum eumTraceType
    {
        none,
        fun,
        exception,
        followErr,
        logicErr,
        mark
    }

    public enum eumFlowType
    {
        none,
        gold,
    }

    #endregion

    #region TraceVm
    //note: tracevm

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

    public class PayTraceBase : TraceBase
    {
        public string errCode { get; set; }
        public int TotalRentMinutes { get; set; }
        public int Discount { get; set; }
        public int CarPoint { get; set; }
        public int MotorPoint { get; set; }
        public List<OrderQueryFullData> OrderDataLists { get; set; }
        public OBIZ_CRNoMonth CRNoMonth { get; set; }
        public OBIZ_NPR270Query NPR270Query { get; set; }
        public OBIZ_ETagCk ETagCk { get; set; }
        public OBIZ_CarMagi CarMagi { get; set; }
        public OBIZ_MonthRent MonthRent { get; set; }
        public CarRentInfo carInfo { get; set; }
        public OBIZ_CRNoMonthDisc CRNoMonthDisc { get; set; }
    }
    public class GetPayDetailTrace : PayTraceBase
    {
        public bool hasFine { get; set; } = false; //是否逾時
        public int TotalPoint { get; set; }
        public int TransferPrice { get; set; }
        public OBIZ_TokenCk TokenCk { get; set; }
        public IAPI_GetPayDetail apiInput { get; set; }
        public OBIZ_InCheck InCheck { get; set; }
        public OAPI_GetPayDetail outputApi { get; set; }
        public SPInput_CalFinalPrice SPInput { get; set; }
    }

    public class ContactSetTrace : PayTraceBase
    {
        #region input
        public Int64 in_tmpOrder { get; set; }
        public string in_IDNO { get; set; }
        public Int64 in_LogID { get; set; }
        public string in_UserID { get; set; }
        public string in_returnDate { get; set; }
        public string in_errCode { get; set; }
        #endregion
        public int TotalPoint { get; set; }
        public int TransferPrice { get; set; }
        public OAPI_GetPayDetail outputApi { get; set; }
        public SPInput_BE_CalFinalPrice SPInput { get; set; }
    }
    #endregion
}