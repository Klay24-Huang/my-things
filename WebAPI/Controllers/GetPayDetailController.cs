using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
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
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetPayDetailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetPayDetail apiInput = null;
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<OrderQueryFullData> OrderDataLists = null;
            int ProjType = 0;
            string Contentjson = "";
            bool isGuest = true;
            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數
            string IDNO = "";
            int Discount = 0; //要折抵的點數
            List<Holiday> lstHoliday = null; //假日列表
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime FineDate = new DateTime();
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;
            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int TotalFineInsuranceMinutes = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
            int days = 0; int hours = 0; int mins = 0; //以分計費總時數
            int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
            int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int CarRentPrice = 0; //車輛租金
            int MonthlyPoint = 0;   //月租折抵點數        20201128 ADD BY ADAM 
            int MonthlyPrice = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
            int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            bool UseMonthMode = false;  //false:無月租;true:有月租
            int InsurancePerHours = 0;  //安心服務每小時價
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetPayDetail>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else
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
                if (flag)
                {
                    if (apiInput.Discount < 0)
                    {
                        flag = false;
                        errCode = "ERR202";
                    }

                    if (apiInput.MotorDiscount < 0)
                    {
                        flag = false;
                        errCode = "ERR202";
                    }

                    Discount = apiInput.Discount + apiInput.MotorDiscount;
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
            }
            #endregion
            #region 取出基本資料
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }

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
                    string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
                    SPOutput_Base spOutBase = new SPOutput_Base();
                    SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                    OrderDataLists = new List<OrderQueryFullData>();
                    DataSet ds = new DataSet();
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
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
            }
            #endregion

            #region 第二階段判斷及計價
            if (flag)
            {
                //判斷狀態
                if (OrderDataLists[0].car_mgt_status < 11 || OrderDataLists[0].cancel_status > 0)
                {
                    flag = false;
                    errCode = "ERR204";
                }
                //取得專案狀態
                if (flag)
                {
                    ProjType = OrderDataLists[0].ProjType;
                    SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                    SD = SD.AddSeconds(SD.Second * -1); //去秒數
                    //機車路邊不計算預計還車時間
                    if (OrderDataLists[0].ProjType == 4)
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    }
                    else
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    }
                    FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                    FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (FED.Subtract(ED).Ticks > 0)
                    {
                        FineDate = ED;
                        hasFine = true;
                        billCommon.CalDayHourMin(SD, ED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                        billCommon.CalDayHourMin(ED, FED, ref FineDays, ref FineHours, ref FineMins);
                        TotalFineRentMinutes = ((FineDays * 10) + FineHours) * 60 + FineMins; //逾時的總時數
                        TotalFineInsuranceMinutes = ((FineDays * 6) + FineHours) * 60 + FineMins;  //逾時的安心服務總計(一日上限6小時)
                    }
                    else
                    {
                        billCommon.CalDayHourMin(SD, FED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes = ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                    }
                }
                if (flag)
                {
                    if (NowTime.Subtract(FED).TotalMinutes >= 30)
                    {
                        flag = false;
                        errCode = "ERR208";
                    }
                }

                #region 與短租查時數
                if (flag)
                {
                    WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
                    HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                    flag = wsAPI.NPR270Query(IDNO, ref wsOutput);
                    if (flag)
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
                                        CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                    }
                                    else
                                    {
                                        MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        flag = true;
                        errCode = "0000";
                    }
                    //判斷輸入的點數有沒有超過總點數
                    if (ProjType == 4)
                    {
                        if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
                        {
                            flag = false;
                            errCode = "ERR205";
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

                        if (flag)
                        {
                            billCommon.CalPointerToDayHourMin(MotorPoint + CarPoint, ref PDays, ref PHours, ref PMins);
                        }
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
                        if (flag)
                        {
                            billCommon.CalPointerToDayHourMin(CarPoint, ref PDays, ref PHours, ref PMins);
                        }
                    }
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
                }
                #endregion
                #region 月租
                if (flag)
                {
                    //1.0 先還原這個單號使用的
                    flag = monthlyRentRepository.RestoreHistory(IDNO, tmpOrder, LogID, ref errCode);
                    int RateType = (ProjType == 4) ? 1 : 0;
                    if (!hasFine)
                    {
                        monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                    }
                    else
                    {
                        monthlyRentDatas = monthlyRentRepository.GetSubscriptionRates(IDNO, SD.ToString("yyyy-MM-dd HH:mm:ss"), FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                    }
                    int MonthlyLen = monthlyRentDatas.Count;
                    //先計算剩餘時數，以免落入基消陷阱
                    int MonthAll = 0;
                    for (int i=0;i< MonthlyLen;i++)
                    {
                        MonthAll += Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);
                    }
                    //先設定一遍
                    outputApi.Rent.RemainMonthlyTimeInterval = MonthlyPoint.ToString();
                    if (MonthlyLen > 0 && MonthAll > 0)
                    {
                        UseMonthMode = true;
                        outputApi.IsMonthRent = 1;
                        if (flag)
                        {
                            if (ProjType == 4)
                            {
                                //機車沒有分平假日，直接送即可
                                for (int i = 0; i < MonthlyLen; i++)
                                {
                                    int MotoTotalMinutes = Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);     //換算分鐘數
                                    if (MotoTotalMinutes >= TotalRentMinutes && TotalRentMinutes >=6) //全部扣光
                                    {
                                        MonthlyPoint += TotalRentMinutes;    //20201128 ADD BY ADAM REASON.月租折抵點數計算
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, TotalRentMinutes, LogID, ref errCode);//寫入記錄
                                        TotalRentMinutes = 0;
                                        
                                    }
                                    else
                                    {
                                        //折抵不能全折時，基本分鐘數會擺在最後折，且要一次折抵掉
                                        //一般時數會先折抵基本分鐘數，所以月租必須先折非基本分鐘，否則兩邊會有牴觸
                                        if (TotalRentMinutes >= 6)
                                        {
                                            if ((TotalRentMinutes - MotoTotalMinutes) >= OrderDataLists[0].BaseMinutes) //扣完有超過基本費
                                            {
                                                MonthlyPoint += MotoTotalMinutes;        //20201128 ADD BY ADAM REASON.月租折抵點數計算
                                                TotalRentMinutes -= MotoTotalMinutes;
                                                flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, MotoTotalMinutes, LogID, ref errCode); //寫入記錄
                                            }
                                            else
                                            {
                                                //折抵時數不夠扣基本費 只能折  租用時數-基本分鐘數
                                                int tmpMonthlyPoint = TotalRentMinutes - OrderDataLists[0].BaseMinutes;
                                                MonthlyPoint += tmpMonthlyPoint;
                                                TotalRentMinutes -= tmpMonthlyPoint;
                                                //MotoTotalMinutes += TotalRentMinutes - MotoTotalMinutes - OrderDataLists[0].BaseMinutes;
                                                //TotalRentMinutes -= MotoTotalMinutes;
                                                flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, monthlyRentDatas[i].MonthlyRentId, 0, 0, tmpMonthlyPoint, LogID, ref errCode); //寫入記錄
                                            }
                                        }
                                    }

                                    outputApi.Rent.RemainMonthlyTimeInterval = MonthlyPoint.ToString();
                                }
                            }
                            else
                            {
                                List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();
                                if (hasFine)
                                {
                                    CarRentPrice = billCommon.CalBillBySubScription(SD, ED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
                                }
                                else
                                {
                                    CarRentPrice = billCommon.CalBillBySubScription(SD, FED, lstHoliday, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, ref errCode, ref monthlyRentDatas, ref UseMonthlyRent);
                                }
                                if (UseMonthlyRent.Count > 0)
                                {
                                    UseMonthMode = true;
                                    int UseLen = UseMonthlyRent.Count;
                                    for (int i = 0; i < UseLen; i++)
                                    {
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, LogID, ref errCode); //寫入記錄
                                    }
                                }
                                else
                                {
                                    UseMonthMode = false;
                                }
                            }
                        }
                    }
                }
                #endregion
                #region 開始計價
                if (flag)
                {
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (ProjType == 4)
                    {
                        //目前折抵還有部分問題還未解
                        //汽車折抵是可以折抵到機車那邊的
                        //此處換算邏輯還未寫入
                        //ActualRedeemableTimePoint 需要針對機車部分換算 機車第一天最多折抵上限199分鐘 第二天為200分鐘
                        
                        if (TotalPoint >= TotalRentMinutes) //可使用總點數 >= 總租車時數
                        {
                            //ActualRedeemableTimePoint = TotalRentMinutes;
                            ActualRedeemableTimePoint = billCommon.GetMotorCanDiscountPoint(TotalRentMinutes);
                        }
                        else
                        {
                            if ((TotalPoint - TotalRentMinutes) < OrderDataLists[0].BaseMinutes)    //(可使用總點數-總租車時數) < 基本分鐘數
                            {
                                //ActualRedeemableTimePoint = TotalRentMinutes - OrderDataLists[0].BaseMinutes;
                                ActualRedeemableTimePoint = billCommon.GetMotorCanDiscountPoint(TotalRentMinutes) - OrderDataLists[0].BaseMinutes;
                            }
                        }

                        if (TotalRentMinutes <= 6 && Discount == 6)
                        {

                        }
                        else if (Discount >= TotalRentMinutes && TotalRentMinutes > 0)   // 要折抵的點數 >= 總租車時數
                        {
                            Discount = (days * 600) + (hours * 60) + (mins);    //自動縮減
                        }
                        else
                        {
                            //int tmp = TotalRentMinutes - Discount;
                            //if (tmp < OrderDataLists[0].BaseMinutes)
                            //{
                            //    Discount += TotalRentMinutes - Discount - OrderDataLists[0].BaseMinutes;
                            //}
                        }
                        TotalRentMinutes -= Discount;   // 總租車時數 = 總租車時數 - 要折抵的點數

                        if (UseMonthMode)   //true:有月租;false:無月租
                        {
                            billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, monthlyRentDatas[0].WorkDayRateForMoto, monthlyRentDatas[0].HoildayRateForMoto, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                            //計算月租折抵換算金額，目前1.0月租專案機車不管平假日都為固定費率
                            //MonthlyPrice = Convert.ToInt32((float)MonthlyPoint * outputApi.MonthRent.WorkdayRate);
                            MonthlyPrice = new BillCommon().MotoRentCompute(SD, ED, monthlyRentDatas[0].WorkDayRateForMoto, OrderDataLists[0].BaseMinutes, OrderDataLists[0].MaxPrice, 0);
                            if (MonthlyPoint <= 6)
                            {
                                //小於6的情況應該在於，可以折抵的分鐘數不是基本分鐘數
                                //這邊手算就好，就不進去FUNCTION裡面跑了
                                MonthlyPrice = Convert.ToInt32((float)MonthlyPoint * outputApi.MonthRent.WorkdayRate);
                            }
                            else
                            {
                                MonthlyPrice -= new BillCommon().MotoRentCompute(SD, ED, monthlyRentDatas[0].WorkDayRateForMoto, OrderDataLists[0].BaseMinutes, OrderDataLists[0].MaxPrice, MonthlyPoint);
                            }
                        }
                        else
                        {
                            billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                        }

                        //outputApi.Rent.CarRental = CarRentPrice;
                        //租金要減掉月租折抵換算金額
                        outputApi.Rent.CarRental = new BillCommon().MotoRentCompute(SD, ED, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].BaseMinutes, OrderDataLists[0].MaxPrice, Discount) - MonthlyPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    }
                    else
                    {
                        int BaseMinutes = 60;
                        int tmpTotalRentMinutes = TotalRentMinutes;

                        if (TotalRentMinutes < BaseMinutes)
                        {
                            TotalRentMinutes = BaseMinutes;
                        }
                        if (UseMonthMode)
                        {
                            TotalRentMinutes -= Convert.ToInt32((billCommon._scriptHolidayHour + billCommon._scriptWorkHour) * 60);
                            if (TotalRentMinutes < 0)
                            {
                                TotalRentMinutes = 0;
                            }
                        }
                        if (TotalPoint >= TotalRentMinutes)
                        {
                            ActualRedeemableTimePoint = TotalRentMinutes;
                        }
                        else
                        {
                            if ((TotalPoint - TotalRentMinutes) < 30)
                            {
                                ActualRedeemableTimePoint = TotalRentMinutes - 30;
                            }
                        }
                        //if (Discount > TotalRentMinutes)
                        //{
                        //    Discount = (days * 600) + (hours * 60);        //自動縮減
                        //    if (mins > 15 && mins < 45)
                        //    {
                        //        Discount += 30;
                        //    }
                        //    else if (mins > 45)
                        //    {
                        //        Discount += 60;
                        //    }
                        //}
                        //else
                        //{
                        //    int tmp = TotalRentMinutes - Discount;
                        //    if (tmp > 0 && tmp < 30)
                        //    {
                        //        Discount += TotalRentMinutes - Discount - 30;
                        //    }
                        //}

                        #region 折扣計算

                        double wDisc = 0;
                        double hDisc = 0;
                        int PayDisc = 0;
                        if (hasFine)
                        {
                            var xre = new BillCommon().CarDiscToPara(SD, ED, 60, 600, lstHoliday, Discount);
                            if(xre != null)
                            {
                                PayDisc = Convert.ToInt32(xre.Item1);
                                wDisc = xre.Item2;
                                hDisc = xre.Item3;
                            }
                        }
                        else
                        {
                            var xre = new BillCommon().CarDiscToPara(SD, FED, 60, 600, lstHoliday, Discount);
                            if (xre != null)
                            {
                                PayDisc = Convert.ToInt32(xre.Item1);
                                wDisc = xre.Item2;
                                hDisc = xre.Item3;
                            }
                        }

                        Discount = PayDisc;

                        #endregion

                        if (TotalRentMinutes > 0)
                        {
                            TotalRentMinutes -= Discount;
                        }
                        else
                        {
                            TotalRentMinutes = 0;
                        }

                        if (UseMonthMode)
                        {
                            outputApi.Rent.CarRental = CarRentPrice;
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForCar;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForCar;
                        }
                        else
                        {
                            int n_price = OrderDataLists[0].PRICE * 10;
                            int h_price = OrderDataLists[0].PRICE_H * 10;

                            if (hasFine)
                            {
                                //CarRentPrice = Convert.ToInt32(new BillCommon().CalSpread(SD, ED, Convert.ToInt32(OrderDataLists[0].PRICE * 10), Convert.ToInt32(OrderDataLists[0].PRICE_H * 10), lstHoliday));

                                int inPrice = new BillCommon().CarRentCompute(SD, ED, n_price, h_price, 10, lstHoliday);
                                CarRentPrice = inPrice;

                                int overPrice = new BillCommon().CarRentCompute(ED, FED, OrderDataLists[0].WeekdayPrice, OrderDataLists[0].HoildayPrice, 6, lstHoliday, true);
                                CarRentPrice += overPrice;
                            }
                            else
                            {
                                //CarRentPrice = Convert.ToInt32(new BillCommon().CalSpread(SD, FED, Convert.ToInt32(OrderDataLists[0].PRICE * 10), Convert.ToInt32(OrderDataLists[0].PRICE_H * 10), lstHoliday));
                                int inPrice = new BillCommon().CarRentCompute(SD, FED, n_price, h_price, 10, lstHoliday);
                                CarRentPrice = inPrice;
                            }
                        }
                        if (Discount > 0)
                        {
                            var result = new BillCommon().GetCarRangeMins(SD, ED, 60, 10 * 60, lstHoliday);

                            //int DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * OrderDataLists[0].PRICE)));

                            double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
                            double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

                            int DiscountPrice = Convert.ToInt32(((wDisc/60) * n_price) + ((hDisc/60) * h_price));

                            if (UseMonthMode)
                            {
                                if (billCommon._scriptHolidayHour > 0)
                                {
                                    DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * monthlyRentDatas[0].HoildayRateForCar)));
                                }
                                else
                                {
                                    DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * monthlyRentDatas[0].WorkDayRateForCar)));
                                }
                            }
                            //else
                            //{
                            //    //if (billCommon._holidayHour > 0)
                            //    if (result.Item2 > 0)
                            //    {
                            //        DiscountPrice = Convert.ToInt32(Math.Floor(((Discount / 60.0) * OrderDataLists[0].PRICE_H)));
                            //    }
                            //}
                            CarRentPrice -= DiscountPrice;
                        }
                        //安心服務
                        InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
                        if (InsurancePerHours > 0)
                        {
                            //基消1小時，之後每半小時計價
                            if (TotalRentMinutes < 60)
                            {
                                outputApi.Rent.InsurancePurePrice = InsurancePerHours;
                            }
                            else
                            {
                                outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((TotalRentMinutes / 30.0) * InsurancePerHours / 2)));
                            }

                            //逾時安心服務計算
                            if (TotalFineRentMinutes > 0)
                            {
                                outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((TotalFineInsuranceMinutes / 30.0) * InsurancePerHours / 2)));
                            }
                        }

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                        outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                        //里程費計算修改，遇到取不到里程數的先以0元為主
                        outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                    }

                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes:0).ToString();
                    outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                    outputApi.Rent.TotalRental = (outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice < 0) ? 0 : outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice;

                    string SPName = new ObjType().GetSPName(ObjType.SPType.CalFinalPrice);
                    SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        final_price = outputApi.Rent.TotalRental,
                        pure_price = outputApi.Rent.CarRental,
                        mileage_price = outputApi.Rent.MileageRent,
                        Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                        fine_price = outputApi.Rent.OvertimeRental,
                        gift_point = apiInput.Discount,
                        gift_motor_point = apiInput.MotorDiscount,
                        Etag = outputApi.Rent.ETAGRental,
                        parkingFee = outputApi.Rent.ParkingFee,
                        TransDiscount = outputApi.Rent.TransferPrice,
                        Token = Access_Token,
                        LogID = LogID,
                    };

                    SPOutput_Base SPOutput = new SPOutput_Base();
                    SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
                    flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
                }
                #endregion
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}