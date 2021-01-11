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
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
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
    public class CreditAuthJobController : ApiController
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

        [HttpGet]
        public Dictionary<string, object> DoCreditAuthJob()
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
            string funName = "CreditAuthJobController";
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
            List<OrderAuthList> OrderAuthList = null;
            #endregion
            #region 防呆
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("NA", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion
            #region TB
            #region 取出訂單資訊
            if (flag)
            {
                SPInput_GetOrderAuthList spInput = new SPInput_GetOrderAuthList()
                {
                    MINUTES = AuthResendMin
                };
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetOrderAuthList);
                SPOutput_Base spOutBase = new SPOutput_Base();
                SQLHelper<SPInput_GetOrderAuthList, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetOrderAuthList, SPOutput_Base>(connetStr);
                OrderAuthList = new List<OrderAuthList>();
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
                        logger.Trace("GetOrderAuthList Error:"+JsonConvert.SerializeObject(lstError));
                    }
                }
            }
            #endregion
            if (flag)
            {
                logger.Trace("OrderAuthList Count:" + OrderAuthList.Count.ToString());
                for (int i = 0; i < OrderAuthList.Count; i++)
                {

                    #region 這邊要再加上查訂單狀態
                    SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                    {
                        IDNO = OrderAuthList[i].IDNO,
                        LogID = LogID,
                        OrderNo = OrderAuthList[i].order_number,
                        Token = Access_Token,
                        transaction_no = ""
                    };

                    apiInput = new IAPI_CreditAuth()
                    {
                        PayType = 0,
                        OrderNo = OrderAuthList[i].order_number.ToString()
                    };

                    Amount = OrderAuthList[0].final_price;
                    WebAPIOutput_Auth WSAuthOutput = new WebAPIOutput_Auth();
                    if (Amount > 0)       //有錢才刷
                    {
                        flag = TaishinCardTrade(apiInput, ref PayInput, ref WSAuthOutput, ref Amount, ref errCode);
                    }
                    //logger.Trace("OrderAuthList Result:" + JsonConvert.SerializeObject(WSAuthOutput));

                    SPInput_UpdateOrderAuthList UpdateOrderAuthList = new SPInput_UpdateOrderAuthList()
                    {
                        authSeq = OrderAuthList[i].authSeq,
                        AuthFlg = WSAuthOutput.ResponseParams.ResultCode != "1000" ? -1 : 1,
                        AuthCode = WSAuthOutput.ResponseParams.ResultCode,
                        AuthMessage = WSAuthOutput.ResponseParams.ResultMessage,
                        OrderNo = OrderAuthList[i].order_number,
                        transaction_no = PayInput.transaction_no,
                        Reward = 0
                    };
                    //20201228 ADD BY ADAM REASON.因為目前授權太久會有回上一頁重新計算的問題
                    //                            所以把存檔功能先提早完成再進行信用卡授權
                    string SPName = new ObjType().GetSPName(ObjType.SPType.UpdateOrderAuthList);
                    SPOutput_GetRewardPoint PayOutput = new SPOutput_GetRewardPoint();
                    SQLHelper<SPInput_UpdateOrderAuthList, SPOutput_GetRewardPoint> SQLPayHelp = new SQLHelper<SPInput_UpdateOrderAuthList, SPOutput_GetRewardPoint>(connetStr);
                    flag = SQLPayHelp.ExecuteSPNonQuery(SPName, UpdateOrderAuthList, ref PayOutput, ref lstError);
                    if (flag == false)
                    {
                        logger.Trace("UpdateOrderAuthList Params:" + JsonConvert.SerializeObject(UpdateOrderAuthList));
                        logger.Trace("UpdateOrderAuthList Error:" + JsonConvert.SerializeObject(lstError));
                    }
                    baseVerify.checkSQLResult(ref flag, PayOutput.Error, PayOutput.ErrorCode, ref lstError, ref errCode);
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }

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
                                    MerchantTradeNo = string.Format(apiInput.PayType == 0 ? "{0}F_{1}" : "{0}G_{1}", apiInput.PayType==0 ? tmpOrder.ToString() : PayInput.IDNO, DateTime.Now.ToString("yyyyMMddHHmmssfff")),     //20210106 ADD BY ADAM REASON.
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
                            }
                            //修正錯誤偵測
                            if (WSAuthOutput.RtnCode == "1000" && WSAuthOutput.ResponseParams.ResultCode != "1000")
                            {
                                flag = false;
                                errCode = "ERR197";
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
                    }
                    #endregion
                }
                else
                {
                    flag = false;
                    errCode = "ERR730";
                }
                ds.Dispose();
            }
            #endregion

            return flag;
        }

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
            string SPName = new ObjType().GetSPName(ObjType.SPType.DonePayBack);
            SPOutput_Base spOutput = new SPOutput_Base();
            SQLHelper<SPInput_DonePayBack, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayBack, SPOutput_Base>(connetStr);
            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, spInput, ref spOutput, ref lstError);
            baseVerify.checkSQLResult(ref flag, ref spOutput, ref lstError, ref errCode);
            return flag;
        }

        private List<NPR330SaveDetail> sp_ArrearsQueryByNPR330ID(int NPR330Save_ID, long LogID, ref string errMsg)
        {
            List<NPR330SaveDetail> saveDetail = new List<NPR330SaveDetail>();

            string SPName = new ObjType().GetSPName(ObjType.SPType.ArrearsQueryByNPR330ID);

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
                errMsg = returnMessage;

            return saveDetail;
        }
    }
}