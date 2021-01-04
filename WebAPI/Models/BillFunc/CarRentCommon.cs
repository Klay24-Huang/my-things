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

        public OBIZ_CRNoMonthDisc CRNoMonthDisc(IBIZ_CRNoMonthDisc sour)
        {
            var re = new OBIZ_CRNoMonthDisc();
            if (sour.UseMonthMode)
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
                re.nor_car_PayDiscPrice = Convert.ToInt32(Math.Floor(discPrice));
                re.UseDisc = re.nor_car_PayDisc;
            }

            return re;
        }

        #region mark-GetCarRentNoMonth
        /// <summary>
        /// 非月租汽車租金
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        //public CarRentInit GetCarRentNoMonth(CarRentInit sour)
        //{
        //    if (sour.flag)
        //    {
        //        sour.car_n_price = sour.OrderDataLists[0].PRICE;
        //        sour.car_h_price = sour.OrderDataLists[0].PRICE_H;

        //        if (sour.ProjType == 4)
        //        {

        //        }
        //        else
        //        {
        //            if (sour.hasFine)
        //            {
        //                var reInMins = sour.billCommon.GetCarRangeMins(sour.SD, sour.ED, sour.carBaseMins, 600, sour.lstHoliday);
        //                if (reInMins != null)
        //                {
        //                    sour.car_payInMins = Convert.ToInt32(reInMins.Item1 + reInMins.Item2);
        //                    sour.car_payAllMins += sour.car_payInMins;
        //                    sour.car_pay_in_wMins = reInMins.Item1;
        //                    sour.car_pay_in_hMins = reInMins.Item2;
        //                }

        //                var reOutMins = sour.billCommon.GetCarOutComputeMins(sour.ED, sour.FED, 0, 360, sour.lstHoliday);
        //                if (reOutMins != null)
        //                {
        //                    sour.car_payOutMins = Convert.ToInt32(reOutMins.Item1 + reOutMins.Item2);
        //                    sour.car_payAllMins += sour.car_payOutMins;
        //                    sour.car_pay_out_wMins = reOutMins.Item1;
        //                    sour.car_pay_out_hMins = reOutMins.Item2;
        //                }

        //                sour.car_inPrice = sour.billCommon.CarRentCompute(sour.SD, sour.ED, sour.car_n_price * 10, sour.car_h_price * 10, 10, sour.lstHoliday);
        //                sour.car_outPrice = sour.billCommon.CarRentCompute(sour.ED, sour.FED, sour.OrderDataLists[0].WeekdayPrice, sour.OrderDataLists[0].HoildayPrice, 6, sour.lstHoliday, true, 0);
        //            }
        //            else
        //            {
        //                var reAllMins = sour.billCommon.GetCarRangeMins(sour.SD, sour.FED, sour.carBaseMins, 600, sour.lstHoliday);
        //                if (reAllMins != null)
        //                {
        //                    sour.car_payAllMins = Convert.ToInt32(reAllMins.Item1 + reAllMins.Item2);
        //                    sour.car_payInMins = sour.car_payAllMins;
        //                    sour.car_pay_in_wMins = reAllMins.Item1;
        //                    sour.car_pay_in_hMins = reAllMins.Item2;
        //                }

        //                sour.car_inPrice = sour.billCommon.CarRentCompute(sour.SD, sour.FED, sour.car_n_price * 10, sour.car_h_price * 10, 10, sour.lstHoliday);
        //            }
        //        }
        //    }
        //    return sour;
        //}
        #endregion

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
        /// 汽機車月租,不含逾時
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public OBIZ_MonthRent MonthRent(IBIZ_MonthRent sour)
        {
            var re = new OBIZ_MonthRent();
            var monthlyRentRepository = new MonthlyRentRepository(connetStr);
            var monthlyRentDatas = new List<MonthlyRentData>();
            var billCommon = new BillCommon();          
            var errCode = re.errCode;
            //1.0 先還原這個單號使用的
            re.flag = monthlyRentRepository.RestoreHistory(sour.IDNO, sour.intOrderNO, sour.LogID, ref errCode);
            re.errCode = errCode;
            int RateType = (sour.ProjType == 4) ? 1 : 0;
            if (!sour.hasFine)
            {
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
            }
            else
            {
                monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
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
                            re.UseMonthMode = true;
                            int UseLen = motoMonth.Count;
                            for (int i = 0; i < UseLen; i++)
                            {
                                re.flag = monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.intOrderNO, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), sour.LogID, ref errCode); //寫入記錄
                            }
                        }
                        else
                        {
                            re.UseMonthMode = false;
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
                            int UseLen = UseMonthlyRent.Count;
                            for (int i = 0; i < UseLen; i++)
                            {
                                re.flag = monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.intOrderNO, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, sour.LogID, ref errCode); //寫入記錄
                            }
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

    }
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
    public enum eumOrderData
    {
        GetPayDetail,
        ContactSetting
    }
}