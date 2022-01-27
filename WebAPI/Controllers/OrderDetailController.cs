using Domain.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Output;
using Domain.TB;
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
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 訂單明細
    /// </summary>
    public class OrderDetailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoOrderDetail(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "OrderDetailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAP_OrderDetail apiInput = null;
            OAPI_OrderDetail outputApi = null;
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            var cr_repo = new CarRentRepo();
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAP_OrderDetail>(Contentjson);
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

            #region TB
            #region Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            #region 取得訂單明細
            if (flag)
            {
                SPInput_GetOrderDetail spInput = new SPInput_GetOrderDetail()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = "usp_GetOrderDetail";
                SPOutput_Base spOut = new SPOutput_Base();
                List<OrderDetailData> orderFinishDataLists = new List<OrderDetailData>();
                DataSet ds = new DataSet();
                SQLHelper<SPInput_GetOrderDetail, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetOrderDetail, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref orderFinishDataLists, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (orderFinishDataLists != null)
                {
                    if (orderFinishDataLists.Count > 0)
                    {
                        int gd = 0, gh = 0, gm = 0;
                        int md = 0, mh = 0, mm = 0;
                        int ud = 0, uh = 0, um = 0;
                        int td = 0, th = 0, tm = 0;
                        int total = Convert.ToInt32(Convert.ToDateTime(orderFinishDataLists[0].EndTime).Subtract(Convert.ToDateTime(orderFinishDataLists[0].StartTime)).TotalMinutes);
                        int useHour = Convert.ToInt32(total - orderFinishDataLists[0].GiftPoint - orderFinishDataLists[0].GiftMotorPoint - (orderFinishDataLists[0].MonthlyHours * 60));
                        BillCommon billComm = new BillCommon();
                        var GiftPoint = orderFinishDataLists[0].GiftPoint + orderFinishDataLists[0].GiftMotorPoint;
                        float UseMile = (float)Math.Round(Convert.ToDecimal(orderFinishDataLists[0].End_mile - orderFinishDataLists[0].Start_mile), 1, MidpointRounding.AwayFromZero);

                        #region 日時分計算
                        var item = orderFinishDataLists[0];
                        var xre = billComm.GetTimePart(Convert.ToDateTime(item.StartTime), Convert.ToDateTime(item.EndTime), item.ProjType);
                        if (xre != null)
                        {
                            td = Convert.ToInt32(xre.Item1);    // 用車使用幾天
                            th = Convert.ToInt32(xre.Item2);    // 用車使用幾時
                            tm = Convert.ToInt32(xre.Item3);    // 用車使用幾分
                        }
                        #region 月租點數
                        if (item.MonthlyHours > 0)
                        {
                            if (item.ProjType == 4)
                            {
                                var vre = billComm.GetTimePart(item.MonthlyHours, 600);     // 20220114 UPD BY YEH REASON:機車單日上限改為600分鐘
                                md = Convert.ToInt32(Math.Floor(vre.Item1));    // 月租使用幾天
                                mh = Convert.ToInt32(Math.Floor(vre.Item2));    // 月租使用幾時
                                mm = Convert.ToInt32(Math.Floor(vre.Item3));    // 月租使用幾分
                            }
                            else
                            {
                                var vre = billComm.GetTimePart(item.MonthlyHours * 60, 600);
                                md = Convert.ToInt32(Math.Floor(vre.Item1));    // 月租使用幾天
                                mh = Convert.ToInt32(Math.Floor(vre.Item2));    // 月租使用幾時
                                mm = Convert.ToInt32(Math.Floor(vre.Item3));    // 月租使用幾分
                            }
                        }
                        #endregion
                        #region 折抵點數
                        if (item.GiftPoint > 0 || item.GiftMotorPoint > 0)
                        {
                            if (item.ProjType == 4)
                            {
                                var allPoints = item.GiftPoint + item.GiftMotorPoint;
                                var vre = billComm.GetTimePart(allPoints, 600);     // 20220114 UPD BY YEH REASON:機車單日上限改為600分鐘
                                if (vre != null)
                                {
                                    gd = Convert.ToInt32(Math.Floor(vre.Item1));    // 折抵使用幾天
                                    gh = Convert.ToInt32(Math.Floor(vre.Item2));    // 折抵使用幾時
                                    gm = Convert.ToInt32(Math.Floor(vre.Item3));    // 折抵使用幾分
                                }
                            }
                            else
                            {
                                var vre = billComm.GetTimePart(item.GiftPoint, 600);
                                if (vre != null)
                                {
                                    gd = Convert.ToInt32(Math.Floor(vre.Item1));    // 折抵使用幾天
                                    gh = Convert.ToInt32(Math.Floor(vre.Item2));    // 折抵使用幾時
                                    gm = Convert.ToInt32(Math.Floor(vre.Item3));    // 折抵使用幾分
                                }
                            }
                        }
                        #endregion
                        #endregion

                        #region 折扣完剩餘日時分
                        int vtd = td;   // 用車使用幾天
                        int vth = th;   // 用車使用幾時
                        int vtm = tm;   // 用車使用幾分

                        double oriPayMins = 0;
                        double lastPayMins = 0;
                        if (item.ProjType == 4)
                        {
                            oriPayMins += vtd * 600;
                            oriPayMins += vth * 60;
                            oriPayMins += vtm;
                            lastPayMins = oriPayMins - (md + gd) * 600 - (mh + gh) * 60 - (mm + gm);
                        }
                        else
                        {
                            oriPayMins += vtd * 600;
                            oriPayMins += vth * 60;
                            oriPayMins += vtm;
                            lastPayMins = oriPayMins - (md + gd) * 600 - (mh + gh) * 60 - (mm + gm);
                        }
                        lastPayMins = lastPayMins < 0 ? 0 : lastPayMins;
                        if (lastPayMins > 0)
                        {
                            if (item.ProjType == 4)
                            {
                                var vre = billComm.GetTimePart(lastPayMins, 600);
                                if (vre != null)
                                {
                                    ud = Convert.ToInt32(Math.Floor(vre.Item1));
                                    uh = Convert.ToInt32(Math.Floor(vre.Item2));
                                    um = Convert.ToInt32(Math.Floor(vre.Item3));
                                }
                            }
                            else
                            {
                                var vre = billComm.GetTimePart(lastPayMins, 600);
                                if (vre != null)
                                {
                                    ud = Convert.ToInt32(Math.Floor(vre.Item1));
                                    uh = Convert.ToInt32(Math.Floor(vre.Item2));
                                    um = Convert.ToInt32(Math.Floor(vre.Item3));
                                }
                            }
                        }
                        #endregion

                        #region 春節訂金
                        int UseOrderPrice = orderFinishDataLists[0].UseOrderPrice;
                        var NYPayLists = cr_repo.GetNYPayList(Convert.ToInt32(tmpOrder));
                        if (NYPayLists != null && NYPayLists.Count() > 0)
                        {
                            var nItem = NYPayLists.FirstOrDefault();
                            UseOrderPrice = UseOrderPrice - nItem.RETURNAMT;
                            UseOrderPrice = UseOrderPrice > 0 ? UseOrderPrice : 0;
                        }
                        #endregion

                        outputApi = new OAPI_OrderDetail()
                        {
                            OrderNo = string.Format("H{0}", orderFinishDataLists[0].OrderNo.ToString().PadLeft(orderFinishDataLists[0].OrderNo.ToString().Length, '0')),
                            ContactURL = "",
                            Operator = orderFinishDataLists[0].Operator,
                            CarTypePic = orderFinishDataLists[0].CarTypePic,
                            CarNo = orderFinishDataLists[0].CarNo,
                            Seat = orderFinishDataLists[0].Seat,
                            CarBrend = orderFinishDataLists[0].CarBrend,
                            CarTypeName = orderFinishDataLists[0].CarTypeName,
                            StationName = orderFinishDataLists[0].StationName,
                            OperatorScore = orderFinishDataLists[0].OperatorScore,
                            ProjName = orderFinishDataLists[0].ProjName,
                            CarRentBill = orderFinishDataLists[0].pure_price,
                            TotalHours = string.Format("{0}天{1}時{2}分", td, th, tm),     // 使用時數
                            MonthlyHours = string.Format("{0}天{1}時{2}分", md, mh, mm),   // 月租折抵
                            GiftPoint = string.Format("{0}天{1}時{2}分", gd, gh, gm),      // 折抵時數
                            PayHours = string.Format("{0}天{1}時{2}分", ud, uh, um),       // 計費時數
                            MileageBill = orderFinishDataLists[0].mileage_price,
                            InsuranceBill = orderFinishDataLists[0].Insurance_price,
                            EtagBill = orderFinishDataLists[0].Etag,
                            OverTimeBill = orderFinishDataLists[0].fine_price,
                            ParkingBill = orderFinishDataLists[0].parkingFee,
                            TransDiscount = orderFinishDataLists[0].TransDiscount,
                            TotalBill = orderFinishDataLists[0].final_price,
                            InvoiceType = orderFinishDataLists[0].InvoiceType,
                            CARRIERID = orderFinishDataLists[0].CARRIERID,
                            NPOBAN = orderFinishDataLists[0].NPOBAN,
                            NPOBAN_Name = orderFinishDataLists[0].NPOBAN_Name,
                            Unified_business_no = orderFinishDataLists[0].Unified_business_no,
                            InvoiceNo = orderFinishDataLists[0].invoiceCode,
                            InvoiceDate = orderFinishDataLists[0].invoice_date,
                            InvoiceBill = orderFinishDataLists[0].invoice_price,
                            InvoiceURL = "",
                            StartTime = Convert.ToDateTime(orderFinishDataLists[0].StartTime).ToString("yyyy-MM-dd HH:mm"),
                            EndTime = Convert.ToDateTime(orderFinishDataLists[0].EndTime).ToString("yyyy-MM-dd HH:mm"),
                            Millage = UseMile <= 0 ? 0 : UseMile,
                            CarOfArea = orderFinishDataLists[0].Area,
                            DiscountAmount = orderFinishDataLists[0].DiscountAmount,
                            DiscountName = orderFinishDataLists[0].DiscountName,
                            //20201212 ADD BY ADAM REASON.增加營損費用，先用預設值
                            CtrlBill = orderFinishDataLists[0].CarDispatch,
                            ClearBill = orderFinishDataLists[0].CleanFee,
                            EquipBill = orderFinishDataLists[0].DestroyFee,
                            ParkingBill2 = orderFinishDataLists[0].ParkingFee2,
                            TowingBill = orderFinishDataLists[0].DraggingFee,
                            OtherBill = orderFinishDataLists[0].OtherFee,
                            UseOrderPrice = UseOrderPrice,
                            ReturnOrderPrice = orderFinishDataLists[0].ReturnOrderPrice,
                            //20210517 ADD BY ADAM REASON.新換電獎勵需求
                            ChangePoint = orderFinishDataLists[0].ChangePoint,
                            ChangeTimes = orderFinishDataLists[0].ChangeTimes,
                            RSOC_S = orderFinishDataLists[0].RSOC_S,
                            RSOC_E = orderFinishDataLists[0].RSOC_E,
                            RewardPoint = orderFinishDataLists[0].RewardPoint,
                            TotalRewardPoint = orderFinishDataLists[0].TotalRewardPoint,
                            RenterType = orderFinishDataLists[0].RenterType
                        };
                    }
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
    }
}