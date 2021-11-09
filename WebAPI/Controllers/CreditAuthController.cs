using Domain.Common;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Car;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.output.HiEasyRentAPI;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using Prometheus; //20210707唐加prometheus
using Reposotory.Implement;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebAPI.Utils;
using WebCommon;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using System.Linq;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using WebAPI.Service;
using Domain.SP.Input.OtherService.Common;
using OtherService.Common;
using Newtonsoft.Json.Linq;
using Domain.WebAPI.output;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 使用信用卡付款
    /// </summary>
    public class CreditAuthController : ApiController
    {
        #region Prometheus
        //唐加prometheus
        private static readonly Gauge ProcessedJobCount1 = Metrics.CreateGauge("CreditAuth_CallTimes", "NUM_CreditAuth_CallTimes");
        private static readonly Gauge ProcessedJobCount2 = Metrics.CreateGauge("CreditAuth_Fail", "NUM_CreditAuth_Fail");
        private static readonly Gauge ProcessedJobCount3 = Metrics.CreateGauge("CreditAuth_Fail_PayType", "NUM_CreditAuth_Fail_PayType");
        private static readonly Gauge ProcessedJobCount4 = Metrics.CreateGauge("CreditAuth_Fail_OrderNoNull", "NUM_CreditAuth_Fail_OrderNoNull");
        private static readonly Gauge ProcessedJobCount5 = Metrics.CreateGauge("CreditAuth_Fail_OrderNoH", "NUM_CreditAuth_Fail_OrderNoH");
        private static readonly Gauge ProcessedJobCount6 = Metrics.CreateGauge("CreditAuth_Fail_tmpOrder", "NUM_CreditAuth_Fail_tmpOrder");
        private static readonly Gauge ProcessedJobCount7 = Metrics.CreateGauge("CreditAuth_Fail_ckTime", "NUM_CreditAuth_Fail_ckTime");
        private static readonly Gauge ProcessedJobCount8 = Metrics.CreateGauge("CreditAuth_Fail_OrderDataLists_Count", "NUM_CreditAuth_Fail_OrderDataLists_Count");
        private static readonly Gauge ProcessedJobCount9 = Metrics.CreateGauge("CreditAuth_Fail_car_mgt_status_15", "NUM_CreditAuth_Fail_car_mgt_status_15");
        private static readonly Gauge ProcessedJobCount10 = Metrics.CreateGauge("CreditAuth_Fail_car_mgt_status_11", "NUM_CreditAuth_Fail_car_mgt_status_11");
        private static readonly Gauge ProcessedJobCount11 = Metrics.CreateGauge("CreditAuth_Fail_PicToAzure", "NUM_CreditAuth_Fail_PicToAzure");
        private static readonly Gauge ProcessedJobCount12 = Metrics.CreateGauge("CreditAuth_Fail_NPR330Save_ID", "NUM_CreditAuth_Fail_NPR330Save_ID");
        private static readonly Gauge ProcessedJobCount13 = Metrics.CreateGauge("CreditAuth_Fail_CacheStringNull", "NUM_CreditAuth_Fail_CacheStringNull");
        private static readonly Gauge ProcessedJobCount14 = Metrics.CreateGauge("CreditAuth_Fail_getBindingList", "NUM_CreditAuth_Fail_getBindingList");
        private static readonly Gauge ProcessedJobCount15 = Metrics.CreateGauge("CreditAuth_Fail_RtnCode_1000", "NUM_CreditAuth_Fail_RtnCode_1000");
        private static readonly Gauge ProcessedJobCount16 = Metrics.CreateGauge("CreditAuth_Fail_ResultCode_1000", "NUM_CreditAuth_Fail_ResultCode_1000");
        private static readonly Gauge ProcessedJobCount17 = Metrics.CreateGauge("CreditAuth_Fail_hasFind", "NUM_CreditAuth_Fail_hasFind");
        private static readonly Gauge ProcessedJobCount18 = Metrics.CreateGauge("CreditAuth_Fail_sp_ArrearsQueryByNPR330ID", "NUM_CreditAuth_Fail_sp_ArrearsQueryByNPR330ID");
        private static readonly Gauge ProcessedJobCount19 = Metrics.CreateGauge("CreditAuth_Fail_isGuest", "NUM_CreditAuth_Fail_isGuest");
        #endregion

        #region 參數宣告
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string BindResultURL = ConfigurationManager.AppSettings["BindResultURL"].ToString();
        private string BindSuccessURL = ConfigurationManager.AppSettings["BindSuccessURL"].ToString();
        private string BindFailURL = ConfigurationManager.AppSettings["BindFailURL"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVer"].ToString();
        private string ApiVerOther = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private static int iButton = (ConfigurationManager.AppSettings["IButtonCheck"] == null) ? 1 : int.Parse(ConfigurationManager.AppSettings["IButtonCheck"]);
        private string RedisConnet = ConfigurationManager.ConnectionStrings["RedisConnectionString"].ConnectionString;
        private string AzureAPIBaseURL = ConfigurationManager.AppSettings["AzureAPIBaseUrl"].ToString();
        private static Lazy<ConnectionMultiplexer> lazyConnection;
        public CreditAuthController()
        {
            if (lazyConnection == null)
            {
                lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(RedisConnet));
            }
        }

        private CommonFunc baseVerify { get; set; }
        #endregion

        [HttpPost]
        public Dictionary<string, object> DoCreditAuth(Dictionary<string, object> value)
        {
            //ProcessedJobCount1.Inc();//唐加prometheus
            SetCount("NUM_CreditAuth_CallTimes");

            #region 初始宣告
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "CreditAuthController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CreditAuth apiInput = null;
            OAPI_CreditAuth apiOutput = null;
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            int Amount = 0;
            List<OrderQueryFullData> OrderDataLists = null;
            int RewardPoint = 0;    //20201201 ADD BY ADAM REASON.換電獎勵
            int DiffAmount = 0;     //20211028 ADD BY YEH REASON:差額
            int RemainingAmount = 0;    //20211028 ADD BY YEH REASON:剩餘金額
            List<TradeCloseList> TradeCloseLists = new List<TradeCloseList>();

            //設定連線字串
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(RedisConnet);
            IDatabase Cache = connection.GetDatabase();
            #endregion

            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                if (flag)
                {
                    //寫入API Log
                    apiInput = JsonConvert.DeserializeObject<IAPI_CreditAuth>(Contentjson);
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                    if (apiInput.PayType < 0 || apiInput.PayType > 1)
                    {
                        flag = false;
                        errCode = "ERR900";
                        //ProcessedJobCount3.Inc();//唐加prometheus
                        SetCount("NUM_CreditAuth_Fail_PayType");//付款方式錯誤，不是租金、罰金/補繳
                    }
                    if (flag)
                    {
                        if (apiInput.PayType == 0)
                        {
                            if (string.IsNullOrWhiteSpace(apiInput.OrderNo))
                            {
                                flag = false;
                                errCode = "ERR900";
                                //ProcessedJobCount4.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_OrderNoNull");
                            }
                            else
                            {
                                if (apiInput.OrderNo.IndexOf("H") < 0)
                                {
                                    flag = false;
                                    errCode = "ERR900";
                                    //ProcessedJobCount5.Inc();//唐加prometheus
                                    SetCount("NUM_CreditAuth_Fail_OrderNoH");
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
                                            //ProcessedJobCount6.Inc();//唐加prometheus
                                            SetCount("NUM_CreditAuth_Fail_tmpOrder");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //不開放訪客
                if (isGuest)
                {
                    flag = false;
                    errCode = "ERR101";
                    //ProcessedJobCount19.Inc();//唐加prometheus
                    SetCount("NUM_CreditAuth_Fail_isGuest");//無token
                }
                #endregion

                trace.traceAdd("apiInCk", new { flag, errCode });

                #region TB
                if (flag && isGuest == false)
                {
                    #region Token判斷
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                    #endregion

                    #region 這邊要再加上查訂單狀態
                    SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        OrderNo = tmpOrder,
                        Token = Access_Token,
                        transaction_no = "",
                        PayMode = apiInput.CheckoutMode
                    };

                    trace.traceAdd("PayInput", PayInput);
                    //訂單
                    if (apiInput.PayType == 0)
                    {
                        #region 0:租金

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
                            CommonService commonService = new CommonService();
                            OrderDataLists = commonService.GetOrderStatusByOrderNo(spInput, ref flag, ref errCode);

                            trace.traceAdd("OrderDataLists", OrderDataLists);

                            //判斷訂單狀態
                            if (flag)
                            {
                                if (OrderDataLists.Count == 0)
                                {
                                    flag = false;
                                    errCode = "ERR203";
                                    //ProcessedJobCount8.Inc();//唐加prometheus
                                    SetCount("NUM_CreditAuth_Fail_OrderDataLists_Count");//找不到符合的訂單編號
                                }
                            }
                        }
                        if (flag)
                        {
                            if (OrderDataLists[0].car_mgt_status >= 15)
                            {
                                flag = false;
                                errCode = "ERR209";
                                //ProcessedJobCount9.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_car_mgt_status_15");//已完成還車付款，請勿重覆付款
                            }
                            else if (OrderDataLists[0].car_mgt_status < 11)
                            {
                                flag = false;
                                errCode = "ERR210";
                                //ProcessedJobCount10.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_car_mgt_status_11");//尚未完成還車步驟，無法還車付款
                            }
                            else
                            {
                                Amount = OrderDataLists[0].final_price;
                            }
                        }
                        trace.traceAdd("OrderDataListsCk", new { flag, errCode });
                        #endregion
                        #region 還車時間檢查 
                        if (flag)
                        {
                            var ckTime = CkFinalStopTime(OrderDataLists[0]);
                            if (!ckTime)
                            {
                                flag = false;
                                errCode = "ERR245";
                                //ProcessedJobCount7.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_ckTime");//使用者超過15分鐘沒還車
                            }
                            trace.traceAdd("ckTime", ckTime);
                        }
                        #endregion
                        #region Adam哥上線記得打開
                        //#region 檢查車機狀態
                        //if (flag && OrderDataLists[0].ProjType != 4)    //汽車才需要檢核 20201212 ADD BY ADAM
                        //{
                        //    flag = new CarCommonFunc().CheckReturnCar(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                        //    trace.traceAdd("CarDevCk", flag);
                        //}
                        //#endregion
                        //#region 檢查iButton
                        //if (flag && OrderDataLists[0].ProjType != 4 && iButton == 1)
                        //{
                        //    SPInput_CheckCariButton spInput = new SPInput_CheckCariButton()
                        //    {
                        //        OrderNo = tmpOrder,
                        //        Token = Access_Token,
                        //        IDNO = IDNO,
                        //        LogID = LogID
                        //    };
                        //    string SPName = "usp_CheckCarIButton";
                        //    SPOutput_Base SPOutputBase = new SPOutput_Base();
                        //    SQLHelper<SPInput_CheckCariButton, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckCariButton, SPOutput_Base>(connetStr);
                        //    flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref SPOutputBase, ref lstError);
                        //    baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);

                        //    trace.traceAdd("iBtnSp", new { spInput, SPOutputBase });
                        //}
                        //#endregion
                        #endregion
                        #region 台新信用卡-Mark
                        //if (flag)
                        //{
                        //    //送台新查詢
                        //    TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                        //    PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                        //    {
                        //        ApiVer = ApiVerOther,
                        //        ApposId = TaishinAPPOS,
                        //        RequestParams = new GetCreditCardListRequestParamasData()
                        //        {
                        //            MemberId = IDNO,
                        //        },
                        //        Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                        //        TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                        //        TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))

                        //    };
                        //    WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                        //    flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                        //    if (flag)
                        //    {
                        //        int Len = wsOutput.ResponseParams.ResultData.Count;
                        //        bool hasFind = false;
                        //        string CardToken = "";
                        //        if (Len > 0)
                        //        {
                        //            CardToken = wsOutput.ResponseParams.ResultData[0].CardToken;
                        //            hasFind = true;
                        //        }

                        //        #region 直接授權
                        //        if (hasFind)//有找到，可以做扣款
                        //        {
                        //            SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                        //            {
                        //                IDNO = IDNO,
                        //                LogID = LogID,
                        //                OrderNo = tmpOrder,
                        //                Token = Access_Token,
                        //                transaction_no = ""
                        //            };

                        //            Thread.Sleep(1000);
                        //            if (Amount > 0)
                        //            {
                        //                Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                        //                {
                        //                    Amount = Amount.ToString() + "00",
                        //                    Name = string.Format("{0}租金", apiInput.OrderNo),
                        //                    NonPoint = "N",
                        //                    NonRedeem = "N",
                        //                    Price = Amount.ToString() + "00",
                        //                    Quantity = "1"
                        //                };
                        //                PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                        //                {
                        //                    ApiVer = "1.0.2",
                        //                    ApposId = TaishinAPPOS,
                        //                    RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                        //                    {
                        //                        CardToken = CardToken,
                        //                        InstallPeriod = "0",
                        //                        InvoiceMark = "N",
                        //                        Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                        //                        MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                        //                        MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                        //                        MerchantTradeNo = string.Format("{0}F{1}", tmpOrder, DateTime.Now.ToString("yyyyMMddHHmmssfff")),
                        //                        NonRedeemAmt = Amount.ToString() + "00",
                        //                        NonRedeemdescCode = "",
                        //                        Remark1 = "",
                        //                        Remark2 = "",
                        //                        Remark3 = "",
                        //                        ResultUrl = BindResultURL,
                        //                        TradeAmount = Amount.ToString() + "00",
                        //                        TradeType = "1",
                        //                        UseRedeem = "N"

                        //                    },
                        //                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                        //                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                        //                };
                        //                WSAuthInput.RequestParams.Item.Add(item);

                        //                WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                        //                flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
                        //                if (WSAuthOutput.RtnCode != "1000" && WSAuthOutput.ResponseParams.ResultCode != "0000")
                        //                {
                        //                    flag = false;
                        //                    errCode = "ERR197";
                        //                }
                        //                if (flag)
                        //                {
                        //                    PayInput.transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                        //                }
                        //            }
                        //            else
                        //            {
                        //                PayInput.transaction_no = "Free";
                        //            }
                        //            if (flag)
                        //            {
                        //                //if (OrderDataLists[0].ProjType == 4)
                        //                //{
                        //                //    bool Motorflag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                        //                //    if (Motorflag == false)
                        //                //    {
                        //                //        //寫入車機錯誤
                        //                //    }
                        //                //}
                        //                bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                        //                if (CarFlag == false)
                        //                {
                        //                    //寫入車機錯誤
                        //                }
                        //                string SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                        //                SPOutput_Base PayOutput = new SPOutput_Base();
                        //                SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                        //                flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                        //                baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                        //            }

                        //        }
                        //        else
                        //        {
                        //            flag = false;
                        //            errCode = "ERR195";
                        //        }
                        //        #endregion

                        //    }
                        //    else
                        //    {
                        //        errCode = "ERR730";
                        //    }
                        //}
                        #endregion

                        //扣款
                        if (flag)
                        {
                            //台新錢包扣款
                            if (apiInput.CheckoutMode == 1)
                            {
                                string TradeType = (OrderDataLists[0].ProjType == 4) ? "Pay_Motor" : "Pay_Car";
                                var orderPayForWallet = PayWalletFlow(tmpOrder, Amount, IDNO, TradeType, true, funName, LogID, Access_Token, ref errCode);
                                flag = orderPayForWallet.flag;
                                trace.traceAdd("PayWalletFlow", new { flag, PayInput, errCode });
                            }
                        }
                        //Mark By Jerry 改為排程取款
                        //if (flag && Amount > 0)       //有錢才刷
                        //{
                        //    WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                        //    flag = TaishinCardTrade(apiInput, ref PayInput, ref WSAuthOutput, ref Amount, ref errCode);
                        //}

                        #region Adam哥上線記得打開
                        //#region 車機指令
                        ////20210102 ADD BY ADAM REASON.車機處理挪到外層呼叫，不放在台新金流內了，偶爾會遇到沒做完就跳出的情況
                        //if (flag)
                        //{
                        //    bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);

                        //    trace.traceAdd("DoCloseRent", new { errCode, dis = "不管車機執行是否成功，都把errCode=000000" });

                        //    if (CarFlag == false)
                        //    {
                        //        //寫入車機錯誤
                        //    }
                        //    errCode = "000000";     //不管車機執行是否成功，都把errCode清掉
                        //}
                        //#endregion
                        #endregion

                        #region 取得預授權金額
                        if (flag)
                        {
                            string SPName = "usp_CreditAuth_Q01";

                            object[][] parms1 = {
                                new object[] {
                                    IDNO,
                                    Access_Token,
                                    tmpOrder,
                                    LogID
                            }};

                            DataSet ds1 = null;
                            string returnMessage = "";
                            string messageLevel = "";
                            string messageType = "";

                            ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                            if (ds1.Tables.Count != 3)
                            {
                                flag = false;
                                errCode = "ERR999";
                                errMsg = returnMessage;
                            }
                            else
                            {
                                baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[2].Rows[0]["Error"]), ds1.Tables[2].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);

                                if (flag)
                                {
                                    //20210524 ADD BY ADAM REASON.針對無資料要判斷
                                    if (ds1.Tables[0].Rows.Count > 0)
                                        DiffAmount = Convert.ToInt32(ds1.Tables[0].Rows[0]["DiffAmount"]);

                                    DataTable dt = ds1.Tables[1];

                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow dr in dt.Rows)
                                        {
                                            var TmpList = new TradeCloseList
                                            {
                                                CloseID = Convert.ToInt32(dr["CloseID"]),
                                                AuthType = Convert.ToInt32(dr["AuthType"]),
                                                CloseAmout = Convert.ToInt32(dr["CloseAmout"]),
                                                ChkClose = Convert.ToInt32(dr["ChkClose"])
                                            };

                                            TradeCloseLists.Add(TmpList);
                                        }
                                    }
                                }
                            }

                            trace.traceAdd("usp_CreditAuth_Q01", new { flag, errCode });
                        }
                        #endregion

                        #region 訂單預授權判斷
                        if (flag)
                        {
                            if (DiffAmount > 0) // 補授權
                            {
                                // 補授權，將目前已收的款項壓上要關帳
                                foreach (var item in TradeCloseLists)
                                {
                                    item.ChkClose = 1;
                                }
                            }
                            else if (DiffAmount == 0)  // 不補不退
                            {
                                // 不補不退，將目前已收的款項壓上要關帳
                                foreach (var item in TradeCloseLists)
                                {
                                    item.ChkClose = 1;
                                }
                            }
                            else if (DiffAmount < 0)   // 調整授權金
                            {
                                // 逐筆更新關帳金額，還有餘額則要退款
                                RemainingAmount = Amount;   // 剩餘金額 = 總價(final_price)
                                foreach (var item in TradeCloseLists)
                                {
                                    if (RemainingAmount > 0)
                                    {
                                        RemainingAmount = RemainingAmount - item.CloseAmout;    // 剩餘金額 = 剩餘金額 - 關帳金額

                                        if (RemainingAmount < 0)    // 剩餘金額<0，代表要退款
                                        {
                                            item.RefundAmount = Math.Abs(RemainingAmount);
                                        }

                                        item.CloseAmout = item.CloseAmout - item.RefundAmount;  // 關帳金額 = 關帳金額 - 退款金額

                                        item.ChkClose = 1;  // 整筆對上或調整金額，壓上1:要關
                                    }
                                    else
                                    {
                                        item.RefundAmount = item.CloseAmout;    // 整筆退的金額壓至退款金額

                                        item.ChkClose = 2;  // 整筆退壓上2:退貨
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 訂單存檔
                        //20201228 ADD BY ADAM REASON.因為目前授權太久會有回上一頁重新計算的問題
                        //所以把存檔功能先提早完成再進行信用卡授權
                        if (flag)
                        {
                            #region 原本存檔(MARK)
                            //string SPName = "usp_DonePayRentBillNew_20210517";

                            ////20201201 ADD BY ADAM REASON.換電獎勵
                            //SPOutput_GetRewardPoint PayOutput = new SPOutput_GetRewardPoint();
                            //SQLHelper<SPInput_DonePayRent, SPOutput_GetRewardPoint> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_GetRewardPoint>(connetStr);
                            //flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                            //baseVerify.checkSQLResult(ref flag, PayOutput.Error, PayOutput.ErrorCode, ref lstError, ref errCode);
                            //if (flag)
                            //{
                            //    RewardPoint = PayOutput.Reward;
                            //}

                            //trace.traceAdd("DonePayRentBill", new { flag, PayInput, PayOutput });
                            #endregion

                            string SPName = "usp_CreditAuth_U01";

                            object[] objparms = new object[TradeCloseLists.Count == 0 ? 1 : TradeCloseLists.Count];

                            if (TradeCloseLists.Count > 0)
                            {
                                for (int i = 0; i < TradeCloseLists.Count; i++)
                                {
                                    objparms[i] = new
                                    {
                                        CloseID = TradeCloseLists[i].CloseID,
                                        AuthType = TradeCloseLists[i].AuthType,
                                        ChkClose = TradeCloseLists[i].ChkClose,
                                        CloseAmout = TradeCloseLists[i].CloseAmout,
                                        RefundAmount = TradeCloseLists[i].RefundAmount
                                    };
                                }
                            }
                            else
                            {
                                objparms[0] = new
                                {
                                    CloseID = 0,
                                    AuthType = 0,
                                    ChkClose = 0,
                                    CloseAmout = 0,
                                    RefundAmount = 0
                                };
                            }

                            object[][] parms1 = {
                                new object[] {
                                    IDNO,
                                    Access_Token,
                                    tmpOrder,
                                    "",
                                    LogID
                                },
                                objparms
                            };

                            DataSet ds1 = new DataSet();
                            string returnMessage = "";
                            string messageLevel = "";
                            string messageType = "";

                            ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                            if (ds1.Tables.Count == 0)
                            {
                                flag = false;
                                errCode = "ERR999";
                                errMsg = returnMessage;
                            }
                            else
                            {
                                baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[1].Rows[0]["Error"]), ds1.Tables[1].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                                if (flag)
                                {
                                    if (ds1.Tables[0].Rows.Count > 0)
                                    {
                                        RewardPoint = Convert.ToInt32(ds1.Tables[0].Rows[0]["Reward"]);
                                    }
                                }
                            }
                            trace.traceAdd("usp_CreditAuth_U01", new { flag, errCode });
                        }
                        #endregion

                        #region 換電獎勵
                        //20201201 ADD BY ADAM REASON.換電獎勵
                        if (flag && OrderDataLists[0].ProjType == 4 && RewardPoint > 0)
                        {
                            WebAPIOutput_NPR380Save wsOutput = new WebAPIOutput_NPR380Save();
                            HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                            flag = wsAPI.NPR380Save(IDNO, RewardPoint.ToString(), apiInput.OrderNo, ref wsOutput);

                            trace.traceAdd("NPR380Save", new { IDNO, RewardPoint, apiInput.OrderNo, wsOutput });

                            //存檔
                            string SPName = "usp_SaveNPR380Result";
                            SPOutput_Base NPR380Output = new SPOutput_Base();
                            SPInput_SetRewardResult NPR380Input = new SPInput_SetRewardResult()
                            {
                                OrderNo = tmpOrder,
                                Result = flag == true ? 1 : 0,
                                LogID = LogID
                            };
                            SQLHelper<SPInput_SetRewardResult, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_SetRewardResult, SPOutput_Base>(connetStr);
                            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, NPR380Input, ref NPR380Output, ref lstError);
                            baseVerify.checkSQLResult(ref flag, ref NPR380Output, ref lstError, ref errCode);

                            trace.traceAdd("SaveNPR380Result", new { flag, NPR380Input, NPR380Output, lstError });
                        }
                        #endregion

                        #region 寫還車照片到azure
                        if (flag)
                        {
                            OtherRepository otherRepository = new OtherRepository(connetStr);
                            List<CarPIC> lstCarPIC = otherRepository.GetCarPIC(tmpOrder, 1);
                            int PICLen = lstCarPIC.Count;
                            for (int i = 0; i < PICLen; i++)
                            {
                                try
                                {
                                    string FileName = string.Format("{0}_{1}_{2}.png", apiInput.OrderNo, (lstCarPIC[i].ImageType == 5) ? "Sign" : "PIC" + lstCarPIC[i].ImageType.ToString(), DateTime.Now.ToString("yyyyMMddHHmmss"));

                                    flag = new AzureStorageHandle().UploadFileToAzureStorage(lstCarPIC[i].Image, FileName, "carpic");
                                    if (flag)
                                    {
                                        bool DelFlag = otherRepository.HandleTempCarPIC(tmpOrder, 1, lstCarPIC[i].ImageType, FileName); //更新為azure的檔名
                                    }
                                }
                                catch (Exception ex)
                                {
                                    flag = true; //先bypass，之後補傳再刪
                                    //ProcessedJobCount11.Inc();//唐加prometheus
                                    SetCount("NUM_CreditAuth_Fail_PicToAzure");//寫還車照片到azure失敗
                                }
                            }

                            trace.traceAdd("reCarToAzure", flag);
                        }
                        #endregion

                        #region Output
                        //機車換電獎勵
                        if (flag)
                        {
                            apiOutput = new OAPI_CreditAuth();
                            apiOutput.RewardPoint = RewardPoint;
                        }
                        #endregion
                        #endregion
                    }
                    else if (apiInput.PayType == 1) //欠費
                    {
                        #region 1:罰金/補繳
                        // 20210220;增加快取機制，當資料存在快取記憶體中，就不再執行並回錯誤訊息。
                        var KeyString = string.Format("{0}-{1}", "CreditAuthController", apiInput.OrderNo);
                        var CacheString = Cache.StringGet("Key1").ToString();

                        if (string.IsNullOrEmpty(CacheString) || KeyString != CacheString)
                        {
                            //Cache.StringSet("Key1", KeyString, TimeSpan.FromSeconds(1));
                            int CreditAuthCheckCacheSeconds = int.Parse(ConfigurationManager.AppSettings["CreditAuthCheckCacheSeconds"].ToString());
                            Cache.StringSet("Key1", KeyString, TimeSpan.FromSeconds(CreditAuthCheckCacheSeconds));        //20210824 ADD BY ADAM REASON.調整重複付款判斷從1秒改為5秒

                            //流水號改由cntrno轉入
                            int NPR330Save_ID = apiInput.CNTRNO == null ? 0 : int.Parse(apiInput.CNTRNO);
                            SPInput_DonePayBack spInput_PayBack = new SPInput_DonePayBack()
                            {
                                NPR330Save_ID = NPR330Save_ID,
                                IDNO = IDNO,
                                MerchantTradeNo = "",
                                TaishinTradeNo = "",
                                Token = Access_Token,
                                LogID = LogID,
                                PayMode = apiInput.CheckoutMode

                            };
                            apiInput.OrderNo = NPR330Save_ID.ToString();    //20201222 ADD BY ADAM REASON.欠費補上id
                            PayInput.OrderNo = NPR330Save_ID;
                            string MSG = "";
                            //先取出要繳的費用
                            var sp_result = sp_ArrearsQueryByNPR330ID(NPR330Save_ID, LogID, ref MSG);

                            trace.traceAdd("DonePayBack", new { spInput_PayBack, sp_result });

                            if (sp_result.Count > 0)
                            {
                                for (int i = 0; i < sp_result.Count; i++)
                                {
                                    Amount += sp_result[i].Amount;
                                }
                            }
                            else
                            {
                                flag = false;
                            }

                            if (flag)
                            {
                                if (NPR330Save_ID > 0)
                                {
                                    string RTNCODE = "";
                                    string RESULTCODE = "";
                                    string MerchantTradeNo = "";
                                    string TaishinTradeNo = "";
                                    string AuthCode = "";
                                    string CardNo = "";
                                    string payCD = "1"; //短租付費類型 1.信用卡 2.錢包扣款
                                    //錢包扣款
                                    if (apiInput.CheckoutMode == 1)
                                    {
                                        string TradeType = "Pay_Arrear";
                                        var orderPayForWallet = PayWalletFlow(NPR330Save_ID, Amount, IDNO, TradeType, true, funName, LogID, Access_Token, ref errCode);
                                        flag = orderPayForWallet.flag;
                                        if (flag)
                                        {
                                            MerchantTradeNo = orderPayForWallet.paymentInfo.StoreTransId;
                                            TaishinTradeNo = orderPayForWallet.paymentInfo.TransId;
                                            RTNCODE = "1000";
                                            RESULTCODE = "1000";
                                            payCD = "2";
                                        }
                                        trace.traceAdd("PayWalletFlow_Arrear", new { flag, PayInput, errCode });
                                    }
                                    else
                                    {
                                        WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();

                                        flag = TaishinCardTrade(apiInput, ref PayInput, ref WSAuthOutput, ref Amount, ref errCode);

                                        trace.traceAdd("TaishinCardTrade", new { apiInput, PayInput, WSAuthOutput, Amount, errCode });

                                        try
                                        {
                                            RTNCODE = WSAuthOutput.RtnCode == null ? "" : WSAuthOutput.RtnCode;
                                            RESULTCODE = WSAuthOutput.ResponseParams.ResultCode == null ? "" : WSAuthOutput.ResponseParams.ResultCode;
                                            payCD = "1";
                                        }
                                        catch (Exception ex)
                                        { }

                                        if (RTNCODE == "1000")
                                        {
                                            MerchantTradeNo = WSAuthOutput.ResponseParams.ResultData.MerchantTradeNo == null
                                                                ? ""
                                                                : WSAuthOutput.ResponseParams.ResultData.MerchantTradeNo;
                                            TaishinTradeNo = WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo == null
                                                                ? ""
                                                                : WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo;
                                            AuthCode = WSAuthOutput.ResponseParams == null
                                                        ? "0000"
                                                        : WSAuthOutput.ResponseParams.ResultData.AuthIdResp;

                                            CardNo = WSAuthOutput.ResponseParams == null
                                                        ? "XXXX-XXXX-XXXX-XXXX"
                                                        : WSAuthOutput.ResponseParams.ResultData.CardNumber;

                                        }

                                    }

                                    if (RTNCODE == "1000")   //20210106 ADD BY ADAM REASON.有成功才呼叫
                                    {
                                        spInput_PayBack.MerchantTradeNo = MerchantTradeNo;
                                        spInput_PayBack.TaishinTradeNo = MerchantTradeNo;
                                        flag = DonePayBack(spInput_PayBack, ref errCode, ref lstError);//欠款繳交

                                        trace.traceAdd("DonePayBack", new { spInput_PayBack, errCode, lstError });
                                    }

                                    if (flag && RTNCODE == "1000" && RESULTCODE == "1000")  //20210106 ADD BY ADAM REASON.有成功才呼叫
                                    {
                                        HiEasyRentAPI webAPI = new HiEasyRentAPI();

                                        //最後再NPR340沖銷
                                        WebAPIInput_NPR340Save wsInput = null;
                                        WebAPIOutput_NPR340Save wsOutput = new WebAPIOutput_NPR340Save();
                                        // string MerchantTradeNo = "";
                                        //string ServiceTradeNo = WSAuthOutput.ResponseParams == null ? "" : WSAuthOutput.ResponseParams.ResultData.ServiceTradeNo; //
                                        string ServiceTradeNo = TaishinTradeNo;
                                        //string AuthCode = WSAuthOutput.ResponseParams == null ? "0000" : WSAuthOutput.ResponseParams.ResultData.AuthIdResp;   //
                                        //string CardNo = WSAuthOutput.ResponseParams == null ? "XXXX-XXXX-XXXX-XXXX" : WSAuthOutput.ResponseParams.ResultData.CardNumber;

                                        wsInput = new WebAPIInput_NPR340Save()
                                        {
                                            tbNPR340SaveServiceVar = new List<NPR340SaveServiceVar>(),
                                            tbNPR340PaymentDetail = new List<NPR340PaymentDetail>()
                                        };

                                        for (int i = 0; i < sp_result.Count; i++)
                                        {
                                            wsInput.tbNPR340SaveServiceVar.Add(new NPR340SaveServiceVar()
                                            {
                                                AMOUNT = sp_result[i].Amount.ToString(),
                                                AUTH_CODE = AuthCode,
                                                CARDNO = CardNo,
                                                CARNO = sp_result[i].CarNo,
                                                CNTRNO = sp_result[i].CNTRNO,
                                                CUSTID = IDNO,
                                                ORDNO = sp_result[i].IRENTORDNO,
                                                POLNO = sp_result[i].POLNO,
                                                PAYMENTTYPE = Convert.ToInt64(sp_result[i].PAYMENTTYPE),
                                                PAYDATE = DateTime.Now.ToString("yyyyMMdd"),
                                                NORDNO = ServiceTradeNo,
                                                CDTMAN = ""
                                            });
                                            //錢包參數
                                            wsInput.tbNPR340PaymentDetail.Add(new NPR340PaymentDetail()
                                            {
                                                CNTRNO = sp_result[i].CNTRNO,
                                                PAYAMT = sp_result[i].Amount.ToString(),
                                                PAYMENTTYPE = sp_result[i].PAYMENTTYPE,
                                                PAYMEMO = "",
                                                PORDNO = sp_result[i].IRENTORDNO,
                                                PAYTCD = payCD
                                            }); 
                                        }

                                        flag = webAPI.NPR340Save(wsInput, ref wsOutput);
                                        {
                                            flag = true;
                                            errCode = "000000";
                                        }

                                        trace.traceAdd("NPR340Save", new { flag, wsInput, wsOutput });
                                    }
                                }
                                else
                                {
                                    errCode = "ERR111";
                                    flag = false;
                                    //ProcessedJobCount12.Inc();//唐加prometheus
                                    SetCount("NUM_CreditAuth_Fail_NPR330Save_ID");//NPR330Save_ID參數遺漏
                                }
                            }
                        }
                        else
                        {
                            errCode = "ERR244";
                            flag = false;
                            //ProcessedJobCount13.Inc();//唐加prometheus
                            SetCount("NUM_CreditAuth_Fail_CacheStringNull");//系統偵測到異常，需重新進入
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                trace.traceAdd("finalFlag", new { flag, errCode });

                #region 寫入錯誤Log
                if (flag == false && isWriteError == false)
                {
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                }
                #endregion
            }
            catch (Exception ex)
            {
                trace.BaseMsg = ex.Message;
                //ProcessedJobCount2.Inc();//唐加prometheus
                SetCount("NUM_CreditAuth_Fail");
            }

            #region TraceLog
            if (string.IsNullOrWhiteSpace(trace.BaseMsg))
            {
                if (flag)
                    carRepo.AddTraceLog(84, funName, eumTraceType.mark, trace);
                else
                    carRepo.AddTraceLog(84, funName, eumTraceType.followErr, trace);
            }
            else
                carRepo.AddTraceLog(84, funName, eumTraceType.exception, trace);
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        #region 台新信用卡交易
        /// <summary>
        /// 台新信用卡交易
        /// </summary>
        private bool TaishinCardTrade(IAPI_CreditAuth apiInput, ref SPInput_DonePayRent PayInput, ref WebAPIOutput_Auth WSAuthOutput, ref int Amount, ref string errCode)
        {
            bool flag = true;
            string IDNO = PayInput.IDNO;
            Int64 LogID = PayInput.LogID;
            Int64 tmpOrder = PayInput.OrderNo;
            string Access_Token = PayInput.Token;

            #region 台新信用卡
            if (flag)
            {
                //送台新查詢
                TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                {
                    ApiVer = ApiVerOther,
                    ApposId = TaishinAPPOS,
                    RequestParams = new GetCreditCardListRequestParamasData()
                    {
                        MemberId = IDNO,
                    },
                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))

                };

                //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                string errMsg = "";
                DataSet ds = Common.getBindingList(IDNO, ref flag, ref errCode, ref errMsg);

                //WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                //flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                if (flag)
                {
                    //int Len = wsOutput.ResponseParams.ResultData.Count;
                    bool hasFind = false;
                    string CardToken = "";
                    //if (Len > 0)
                    //{
                    //    CardToken = wsOutput.ResponseParams.ResultData[0].CardToken;
                    //    hasFind = true;
                    //}
                    //20201219 ADD BY JERRY 更新綁卡查詢邏輯，改由資料庫查詢
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        CardToken = ds.Tables[0].Rows[0]["CardToken"].ToString();
                        hasFind = true;
                    }

                    #region 直接授權
                    if (hasFind)//有找到，可以做扣款
                    {
                        //SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                        //{
                        //    IDNO = IDNO,
                        //    LogID = LogID,
                        //    OrderNo = tmpOrder,
                        //    Token = Access_Token,
                        //    transaction_no = ""
                        //};

                        Thread.Sleep(1000);
                        if (Amount > 0)
                        {
                            Domain.WebAPI.Input.Taishin.AuthItem item = new Domain.WebAPI.Input.Taishin.AuthItem()
                            {
                                Amount = Amount.ToString() + "00",
                                //20210106 ADD BY ADAM REASON.切分租金跟補繳項目
                                Name = apiInput.PayType == 0 ? string.Format("{0}租金", apiInput.OrderNo) : string.Format("{0}補繳", apiInput.OrderNo),
                                NonPoint = "N",
                                NonRedeem = "N",
                                Price = Amount.ToString() + "00",
                                Quantity = "1"
                            };
                            PartOfCreditCardAuth WSAuthInput = new PartOfCreditCardAuth()
                            {
                                ApiVer = "1.0.2",
                                ApposId = TaishinAPPOS,
                                RequestParams = new Domain.WebAPI.Input.Taishin.AuthRequestParams()
                                {
                                    CardToken = CardToken,
                                    InstallPeriod = "0",
                                    InvoiceMark = "N",
                                    Item = new List<Domain.WebAPI.Input.Taishin.AuthItem>(),
                                    MerchantTradeDate = DateTime.Now.ToString("yyyyMMdd"),
                                    MerchantTradeTime = DateTime.Now.ToString("HHmmss"),
                                    //MerchantTradeNo = string.Format("{0}F_{1}", tmpOrder, DateTime.Now.ToString("yyyyMMddHHmmssfff")),   //20201209 ADD BY ADAM REASON.財務又說要改回來
                                    MerchantTradeNo = string.Format(apiInput.PayType == 0 ? "{0}F_{1}" : "{0}G_{1}", apiInput.PayType == 0 ? tmpOrder.ToString() : PayInput.IDNO, DateTime.Now.ToString("yyyyMMddHHmmssfff")),     //20210106 ADD BY ADAM REASON.
                                    //MerchantTradeNo = string.Format("{0}F_{1}", tmpOrder, DateTime.Now.ToString("yyMMddHHmm")),      //20201130 ADD BY ADAM 因應短租財務長度20進行調整
                                    NonRedeemAmt = Amount.ToString() + "00",
                                    NonRedeemdescCode = "",
                                    Remark1 = "",
                                    Remark2 = "",
                                    Remark3 = "",
                                    ResultUrl = BindResultURL,
                                    TradeAmount = Amount.ToString() + "00",
                                    TradeType = "1",
                                    UseRedeem = "N"

                                },
                                Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                                TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),

                            };
                            WSAuthInput.RequestParams.Item.Add(item);

                            //WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                            //flag = WebAPI.DoCreditCardAuth(WSAuthInput, ref errCode, ref WSAuthOutput);
                            flag = WebAPI.DoCreditCardAuthV2(WSAuthInput, IDNO, ref errCode, ref WSAuthOutput);

                            logger.Trace("DoCreditCardAuthV2:" + JsonConvert.SerializeObject(WSAuthOutput));
                            if (WSAuthOutput.RtnCode != "1000")
                            {
                                flag = false;
                                errCode = "ERR197";
                                //ProcessedJobCount15.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_RtnCode_1000");//刷卡授權失敗，請洽發卡銀行，送去台新就失敗
                            }
                            //修正錯誤偵測
                            if (WSAuthOutput.RtnCode == "1000" && WSAuthOutput.ResponseParams.ResultCode != "1000")
                            {
                                flag = false;
                                errCode = "ERR197";
                                //ProcessedJobCount16.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_ResultCode_1000");//刷卡授權失敗，請洽發卡銀行，送去台新檢查內容後失敗
                            }
                            if (flag)
                            {
                                PayInput.transaction_no = WSAuthInput.RequestParams.MerchantTradeNo;
                            }
                        }
                        else
                        {
                            PayInput.transaction_no = "Free";
                        }
                        if (flag)
                        {
                            //if (OrderDataLists[0].ProjType == 4)
                            //{
                            //    bool Motorflag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                            //    if (Motorflag == false)
                            //    {
                            //        //寫入車機錯誤
                            //    }
                            //}

                            //20210101 ADD BY ADAM REASON.這段語法移到外面，目前常會遇到此段語法被跳過的
                            //if (apiInput.PayType == 0)
                            //{
                            //    bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                            //    if (CarFlag == false)
                            //    {
                            //        //寫入車機錯誤
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR195";
                        //ProcessedJobCount17.Inc();//唐加prometheus
                        SetCount("NUM_CreditAuth_Fail_hasFind");//找不到此卡號
                    }
                    #endregion
                }
                else
                {
                    flag = false;
                    errCode = "ERR730";
                    //ProcessedJobCount14.Inc();//唐加prometheus
                    SetCount("NUM_CreditAuth_Fail_getBindingList");//查詢綁定卡號失敗
                }
                ds.Dispose();
            }
            #endregion

            return flag;
        }
        #endregion

        #region 欠費繳交
        /// <summary>
        /// 欠費繳交
        /// </summary>
        /// <param name="spInput"></param>
        /// <param name="errCode"></param>
        /// <param name="lstError"></param>
        /// <returns></returns>
        private bool DonePayBack(SPInput_DonePayBack spInput, ref string errCode, ref List<ErrorInfo> lstError)
        {
            bool flag = true;
            string SPName = "usp_DonePayBack_V2";
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_DonePayBack, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayBack, SPOutput_Base>(connetStr);
            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, spInput, ref spOutput, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutput, ref lstError, ref errCode);
            return flag;
        }

        private List<NPR330SaveDetail> sp_ArrearsQueryByNPR330ID(int NPR330Save_ID, long LogID, ref string errMsg)
        {
            List<NPR330SaveDetail> saveDetail = new List<NPR330SaveDetail>();

            string SPName = "usp_ArrearsQuery_Q1";

            object[] param = new object[2];
            param[0] = NPR330Save_ID;
            param[1] = LogID;

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
                        saveDetail = objUti.ConvertToList<NPR330SaveDetail>(ds1.Tables[0]);
                    else if (ds1.Tables.Count == 1)
                    {
                        var re_db = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                        if (re_db != null && re_db.Error != 0 && !string.IsNullOrWhiteSpace(re_db.ErrorMsg))
                            errMsg = re_db.ErrorMsg;
                    }
                }
            }
            else
            {
                errMsg = returnMessage;
                //ProcessedJobCount18.Inc();//唐加prometheus
                SetCount("NUM_CreditAuth_Fail_sp_ArrearsQueryByNPR330ID");
            }

            return saveDetail;
        }
        #endregion

        #region 還車時間檢查
        private bool CkFinalStopTime(OrderQueryFullData OrderList)
        {
            bool flag = false;
            if (OrderList != null && !string.IsNullOrWhiteSpace(OrderList.final_stop_time))
            {
                if (DateTime.TryParse(OrderList.final_stop_time, out DateTime FD))
                {
                    if (DateTime.Now.Subtract(FD).TotalMinutes < 15)
                    {
                        flag = true;
                    }
                }
            }
            return flag;
        }
        private bool CkFinalStopTime(string final_stop_time)
        {

            if (DateTime.TryParse(final_stop_time, out DateTime FD))
            {
                if (DateTime.Now.Subtract(FD).TotalMinutes < 15)
                    return true;
            }
            return false;
        }
        
        #endregion

        #region Prometheus
        private void SetCount(string memo)
        {
            var value = 1;
            try
            {
                ConnectionMultiplexer connection = lazyConnection.Value;
                IDatabase cache = connection.GetDatabase();

                var key = memo;
                var cacheString = cache.StringGet(key);
                if (cacheString.HasValue)
                {
                    int.TryParse(cacheString.ToString(), out value);
                    value++;
                }
                cache.StringSet(key, value);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            switch (memo)
            {
                case "NUM_CreditAuth_CallTimes":
                    ProcessedJobCount1.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail":
                    ProcessedJobCount2.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_PayType":
                    ProcessedJobCount3.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_OrderNoNull":
                    ProcessedJobCount4.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_OrderNoH":
                    ProcessedJobCount5.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_tmpOrder":
                    ProcessedJobCount6.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_ckTime":
                    ProcessedJobCount7.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_OrderDataLists_Count":
                    ProcessedJobCount8.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_car_mgt_status_15":
                    ProcessedJobCount9.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_car_mgt_status_11":
                    ProcessedJobCount10.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_PicToAzure":
                    ProcessedJobCount11.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_NPR330Save_ID":
                    ProcessedJobCount12.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_CacheStringNull":
                    ProcessedJobCount13.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_getBindingList":
                    ProcessedJobCount14.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_RtnCode_1000":
                    ProcessedJobCount15.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_ResultCode_1000":
                    ProcessedJobCount16.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_hasFind":
                    ProcessedJobCount17.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_sp_ArrearsQueryByNPR330ID":
                    ProcessedJobCount18.Set(value); //宣告Guage才能用set
                    break;
                case "NUM_CreditAuth_Fail_isGuest":
                    ProcessedJobCount19.Set(value); //宣告Guage才能用set
                    break;
            }
        }
        
        private (bool flag, SPInput_WalletPay paymentInfo) PayWalletFlow(long OrderNo, int Amount, string IDNO, string TradeType, bool breakAutoStore, string funName, long LogID, string Access_Token, ref string errCode)
        {
            (bool flag, SPInput_WalletPay paymentInfo) result = (false, new SPInput_WalletPay());

            //扣款金額
            int PayAmount = 0;
            //取得錢包狀態
            var WalletStatus = GetWalletInfo(IDNO, LogID, Access_Token);
            result.flag = WalletStatus.flag;
            if (!result.flag)
            {
                //未開通
                errCode = "ERR932";
                return result;
            }
            //錢包餘額<訂單金額
            if (WalletStatus.WalletInfo.Balance < Amount)
            {
                //這段APP 會做，所以取消
                ////如果自動儲值是on
                //if (breakAutoStore && WalletStatus.WalletInfo.AutoStoreFlag == 1)
                //{
                //    //儲值.....儲值金額(訂單-錢包)
                //    var storeAmount = Amount - WalletStatus.WalletInfo.Balance;

                //    bool storeSataus = WalletStoreByCredit(storeAmount, Access_Token, funName, ref errCode);

                //    if (storeSataus)
                //    {
                //        return PayWalletFlow(OrderNo, Amount, IDNO, TradeType, true, funName, LogID, Access_Token, ref errCode);
                //    }
                //    else
                //    {
                //        if (TradeType != "Pay_Arrear")
                //        {
                //            return PayWalletFlow(OrderNo, Amount, IDNO, TradeType, false, funName, LogID, Access_Token, ref errCode);
                //        }
                //    }
                //}

                PayAmount = WalletStatus.WalletInfo.Balance;
            }
            else //錢包餘額>=訂單金額
            {
                PayAmount = Amount;
            }

            //欠費金額判斷
            if (TradeType == "Pay_Arrear")
            {
                //欠費一定要全繳
                result.flag = IsWalletPayAmountEnough(PayAmount, Amount);
                if (!result.flag)
                {
                    //餘額不足
                    errCode = "ERR934";
                    return result;
                }
            }
            //扣款
            return DoWalletPay(PayAmount, IDNO, OrderNo, TradeType, funName, LogID, Access_Token, ref errCode);


        }
        /// <summary>
        /// 錢包扣款
        /// </summary>
        /// <param name="Amount">扣款金額</param>
        /// <param name="IDNO">扣款帳號</param>
        /// <param name="OrderNo">扣款訂單編號</param>
        /// <returns></returns>
        private (bool flag, SPInput_WalletPay paymentInfo) DoWalletPay(int Amount, string IDNO, long OrderNo, string TradeType, string PRGName, long LogID, string Access_Token, ref string errCode)
        {
            (bool flag, SPInput_WalletPay paymentInfo) result = (false, new SPInput_WalletPay());

            result.flag = IsWalletPayAmountEnough(Amount, 0);

            if (!result.flag)
            {
                errCode = "ERR934";
                return result;
            }

            DateTime NowTime = DateTime.Now;
            //設定錢包付款參數
            WebAPI_PayTransaction wallet = SetForWalletPay(IDNO, OrderNo, Amount, NowTime);
            WebAPIOutput_PayTransaction taishinResponse = null;

            if (result.flag)
            {
                var body = JsonConvert.SerializeObject(wallet);
                TaishinWallet WalletAPI = new TaishinWallet();
                string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                result.flag = WalletAPI.DoPayTransaction(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref taishinResponse);
            }
            if (result.flag)
            {
                var wsp = new WalletSp();
                //設定錢包付款參數寫入
                SPInput_WalletPay spInput = SetForWalletPayLog(wallet, taishinResponse,
                    IDNO, OrderNo, LogID, Access_Token, NowTime, TradeType, PRGName);

                result.flag = wsp.sp_WalletPay(spInput, ref errCode);
                result.paymentInfo = spInput;
            }
            else
            {
                errCode = "ERR933";//扣款失敗
            }
            return result;
        }


        private (bool flag, PayModeObj WalletInfo) GetWalletInfo(string IDNO, long LogID, string Access_Token)
        {
            var lstError = new List<ErrorInfo>();
            //string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            OAPI_GetPayInfo apiOutput = null;
            (bool flag, PayModeObj WalletInfo) re = (false, new PayModeObj());

            string SPName = "usp_GetPayInfo_Q1";
            SPInput_GetPayInfo spInput = new SPInput_GetPayInfo()
            {
                LogID = LogID,
                Token = Access_Token,
                IDNO = IDNO
            };

            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_GetPayInfo, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetPayInfo, SPOutput_Base>(connetStr);
            List<SPOutput_GetPayInfo> PayMode = new List<SPOutput_GetPayInfo>();

            DataSet ds = new DataSet();
            bool flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref PayMode, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            if (flag && PayMode.Count > 0)
            {
                apiOutput = PayMode
                    .Select(t => new OAPI_GetPayInfo
                    {
                        DefPayMode = t.DefPayMode,
                        PayModeBindCount = t.PayModeBindCount,
                        PayModeList = System.Text.Json.JsonSerializer.Deserialize<List<PayModeObj>>(PayMode[0].PayModeList)
                    }).FirstOrDefault();
            }

            PayModeObj WalletInfo = apiOutput?.PayModeList.Where(t => t.PayMode == 1).FirstOrDefault();

            if (WalletInfo?.HasBind == 1)
            {
                re.flag = true;
                re.WalletInfo = WalletInfo;
            }
            //usp_WalletPay_I01
            return re;

        }

        private string GetWalletAccountId(string IDNO, int cnt)
        {
            return $"{IDNO}Wallet{cnt.ToString().PadLeft(4, '0')}";
        }

        private int GetWalletHistoryMode(string TradeType)
        {
            switch (TradeType)
            {
                case "Pay_Arrear":
                    return 5;
                case "pay_Car":
                case "Pay_Motor":
                default:
                    return 0;
            }

        }

        /// <summary>
        /// 設定扣款參數
        /// </summary>
        /// <param name="IDNO"></param>
        /// <param name="OrderNo"></param>
        /// <param name="Amount"></param>
        /// <param name="NowTime"></param>
        /// <returns></returns>
        private WebAPI_PayTransaction SetForWalletPay(string IDNO, long OrderNo, int Amount, DateTime NowTime)
        {
            var accountId = GetWalletAccountId(IDNO, 1);
            string guid = Guid.NewGuid().ToString().Replace("-", "");

            return new WebAPI_PayTransaction()
            {
                AccountId = accountId,
                ApiVersion = "0.1.01",
                GUID = guid,
                MerchantId = MerchantId,
                POSId = "",
                SourceFrom = "9",
                StoreId = "",
                StoreName = "",
                StoreTransId = string.Format("{0}P{1}", OrderNo, (NowTime.ToString("yyMMddHHmmss")).Substring(1)),//限制長度為20以下所以減去1碼
                Amount = Amount,
                BarCode = "",
                StoreTransDate = NowTime.ToString("yyyyMMddHHmmss")
            };

        }

        /// <summary>
        /// 設定歷程寫入參數
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="taishinResponse"></param>
        /// <param name="IDNO"></param>
        /// <param name="OrderNo"></param>
        /// <param name="LogID"></param>
        /// <param name="Access_Token"></param>
        /// <param name="NowTime"></param>
        /// <param name="TradeType"></param>
        /// <param name="PRGName"></param>
        /// <param name="InputSource"></param>
        /// <returns></returns>
        private SPInput_WalletPay SetForWalletPayLog(WebAPI_PayTransaction wallet, WebAPIOutput_PayTransaction taishinResponse
            , string IDNO, long OrderNo, long LogID, string Access_Token, DateTime NowTime, string TradeType, string PRGName)
        {
            return new SPInput_WalletPay()
            {
                LogID = LogID,
                Token = Access_Token,
                IDNO = IDNO,
                OrderNo = OrderNo,
                WalletMemberID = wallet.AccountId,
                WalletAccountID = wallet.AccountId,
                Amount = wallet.Amount,
                WalletBalance = taishinResponse.Result.Amount,
                TransDate = NowTime,
                StoreTransId = taishinResponse.Result.StoreTransId,
                TransId = taishinResponse.Result.TransId,
                TradeType = TradeType,
                PRGName = PRGName,
                Mode = GetWalletHistoryMode(TradeType),
                InputSource = 1
            };

        }

        ////信用卡錢包儲值
        public bool WalletStoreByCredit(int storeMoney, string accessToken, string funName, ref string errCode)
        {

            IAPI_WalletStoreBase Input =
                new IAPI_WalletStoreBase { StoreMoney = storeMoney };


            List<ErrorInfo> lstError = new List<ErrorInfo>();
            DateTime MKTime = DateTime.Now;
            DateTime RTime = MKTime;

            bool flag = false;

            string url = $@"{AzureAPIBaseURL}api/WalletStoredByCredit";


            var resault = ApiPost.DoApiPost<JObject, IAPI_WalletStoreBase>(Input, url, accessToken);

            try
            {
                if (resault.Succ)
                {
                    RTime = DateTime.Now;
                    JsonSerializer serializer = new JsonSerializer();
                    var p =
                        (IRentAPIOutput_Generic<OAPI_WalletStoredByCredit>)serializer.Deserialize(new JTokenReader(resault.Data), typeof(IRentAPIOutput_Generic<OAPI_WalletStoredByCredit>));
                    if (p.result == 1)
                    {
                        flag = (p.Data.StroeResult == 1) ? true : false;
                    }
                    else
                    {
                        errCode = p.ErrorCode;
                    }
                }
                else
                {
                    errCode = resault.errCode;
                }
            }
            finally
            {
                SPInut_WebAPILog SPInput = new SPInut_WebAPILog()
                {
                    MKTime = MKTime,
                    UPDTime = RTime,
                    WebAPIInput = JsonConvert.SerializeObject(Input),
                    WebAPIName = funName,
                    WebAPIOutput = JsonConvert.SerializeObject(resault),
                    WebAPIURL = url
                };
                new WebAPILogCommon().InsWebAPILog(SPInput, ref flag, ref errCode, ref lstError);
            }
            return flag;
        }

        private bool IsWalletPayAmountEnough(int payAmount, int baseAmount)
        {
            if (baseAmount == 0)
                return (payAmount > baseAmount);
            return payAmount >= baseAmount;
        }
        #endregion
    }
}