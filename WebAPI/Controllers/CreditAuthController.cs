using Domain.Common;
using Domain.Log;
using Domain.SP.Input.Bill;
using Domain.SP.Input.Car;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Bill;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
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
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 付款與還款
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
        private static readonly Gauge ProcessedJobCount20 = Metrics.CreateGauge("CreditAuth_Fail_car_mgt_status_13", "NUM_CreditAuth_Fail_car_mgt_status_13");
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
        private string isDebug = ConfigurationManager.AppSettings["isDebug"].ToString();

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
            CommonService commonService = new CommonService();
            PreAmountData PreAmount = new PreAmountData();
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

                    if (apiInput.PayType == 0)
                    {
                        #region 租金

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
                            trace.OrderNo = OrderDataLists[0].OrderNo;
                            if (OrderDataLists[0].car_mgt_status >= 15)
                            {
                                flag = false;
                                errCode = "ERR209";
                                //ProcessedJobCount9.Inc();//唐加prometheus
                                SetCount("NUM_CreditAuth_Fail_car_mgt_status_15");//已完成還車付款，請勿重覆付款
                            }
                            //else if (OrderDataLists[0].car_mgt_status < 13) // 20220120 UPD BY YEH REASON:狀態非13不可還車
                            //{
                            //    flag = false;
                            //    errCode = "ERR210";
                            //    SetCount("NUM_CreditAuth_Fail_car_mgt_status_13");  //尚未完成還車步驟，無法還車付款
                            //}
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
                        #region 車機
                        if (isDebug == "0") // isDebug = 1，不送車機指令
                        {
                            #region 檢查車機狀態
                            if (flag && OrderDataLists[0].ProjType != 4)    //汽車才需要檢核 20201212 ADD BY ADAM
                            {
                                flag = new CarCommonFunc().CheckReturnCar(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                                trace.traceAdd("CarDevCk", flag);
                            }
                            #endregion
                            #region 檢查iButton
                            if (flag && OrderDataLists[0].ProjType != 4 && iButton == 1)
                            {
                                SPInput_CheckCariButton spInput = new SPInput_CheckCariButton()
                                {
                                    OrderNo = tmpOrder,
                                    Token = Access_Token,
                                    IDNO = IDNO,
                                    LogID = LogID
                                };
                                string SPName = "usp_CheckCarIButton";
                                SPOutput_Base SPOutputBase = new SPOutput_Base();
                                SQLHelper<SPInput_CheckCariButton, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckCariButton, SPOutput_Base>(connetStr);
                                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref SPOutputBase, ref lstError);
                                baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);

                                trace.traceAdd("iBtnSp", new { spInput, SPOutputBase });
                            }
                            #endregion
                        }
                        #endregion
                        #region 車機指令
                        //20210102 ADD BY ADAM REASON.車機處理挪到外層呼叫，不放在台新金流內了，偶爾會遇到沒做完就跳出的情況
                        if (flag)
                        {
                            if (isDebug == "0") // isDebug = 1，不送車機指令
                            {
                                bool CarFlag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);

                                trace.traceAdd("DoCloseRent", new { errCode, dis = "不管車機執行是否成功，都把errCode=000000" });

                                if (CarFlag == false)
                                {
                                    //寫入車機錯誤
                                }
                                errCode = "000000";     //不管車機執行是否成功，都把errCode清掉
                            }
                        }
                        #endregion

                        #region 取得預授權金額
                        if (flag)
                        {
                            PreAmount = commonService.GetPreAmount(IDNO, Access_Token, tmpOrder, "Y", LogID, ref flag, ref errCode);

                            trace.traceAdd("GetPreAmount", new { flag, errCode });
                            trace.traceAdd("PreAmountData", PreAmount);
                        }
                        #endregion

                        #region 訂單預授權判斷
                        if (flag)
                        {
                            TradeCloseLists = commonService.DoPreAmount(PreAmount, Amount);

                            trace.traceAdd("DoPreAmount", new { flag, errCode });
                            trace.traceAdd("TradeCloseLists", TradeCloseLists);
                        }
                        #endregion

                        #region 訂單存檔
                        //20201228 ADD BY ADAM REASON.因為目前授權太久會有回上一頁重新計算的問題
                        //所以把存檔功能先提早完成再進行信用卡授權
                        if (flag)
                        {
                            string SPName = "usp_CreditAuth_U01";

                            object[] objparms = new object[TradeCloseLists.Count == 0 ? 1 : TradeCloseLists.Count];

                            if (TradeCloseLists.Count > 0)
                            {
                                for (int i = 0; i < TradeCloseLists.Count; i++)
                                {
                                    objparms[i] = new
                                    {
                                        CloseID = TradeCloseLists[i].CloseID,
                                        CardType = TradeCloseLists[i].CardType,
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
                                    CardType = 0,
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
                                    apiInput.CheckoutMode,
                                    funName,
                                    apiInput.OnceStore,
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
                            trace.traceAdd("CreditAuth_SaveObject", parms1);
                            trace.traceAdd("usp_CreditAuth_U01", new { flag, errCode });
                        }
                        #endregion

                        #region 換電獎勵
                        //20201201 ADD BY ADAM REASON.換電獎勵
                        //if (flag && OrderDataLists[0].ProjType == 4 && RewardPoint > 0)
                        //{
                        //    WebAPIOutput_NPR380Save wsOutput = new WebAPIOutput_NPR380Save();
                        //    HiEasyRentAPI wsAPI = new HiEasyRentAPI();
                        //    flag = wsAPI.NPR380Save(IDNO, RewardPoint.ToString(), apiInput.OrderNo, ref wsOutput);

                        //    trace.traceAdd("NPR380Save", new { IDNO, RewardPoint, apiInput.OrderNo, wsOutput });

                        //    //存檔
                        //    string SPName = "usp_SaveNPR380Result";
                        //    SPOutput_Base NPR380Output = new SPOutput_Base();
                        //    SPInput_SetRewardResult NPR380Input = new SPInput_SetRewardResult()
                        //    {
                        //        OrderNo = tmpOrder,
                        //        Result = flag == true ? 1 : 0,
                        //        LogID = LogID
                        //    };
                        //    SQLHelper<SPInput_SetRewardResult, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_SetRewardResult, SPOutput_Base>(connetStr);
                        //    flag = SQLPayHelp.ExecuteSPNonQuery(SPName, NPR380Input, ref NPR380Output, ref lstError);
                        //    baseVerify.checkSQLResult(ref flag, ref NPR380Output, ref lstError, ref errCode);

                        //    trace.traceAdd("SaveNPR380Result", new { flag, NPR380Input, NPR380Output, lstError });
                        //}
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
                        var KeyString = string.Format("{0}-{1}", "CreditAuthController", apiInput.CNTRNO);
                        //20220211 ADD BY ADAM REASON.調整快取邏輯
                        var CacheString = Cache.StringGet(KeyString).ToString();

                        if (string.IsNullOrEmpty(CacheString) || KeyString != CacheString)
                        {
                            //Cache.StringSet("Key1", KeyString, TimeSpan.FromSeconds(1));
                            int CreditAuthCheckCacheSeconds = int.Parse(ConfigurationManager.AppSettings["CreditAuthCheckCacheSeconds"].ToString());
                            //Cache.StringSet("Key1", KeyString, TimeSpan.FromSeconds(CreditAuthCheckCacheSeconds));        //20210824 ADD BY ADAM REASON.調整重複付款判斷從1秒改為5秒
                            //20211210 UPD BY JERRY 快取邏輯調整，快取名稱應該不同，不然快取會一直被複寫
                            Cache.StringSet(KeyString, KeyString, TimeSpan.FromSeconds(CreditAuthCheckCacheSeconds));        //20210824 ADD BY ADAM REASON.調整重複付款判斷從1秒改為5秒

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
                                    int CardType = 1;   //0:和泰PAY 1:台新(預設) 2:錢包 //20220206 ADD BY ADAM REASON.和泰pay
                                    string MerchantID = ""; //TaishinAPPOS  

                                    var AuthInput = new IFN_CreditAuthRequest
                                    {
                                        CheckoutMode = apiInput.CheckoutMode,
                                        OrderNo = PayInput.OrderNo,
                                        IDNO = PayInput.IDNO,
                                        Amount = Amount,
                                        PayType = 3,
                                        autoClose = 1,
                                        funName = funName,
                                        insUser = funName,
                                        AuthType = 6,
                                        InputSource = 1,
                                        Token = Access_Token,
                                        LogID = LogID
                                    };

                                    //錢包扣款
                                    if (apiInput.CheckoutMode == 1)
                                    {
                                        AuthInput.TradeType = "Pay_Arrear";
                                        AuthInput.AutoStore = true;
                                        AuthInput.OnceStore = apiInput.OnceStore;
                                        payCD = "2";
                                    }

                                    var payStatus = false;
                                    var AuthOutput = new OFN_CreditAuthResult();
                                    var creditAuthComm = new CreditAuthComm();

                                    payStatus = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
                                    trace.traceAdd("CardTrade", new { apiInput, PayInput, AuthInput, AuthOutput, Amount, errCode });
                                    flag = payStatus;

                                    if (flag)
                                    {
                                        MerchantTradeNo = AuthOutput?.Transaction_no ?? "";
                                        TaishinTradeNo = AuthOutput?.BankTradeNo ?? "";
                                        RTNCODE = "1000";
                                        RESULTCODE = "1000";
                                        CardType = AuthOutput.CardType; //20220206 ADD BY ADAM REASON.和泰pay
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
                                        //string MerchantTradeNo = "";
                                        //string ServiceTradeNo = TaishinTradeNo;
                                        //string ServiceTradeNo = CardType == 1 ? TaishinTradeNo : MerchantTradeNo;     //20220206 ADD BY ADAM REASON.和泰pay
                                        string ServiceTradeNo = CardType == 0 ? MerchantTradeNo : TaishinTradeNo;      //20220224 Update BY Umeko REASON Wallet
                                        //string AuthCode = AuthOutput?.AuthIdResp ?? "0000";
                                        //string CardNo = AuthOutput?.CardNo ?? "XXXX-XXXX-XXXX-XXXX";

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
                                                CDTMAN = "",
                                                OPERATOR = GetNPR340Operator(CardType)
                                            });
                                            //錢包參數(20220206上班前才發現，這個目前iRentService沒上)
                                            wsInput.tbNPR340PaymentDetail.Add(new NPR340PaymentDetail()
                                            {
                                                CNTRNO = sp_result[i].CNTRNO,
                                                PAYAMT = sp_result[i].Amount.ToString(),
                                                PAYMENTTYPE = sp_result[i].PAYMENTTYPE,
                                                PAYMEMO = "",
                                                PORDNO = sp_result[i].IRENTORDNO,
                                                PAYTCD = payCD
                                                //OPERATOR = (CardType == 1 ? 0 : 1)      //0:台新 1:中信
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
                flag = false;
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
                case "NUM_CreditAuth_Fail_car_mgt_status_13":
                    ProcessedJobCount20.Set(value); //宣告Guage才能用set
                    break;
            }
        }
        #endregion

        /// <summary>
        /// 取得短租Operator
        /// </summary>
        /// <param name="CardType"></param>
        /// <returns></returns>
        private int GetNPR340Operator(int CardType)
        {
            //0:台新信用卡 1:中信信用卡 2:台新錢包
            switch (CardType)
            {
                case 0:
                    return 1;
                case 2:
                    return 2;
                case 1:
                default:
                    return 0;
            }
        }
    }
}