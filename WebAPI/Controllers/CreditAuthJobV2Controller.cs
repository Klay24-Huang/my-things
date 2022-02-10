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
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.ComboFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Bill.Input;
using WebAPI.Models.Param.Bill.Output;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 使用信用卡付款
    /// </summary>
    public class CreditAuthJobV2Controller : ApiController
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
            string funName = "CreditAuthJobV2Controller";
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
                logger.Trace("OrderAuthList Count:" + OrderAuthList.Count.ToString());

                foreach (var OrderAuth in OrderAuthList)
                {
                    SPInput_UpdateOrderAuthListV2 UpdateOrderAuthList = new SPInput_UpdateOrderAuthListV2
                    {
                        authSeq = OrderAuth.authSeq,
                        OrderNo = OrderAuth.order_number,
                        AuthType = OrderAuth.AuthType,
                        isRetry = OrderAuth.isRetry,
                        IDNO = OrderAuth.IDNO,
                        AutoClosed = OrderAuth.AutoClosed,
                        final_price = OrderAuth.final_price,
                        ProName = "CreditAuthJobV2",
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
                                CheckoutMode = (OrderAuth.CardType == 1) ? 0 : -1,
                                OrderNo = OrderAuth.order_number,
                                IDNO = OrderAuth.IDNO,
                                Amount = Amount,
                                PayType = 0,
                                autoClose = OrderAuth.AutoClosed,
                                funName = funName,
                                insUser = funName,
                                AuthType = OrderAuth.AuthType
                            };

                            payStatus = creditAuthComm.DoAuthV4(AuthInput, ref errCode, ref AuthOutput);
                            logger.Trace("OrderAuthList Result:" + JsonConvert.SerializeObject(AuthOutput));
                            List<string> exCodeList = new List<string>{ "ER00A", "ER00B", "ERR918", "ERR917", "ERR913" };

                            UpdateOrderAuthList.AuthFlg = payStatus ? 1 : (exCodeList.Any(p=>p== errCode) ?-9:- 1);
                            UpdateOrderAuthList.AuthCode = AuthOutput.AuthCode;
                            UpdateOrderAuthList.AuthMessage = AuthOutput.AuthMessage;
                            UpdateOrderAuthList.transaction_no = AuthOutput.Transaction_no;
                            UpdateOrderAuthList.CardNumber = AuthOutput.CardNo;
                        }
                        else
                        {
                            UpdateOrderAuthList.AuthFlg = 1;
                            UpdateOrderAuthList.AuthCode = "1000";
                            UpdateOrderAuthList.AuthMessage = "金額為0免刷卡";
                        }
                        

                        //SPInput_UpdateOrderAuthListV2 UpdateOrderAuthList = new SPInput_UpdateOrderAuthListV2
                        //{
                        //    authSeq = OrderAuth.authSeq,
                        //    AuthFlg = payStatus ? 1 : -1,
                        //    AuthCode = AuthOutput.AuthCode,
                        //    AuthMessage = AuthOutput.AuthMessage,
                        //    OrderNo = OrderAuth.order_number,
                        //    transaction_no = AuthOutput.Transaction_no,
                        //    AuthType = OrderAuth.AuthType,
                        //    isRetry = OrderAuth.isRetry,
                        //    IDNO = OrderAuth.IDNO,
                        //    AutoClosed = OrderAuth.AutoClosed,
                        //    final_price = OrderAuth.final_price,
                        //    ProName = "CreditAuthJobV2",
                        //    CardNumber = AuthOutput.CardNo,
                        //};

                        //if(payStatus == false && OrderAuth.AuthType == 1 && OrderAuth.isRetry == 0 && !string.IsNullOrWhiteSpace( OrderAuth.Mobile))
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

                        //SPInput_UpdateOrderAuthListV2 UpdateOrderAuthList = new SPInput_UpdateOrderAuthListV2
                        //{
                        //    authSeq = OrderAuth.authSeq,
                        //    AuthFlg = -9,
                        //    AuthCode = "",
                        //    AuthMessage = "",
                        //    OrderNo = OrderAuth.order_number,
                        //    transaction_no = "",
                        //    AuthType = OrderAuth.AuthType,
                        //    isRetry = OrderAuth.isRetry,
                        //    IDNO = OrderAuth.IDNO,
                        //    AutoClosed = OrderAuth.AutoClosed,
                        //    final_price = OrderAuth.final_price,
                        //    ProName = "CreditAuthJobV2",
                        //    CardNumber = "",
                        //};
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
            string SPName = "usp_GetOrderAuthList_Q02";
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
                    logger.Trace("GetOrderAuthList Error:" + JsonConvert.SerializeObject(lstError));
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
            string SPName = "usp_UpdateOrderAuthList_U02";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_UpdateOrderAuthListV2, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_UpdateOrderAuthListV2, SPOutput_Base>(connetStr);
            var flag = SQLPayHelp.ExecuteSPNonQuery(SPName, input, ref spOut, ref lstError);
            
            if (flag == false)
            {
                logger.Trace("UpdateOrderAuthList Params:" + JsonConvert.SerializeObject(input));
                logger.Trace("UpdateOrderAuthList Error:" + JsonConvert.SerializeObject(lstError));
            }
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

            return flag;
        }
    }
}