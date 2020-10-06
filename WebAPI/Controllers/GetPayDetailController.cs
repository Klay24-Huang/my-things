using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
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
           int IsCens = 0;
            int IsMotor = 0;
            string deviceToken = "";
            string StationID = "";
            string CID = "";
            int ProjType =0;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數
            int ETag = 0; //ETag
            string IDNO = "";
            int Discount = 0; //要折抵的點數
            List<Holiday> lstHoliday = null; //假日列表
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime FineDate=new DateTime();
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;
            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int days = 0; int hours = 0; int mins = 0; //以分計費總時數
            int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
            int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int RemainRentalTime = 0;

            BillCommon billCommon = new BillCommon();
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
                    if (apiInput.Discount<0)
                    {
                        flag = false;
                            errCode = "ERR202";

                    }
                    else
                    {
                        Discount = apiInput.Discount;
                    }
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
                        OrderNo=tmpOrder,
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
                        if (OrderDataLists.Count == 0) {
                            flag = false;
                            errCode = "ERR203";
                        }
                    }
                
                }

                #endregion
            }
            #endregion
            #region 第二階段判斷及計價
            if(flag )
            {
                //判斷狀態
                if(OrderDataLists[0].car_mgt_status<11 || OrderDataLists[0].cancel_status > 0)
                {
                    flag = false;
                    errCode = "ERR204";
                }
                //取得專案狀態
                if (flag)
                {
                    ProjType = OrderDataLists[0].ProjType;
                    SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                    //  SD = new DateTime(SD.Year,SD.Month,SD.Day,SD.Hour,SD.Minute,0); //去秒數
                    SD = SD.AddSeconds(SD.Second * -1); //去秒數
                    ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                    ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                    FED = FED.AddSeconds(FED.Second * -1); //去秒數
                    if (FED.Subtract(ED).Ticks > 0)
                    {
                        FineDate = ED;
                        hasFine = true;
                        billCommon.CalDayHourMin(SD, ED, ref days, ref hours, ref mins); //未逾時的總時數
                        TotalRentMinutes= ((days * 10) + hours) * 60 + mins; //未逾時的總時數
                        billCommon.CalDayHourMin(ED, FED, ref FineDays, ref FineHours, ref FineMins);
                        TotalFineRentMinutes = ((FineDays * 10) + FineHours) * 60 + FineMins; //逾時的總時數

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
                //與短租查時數
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
                                    if(wsOutput.Data[i].GIFTTYPE == "01")
                                    {
                                        MotorPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
                                    }
                                    else
                                    {
                                        CarPoint += string.IsNullOrEmpty(wsOutput.Data[i].LASTPOINT) ? 0 : Convert.ToInt32(wsOutput.Data[i].LASTPOINT);
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
                        if (Discount < OrderDataLists[0].BaseMinutes)
                        {
                            flag = false;
                            errCode = "ERR205";
                        }
                        else
                        {
                            if (Discount > (MotorPoint + CarPoint))
                            {
                                flag = false;
                                errCode = "ERR207";
                            }
                        }

                        if (flag)
                        {
                            billCommon.CalPointerToDayHourMin(MotorPoint + CarPoint, ref PDays, ref PHours, ref PMins);
                        }
                       
                    }
                    else
                    {
                        if (Discount>0  && Discount%30>0)
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
                            billCommon.CalPointerToDayHourMin( CarPoint, ref PDays, ref PHours, ref PMins);
                        }

                    }
                }
                #endregion
                #region 建空模及塞入要輸出的值
                if (flag)
                {
                    outputApi.CanUseDiscount = 1; //先暫時寫死，之後改專案設定，由專案設定引入
                    outputApi.CanUseMonthRent= 1;//先暫時寫死，之後改專案設定，由專案設定引入
                    outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
                    outputApi.DiscountAlertMsg = "";
                    outputApi.IsMonthRent = 0; //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
                    outputApi.IsMotor = (ProjType == 4) ? 1 : 0; //是否為機車
                    outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase(); //月租資訊
                    outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();
                    outputApi.PayMode = (ProjType == 4) ? 1 : 0; //目前只有機車才會有以分計費模式
                    outputApi.ProType = ProjType;
                    outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() { 
                     BookingEndDate=ED.ToString("yyyy-MM-dd HH:mm:ss"),
                      BookingStartDate=SD.ToString("yyyy-MM-dd HH:mm:ss"),
                       CarNo=OrderDataLists[0].CarNo,
                       RedeemingTimeCarInterval=CarPoint.ToString(),
                       RedeemingTimeMotorInterval=MotorPoint.ToString(),
                        RedeemingTimeInterval=(ProjType==4)?(CarPoint+MotorPoint).ToString():CarPoint.ToString(),
                         RentalDate=FED.ToString("yyyy-MM-dd HH:mm:ss"),
                          RentalTimeInterval=(TotalRentMinutes+TotalFineRentMinutes).ToString(),
                          
                           



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
                }
                #endregion
                #region 開始計價

                if (flag) {
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (ProjType == 4)
                    {
                        if (TotalPoint >= TotalRentMinutes)
                        {
                            ActualRedeemableTimePoint = TotalRentMinutes;
                        }
                        else
                        {
                            if((TotalPoint-TotalRentMinutes) < OrderDataLists[0].BaseMinutes)
                            {
                                ActualRedeemableTimePoint = TotalRentMinutes - OrderDataLists[0].BaseMinutes;
                            }
                        }
                        if (Discount >= TotalRentMinutes)               
                        {
                            Discount = (days*600)+(hours*60)+(mins);        //自動縮減
                            
                        }
                        else
                        {
                            int tmp = TotalRentMinutes - Discount;
                            if (tmp< OrderDataLists[0].BaseMinutes)
                            {
                                Discount += TotalRentMinutes - Discount - OrderDataLists[0].BaseMinutes;
                            }
                     
                        }
                        TotalRentMinutes -= Discount;
                        int CarRentPrice = 0;
                        billCommon.CalFinalPriceByMinutes(TotalRentMinutes, OrderDataLists[0].BaseMinutes, OrderDataLists[0].BaseMinutesPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MinuteOfPrice, OrderDataLists[0].MaxPrice, ref CarRentPrice);
                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;




                    }
                    else
                    {

                    }

                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes).ToString();

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
