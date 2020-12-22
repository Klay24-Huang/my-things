﻿using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.OrderList;
using Domain.SP.Output;
using Domain.SP.Output.Common;
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
using WebAPI.Models.Enum;
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
            }

            //取得訂單明細
            if (flag)
            {
                SPInput_GetOrderDetail spInput = new SPInput_GetOrderDetail()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.OrderDetail);
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
                        billComm.CalMinuteToDayHourMin(Convert.ToInt32(GiftPoint), ref gd, ref gh, ref gm);
                        billComm.CalMinuteToDayHourMin(Convert.ToInt32(orderFinishDataLists[0].MonthlyHours * 60), ref md, ref mh, ref mm);
                        billComm.CalMinuteToDayHourMin(Convert.ToInt32(useHour), ref ud, ref uh, ref um);
                        billComm.CalMinuteToDayHourMin(Convert.ToInt32(total), ref td, ref th, ref tm);
                        float UseMile = (float)Math.Round(Convert.ToDecimal(orderFinishDataLists[0].End_mile - orderFinishDataLists[0].Start_mile), 1, MidpointRounding.AwayFromZero);

                        var item = orderFinishDataLists[0];
                        var xre = billComm.GetTimePart(Convert.ToDateTime(item.StartTime), Convert.ToDateTime(item.EndTime), item.ProjType);
                        if(xre != null)
                        {
                            td = Convert.ToInt32(xre.Item1);
                            th = Convert.ToInt32(xre.Item2);
                            tm = Convert.ToInt32(xre.Item3);
                        }

                        outputApi = new OAPI_OrderDetail()
                        {
                            OrderNo = string.Format("H{0}", orderFinishDataLists[0].OrderNo.ToString().PadLeft(7, '0')),
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
                            TotalHours = string.Format("{0}天{1}時{2}分", td, th, tm),
                            MonthlyHours = string.Format("{0}天{1}時{2}分", md, mh, mm),
                            GiftPoint = string.Format("{0}天{1}時{2}分", gd, gh, gm),
                            PayHours = string.Format("{0}天{1}時{2}分", ud, uh, um),
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
                            CtrlBill = 0,
                            ClearBill = 0,
                            EquipBill = 0,
                            ParkingBill2 = 0,
                            TowingBill = 0,
                            OtherBill = 0
                        };
                    }
                }
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