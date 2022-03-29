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
                        var item = orderFinishDataLists[0];
                        int gd = 0, gh = 0, gm = 0;
                        int md = 0, mh = 0, mm = 0;
                        int ud = 0, uh = 0, um = 0;
                        int td = 0, th = 0, tm = 0;
                        int gud = 0, guh = 0, gum = 0;
                        BillCommon billComm = new BillCommon();
                        float UseMile = (float)Math.Round(Convert.ToDecimal(item.End_mile - item.Start_mile), 1, MidpointRounding.AwayFromZero);

                        #region 日時分計算
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
                            var vre = billComm.GetTimePart(item.MonthlyHours * 60, 600);
                            md = Convert.ToInt32(vre.Item1);    // 月租使用幾天
                            mh = Convert.ToInt32(vre.Item2);    // 月租使用幾時
                            mm = Convert.ToInt32(vre.Item3);    // 月租使用幾分
                        }
                        #endregion
                        #region 折抵點數
                        if (item.GiftPoint > 0 || item.GiftMotorPoint > 0)
                        {
                            var allPoints = 0;
                            if (item.ProjType == 4)
                                allPoints = item.GiftPoint + item.GiftMotorPoint;
                            else
                                allPoints = item.GiftPoint;
                            var vre = billComm.GetTimePart(item.GiftPoint, 600);
                            if (vre != null)
                            {
                                gd = Convert.ToInt32(vre.Item1);    // 折抵使用幾天
                                gh = Convert.ToInt32(vre.Item2);    // 折抵使用幾時
                                gm = Convert.ToInt32(vre.Item3);    // 折抵使用幾分
                            }
                        }
                        #endregion
                        #region 優惠標籤使用時數
                        if (item.UseGiveMinute > 0)
                        {
                            var vre = billComm.GetTimePart(item.UseGiveMinute, 600);
                            if (vre != null)
                            {
                                gud = Convert.ToInt32(vre.Item1);    // 優惠標籤使用幾天
                                guh = Convert.ToInt32(vre.Item2);    // 優惠標籤使用幾時
                                gum = Convert.ToInt32(vre.Item3);    // 優惠標籤使用幾分
                            }
                        }
                        #endregion
                        #endregion

                        #region 折扣完剩餘日時分
                        double oriPayMins = td * 600 + th * 60 + tm;
                        double lastPayMins = oriPayMins - (md + gd + gud) * 600 - (mh + gh + guh) * 60 - (mm + gm + gum);
                        lastPayMins = lastPayMins < 0 ? 0 : lastPayMins;
                        if (lastPayMins > 0)
                        {
                            var vre = billComm.GetTimePart(lastPayMins, 600);
                            if (vre != null)
                            {
                                ud = Convert.ToInt32(vre.Item1);
                                uh = Convert.ToInt32(vre.Item2);
                                um = Convert.ToInt32(vre.Item3);
                            }
                        }
                        #endregion

                        #region 春節訂金
                        int UseOrderPrice = item.UseOrderPrice;
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
                            OrderNo = string.Format("H{0}", item.OrderNo.ToString().PadLeft(item.OrderNo.ToString().Length, '0')),
                            ContactURL = "",
                            Operator = item.Operator,
                            CarTypePic = item.CarTypePic,
                            CarNo = item.CarNo,
                            Seat = item.Seat,
                            CarBrend = item.CarBrend,
                            CarTypeName = item.CarTypeName,
                            StationName = item.StationName,
                            OperatorScore = item.OperatorScore,
                            ProjName = item.ProjName,
                            CarRentBill = item.pure_price,
                            TotalHours = string.Format("{0}天{1}時{2}分", td, th, tm),     // 使用時數
                            MonthlyHours = string.Format("{0}天{1}時{2}分", md, mh, mm),   // 月租折抵
                            GiftPoint = string.Format("{0}天{1}時{2}分", gd, gh, gm),      // 折抵時數
                            PayHours = string.Format("{0}天{1}時{2}分", ud, uh, um),       // 計費時數
                            UseGiveMinute = string.Format("{0}天{1}時{2}分", gud, guh, gum),   // 優惠標籤使用時數
                            MileageBill = item.mileage_price,
                            InsuranceBill = item.Insurance_price,
                            EtagBill = item.Etag,
                            OverTimeBill = item.fine_price,
                            ParkingBill = item.parkingFee,
                            TransDiscount = item.TransDiscount,
                            TotalBill = item.final_price,
                            InvoiceType = item.InvoiceType,
                            CARRIERID = item.CARRIERID,
                            NPOBAN = item.NPOBAN,
                            NPOBAN_Name = item.NPOBAN_Name,
                            Unified_business_no = item.Unified_business_no,
                            InvoiceNo = item.invoiceCode,
                            InvoiceDate = item.invoice_date,
                            InvoiceBill = item.invoice_price,
                            InvoiceURL = "",
                            StartTime = Convert.ToDateTime(item.StartTime).ToString("yyyy-MM-dd HH:mm"),
                            EndTime = Convert.ToDateTime(item.EndTime).ToString("yyyy-MM-dd HH:mm"),
                            Millage = UseMile <= 0 ? 0 : UseMile,
                            CarOfArea = item.Area,
                            DiscountAmount = item.DiscountAmount,
                            DiscountName = item.DiscountName,
                            //20201212 ADD BY ADAM REASON.增加營損費用，先用預設值
                            CtrlBill = item.CarDispatch,
                            ClearBill = item.CleanFee,
                            EquipBill = item.DestroyFee,
                            ParkingBill2 = item.ParkingFee2,
                            TowingBill = item.DraggingFee,
                            OtherBill = item.OtherFee,
                            UseOrderPrice = UseOrderPrice,
                            ReturnOrderPrice = item.ReturnOrderPrice,
                            //20210517 ADD BY ADAM REASON.新換電獎勵需求
                            ChangePoint = item.ChangePoint,
                            ChangeTimes = item.ChangeTimes,
                            RSOC_S = item.RSOC_S,
                            RSOC_E = item.RSOC_E,
                            RewardPoint = item.RewardPoint,
                            TotalRewardPoint = item.TotalRewardPoint,
                            RenterType = item.RenterType
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