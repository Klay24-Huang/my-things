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

        public delegate CarRentInit flowData(CarRentInit sour);

        public OBIZ_GetPayDetail GetPayDetail(IBIZ_GetPayDetail bizIn)
        {
            if (bizIn.OrderData == eumOrderData.GetPayDetail)
            {
                var orderDt = new flowData(OrderDt_GetPayDetail);
                var spSave = new flowData(CalFinalPrice);
                var timeComp = new flowData(TimeCompute);
                return vGetPayDetail(bizIn, orderDt, timeComp, spSave);
            }
            else if (bizIn.OrderData == eumOrderData.ContactSetting)
            {
                var orderDt = new flowData(OrderDt_ContactSetting);
                var spSave = new flowData(BE_CalFinalPrice);
                var timeComp = new flowData(TimeCompute_ContactSetting);                
                return vGetPayDetail(bizIn, orderDt, timeComp, spSave);
            }
            else
                throw new Exception("OrderData錯誤");
        }

        public OBIZ_GetPayDetail vGetPayDetail(IBIZ_GetPayDetail bizIn, 
            flowData getOrderDt,
            flowData timeCompute,
            flowData spSave)
        {
            var re = new OBIZ_GetPayDetail();
            bizIn.Call_CarMagi = false;//20201224 add by adam 問題未確定前先關掉車麻吉

            #region 參數設定

            CarRentInit sour = new CarRentInit(connetStr);
            sour.Access_Token = bizIn.Access_Token;
            sour.errCode = bizIn.errCode;
            sour.funName = bizIn.funName;
            sour.isGuest = bizIn.isGuest;
            sour.ClientIP = bizIn.ClientIP;
            sour.apiInput = objUti.TTMap<IBIZ_GetPayDetail, IAPI_GetPayDetail>(bizIn);
            sour.UserID = bizIn.UserID;
            sour.LogID = bizIn.LogID;
            sour.returnDate = bizIn.returnDate;

            #endregion

            if (bizIn.InCheck)//防呆
                sour = InCheck(sour);

            if (bizIn.Ck_Token)//token檢查
                sour = TokenCk(sour);

            //取出訂單資訊
            //if (bizIn.OrderData == eumOrderData.GetPayDetail)
            //    sour = OrderDt_GetPayDetail(sour);
            //else if (bizIn.OrderData == eumOrderData.ContactSetting)
            //    sour = OrderDt_ContactSetting(sour);
            if (getOrderDt != null)
                sour = getOrderDt(sour);

            //sour = TimeCompute(sour);//時間計算
            if (timeCompute != null)
                sour = timeCompute(sour);

            sour = GetCarRentNoMonth(sour);//非月租汽車租金

            if (bizIn.Call_NPR270Query)
                sour = NPR270Query(sour);

            sour = DiscCk(sour);

            if (bizIn.Call_ETAG)
                sour = ETagCk(sour);

            sour = CreateOptModel(sour);

            if (bizIn.Call_CarMagi)
                sour = CarMagi(sour);

            sour = MonthRent(sour, bizIn);

            if (bizIn.db_CalFinalPrice)
            {
                //if (bizIn.OrderData == eumOrderData.GetPayDetail)
                //    sour = CalFinalPrice(sour);
                //else if (bizIn.OrderData == eumOrderData.ContactSetting)
                //    sour = BE_CalFinalPrice(sour);
                if (spSave != null)
                    sour = spSave(sour);
            }

            #region 寫入錯誤Log
            if (false == sour.flag && false == sour.isWriteError)
            {
                sour.baseVerify.InsErrorLog(sour.funName, sour.errCode, sour.ErrType, sour.LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            var objOutput = sour.objOutput;
            sour.baseVerify.GenerateOutput(ref objOutput, sour.flag, sour.errCode, sour.errMsg, sour.outputApi, sour.token);
            sour.objOutput = objOutput;
            if (sour.outputApi != null)
                re = objUti.TTMap<OAPI_GetPayDetail, OBIZ_GetPayDetail>(sour.outputApi);

            re.objOutput = sour.objOutput;
            re.flag = sour.flag;
            #endregion

            return re;
        }

        public OBIZ_GetPayDetail xGetPayDetail(IBIZ_GetPayDetail input)
        {
            var re = new OBIZ_GetPayDetail();
            input.Call_CarMagi = false;//20201224 add by adam 問題未確定前先關掉車麻吉

            #region 初始宣告

            string Access_Token = input.Access_Token;
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg =  input.errMsg; //預設成功
            string errCode =  input.errCode; //預設成功
            string funName = input.funName;
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
            bool isGuest = input.isGuest;
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
            int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
            CarRentInfo carInfo = new CarRentInfo();//車資料
            int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM

            double nor_car_wDisc = 0;//只有一般時段時平日折扣
            double nor_car_hDisc = 0;//只有一般時段時價日折扣
            int nor_car_PayDisc = 0;//只有一般時段時總折扣
            int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

            int gift_point = 0;//使用時數(汽車)
            int gift_motor_point = 0;//使用時數(機車)
            int motoBaseMins = 6;//機車基本分鐘數
            int carBaseMins = 60;//汽車基本分鐘數

            #endregion

            #region 防呆
            if (flag)
            {
                apiInput = objUti.TTMap<IBIZ_GetPayDetail, IAPI_GetPayDetail>(input);
                if (apiInput != null)
                    Contentjson = JsonConvert.SerializeObject(apiInput);
                //寫入API Log
                //string ClientIP = baseVerify.GetClientIp(Request);
                string ClientIP = input.ClientIP;

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
                if (input.Ck_Token)
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
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(input.xIDNO))
                        throw new Exception("不驗證token時xIDNO為必填");
                    else
                        IDNO = input.xIDNO;
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

                if (OrderDataLists != null && OrderDataLists.Count() > 0)
                    motoBaseMins = OrderDataLists[0].BaseMinutes > 0 ? OrderDataLists[0].BaseMinutes : motoBaseMins;
            }
            #endregion

            #region 第二階段判斷及計價
            if (flag)
            {
                //判斷狀態
                if (OrderDataLists[0].car_mgt_status == 16 || OrderDataLists[0].car_mgt_status < 11 || OrderDataLists[0].cancel_status > 0)
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

                #region 汽車計費資訊 
                //note:汽車計費資訊PayDetail
                int car_payAllMins = 0; //全部計費租用分鐘
                int car_payInMins = 0;//未超時計費分鐘
                int car_payOutMins = 0;//超時分鐘-顯示用

                double car_pay_in_wMins = 0;//未超時平日計費分鐘
                double car_pay_in_hMins = 0;//未超時假日計費分鐘
                double car_pay_out_wMins = 0;//超時平日計費分鐘
                double car_pay_out_hMins = 0;//超時假日計費分鐘

                int car_inPrice = 0;//未超時費用
                int car_outPrice = 0;//超時費用

                int car_n_price = OrderDataLists[0].PRICE;
                int car_h_price = OrderDataLists[0].PRICE_H;

                if (flag)
                {
                    if (ProjType == 4)
                    {

                    }
                    else
                    {
                        if (hasFine)
                        {
                            var reInMins = billCommon.GetCarRangeMins(SD, ED, carBaseMins, 600, lstHoliday);
                            if (reInMins != null)
                            {
                                car_payInMins = Convert.ToInt32(reInMins.Item1 + reInMins.Item2);
                                car_payAllMins += car_payInMins;
                                car_pay_in_wMins = reInMins.Item1;
                                car_pay_in_hMins = reInMins.Item2;
                            }

                            var reOutMins = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                            if (reOutMins != null)
                            {
                                car_payOutMins = Convert.ToInt32(reOutMins.Item1 + reOutMins.Item2);
                                car_payAllMins += car_payOutMins;
                                car_pay_out_wMins = reOutMins.Item1;
                                car_pay_out_hMins = reOutMins.Item2;
                            }

                            car_inPrice = billCommon.CarRentCompute(SD, ED, car_n_price * 10, car_h_price * 10, 10, lstHoliday);
                            car_outPrice = billCommon.CarRentCompute(ED, FED, OrderDataLists[0].WeekdayPrice, OrderDataLists[0].HoildayPrice, 6, lstHoliday, true, 0);
                        }
                        else
                        {
                            var reAllMins = billCommon.GetCarRangeMins(SD, FED, carBaseMins, 600, lstHoliday);
                            if (reAllMins != null)
                            {
                                car_payAllMins = Convert.ToInt32(reAllMins.Item1 + reAllMins.Item2);
                                car_payInMins = car_payAllMins;
                                car_pay_in_wMins = reAllMins.Item1;
                                car_pay_in_hMins = reAllMins.Item2;
                            }

                            car_inPrice = billCommon.CarRentCompute(SD, FED, car_n_price * 10, car_h_price * 10, 10, lstHoliday);
                        }
                    }
                }

                #endregion

                #region 與短租查時數
                if (flag)
                {
                    if (input.Call_NPR270Query)
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
                    }

                    #region 判斷輸入的點數有沒有超過總點數
                    flag = true;
                    errCode = "000000";
                    if (ProjType == 4)
                    {
                        if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
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
                    #endregion

                }
                #endregion                

                #region 查ETAG 20201202 ADD BY ADAM
                if (flag && OrderDataLists[0].ProjType != 4 && input.Call_ETAG)    //汽車才需要進來
                {
                    WebAPIOutput_ETAG010 wsOutput = new WebAPIOutput_ETAG010();
                    HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                    //ETAG查詢失敗也不影響流程
                    flag = wsAPI.ETAG010Send(apiInput.OrderNo, "", ref wsOutput);
                    if (flag)
                    {
                        if (wsOutput.RtnCode == "0")
                        {
                            //取出ETAG費用
                            if (wsOutput.Data.Length > 0)
                            {
                                etagPrice = wsOutput.Data[0].TAMT == "" ? 0 : int.Parse(wsOutput.Data[0].TAMT);

                            }
                        }
                    }
                    flag = true;
                    errCode = "000000";
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
                if (flag && OrderDataLists[0].ProjType != 4 && input.Call_CarMagi) 
                {
                    //檢查有無車麻吉停車費用
                    WebAPIOutput_QueryBillByCar mochiOutput = new WebAPIOutput_QueryBillByCar();
                    MachiComm mochi = new MachiComm();
                    flag = mochi.GetParkingBill(LogID, OrderDataLists[0].CarNo, SD, FED.AddDays(1), ref ParkingPrice, ref mochiOutput);
                    if (flag)
                    {
                        outputApi.Rent.ParkingFee = ParkingPrice;
                    }
                }

                #endregion
                #region 月租
                //note: 月租GetPayDetail
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
                    //int MonthAll = 0;
                    //for (int i=0;i< MonthlyLen;i++)
                    //{
                    //    MonthAll += Convert.ToInt32(monthlyRentDatas[i].MotoTotalHours);
                    //}
                    //先設定一遍
                    //outputApi.Rent.UseMonthlyTimeInterval = MonthlyPoint.ToString();
                    if (MonthlyLen > 0)
                    {
                        UseMonthMode = true;
                        outputApi.IsMonthRent = 1;

                        if (flag)
                        {
                            if (ProjType == 4)
                            {
                                var motoMonth = objUti.Clone(monthlyRentDatas);
                                var item = OrderDataLists[0];
                                var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                                int motoDisc = Discount;
                                carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, lstHoliday, motoMonth, motoDisc);

                                if (carInfo != null)
                                {
                                    outputApi.Rent.CarRental += carInfo.RentInPay;
                                    if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
                                        motoMonth = carInfo.mFinal;
                                    Discount = carInfo.useDisc;
                                }

                                motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
                                if (motoMonth.Count > 0 && input.db_InsMonthlyHistory)
                                {
                                    UseMonthMode = true;
                                    int UseLen = motoMonth.Count;
                                    for (int i = 0; i < UseLen; i++)
                                    {
                                        flag = monthlyRentRepository.InsMonthlyHistory(IDNO, tmpOrder, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), LogID, ref errCode); //寫入記錄
                                    }
                                }
                                else
                                {
                                    UseMonthMode = false;
                                }
                            }
                            else
                            {
                                List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();

                                UseMonthlyRent = monthlyRentDatas;

                                int xDiscount = Discount;//帶入月租運算的折扣
                                if (hasFine)
                                {
                                    carInfo = billCommon.CarRentInCompute(SD, ED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, carBaseMins, 10, lstHoliday, UseMonthlyRent, xDiscount);
                                    if (carInfo != null)
                                    {
                                        CarRentPrice += carInfo.RentInPay;
                                        if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
                                            UseMonthlyRent = carInfo.mFinal;
                                        Discount = carInfo.useDisc;
                                    }
                                    CarRentPrice += car_outPrice;
                                }
                                else
                                {
                                    carInfo = billCommon.CarRentInCompute(SD, FED, OrderDataLists[0].PRICE, OrderDataLists[0].PRICE_H, carBaseMins, 10, lstHoliday, UseMonthlyRent, xDiscount);
                                    if (carInfo != null)
                                    {
                                        CarRentPrice += carInfo.RentInPay;
                                        if (carInfo.mFinal != null && carInfo.mFinal.Count > 0)
                                            UseMonthlyRent = carInfo.mFinal;
                                        Discount = carInfo.useDisc;
                                    }
                                }

                                if (UseMonthlyRent.Count > 0 && input.db_InsMonthlyHistory)
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
                        if (UseMonthMode)   //true:有月租;false:無月租
                        {
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                        }
                        else
                        {
                            var item = OrderDataLists[0];
                            var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                            carInfo = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, motoBaseMins, dayMaxMinns, null, null, Discount);
                            if (carInfo != null)
                                outputApi.Rent.CarRental = carInfo.RentInPay;
                        }

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

                        #region 非月租折扣計算
                        //note: 折扣計算
                        //double wDisc = 0;
                        //double hDisc = 0;
                        //int PayDisc = 0;
                        if (!UseMonthMode)
                        {
                            if (hasFine)
                            {
                                var xre = new BillCommon().CarDiscToPara(SD, ED, carBaseMins, 600, lstHoliday, Discount);
                                if (xre != null)
                                {
                                    nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
                                    nor_car_wDisc = xre.Item2;
                                    nor_car_hDisc = xre.Item3;
                                }
                            }
                            else
                            {
                                var xre = new BillCommon().CarDiscToPara(SD, FED, carBaseMins, 600, lstHoliday, Discount);
                                if (xre != null)
                                {
                                    nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
                                    nor_car_wDisc = xre.Item2;
                                    nor_car_hDisc = xre.Item3;
                                }
                            }

                            var discPrice = Convert.ToDouble(car_n_price) * (nor_car_wDisc / 60) + Convert.ToDouble(car_h_price) * (nor_car_hDisc / 60);
                            nor_car_PayDiscPrice = Convert.ToInt32(Math.Floor(discPrice));
                            Discount = nor_car_PayDisc;
                        }

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
                            CarRentPrice = car_inPrice;//未逾時租用費用
                            if (hasFine)
                                outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                        }

                        if (Discount > 0)
                        {
                            double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
                            double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

                            if (UseMonthMode)
                            {

                            }
                            else
                            {
                                //非月租折扣
                                CarRentPrice -= nor_car_PayDiscPrice;
                                CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
                            }
                        }
                        //安心服務
                        InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
                        if (InsurancePerHours > 0)
                        {
                            outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((car_payInMins / 30.0) * InsurancePerHours / 2)));

                            //逾時安心服務計算
                            if (TotalFineRentMinutes > 0)
                            {
                                outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((car_payOutMins / 30.0) * InsurancePerHours / 2)));
                            }
                        }

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                        outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                        //里程費計算修改，遇到取不到里程數的先以0元為主
                        //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                        // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                        if (OrderDataLists[0].start_mile == 0 ||
                            OrderDataLists[0].end_mile == 0 ||
                            ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) > 1000) ||
                            ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) < 0)
                            )
                        {
                            outputApi.Rent.MileageRent = 0;
                        }
                        else
                        {
                            outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                        }
                    }

                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                    outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                    //20201202 ADD BY ADAM REASON.ETAG費用
                    outputApi.Rent.ETAGRental = etagPrice;

                    var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
                    xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                    outputApi.Rent.TotalRental = xTotalRental;

                    #region 修正輸出欄位

                    var tra = OrderDataLists[0].init_TransDiscount;
                    if (xTotalRental == 0)
                    {
                        var carPri = outputApi.Rent.CarRental;
                        if (carPri > 0)
                            outputApi.Rent.TransferPrice = carPri;
                    }

                    //note: 修正輸出欄位PayDetail
                    if (ProjType == 4)
                    {
                        outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                        outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                        outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

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
                            outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_pay_in_wMins + car_pay_in_hMins).ToString();//可折抵租用時數
                            outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
                            gift_point = nor_car_PayDisc;
                        }

                        gift_motor_point = 0;
                        outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                    }

                    #endregion

                    if (input.db_CalFinalPrice)
                    {
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
                            //gift_point = apiInput.Discount,
                            //gift_motor_point = apiInput.MotorDiscount,
                            gift_point = gift_point,
                            gift_motor_point = gift_motor_point,

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

            if (outputApi != null)
                re = objUti.TTMap<OAPI_GetPayDetail, OBIZ_GetPayDetail>(outputApi);

            re.objOutput = objOutput;
            re.flag = flag;
            #endregion

            return re;
        }  
    
        public CarRentInit InCheck(CarRentInit sour)
        {
            if (sour.flag)
            {
                string errCode = sour.errCode;
                long LogID = sour.LogID;
                long tmpOrder = sour.tmpOrder;

                if (sour.apiInput != null)
                    sour.Contentjson = JsonConvert.SerializeObject(sour.apiInput);
                //寫入API Log
                //string ClientIP = baseVerify.GetClientIp(Request);
                string ClientIP = sour.ClientIP;

                sour.flag = sour.baseVerify.InsAPLog(sour.Contentjson, ClientIP, sour.funName, ref errCode, ref LogID);
                sour.errCode = errCode;
                sour.LogID = LogID;

                if (string.IsNullOrWhiteSpace(sour.apiInput.OrderNo))
                {
                    sour.flag = false;
                    sour.errCode = "ERR900";
                }
                else
                {
                    if (sour.apiInput.OrderNo.IndexOf("H") < 0)
                    {
                        sour.flag = false;
                        sour.errCode = "ERR900";
                    }
                    if (sour.flag)
                    {
                        sour.flag = Int64.TryParse(sour.apiInput.OrderNo.Replace("H", ""), out tmpOrder);
                        sour.tmpOrder = tmpOrder;
                        if (sour.flag)
                        {
                            if (sour.tmpOrder <= 0)
                            {
                                sour.flag = false;
                                sour.errCode = "ERR900";
                            }
                        }
                    }
                }
                if (sour.flag)
                {
                    if (sour.apiInput.Discount < 0)
                    {
                        sour.flag = false;
                        sour.errCode = "ERR202";
                    }

                    if (sour.apiInput.MotorDiscount < 0)
                    {
                        sour.flag = false;
                        sour.errCode = "ERR202";
                    }

                    sour.Discount = sour.apiInput.Discount + sour.apiInput.MotorDiscount;
                }

                //不開放訪客
                if (sour.flag)
                {
                    if (sour.isGuest)
                    {
                        sour.flag = false;
                        sour.errCode = "ERR101";
                    }
                }

                sour.errCode = errCode;
                sour.LogID = LogID;
                sour.tmpOrder = tmpOrder;
            }
            return sour;
        }
    
        public CarRentInit TokenCk(CarRentInit sour)
        {
            //Token判斷
            if (sour.flag && sour.isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = sour.LogID,
                    Token = sour.Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                var lstError = sour.lstError;
                sour.flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                sour.lstError = lstError;
                bool flag = sour.flag;
                string errCode = sour.errCode;
                sour.baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                sour.flag = flag;
                sour.errCode = errCode;
                if (sour.flag)
                {
                    sour.IDNO = spOut.IDNO;
                }                
            }
            return sour;
        }

        /// <summary>
        /// 取出訂單資訊GetPayDetail
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit OrderDt_GetPayDetail(CarRentInit sour)
        {
            if (sour.flag)
            {
                SPInput_GetOrderStatusByOrderNo spInput = new SPInput_GetOrderStatusByOrderNo()
                {
                    IDNO = sour.IDNO,
                    OrderNo = sour.tmpOrder,
                    LogID = sour.LogID,
                    Token = sour.Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderStatusByOrderNo);
                SPOutput_Base spOutBase = new SPOutput_Base();
                SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                var OrderDataLists = sour.OrderDataLists;
                var lstError = sour.lstError;
                var flag = sour.flag;
                var errCode = sour.errCode;
                DataSet ds = new DataSet();
                sour.flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
                sour.baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

                sour.flag = flag;
                sour.errCode = errCode;
                sour.lstError = lstError;
                sour.OrderDataLists = OrderDataLists;                

                //判斷訂單狀態
                if (flag)
                {
                    if (sour.OrderDataLists.Count == 0)
                    {
                        flag = false;
                        errCode = "ERR203";
                    }
                }
            }

            if (sour.OrderDataLists != null && sour.OrderDataLists.Count() > 0)
            {
                sour.motoBaseMins = sour.OrderDataLists[0].BaseMinutes > 0 ? sour.OrderDataLists[0].BaseMinutes : sour.motoBaseMins;
                sour.ProjType = sour.OrderDataLists[0].ProjType;

                //判斷狀態
                if (sour.OrderDataLists[0].car_mgt_status == 16 || sour.OrderDataLists[0].car_mgt_status < 11 || sour.OrderDataLists[0].cancel_status > 0)
                {
                    sour.flag = false;
                    sour.errCode = "ERR204";
                }
            }

            return sour;
        }

        /// <summary>
        /// 取出訂單資訊ContactSetting
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit OrderDt_ContactSetting(CarRentInit sour)
        {
            if (sour.flag)
            {
                if (string.IsNullOrWhiteSpace(sour.UserID))
                    throw new Exception("ContactSetting取訂單資料UserID為必填");

                SPInput_BE_GetOrderStatusByOrderNo spInput = new SPInput_BE_GetOrderStatusByOrderNo()
                {
                    IDNO = sour.IDNO,
                    OrderNo = sour.tmpOrder,
                    LogID = sour.LogID,
                    UserID = sour.UserID
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
                SPOutput_Base spOutBase = new SPOutput_Base();
                SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                sour.OrderDataLists = new List<OrderQueryFullData>();
                DataSet ds = new DataSet();
                var OrderDataLists = sour.OrderDataLists;
                var lstError = sour.lstError;
                var flag = sour.flag;
                var errCode = sour.errCode;
                sour.flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderDataLists, ref ds, ref lstError);
                sour.baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

                sour.flag = flag;
                sour.errCode = errCode;
                sour.lstError = lstError;
                sour.OrderDataLists = OrderDataLists;

                //判斷訂單狀態
                if (sour.flag)
                {
                    if (sour.OrderDataLists.Count == 0)
                    {
                        sour.flag = false;
                        sour.errCode = "ERR203";
                    }
                }
            }

            if (sour.OrderDataLists != null && sour.OrderDataLists.Count() > 0)
            {
                sour.motoBaseMins = sour.OrderDataLists[0].BaseMinutes > 0 ? sour.OrderDataLists[0].BaseMinutes : sour.motoBaseMins;
                sour.ProjType = sour.OrderDataLists[0].ProjType;
            }

            return sour;
        }

        /// <summary>
        /// 時間計算
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit TimeCompute(CarRentInit sour)
        {
            if (sour.flag)
            {
                sour.SD = Convert.ToDateTime(sour.OrderDataLists[0].final_start_time);
                sour.SD = sour.SD.AddSeconds(sour.SD.Second * -1); //去秒數
                                                    //機車路邊不計算預計還車時間
                if (sour.OrderDataLists[0].ProjType == 4)
                {
                    sour.ED = Convert.ToDateTime(sour.OrderDataLists[0].final_stop_time);
                    sour.ED = sour.ED.AddSeconds(sour.ED.Second * -1); //去秒數
                }
                else
                {
                    sour.ED = Convert.ToDateTime(sour.OrderDataLists[0].stop_time);
                    sour.ED = sour.ED.AddSeconds(sour.ED.Second * -1); //去秒數
                }
                sour.FED = Convert.ToDateTime(sour.OrderDataLists[0].final_stop_time);
                sour.FED = sour.FED.AddSeconds(sour.FED.Second * -1);  //去秒數
                sour.lstHoliday = new CommonRepository(connetStr).GetHolidays(sour.SD.ToString("yyyyMMdd"), sour.FED.ToString("yyyyMMdd"));
                if (sour.FED.Subtract(sour.ED).Ticks > 0)
                {
                    sour.FineDate = sour.ED;
                    sour.hasFine = true;                
                }
            }
            if (sour.flag)
            {
                if (sour.NowTime.Subtract(sour.FED).TotalMinutes >= 30)
                {
                    sour.flag = false;
                    sour.errCode = "ERR208";
                }
            }

            return sour;
        }

        public CarRentInit TimeCompute_ContactSetting(CarRentInit sour)
        {
            if (sour.flag)
            {
                sour.ProjType = sour.OrderDataLists[0].ProjType;
                sour.SD = Convert.ToDateTime(sour.OrderDataLists[0].final_start_time);
                sour.SD = sour.SD.AddSeconds(sour.SD.Second * -1); //去秒數
                //機車路邊不計算預計還車時間
                if (sour.OrderDataLists[0].ProjType == 4)
                {
                    sour.ED = Convert.ToDateTime(sour.returnDate);
                    sour.ED = sour.ED.AddSeconds(sour.ED.Second * -1); //去秒數
                }
                else
                {
                    sour.ED = Convert.ToDateTime(sour.returnDate);
                    sour.ED = sour.ED.AddSeconds(sour.ED.Second * -1); //去秒數
                }
                sour.FED = Convert.ToDateTime(sour.returnDate);
                sour.FED = sour.FED.AddSeconds(sour.FED.Second * -1);  //去秒數
                sour.lstHoliday = new CommonRepository(connetStr).GetHolidays(sour.SD.ToString("yyyyMMdd"), sour.FED.ToString("yyyyMMdd"));
                if (sour.FED < sour.SD)
                {
                    sour.flag = false;
                    sour.errCode = "ERR740";
                }
                if (sour.flag)
                {
                    if (sour.FED.Subtract(sour.ED).Ticks > 0)
                    {
                        sour.FineDate = sour.ED;
                        sour.hasFine = true;                    
                    }
                }
            }
            return sour;
        }

        /// <summary>
        /// 非月租汽車租金
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit GetCarRentNoMonth(CarRentInit sour)
        {
            if (sour.flag)
            {
                sour.car_n_price = sour.OrderDataLists[0].PRICE;
                sour.car_h_price = sour.OrderDataLists[0].PRICE_H;

                if (sour.ProjType == 4)
                {

                }
                else
                {
                    if (sour.hasFine)
                    {
                        var reInMins = sour.billCommon.GetCarRangeMins(sour.SD, sour.ED, sour.carBaseMins, 600, sour.lstHoliday);
                        if (reInMins != null)
                        {
                            sour.car_payInMins = Convert.ToInt32(reInMins.Item1 + reInMins.Item2);
                            sour.car_payAllMins += sour.car_payInMins;
                            sour.car_pay_in_wMins = reInMins.Item1;
                            sour.car_pay_in_hMins = reInMins.Item2;
                        }

                        var reOutMins = sour.billCommon.GetCarOutComputeMins(sour.ED, sour.FED, 0, 360, sour.lstHoliday);
                        if (reOutMins != null)
                        {
                            sour.car_payOutMins = Convert.ToInt32(reOutMins.Item1 + reOutMins.Item2);
                            sour.car_payAllMins += sour.car_payOutMins;
                            sour.car_pay_out_wMins = reOutMins.Item1;
                            sour.car_pay_out_hMins = reOutMins.Item2;
                        }

                        sour.car_inPrice = sour.billCommon.CarRentCompute(sour.SD, sour.ED, sour.car_n_price * 10, sour.car_h_price * 10, 10, sour.lstHoliday);
                        sour.car_outPrice = sour.billCommon.CarRentCompute(sour.ED, sour.FED, sour.OrderDataLists[0].WeekdayPrice, sour.OrderDataLists[0].HoildayPrice, 6, sour.lstHoliday, true, 0);
                    }
                    else
                    {
                        var reAllMins = sour.billCommon.GetCarRangeMins(sour.SD, sour.FED, sour.carBaseMins, 600, sour.lstHoliday);
                        if (reAllMins != null)
                        {
                            sour.car_payAllMins = Convert.ToInt32(reAllMins.Item1 + reAllMins.Item2);
                            sour.car_payInMins = sour.car_payAllMins;
                            sour.car_pay_in_wMins = reAllMins.Item1;
                            sour.car_pay_in_hMins = reAllMins.Item2;
                        }

                        sour.car_inPrice = sour.billCommon.CarRentCompute(sour.SD, sour.FED, sour.car_n_price * 10, sour.car_h_price * 10, 10, sour.lstHoliday);
                    }
                }
            }
            return sour;
        }

        /// <summary>
        /// 與短租查時數
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit NPR270Query(CarRentInit sour)
        {
            if (sour.flag)
            {
                WebAPIOutput_NPR270Query wsOutput = new WebAPIOutput_NPR270Query();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                sour.flag = wsAPI.NPR270Query(sour.IDNO, ref wsOutput);
                if (sour.flag)
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
                                    sour.CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                }
                                else
                                {
                                    sour.MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                }
                            }
                        }
                    }
                }
                else
                {
                    sour.flag = true;
                    sour.errCode = "0000";
                }
            }
            return sour;
        }

        /// <summary>
        /// 判斷輸入的點數有沒有超過總點數
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit DiscCk(CarRentInit sour)
        {
            if (sour.flag)
            {
                if (sour.ProjType == 4)
                {
                    if (sour.Discount > 0 && sour.Discount < sour.OrderDataLists[0].BaseMinutes)
                    {
                        if (sour.Discount > (sour.MotorPoint + sour.CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
                        {
                            sour.flag = false;
                            sour.errCode = "ERR207";
                        }
                    }

                    if (sour.TotalRentMinutes <= 6 && sour.Discount == 6)
                    {

                    }

                    //hack: fix
                    //else if (sour.Discount > (sour.TotalRentMinutes + sour.TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
                    //{
                    //    sour.flag = false;
                    //    sour.errCode = "ERR303";
                    //}

                    //if (flag)
                    //{
                    //    billCommon.CalPointerToDayHourMin(MotorPoint + CarPoint, ref PDays, ref PHours, ref PMins);
                    //}
                }
                else
                {
                    if (sour.Discount > 0 && sour.Discount % 30 > 0)
                    {
                        sour.flag = false;
                        sour.errCode = "ERR206";
                    }
                    else
                    {
                        if (sour.Discount > sour.CarPoint)
                        {
                            sour.flag = false;
                            sour.errCode = "ERR207";
                        }
                    }   
                }
            }

            return sour;
        }
    
        /// <summary>
        /// 查ETag
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit ETagCk(CarRentInit sour)
        {
            if (sour.flag && sour.OrderDataLists[0].ProjType != 4 )    //汽車才需要進來
            {              
                WebAPIOutput_ETAG010 wsOutput = new WebAPIOutput_ETAG010();
                HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                //ETAG查詢失敗也不影響流程
                sour.flag = wsAPI.ETAG010Send(sour.apiInput.OrderNo, "", ref wsOutput);
                if (sour.flag)
                {
                    if (wsOutput.RtnCode == "0")
                    {
                        //取出ETAG費用
                        if (wsOutput.Data.Length > 0)
                        {
                            sour.etagPrice = wsOutput.Data[0].TAMT == "" ? 0 : int.Parse(wsOutput.Data[0].TAMT);
                        }
                    }
                }
                sour.flag = true;
                sour.errCode = "000000";
            }
            return sour;
        }

        /// <summary>
        /// 建空模及塞入要輸出的值
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit CreateOptModel(CarRentInit sour)
        {
            if (sour.flag)
            {
                sour.outputApi.CanUseDiscount = 1;   //先暫時寫死，之後改專案設定，由專案設定引入
                sour.outputApi.CanUseMonthRent = 1;  //先暫時寫死，之後改專案設定，由專案設定引入
                sour.outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
                sour.outputApi.DiscountAlertMsg = "";
                sour.outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
                sour.outputApi.IsMotor = (sour.ProjType == 4) ? 1 : 0;    //是否為機車
                sour.outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase();  //月租資訊
                sour.outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();  //機車資訊
                sour.outputApi.PayMode = (sour.ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
                sour.outputApi.ProType = sour.ProjType;
                sour.outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() //訂單基本資訊
                {
                    BookingEndDate = sour.ED.ToString("yyyy-MM-dd HH:mm:ss"),
                    BookingStartDate = sour.SD.ToString("yyyy-MM-dd HH:mm:ss"),
                    CarNo = sour.OrderDataLists[0].CarNo,
                    RedeemingTimeCarInterval = sour.CarPoint.ToString(),
                    RedeemingTimeMotorInterval = sour.MotorPoint.ToString(),
                    RedeemingTimeInterval = (sour.ProjType == 4) ? (sour.CarPoint + sour.MotorPoint).ToString() : sour.CarPoint.ToString(),
                    RentalDate = sour.FED.ToString("yyyy-MM-dd HH:mm:ss"),
                    //RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
                };

                if (sour.ProjType == 4)
                {
                    sour.TotalPoint = (sour.CarPoint + sour.MotorPoint);
                    sour.outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
                    {
                        BaseMinutePrice = sour.OrderDataLists[0].BaseMinutesPrice,
                        BaseMinutes = sour.OrderDataLists[0].BaseMinutes,
                        MinuteOfPrice = sour.OrderDataLists[0].MinuteOfPrice
                    };
                }
                else
                {
                    sour.TotalPoint = sour.CarPoint;
                    sour.outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
                    {
                        HoildayOfHourPrice = sour.OrderDataLists[0].PRICE_H,
                        HourOfOneDay = 10,
                        WorkdayOfHourPrice = sour.OrderDataLists[0].PRICE,
                        WorkdayPrice = sour.OrderDataLists[0].PRICE * 10,
                        MilUnit = sour.OrderDataLists[0].MilageUnit,
                        HoildayPrice = sour.OrderDataLists[0].PRICE_H * 10
                    };
                }
                //20201201 ADD BY ADAM REASON.轉乘優惠
                sour.TransferPrice = sour.OrderDataLists[0].init_TransDiscount;
            }
            return sour;
        }        
    
        /// <summary>
        /// 車麻吉
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit CarMagi(CarRentInit sour)
        {
            if (sour.flag && sour.OrderDataLists[0].ProjType != 4)
            {
                //檢查有無車麻吉停車費用
                WebAPIOutput_QueryBillByCar mochiOutput = new WebAPIOutput_QueryBillByCar();
                MachiComm mochi = new MachiComm();
                var ParkingPrice = sour.ParkingPrice;
                sour.flag = mochi.GetParkingBill(sour.LogID, sour.OrderDataLists[0].CarNo, sour.SD, sour.FED.AddDays(1), ref ParkingPrice, ref mochiOutput);
                if (sour.flag)
                {
                    sour.outputApi.Rent.ParkingFee = ParkingPrice;
                }
            }
            return sour;
        }
    
        /// <summary>
        /// 月租
        /// </summary>
        /// <param name="sour"></param>
        /// <returns></returns>
        public CarRentInit MonthRent(CarRentInit sour, IBIZ_GetPayDetail input)
        {
            if (sour.flag)
            {
                var errCode = sour.errCode;

                //1.0 先還原這個單號使用的
                sour.flag = sour.monthlyRentRepository.RestoreHistory(sour.IDNO, sour.tmpOrder, sour.LogID, ref errCode);

                int RateType = (sour.ProjType == 4) ? 1 : 0;
                if (!sour.hasFine)
                {
                    sour.monthlyRentDatas = sour.monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.ED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                }
                else
                {
                    sour.monthlyRentDatas = sour.monthlyRentRepository.GetSubscriptionRates(sour.IDNO, sour.SD.ToString("yyyy-MM-dd HH:mm:ss"), sour.FED.ToString("yyyy-MM-dd HH:mm:ss"), RateType);
                }
                int MonthlyLen = sour.monthlyRentDatas.Count;
                if (MonthlyLen > 0)
                {
                    sour.UseMonthMode = true;
                    sour.outputApi.IsMonthRent = 1;

                    if (sour.flag)
                    {
                        if (sour.ProjType == 4)
                        {
                            var motoMonth = objUti.Clone(sour.monthlyRentDatas);
                            var item = sour.OrderDataLists[0];
                            var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                            int motoDisc = sour.Discount;
                            sour.carInfo = sour.billCommon.MotoRentMonthComp(sour.SD, sour.ED, item.MinuteOfPrice, item.MinuteOfPrice, sour.motoBaseMins, dayMaxMinns, sour.lstHoliday, motoMonth, motoDisc);

                            if (sour.carInfo != null)
                            {
                                sour.outputApi.Rent.CarRental += sour.carInfo.RentInPay;
                                if (sour.carInfo.mFinal != null && sour.carInfo.mFinal.Count > 0)
                                    motoMonth = sour.carInfo.mFinal;
                                sour.Discount = sour.carInfo.useDisc;
                            }

                            motoMonth = motoMonth.Where(x => x.MotoTotalHours > 0).ToList();
                            if (motoMonth.Count > 0 && input.db_InsMonthlyHistory)
                            {
                                sour.UseMonthMode = true;
                                int UseLen = motoMonth.Count;
                                for (int i = 0; i < UseLen; i++)
                                {
                                    sour.flag = sour.monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.tmpOrder, motoMonth[i].MonthlyRentId, 0, 0, Convert.ToInt32(motoMonth[i].MotoTotalHours), sour.LogID, ref errCode); //寫入記錄
                                }
                            }
                            else
                            {
                                sour.UseMonthMode = false;
                            }
                        }
                        else
                        {
                            List<MonthlyRentData> UseMonthlyRent = new List<MonthlyRentData>();

                            UseMonthlyRent = sour.monthlyRentDatas;

                            int xDiscount = sour.Discount;//帶入月租運算的折扣
                            if (sour.hasFine)
                            {
                                sour.carInfo = sour.billCommon.CarRentInCompute(sour.SD, sour.ED, sour.OrderDataLists[0].PRICE, sour.OrderDataLists[0].PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount);
                                if (sour.carInfo != null)
                                {
                                    sour.CarRentPrice += sour.carInfo.RentInPay;
                                    if (sour.carInfo.mFinal != null && sour.carInfo.mFinal.Count > 0)
                                        UseMonthlyRent = sour.carInfo.mFinal;
                                    sour.Discount = sour.carInfo.useDisc;
                                }
                                sour.CarRentPrice += sour.car_outPrice;
                            }
                            else
                            {
                                sour.carInfo = sour.billCommon.CarRentInCompute(sour.SD, sour.FED, sour.OrderDataLists[0].PRICE, sour.OrderDataLists[0].PRICE_H, sour.carBaseMins, 10, sour.lstHoliday, UseMonthlyRent, xDiscount);
                                if (sour.carInfo != null)
                                {
                                    sour.CarRentPrice += sour.carInfo.RentInPay;
                                    if (sour.carInfo.mFinal != null && sour.carInfo.mFinal.Count > 0)
                                        UseMonthlyRent = sour.carInfo.mFinal;
                                    sour.Discount = sour.carInfo.useDisc;
                                }
                            }

                            if (UseMonthlyRent.Count > 0 && input.db_InsMonthlyHistory)
                            {
                                sour.UseMonthMode = true;
                                int UseLen = UseMonthlyRent.Count;
                                for (int i = 0; i < UseLen; i++)
                                {
                                    sour.flag = sour.monthlyRentRepository.InsMonthlyHistory(sour.IDNO, sour.tmpOrder, UseMonthlyRent[i].MonthlyRentId, Convert.ToInt32(UseMonthlyRent[i].WorkDayHours * 60), Convert.ToInt32(UseMonthlyRent[i].HolidayHours * 60), 0, sour.LogID, ref errCode); //寫入記錄
                                }
                            }
                            else
                            {
                                sour.UseMonthMode = false;
                            }
                        }
                    }
                }

                sour.errCode = errCode;
            }
            return sour;
        }
    
        public CarRentInit GetTotal(CarRentInit sour)
        {
            if (sour.flag)
            {
                sour.lstHoliday = new CommonRepository(connetStr).GetHolidays(sour.SD.ToString("yyyyMMdd"), sour.FED.ToString("yyyyMMdd"));
                if (sour.ProjType == 4)
                {
                    if (sour.UseMonthMode)   //true:有月租;false:無月租
                    {
                        sour.outputApi.MonthRent.HoildayRate = sour.monthlyRentDatas[0].HoildayRateForMoto;
                        sour.outputApi.MonthRent.WorkdayRate = sour.monthlyRentDatas[0].WorkDayRateForMoto;
                    }
                    else
                    {
                        var item = sour.OrderDataLists[0];
                        var dayMaxMinns = Convert.ToDouble(item.MaxPrice) / Convert.ToDouble(item.MinuteOfPrice);

                        sour.carInfo = sour.billCommon.MotoRentMonthComp(sour.SD, sour.ED, item.MinuteOfPrice, item.MinuteOfPrice, sour.motoBaseMins, dayMaxMinns, null, null, sour.Discount);
                        if (sour.carInfo != null)
                            sour.outputApi.Rent.CarRental = sour.carInfo.RentInPay;
                    }

                    sour.outputApi.Rent.RentBasicPrice = sour.OrderDataLists[0].BaseMinutesPrice;
                }
                else
                {
                    #region 非月租折扣計算
                    if (!sour.UseMonthMode)
                    {
                        if (sour.hasFine)
                        {
                            var xre = new BillCommon().CarDiscToPara(sour.SD, sour.ED, sour.carBaseMins, 600, sour.lstHoliday, sour.Discount);
                            if (xre != null)
                            {
                                sour.nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
                                sour.nor_car_wDisc = xre.Item2;
                                sour.nor_car_hDisc = xre.Item3;
                            }
                        }
                        else
                        {
                            var xre = new BillCommon().CarDiscToPara(sour.SD, sour.FED, sour.carBaseMins, 600, sour.lstHoliday, sour.Discount);
                            if (xre != null)
                            {
                                sour.nor_car_PayDisc = Convert.ToInt32(Math.Floor(xre.Item1));
                                sour.nor_car_wDisc = xre.Item2;
                                sour.nor_car_hDisc = xre.Item3;
                            }
                        }

                        var discPrice = Convert.ToDouble(sour.car_n_price) * (sour.nor_car_wDisc / 60) + Convert.ToDouble(sour.car_h_price) * (sour.nor_car_hDisc / 60);
                        sour.nor_car_PayDiscPrice = Convert.ToInt32(Math.Floor(discPrice));
                        sour.Discount = sour.nor_car_PayDisc;
                    }

                    #endregion

                    if (sour.UseMonthMode)
                    {
                        sour.outputApi.Rent.CarRental = sour.CarRentPrice;
                        sour.outputApi.MonthRent.HoildayRate = sour.monthlyRentDatas[0].HoildayRateForCar;
                        sour.outputApi.MonthRent.WorkdayRate = sour.monthlyRentDatas[0].WorkDayRateForCar;
                    }
                    else
                    {
                        sour.CarRentPrice = sour.car_inPrice;//未逾時租用費用
                        if (sour.hasFine)
                            sour.outputApi.Rent.OvertimeRental = sour.car_outPrice;//逾時費用
                    }

                    if (sour.Discount > 0)
                    {
                        double n_price = Convert.ToDouble(sour.OrderDataLists[0].PRICE);
                        double h_price = Convert.ToDouble(sour.OrderDataLists[0].PRICE_H);

                        if (sour.UseMonthMode)
                        {

                        }
                        else
                        {
                            //非月租折扣
                            sour.CarRentPrice -= sour.nor_car_PayDiscPrice;
                            sour.CarRentPrice = sour.CarRentPrice > 0 ? sour.CarRentPrice : 0;
                        }
                    }
                    //安心服務
                    sour.InsurancePerHours = sour.OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(sour.OrderDataLists[0].InsurancePerHours) : 0;
                    if (sour.InsurancePerHours > 0)
                    {
                        sour.outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((sour.car_payInMins / 30.0) * sour.InsurancePerHours / 2)));

                        //逾時安心服務計算
                        if (sour.TotalFineRentMinutes > 0)
                        {
                            sour.outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((sour.car_payOutMins / 30.0) * sour.InsurancePerHours / 2)));
                        }
                    }

                    sour.outputApi.Rent.CarRental = sour.CarRentPrice;
                    sour.outputApi.Rent.RentBasicPrice = sour.OrderDataLists[0].BaseMinutesPrice;
                    sour.outputApi.CarRent.MilUnit = (sour.OrderDataLists[0].MilageUnit <= 0) ? Mildef : sour.OrderDataLists[0].MilageUnit;
                    //里程費計算修改，遇到取不到里程數的先以0元為主
                    //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                    // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                    if (sour.OrderDataLists[0].start_mile == 0 ||
                        sour.OrderDataLists[0].end_mile == 0 ||
                        ((sour.OrderDataLists[0].end_mile - sour.OrderDataLists[0].start_mile) > 1000) ||
                        ((sour.OrderDataLists[0].end_mile - sour.OrderDataLists[0].start_mile) < 0)
                        )
                    {
                        sour.outputApi.Rent.MileageRent = 0;
                    }
                    else
                    {
                        sour.outputApi.Rent.MileageRent = Convert.ToInt32(sour.OrderDataLists[0].MilageUnit * (sour.OrderDataLists[0].end_mile - sour.OrderDataLists[0].start_mile));
                    }
                }

                sour.outputApi.Rent.ActualRedeemableTimeInterval = sour.ActualRedeemableTimePoint.ToString();
                sour.outputApi.Rent.RemainRentalTimeInterval = (sour.TotalRentMinutes > 0 ? sour.TotalRentMinutes : 0).ToString();
                sour.outputApi.Rent.TransferPrice = (sour.OrderDataLists[0].init_TransDiscount > 0) ? sour.OrderDataLists[0].init_TransDiscount : 0;
                //20201202 ADD BY ADAM REASON.ETAG費用
                sour.outputApi.Rent.ETAGRental = sour.etagPrice;

                var xTotalRental = 
                    sour.outputApi.Rent.CarRental + sour.outputApi.Rent.ParkingFee + 
                    sour.outputApi.Rent.MileageRent + sour.outputApi.Rent.OvertimeRental + 
                    sour.outputApi.Rent.InsurancePurePrice + sour.outputApi.Rent.InsuranceExtPrice 
                    - sour.outputApi.Rent.TransferPrice 
                    + sour.outputApi.Rent.ETAGRental;
                xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                sour.outputApi.Rent.TotalRental = xTotalRental;

                #region 修正輸出欄位

                var tra = sour.OrderDataLists[0].init_TransDiscount;
                if (xTotalRental == 0)
                {
                    var carPri = sour.outputApi.Rent.CarRental;
                    if (carPri > 0)
                        sour.outputApi.Rent.TransferPrice = carPri;
                }

                //note: 修正輸出欄位PayDetail
                if (sour.ProjType == 4)
                {
                    sour.outputApi.Rent.UseMonthlyTimeInterval = sour.carInfo.useMonthDisc.ToString();
                    sour.outputApi.Rent.UseNorTimeInterval = sour.carInfo.useDisc.ToString();
                    sour.outputApi.Rent.RentalTimeInterval = (sour.carInfo.RentInMins).ToString();//租用時數(未逾時)

                    //2020-12-29 所有點數改成皆可折抵
                    //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                    sour.outputApi.Rent.ActualRedeemableTimeInterval = sour.carInfo.AfterDiscRentInMins.ToString();

                    sour.outputApi.Rent.RemainRentalTimeInterval = sour.carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

                    var cDisc = sour.apiInput.Discount;
                    var mDisc = sour.apiInput.MotorDiscount;
                    if (sour.carInfo.useDisc > 0)
                    {
                        int lastDisc = sour.carInfo.useDisc;
                        var useMdisc = mDisc > sour.carInfo.useDisc ? sour.carInfo.useDisc : mDisc;
                        lastDisc -= useMdisc;
                        sour.gift_motor_point = useMdisc;
                        if (lastDisc > 0)
                        {
                            var useCdisc = cDisc > lastDisc ? lastDisc : cDisc;
                            lastDisc -= useCdisc;
                            sour.gift_point = useCdisc;
                        }
                    }
                }
                else
                {
                    if (sour.UseMonthMode)
                    {
                        sour.outputApi.Rent.UseMonthlyTimeInterval = sour.carInfo.useMonthDisc.ToString();
                        sour.outputApi.Rent.UseNorTimeInterval = sour.carInfo.useDisc.ToString();
                        sour.outputApi.Rent.RentalTimeInterval = (sour.carInfo.RentInMins).ToString();//租用時數(未逾時)

                        //2020 - 12 - 29 所有點數改成皆可折抵
                        //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                        sour.outputApi.Rent.ActualRedeemableTimeInterval = sour.carInfo.AfterDiscRentInMins.ToString();

                        sour.outputApi.Rent.RemainRentalTimeInterval = sour.carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
                        if (sour.carInfo != null && sour.carInfo.useDisc > 0)
                            sour.gift_point = sour.carInfo.useDisc;
                    }
                    else
                    {
                        sour.outputApi.Rent.UseNorTimeInterval = sour.Discount.ToString();
                        sour.outputApi.Rent.RentalTimeInterval = sour.car_payInMins.ToString(); //租用時數(未逾時)
                        sour.outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(sour.car_pay_in_wMins + sour.car_pay_in_hMins).ToString();//可折抵租用時數
                        sour.outputApi.Rent.RemainRentalTimeInterval = (sour.car_payInMins - sour.Discount).ToString();//未逾時折抵後的租用時數
                        sour.gift_point = sour.nor_car_PayDisc;
                    }

                    sour.gift_motor_point = 0;
                    sour.outputApi.Rent.OvertimeRental = sour.car_outPrice;//逾時費用
                }

                #endregion
            }
            return sour;
        }
    
        public CarRentInit CalFinalPrice(CarRentInit sour)
        {
            string SPName = new ObjType().GetSPName(ObjType.SPType.CalFinalPrice);
            SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
            {
                IDNO = sour.IDNO,
                OrderNo = sour.tmpOrder,
                final_price = sour.outputApi.Rent.TotalRental,
                pure_price = sour.outputApi.Rent.CarRental,
                mileage_price = sour.outputApi.Rent.MileageRent,
                Insurance_price = sour.outputApi.Rent.InsurancePurePrice + sour.outputApi.Rent.InsuranceExtPrice,
                fine_price = sour.outputApi.Rent.OvertimeRental,
                gift_point = sour.gift_point,
                gift_motor_point = sour.gift_motor_point,
                Etag = sour.outputApi.Rent.ETAGRental,
                parkingFee = sour.outputApi.Rent.ParkingFee,
                TransDiscount = sour.outputApi.Rent.TransferPrice,
                Token = sour.Access_Token,
                LogID = sour.LogID,
            };

            var flag = sour.flag;
            var lstError = sour.lstError;
            var errCode = sour.errCode;

            SPOutput_Base SPOutput = new SPOutput_Base();
            SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
            sour.flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
            sour.baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);

            sour.flag = flag;
            sour.lstError = lstError;
            sour.errCode = errCode;

            return sour;
        }
        
        public CarRentInit BE_CalFinalPrice(CarRentInit sour)
        {
            string SPName = new ObjType().GetSPName(ObjType.SPType.BE_CalFinalPrice);
            SPInput_BE_CalFinalPrice SPInput = new SPInput_BE_CalFinalPrice()
            {
                IDNO = sour.IDNO,
                OrderNo = sour.tmpOrder,
                final_price = sour.outputApi.Rent.TotalRental,
                pure_price = sour.outputApi.Rent.CarRental,
                mileage_price = sour.outputApi.Rent.MileageRent,
                Insurance_price = sour.outputApi.Rent.InsurancePurePrice + sour.outputApi.Rent.InsuranceExtPrice,
                fine_price = sour.outputApi.Rent.OvertimeRental,
                gift_point = sour.gift_point,
                //gift_motor_point = gift_motor_point,  //等等補上

                Etag = sour.outputApi.Rent.ETAGRental,
                parkingFee = sour.outputApi.Rent.ParkingFee,
                TransDiscount = sour.outputApi.Rent.TransferPrice,
                LogID = sour.LogID,
                UserID = sour.UserID
            };
            SPOutput_Base SPOutput = new SPOutput_Base();
            SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_BE_CalFinalPrice, SPOutput_Base>(connetStr);
            var lstError = sour.lstError;
            var flag = sour.flag;
            var errCode = sour.errCode;
            sour.flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
            sour.baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
            sour.lstError = lstError;
            sour.flag = flag;
            sour.errCode = errCode;
            return sour;
        }

    }

    public class CarRentInit
    {
        public CarRentInit(string connetStr ="")
        {
            outputApi = new OAPI_GetPayDetail();
            baseVerify = new CommonFunc();
            lstError = new List<ErrorInfo>();
            billCommon = new BillCommon();
            carInfo = new CarRentInfo();
            OrderDataLists = new List<OrderQueryFullData>();

            if (!string.IsNullOrWhiteSpace(connetStr))
              monthlyRentRepository = new MonthlyRentRepository(connetStr);
        }

        public string UserID { get; set; }//呼叫BE_ContactSettingController需要
        public string returnDate { get; set; }//呼叫BE_ContactSettingController需要
        public string ClientIP { get; set; }
        public string Access_Token { get; set; }
        public Dictionary<string, object> objOutput { get; set; }//輸出
        public bool flag = true;
        public bool isWriteError = false;
        public string errMsg { get; set; } = "Success";//預設成功
        public string errCode { get; set; } = "000000";//預設成功
        public string funName { get; set; }
        public Int64 LogID { get; set; } = 0;
        public Int16 ErrType { get; set; } = 0;
        public IAPI_GetPayDetail apiInput { get; set;}
        public OAPI_GetPayDetail outputApi { get; set;}
        public Int64 tmpOrder { get; set; } = -1;
        public Token token { get; set; }
        public CommonFunc baseVerify { get; set; }
        public List<ErrorInfo> lstError { get; set; }
        public List<OrderQueryFullData> OrderDataLists { get; set; }
        public int ProjType { get; set; } = 0;
        public string Contentjson { get; set; } = "";
        public bool isGuest { get; set; } = true;
        public int TotalPoint { get; set; } = 0; //總點數
        public int MotorPoint { get; set; } = 0; //機車點數
        public int CarPoint { get; set; } = 0;   //汽車點數
        public string IDNO { get; set; } = "";
        public int Discount { get; set; } = 0; //要折抵的點數
        public List<Holiday> lstHoliday { get; set; } //假日列表
        public DateTime SD { get; set; } = new DateTime();
        public DateTime ED { get; set; } = new DateTime();
        public DateTime FED { get; set; } = new DateTime();
        public DateTime FineDate { get; set; } = new DateTime();
        public bool hasFine { get; set; } = false; //是否逾時
        public DateTime NowTime { get; set; } = DateTime.Now;
        public int TotalRentMinutes { get; set; } = 0; //總租車時數
        public int TotalFineRentMinutes { get; set; } = 0; //總逾時時數
        public int TotalFineInsuranceMinutes { get; set; } = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
        public int days { get; set; } = 0;
        public int hours { get; set; } = 0; 
        public int mins { get; set; } = 0; //以分計費總時數
        public int FineDays { get; set; } = 0;
        public int FineHours { get; set; } = 0;
        public int FineMins { get; set; } = 0; //以分計費總時數
        public int PDays { get; set; } = 0;
        public int PHours { get; set; } = 0;
        public int PMins { get; set; } = 0; //將點數換算成天、時、分
        public int ActualRedeemableTimePoint { get; set; } = 0; //實際可抵折點數
        public int CarRentPrice { get; set; } = 0; //車輛租金
        public int MonthlyPoint { get; set; } = 0;   //月租折抵點數        20201128 ADD BY ADAM 
        public int MonthlyPrice { get; set; } = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
        public int TransferPrice { get; set; } = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
        public MonthlyRentRepository monthlyRentRepository { get; set; }
        public BillCommon billCommon { get; set; }
        public List<MonthlyRentData> monthlyRentDatas { get; set; } //月租列表
        public bool UseMonthMode { get; set; } = false;  //false:無月租;true:有月租
        public int InsurancePerHours { get; set; } = 0;  //安心服務每小時價
        public int etagPrice { get; set; } = 0;      //ETAG費用 20201202 ADD BY ADAM
        public CarRentInfo carInfo { get; set; }//車資料
        public int ParkingPrice { get; set; } = 0;  //車麻吉停車費    20201209 ADD BY ADAM
        public double nor_car_wDisc { get; set; } = 0;//只有一般時段時平日折扣
        public double nor_car_hDisc { get; set; } = 0;//只有一般時段時價日折扣
        public int nor_car_PayDisc { get; set; } = 0;//只有一般時段時總折扣
        public int nor_car_PayDiscPrice { get; set; } = 0;//只有一般時段時總折扣金額

        public int gift_point { get; set; } = 0;//使用時數(汽車)
        public int gift_motor_point { get; set; } = 0;//使用時數(機車)
        public int motoBaseMins { get; set; } = 6;//機車基本分鐘數
        public int carBaseMins { get; set; } = 60;//汽車基本分鐘數

        public int car_payAllMins { get; set; } = 0; //全部計費租用分鐘
        public int car_payInMins { get; set; } = 0;//未超時計費分鐘
        public int car_payOutMins { get; set; } = 0;//超時分鐘-顯示用

        #region 汽車非月租

        public double car_pay_in_wMins { get; set; } = 0;//未超時平日計費分鐘
        public double car_pay_in_hMins { get; set; } = 0;//未超時假日計費分鐘
        public double car_pay_out_wMins { get; set; } = 0;//超時平日計費分鐘
        public double car_pay_out_hMins { get; set; } = 0;//超時假日計費分鐘
        public int car_inPrice { get; set; } = 0;//未超時費用
        public int car_outPrice { get; set; } = 0;//超時費用
        public int car_n_price { get; set; } = 0;//汽車平日價
        public int car_h_price { get; set; } = 0;//汽車假日價

        #endregion
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

    public enum eumOrderData
    {
        GetPayDetail,
        ContactSetting
    }
}