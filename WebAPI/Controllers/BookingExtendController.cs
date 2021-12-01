using Domain.Common;
using Domain.SP.Input.Notification;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.Rent;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 延長用車
    /// </summary>
    public class BookingExtendController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoBookingExtend(Dictionary<string, object> value)
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
            string funName = "BookingExtendController";
            string spName = "";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BookingExtend apiInput = null;
            NullOutput outputApi = new NullOutput();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            SPOutput_GetBookingStartTime spOut = null;
            OFN_CreditAuthResult AuthOutput = new OFN_CreditAuthResult();
            var trace = new TraceCom();
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            DateTime StopTime = new DateTime();     //延長用車時間
            DateTime SD = new DateTime();
            DateTime StartTime = new DateTime();    //實際出車時間
            DateTime EndTime = new DateTime();      //預計還車時間
            #endregion
            #region 防呆
            trace.traceAdd("apiIn", value);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BookingExtend>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                string[] checkList = { apiInput.OrderNo, apiInput.ED };
                string[] errList = { "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                //2.格式判斷
                if (flag)
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
            //時間判斷
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(apiInput.ED))
                {
                    flag = DateTime.TryParse(apiInput.ED, out StopTime);
                    if (flag == false)
                    {
                        errCode = "ERR176";
                    }
                    else
                    {
                        if (StopTime <= DateTime.Now)
                        {
                            flag = false;
                            errCode = "ERR177";
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
            #region 延長用車前先取得原始用車時間
            if (flag)
            {
                SPInput_GetBookingStartTime spInput = new SPInput_GetBookingStartTime()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                spOut = new SPOutput_GetBookingStartTime();
                spName = "usp_GetBookingStartTime";
                SQLHelper<SPInput_GetBookingStartTime, SPOutput_GetBookingStartTime> sqlHelp = new SQLHelper<SPInput_GetBookingStartTime, SPOutput_GetBookingStartTime>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    StartTime = spOut.SD;
                    EndTime = spOut.ED;

                    if (StopTime.Subtract(spOut.SD).TotalDays == 7)
                    {
                        if (StopTime.Subtract(spOut.SD).TotalHours > 0 || StopTime.Subtract(spOut.SD).TotalMinutes > 0)
                        {
                            flag = false;
                            errCode = "ERR179";
                        }
                    }
                    else if (StopTime.Subtract(spOut.SD).TotalDays > 7)
                    {
                        flag = false;
                        errCode = "ERR179";
                    }

                    var DiffMinute = StopTime.Subtract(EndTime).TotalMinutes;
                    if (DiffMinute < 60)
                    {
                        flag = false;
                        errCode = "ERR237";
                    }
                }
                else
                {
                    errCode = spOut.ErrorCode;
                }
                if (StopTime < spOut.ED)
                {
                    flag = false;
                    errCode = "ERR178";
                }
                if (flag)
                {
                    SD = spOut.ED; //前一次的結束時間
                }

            }
            #endregion
            #region 預授權機制
            if (flag)
            {
                try
                {
                    CommonService commonService = new CommonService();
                    #region 計算預授權金
                    SPOutput_OrderForPreAuth orderInfo = commonService.GetOrderForPreAuth(tmpOrder);
                    //預授權不處理專案(長租客服月結E077)
                    string notHandle = new CommonRepository(connetStr).GetCodeData("PreAuth").FirstOrDefault().MapCode;
                    int preAuthAmt = 0;
                    bool canAuth = false;
                    if (orderInfo != null && (orderInfo.ProjType == 0 || orderInfo.ProjType == 3) && !notHandle.Contains(orderInfo.ProjID))
                    {
                        EstimateData estimateData = new EstimateData()
                        {
                            ProjID = orderInfo.ProjID,
                            ProjType = orderInfo.ProjType,
                            SD = orderInfo.SD,
                            ED = orderInfo.ED,
                            CarNo = orderInfo.CarNo,
                            CarTypeGroupCode = orderInfo.CarTypeGroupCode,
                            WeekdayPrice = orderInfo.PRICE,
                            HoildayPrice = orderInfo.PRICE_H,
                            Insurance = orderInfo.Insurance,
                            InsurancePerHours = orderInfo.InsurancePerHours
                        };

                        //原預計用車時數
                        double oriHour = orderInfo.ED.Subtract(orderInfo.SD).TotalHours;
                        //延長用車總時數
                        double extendHour = StopTime.Subtract(orderInfo.ED).TotalHours;
                        //扣除預授權金額註記
                        bool deduct = false;
                        var estimateDetail = new EstimateDetail();
                        //首次延長
                        if (orderInfo.ExtendTimes == 0)
                        {
                            //原預計用車>=10小時
                            if (oriHour >= 10)
                            {
                                //原預計用車+延長用車時間>24小時
                                if (oriHour + extendHour > 24)
                                {
                                    canAuth = true;
                                    if (extendHour > 6)
                                    {
                                        //新的預估租金與原預授權的差額進行預授權
                                        deduct = true;
                                        estimateData.ED = StopTime;
                                    }
                                    else
                                    {
                                        //收6小時
                                        estimateData.SD = orderInfo.ED;
                                        estimateData.ED = orderInfo.ED.AddHours(6);
                                    }
                                }
                            }
                            else
                            {
                                canAuth = true;
                                if (extendHour > 6)
                                {
                                    //新的預估租金與原預授權的差額進行預授權
                                    deduct = true;
                                    estimateData.ED = StopTime;
                                }
                                else
                                {
                                    //收6小時
                                    estimateData.SD = orderInfo.ED;
                                    estimateData.ED = orderInfo.ED.AddHours(6);
                                }
                            }
                        }
                        else
                        {
                            //延長用車總時數 超過6小時則取超出時數的預估總金額【租金+里程+安心】
                            if (extendHour > 6)
                            {
                                canAuth = true;
                                //新的預估租金與原預授權的差額進行預授權
                                deduct = true;
                                estimateData.ED = StopTime;
                            }
                        }

                        if (canAuth)
                        {
                            EstimateDetail outData;
                            commonService.EstimatePreAuthAmt(estimateData, out outData);
                            preAuthAmt = deduct ? (orderInfo.PreAuthAmt > 0 ? outData.estimateAmt - orderInfo.PreAuthAmt : outData.estimateAmt) : outData.estimateAmt;
                            estimateDetail = outData;
                        }
                        trace.traceAdd("EstimatePreAuthAmt", new { canAuth, oriHour, extendHour, estimateData, estimateDetail, preAuthAmt });
                        trace.FlowList.Add("計算預授權金");
                    }
                    #endregion
                    #region 後續流程
                    if (canAuth && preAuthAmt > 0)
                    {
                        bool authFlag = false;
                        string error = "";

                        #region 刷卡授權
                        CreditAuthComm creditAuthComm = new CreditAuthComm();
                        var AuthInput = new IFN_CreditAuthRequest
                        {
                            CheckoutMode = 4,
                            OrderNo = tmpOrder,
                            IDNO = IDNO,
                            Amount = preAuthAmt,
                            PayType = 0,
                            autoClose = 0,
                            funName = funName,
                            insUser = funName,
                            AuthType = 4
                        };
                        authFlag = creditAuthComm.DoAuthV4(AuthInput, ref error, ref AuthOutput);

                        trace.traceAdd("DoAuthV4", new { AuthInput, AuthOutput, error });
                        trace.FlowList.Add("刷卡授權");

                        #endregion
                        #region 授權結果
                        if (authFlag)
                        {


                            #region 寫入預授權
                            SPInput_InsOrderAuthAmount input_AuthAmount = new SPInput_InsOrderAuthAmount()
                            {
                                IDNO = IDNO,
                                LogID = LogID,
                                Token = Access_Token,
                                AuthType = 4,
                                CardType = AuthOutput == null ? -1 : AuthOutput.CardType,
                                final_price = preAuthAmt,
                                OrderNo = tmpOrder,
                                PRGName = funName,
                                MerchantTradNo = AuthOutput == null ? "" : AuthOutput.Transaction_no,
                                BankTradeNo = AuthOutput == null ? "" : AuthOutput.BankTradeNo,
                                Status = 2
                            };
                            commonService.sp_InsOrderAuthAmount(input_AuthAmount, ref error);

                            trace.traceAdd("sp_InsOrderAuthAmount", new { input_AuthAmount, error });
                            trace.FlowList.Add("寫入預授權");
                            #endregion
                            #region 授權成功新增推播訊息
                            string cardNo = (AuthOutput.CardNo.Substring((AuthOutput.CardNo.Length - 4) > 0 ? AuthOutput.CardNo.Length - 4 : 0));
                            SPInput_InsPersonNotification input_Notification = new SPInput_InsPersonNotification()
                            {
                                OrderNo = Convert.ToInt32(tmpOrder),
                                IDNO = IDNO,
                                LogID = LogID,
                                NType = 19,
                                STime = DateTime.Now.AddSeconds(10),
                                Title = "取授權成功通知",
                                imageurl = "",
                                url = "",
                                Message = $"已於{DateTime.Now.ToString("MM/dd hh:mm")}以末四碼{cardNo}信用卡延長用車取授權成功，金額 {preAuthAmt}，謝謝!"

                            };
                            commonService.sp_InsPersonNotification(input_Notification, ref error);

                            trace.traceAdd("sp_InsPersonNotification", new { input_Notification, error });
                            trace.FlowList.Add("新增推播訊息");
                            #endregion
                        }
                        else
                        {
                            //回傳錯誤代碼，但仍可延長用車
                            errCode = "ERR604";

                            #region Adam哥上線記得打開
                            ////發送MAIL通知據點人員
                            //if (!string.IsNullOrWhiteSpace(orderInfo.StationID))
                            //{
                            //    SendMail send = new SendMail();
                            //    string Receiver = $"{orderInfo.StationID.Trim()}@hotaimotor.com.tw";
                            //    string Title = $"({apiInput.OrderNo})延長用車取授權失敗通知";
                            //    string Body = "再麻煩協助聯繫用戶，告知延長用車取授權失敗且需在還車前確認卡片餘額或是重新綁卡，謝謝!";
                            //    send.DoSendMail(Title, Body, Receiver);
                            //}
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    trace.BaseMsg = ex.Message;
                }

                trace.traceAdd("TraceFinal", new { errCode, errMsg });
                trace.OrderNo = tmpOrder;
                var carRepo = new CarRentRepo();
                carRepo.AddTraceLog(51, funName, trace, flag);

            }
            #endregion
            #region 延長用車
            if (flag)
            {
                SPInput_BookingExtend spExtend = new SPInput_BookingExtend()
                {
                    OrderNo = tmpOrder,
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID,
                    SD = SD,
                    ED = StopTime,
                    CarNo = spOut.CarNo
                };
                SPOutput_Base spOutExtend = new SPOutput_Base();
                spName = "usp_BookingExtend";
                SQLHelper<SPInput_BookingExtend, SPOutput_Base> sqlHelpCheck = new SQLHelper<SPInput_BookingExtend, SPOutput_Base>(connetStr);
                flag = sqlHelpCheck.ExecuteSPNonQuery(spName, spExtend, ref spOutExtend, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOutExtend, ref lstError, ref errCode);
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