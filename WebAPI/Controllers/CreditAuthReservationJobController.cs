using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 授權排程_預約
    /// </summary>
    public class CreditAuthReservationJobController: ApiController
    {
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
        private int AuthResendMin = int.Parse(ConfigurationManager.AppSettings["AuthResendMin"]);

        private CommonFunc baseVerify { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoCreditAuthJob(Dictionary<string, object> value)
        {
            logger.Trace("Init");
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "CreditAuthReservationJobController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            IAPI_CreditAuthJobV2 apiInput = new IAPI_CreditAuthJobV2();
            NullOutput apiOutput = new NullOutput();

            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            int Amount = 0;

            //input
            int GateNo = apiInput.GateNo;
            int isRetry = apiInput.isRetry;

            List<OrderAuthListV2> OrderAuthList = null;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_CreditAuthJobV2>(Contentjson);

                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);

                GateNo = apiInput.GateNo;
                isRetry = apiInput.isRetry;
            }
            #endregion
            #region TB
            #region 取出訂單資訊
            if (flag)
            {
                OrderAuthList = GetOrderAuthList(GateNo, isRetry, ref flag, ref lstError, ref errCode);
            }
            #endregion
            if (flag)
            {
                logger.Trace("OrderAuthReservationList Count:" + OrderAuthList.Count.ToString());

                //PayUpList 預約，欠費，用車10小時需全額繳清
                List<int> payUpList = new List<int>{ 1 ,6, 11 };

                List<string> exCodeList = new List<string> { "ER00A", "ER00B", "ERR918", "ERR917", "ERR913" };
                foreach (var OrderAuth in OrderAuthList)
                {
                    //重置
                    errCode = "000000";
                    SPInput_UpdateOrderAuthListV2 UpdateOrderAuthList = new SPInput_UpdateOrderAuthListV2
                    {
                        authSeq = OrderAuth.authSeq,
                        OrderNo = OrderAuth.order_number,
                        AuthType = OrderAuth.AuthType,
                        isRetry = OrderAuth.isRetry,
                        IDNO = OrderAuth.IDNO,
                        AutoClosed = OrderAuth.AutoClosed,
                        final_price = OrderAuth.final_price,
                        ProName = "ReservationJob",
                        CardType = OrderAuth.CardType,
                    };

                    try
                    {

                        Amount = OrderAuth.final_price;
                        var payStatus = true;
                        var AuthOutput = new OFN_CreditAuthResult();
                        if (Amount > 0)       //有錢才刷
                        {
                            var creditAuthComm = new CreditAuthComm();
                            var AuthInput = new IFN_CreditAuthRequest
                            {
                                CheckoutMode = creditAuthComm.GetCheckoutModeByCardType(OrderAuth.CardType),
                                OrderNo = OrderAuth.order_number,
                                IDNO = OrderAuth.IDNO,
                                Amount = Amount,
                                PayType = 0,
                                autoClose = OrderAuth.AutoClosed,
                                funName = funName,
                                insUser = funName,
                                AuthType = OrderAuth.AuthType,
                                ProjType = OrderAuth.ProjType,
                                TradeType = (OrderAuth.CardType == 2) ? GetWalletTradeType(OrderAuth.ProjType, OrderAuth.AuthType) : "",
                                OnceStore = OrderAuth.OnceStore,
                            };

                            if (AuthInput.CheckoutMode == 1 && AuthInput.AuthType == 7)
                            {
                                AuthInput.AutoStore = true;
                            }

                            //必須全繳
                            if(AuthInput.CheckoutMode == 1 && payUpList.Any(p => p == AuthInput.AuthType))
                            {
                                AuthInput.PayUp = 1;
                            }


                            payStatus = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
                            logger.Trace($"OrderAuthReservationList Result: {JsonConvert.SerializeObject(AuthOutput)} | payStatus:{payStatus} | errCode:{errCode}");

                            UpdateOrderAuthList.AuthFlg = payStatus ? 1 : (exCodeList.Any(p => p == errCode) ? -9 : -1);
                            
                            if (AuthInput.CheckoutMode == 1 && AuthInput.AuthType == 11 &&
                                UpdateOrderAuthList.AuthFlg != 1)
                            {
                                //reset
                                payStatus = true;
                                errCode = "000000";
                                AuthInput.CheckoutMode = 4;
                                UpdateOrderAuthList.AuthFlg = 0;

                                payStatus = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
                                logger.Trace($"OrderAuthReservationList(2) Result: {JsonConvert.SerializeObject(AuthOutput)} | payStatus:{payStatus} | errCode:{errCode}");
                                
                                UpdateOrderAuthList.AuthFlg = payStatus ? 1 : (exCodeList.Any(p => p == errCode) ? -9 : -1);
                            }

                            UpdateOrderAuthList.AuthCode = AuthOutput.AuthCode??"";
                            UpdateOrderAuthList.AuthMessage = AuthOutput.AuthMessage??"";
                            UpdateOrderAuthList.transaction_no = AuthOutput.Transaction_no??"";
                            UpdateOrderAuthList.CardNumber = AuthOutput.CardNo??"";
                            UpdateOrderAuthList.CardType = AuthOutput.CardType;
                        }
                        else
                        {
                            UpdateOrderAuthList.AuthFlg = 1;
                            UpdateOrderAuthList.AuthCode = "1000";
                            UpdateOrderAuthList.AuthMessage = "金額為0免刷卡";
                        }

                        //var updateFlag = UpdateOrdarAuthStatus(UpdateOrderAuthList, ref lstError, ref errCode);
                        
                        //if (payStatus == false && OrderAuth.AuthType == 1 && OrderAuth.isRetry == 0 && !string.IsNullOrWhiteSpace(OrderAuth.Mobile))
                        //{
                        //    CreditAuthJobComm creditAuthJobComm = new CreditAuthJobComm();

                        //    var sendSMS = creditAuthJobComm.SendSMS(OrderAuth.Mobile);
                        //}
                    }
                    catch (Exception ex)
                    {
                        logger.Error($"authSeq:{OrderAuth.authSeq}-- OrderAuthListloop Error:{ex.Message}");
                        UpdateOrderAuthList.AuthFlg = -9;
                        UpdateOrderAuthList.AuthMessage = ex.Message;
                    }
                    finally
                    {
                        var updateFlag = UpdateOrdarAuthStatus(UpdateOrderAuthList, ref lstError, ref errCode);
                    }
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

        //取出訂單
        private List<OrderAuthListV2> GetOrderAuthList(int GateNo, int isRetry, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var OrderAuthList = new List<OrderAuthListV2>();

            SPInput_GetOrderAuthListV2 spInput = new SPInput_GetOrderAuthListV2()
            {
                GateNo = GateNo,
                Retry = isRetry
            };
            string SPName = "usp_GetOrderAuthReservationList_Q01";
            SPOutput_Base spOutBase = new SPOutput_Base();
            SQLHelper<SPInput_GetOrderAuthListV2, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderAuthListV2, SPOutput_Base>(connetStr);

            DataSet ds = new DataSet();
            flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref OrderAuthList, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
            //判斷訂單狀態
            if (flag)
            {
                if (OrderAuthList.Count == 0)
                {
                    flag = false;
                    errCode = "ERR203";
                    logger.Trace("GetOrderAuthReservationList Error:" + JsonConvert.SerializeObject(lstError));
                }
            }
            return OrderAuthList;

        }

        /// <summary>
        /// 更新授權
        /// </summary>
        /// <param name="input"></param>
        /// <param name="lstError"></param>
        /// <param name="errCode"></param>
        /// <returns></returns>
        private bool UpdateOrdarAuthStatus(SPInput_UpdateOrderAuthListV2 input, ref List<ErrorInfo> lstError, ref string errCode)
        {
            string SPName = "usp_UpdateOrderAuthReservationList_U01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_UpdateOrderAuthListV2, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_UpdateOrderAuthListV2, SPOutput_Base>(connetStr);
            var flag = SQLPayHelp.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            
            if (flag == false)
            {
                logger.Trace("UpdateOrderAuthReservationList Params:" + JsonConvert.SerializeObject(input));
                logger.Trace("UpdateOrderAuthReservationList Error:" + JsonConvert.SerializeObject(lstError));
            }
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }

        private string GetWalletTradeType(int projType, int authType)
        {
            string tradeType = "";

            /// 授權目的(1、預約,2、訂金,4、延長用車,3、取車,5、逾時,6、欠費,7、還車,8、訂閱制,9、錢包儲值,10、主動取款,11、使用10小時)
            ///
            //新增TradeType： PreAuth_Motor、PreAuth_Car
            /*case "Pay_Arrear":
                   return 5;
               case "pay_Car":
               case "Pay_Motor":
               default:
               */

            string carType = "";

            switch (projType)
            {
                case 0:
                case 3:
                    carType = "Car";
                    break;
                case 4:
                    carType = "Motor";
                    break;
                default:
                    carType = "";
                    break;
            }
            switch (authType)
            {
                case 1:
                    tradeType = $"PreAuth_{carType}";
                    break;
                case 6:
                    tradeType = $"Pay_Arrear";
                    break;
                case 7:
                    tradeType = $"Pay_{carType}";
                    break;
                case 11:
                    tradeType = $"PreAuth_Addition";
                    break;
                default:
                    tradeType = "";
                    break;
            }


            return tradeType;
        }
    }
}