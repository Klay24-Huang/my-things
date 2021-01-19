using Domain.SP.Input.Rent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Enum;
using Domain.SP.Output;
using WebCommon;
using Domain.SP.Output.OrderList;
using System.Data;
using WebAPI.Models.BaseFunc;
using Reposotory.Implement;
using System.Configuration;
using Domain.TB;
using Domain.Common;
using Newtonsoft.Json;
using WebAPI.Utils;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Domain.WebAPI.output.Mochi;
using WebAPI.Models.ComboFunc;
using Domain.SP.BE.Input;
using System.Data.SqlClient;
using WebAPI.Models.Param.Output.PartOfParam;

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

            if (sour.LogID>0 && !string.IsNullOrWhiteSpace(sour.Access_Token))
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
            re.flag = mochi.GetParkingBill(sour.LogID, sour.CarNo, sour.SD, sour.ED.AddDays(1), ref ParkingPrice, ref mochiOutput);
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
            var monthlyRentRepository = new MonthlyRentRepository(connetStr);
            var monthlyRentDatas = new List<MonthlyRentData>();
            var billCommon = new BillCommon();          
            var errCode = re.errCode;
            re.flag = true;
            //1.0 先還原這個單號使用的
            re.flag = monthlyRentRepository.RestoreHistory(sour.IDNO, sour.intOrderNO, sour.LogID, ref errCode);
            re.errCode = errCode;
            int RateType = (sour.ProjType == 4) ? 1 : 0;
            if (sour.hasFine)
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
            else
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);

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
                        var dayMaxMinns = sour.MotoDayMaxMins;
                        int motoDisc = sour.Discount;
                        re.carInfo = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPrice, sour.MotoBaseMins, dayMaxMinns, sour.lstHoliday, motoMonth, motoDisc);

                        if (re.carInfo != null)
                        {
                            re.CarRental += re.carInfo.RentInPay;
                            if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                motoMonth = re.carInfo.mFinal;
                            re.useDisc = re.carInfo.useDisc;
                        }

                        motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
                        if (motoMonth.Count > 0)
                        {
                            int UseLen = motoMonth.Count;
                            for (int i = 0; i < UseLen; i++)
                            {
                                re.flag = monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.intOrderNO, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), sour.LogID, ref errCode); //寫入記錄
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
                            re.carInfo = billCommon.CarRentInCompute(sour.SD, sour.ED, sour.PRICE, sour.PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount);
                            if (re.carInfo != null)
                            {
                                re.CarRental += re.carInfo.RentInPay;
                                if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                    UseMonthlyRent = re.carInfo.mFinal;
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
                                re.useDisc = re.carInfo.useDisc;
                            }
                        }

                        if (UseMonthlyRent.Count > 0)
                        {
                            int UseLen = UseMonthlyRent.Count;
                            for (int i = 0; i < UseLen; i++)
                            {
                                  re.flag = monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.intOrderNO, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, sour.LogID, ref errCode); //寫入記錄
                            }
                        } 
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
            var monthlyRentRepository = new MonthlyRentRepository(connetStr);
            var monthlyRentDatas = new List<MonthlyRentData>();
            var monthlyHistory = new List<MonthlyRentHis>();
            var billCommon = new BillCommon();
            var errCode = re.errCode;
            //1.0 先還原這個單號使用的
            //re.flag = monthlyRentRepository.RestoreHistory(sour.IDNO, sour.intOrderNO, sour.LogID, ref errCode);
            //re.errCode = errCode;      
            re.flag = true;

            int RateType = (sour.ProjType == 4) ? 1 : 0;
            if (sour.hasFine)
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
            else
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);

            //還原單號使用
            if (monthlyRentDatas != null && monthlyRentDatas.Count()>0)
            {
                re.monthlyRentDatas = monthlyRentDatas;
                var temp = monthlyRentDatas.Select(x => x.MonthlyRentId.ToString()).ToList();
                string MonthlyRentIds = String.Join(",", temp);
                var monHis = repo.GetMonthlyRentHistory(MonthlyRentIds, sour.intOrderNO.ToString());
                if(monHis != null && monHis.Count() > 0)
                {
                    monthlyRentDatas.ForEach(x =>
                    {
                        x.WorkDayHours += Convert.ToSingle(monHis.Where(a => a.MonthlyRentId == x.MonthlyRentId).Select(b => b.UseWorkDayHours).Sum());
                        x.HolidayHours += Convert.ToSingle(monHis.Where(c => c.MonthlyRentId == x.MonthlyRentId).Select(d => d.UseHolidayHours).Sum());
                        x.MotoTotalHours += Convert.ToSingle(monHis.Where(e => e.MonthlyRentId == x.MonthlyRentId).Select(f => f.UseMotoTotalHours).Sum());
                    });
                }
            }

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
                        var dayMaxMinns = sour.MotoDayMaxMins;
                        int motoDisc = sour.Discount;
                        re.carInfo = billCommon.MotoRentMonthComp(sour.SD, sour.ED, sour.MinuteOfPrice, sour.MinuteOfPrice, sour.MotoBaseMins, dayMaxMinns, sour.lstHoliday, motoMonth, motoDisc);

                        if (re.carInfo != null)
                        {
                            re.CarRental += re.carInfo.RentInPay;
                            if (re.carInfo.mFinal != null && re.carInfo.mFinal.Count > 0)
                                motoMonth = re.carInfo.mFinal;
                            re.useDisc = re.carInfo.useDisc;
                        }

                        motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
                        if (motoMonth.Count > 0)
                        {
                            //int UseLen = motoMonth.Count;
                            //for (int i = 0; i < UseLen; i++)
                            //{
                            //    if (dbSave)
                            //        re.flag = monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.intOrderNO, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), sour.LogID, ref errCode); //寫入記錄
                            //}
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
                                re.useDisc = re.carInfo.useDisc;
                            }
                        }

                        if (UseMonthlyRent.Count > 0)
                        {
                            re.UseMonthMode = true;
                            //int UseLen = UseMonthlyRent.Count;
                            //for (int i = 0; i < UseLen; i++)
                            //{
                            //    if (dbSave)
                            //        re.flag = monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.intOrderNO, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, sour.LogID, ref errCode); //寫入記錄
                            //}
                        }
                        else
                        {
                            re.UseMonthMode = false;
                        }
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

        /// <summary>
        /// 春節專案
        /// </summary>
        /// <param name="sour"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public CarRentInfo GetSpringInit(IBIZ_SpringInit sour, string conStr)
        {           
            if (sour == null || string.IsNullOrWhiteSpace(conStr))
                throw new Exception("sour, conStr不可為空");
            var xsour = objUti.Clone(sour);
            if (sour.PRICE <= 0 || sour.PRICE_H <= 0)
            {//一般平假日價格
                string errMsg = "";
                //P735暫時寫死:GetEstimate使用
                var norPri = new CarRentSp().sp_GetEstimate("P735", sour.CarType, sour.LogID, ref errMsg);
                if (norPri != null)
                {
                    xsour.PRICE = norPri.PRICE/10;
                    xsour.PRICE_H = norPri.PRICE_H/10;
                }
            }
            if(sour.ProDisPRICE <= 0 || sour.ProDisPRICE_H <0)
            {//春節專案平假日價格
                if(!string.IsNullOrWhiteSpace(sour.ProjID) && !string.IsNullOrWhiteSpace(sour.CarType))
                {
                    if (isSpring(sour.SD, sour.ED))
                    {
                        //春節期間增加:GetProgect使用
                        var xre = new CarRentRepo(conStr).GetFirstProDisc("R129", sour.CarType);
                        if (xre != null)
                        {
                            xsour.ProDisPRICE = xre.PRICE / 10;
                            xsour.ProDisPRICE_H = xre.PRICE_H / 10;
                        }
                    }
                }
            }
            return xGetSpringInit(xsour, conStr);
        }

        /// <summary>
        /// 春節月租
        /// </summary>
        /// <param name="sour"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        private CarRentInfo xGetSpringInit(IBIZ_SpringInit sour, string conStr)
        {
            var re = new CarRentInfo();
            string funNm = "xGetSpringInit";
            var carReo = new CarRentRepo(connetStr);
            var monRents = new List<MonthlyRentData>();
            var traceLog = new TraceLogVM()
            {
                ApiId = 99901,
                ApiNm = funNm,
                CodeVersion = SiteUV.codeVersion                
            };
            var trace = new TraceBase();
            string apiMsg = "";

            try
            {
                var monRepo = new MonthlyRentRepository(conStr);
                var carRentSp = new CarRentSp();
                var bill = new BillCommon();
                if (string.IsNullOrWhiteSpace(conStr))
                    throw new Exception("連線字串必填");

                if (sour == null || string.IsNullOrWhiteSpace(sour.ProjID)
                    || string.IsNullOrWhiteSpace(sour.CarType)
                    || sour.SD == null || sour.ED == null || sour.SD > sour.ED
                    || string.IsNullOrWhiteSpace(sour.IDNO)
                    )
                    throw new Exception("sour資料錯誤");
                trace.FlowList.Add("sour檢核完成");
                apiMsg += JsonConvert.SerializeObject(sour);

                if (sour.PRICE <= 0)  sour.PRICE = 99;
                if (sour.PRICE_H <= 0) sour.PRICE_H = 168;
                if (sour.ProDisPRICE <= 0) sour.ProDisPRICE = 99;
                if (sour.ProDisPRICE_H <= 0) sour.ProDisPRICE = 168;

                //一般月租
                var month = monRepo.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), 0);
                if (month != null && month.Count() > 0)
                {
                    monRents.AddRange(month);
                    monRents.ForEach(x => { x.WorkDayHours = 0; x.HolidayHours = 0; x.MotoTotalHours = 0; });
                }
                trace.FlowList.Add("一般月租");

                //春節期間才會加入虛擬春節月租
                var monSpring = new MonthlyRentData();
                if (isSpring(sour.SD, sour.ED))
                {
                    monSpring = new MonthlyRentData()
                    {
                        MonthlyRentId = 99999,
                        StartDate = Convert.ToDateTime("2021-02-09 00:00:00"),
                        EndDate = Convert.ToDateTime("2021-02-17 00:00:00"),
                        WorkDayRateForCar = Convert.ToSingle(sour.ProDisPRICE),
                        HoildayRateForCar = Convert.ToSingle(sour.ProDisPRICE_H),
                        Mode = 0
                    };
                    monRents.Add(monSpring);
                    trace.FlowList.Add("加入春節月租");
                    apiMsg += JsonConvert.SerializeObject(monSpring); 
                }

                re = bill.CarRentInCompute(sour.SD, sour.ED, sour.PRICE, sour.PRICE_H, 60, 10, sour.lstHoliday, monRents, 0);
                apiMsg += JsonConvert.SerializeObject(re);
                trace.FlowList.Add("月租計算");               
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                traceLog.TraceType = eumTraceType.exception;
                traceLog.ApiMsg = apiMsg+JsonConvert.SerializeObject(trace.FlowList);
                carReo.AddTraceLog(traceLog);
                throw;
            }

            return re;
        }

        private bool isSpring(DateTime SD, DateTime ED)
        {
            DateTime vsd = Convert.ToDateTime("2021-02-09 00:00:00");
            DateTime ved = Convert.ToDateTime("2021-02-17 00:00:00");
            if (ED > vsd && ED <= ved)
                return true;
            else if (SD >= vsd && SD < ved)
                return true;
            return false;
        }
    }

    public static class SiteUV
    {
        /// <summary>
        /// 版號
        /// </summary>
        public static readonly string codeVersion = "202101151740";//hack: 修改程式請修正此版號
    }

    #region repo
    //note: repo
    public class CarRentRepo : BaseRepository
    {
        private string _connectionString;

        public CarRentRepo(string ConnStr)
        {
            this.ConnectionString = ConnStr;
        }
        public List<MonthlyRentHis> GetMonthlyRentHistory(string MonthlyRentIds, string OrderNo)
        {
            bool flag = false;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            var mHis = new List<MonthlyRentHis>();
            string SQL = "SELECT MonthlyRentId, UseWorkDayHours, UseHolidayHours, UseMotoTotalHours FROM TB_MonthlyRentHistory WHERE OrderNo = " + OrderNo + " AND  MonthlyRentId IN (" + MonthlyRentIds + ")";
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

        public  ProjectDiscountTBVM GetFirstProDisc(string ProjID, string CarTypeNm)
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
            if(!string.IsNullOrWhiteSpace(CarTypeNm))
                SQL += " AND LOWER(v.CarTypeGroupCode) = LOWER('" + CarTypeNm + "')";

            var xre = GetObjList<ProjectDiscountTBVM>(ref flag, ref lstError, SQL, null, "");
            if (xre != null && xre.Count() > 0)
                re = xre.FirstOrDefault();
            return re;
        }

        public bool SetInitPriceByOrderNo(OrderQueryFullData sour)
        {
            bool flag = true;
            string SQL = "";

            SQL = "UPDATE TB_OrderMain SET init_price= " + sour.init_price + " WHERE OrderNo = " + sour.OrderNo.ToString();
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

        public bool AddTraceLog(TraceLogVM sour)
        {
            if (sour != null)
            {
                var def = new TraceLogVM();
                if (string.IsNullOrWhiteSpace(sour.CodeVersion))
                    sour.CodeVersion = def.CodeVersion;
                if (string.IsNullOrWhiteSpace(sour.ApiNm))
                    sour.ApiNm = def.ApiNm;
                if (string.IsNullOrWhiteSpace(sour.ApiMsg))
                    sour.ApiMsg = def.ApiMsg;
                if (string.IsNullOrWhiteSpace(sour.FlowStep))
                    sour.FlowStep = def.FlowStep;
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
        public GetFullProjectVM sp_GetEstimate(string PROJID, string CARTYPE, long LogID, ref string errMsg)
        {
            var re = new GetFullProjectVM();
            if (string.IsNullOrWhiteSpace(PROJID) || string.IsNullOrWhiteSpace(CARTYPE))
                throw new Exception("PROJID, CARTYPE 必填");

            List<GetFullProjectVM> GetFullProjectVMs = new List<GetFullProjectVM>();

            string SPName = new ObjType().GetSPName(ObjType.SPType.GetEstimate);

            object[] param = new object[3];
            param[0] = PROJID;
            param[1] = CARTYPE;
            param[2] = LogID;

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
    //public class OBIZ_SpringInit
    //{

    //}
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
    public class OBIZ_TokenCk: BIZ_CRBase
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
    public class OBIZ_CRNoMonth: BIZ_CRBase
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
    public class OBIZ_CRNoMonthDisc: BIZ_CRBase
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
    public class OBIZ_NPR270Query: BIZ_CRBase
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
    public class OBIZ_ETagCk: BIZ_CRBase
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
    }
    public class OBIZ_CarMagi: BIZ_CRBase
    {
        /// <summary>
        /// 車麻吉費用
        /// </summary>
        public int ParkingFee { set; get; }
    }
    public class IBIZ_MonthRent
    {
        public string IDNO { get; set; }
        public Int64 LogID { get; set; }
        public Int64 intOrderNO { get; set; }
        public int ProjType { get; set; }
        /// <summary>
        /// 單日計費最大分鐘數
        /// </summary>
        public double MotoDayMaxMins { get; set; }
        /// <summary>
        /// 每分鐘多少-機車
        /// </summary>
        public double MinuteOfPrice { set; get; }
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
    }
    public class OBIZ_MonthRent: BIZ_CRBase
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
        public  bool isGuest { get; set; }
    }
    public class OBIZ_InCheck: BIZ_CRBase
    {
       public long longOrderNo { get; set; }
       public int Discount { set; get; }
    }
    public class MonthlyRentHis
    {
        public int MonthlyRentId { get; set; }
        public double UseWorkDayHours { get; set; }
        public double UseHolidayHours { get; set; }
        public double UseMotoTotalHours { get; set; }
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
        public new List<string> FlowList { get; set; } = new List<string>();
        public string FlowStep()
        {
            string re = "";
            if(FlowList != null && FlowList.Count() > 0)
            {
                re = string.Join("=>", FlowList);
            }
            return re;
        }
    }

    public class TraceCom : TraceBase
    {
        public Dictionary<string, object> Trace = new Dictionary<string, object>();
    }

    public class PayTraceBase: TraceBase
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

    public class ContactSetTrace: PayTraceBase
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