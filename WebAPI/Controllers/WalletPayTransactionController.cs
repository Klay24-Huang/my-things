using Domain.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.OrderList;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 使用錢包付款
    /// </summary>
    public class WalletPayTransactionController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private string BaseURL = ConfigurationManager.AppSettings["TaishinWalletBaseURL"].ToString();

        [HttpPost]
        public Dictionary<string, object> DoWalletPayTransaction(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletPayTransactionController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletPayTransaction apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            int Amount = 0;
            bool HasETAG = false;
            List<OrderQueryFullData> OrderDataLists = null;
            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //寫入API Log
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletPayTransaction>(Contentjson);
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (apiInput.PayType < 0 || apiInput.PayType>1)
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
                    if (apiInput.PayType == 0)
                    {
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
            else
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
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                #region 這邊要再加上查訂單狀態
                if (apiInput.PayType == 0)
                {
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
                            if (OrderDataLists.Count == 0)
                            {
                                flag = false;
                                errCode = "ERR203";
                            }
                        }

                    }
                    if (flag)
                    {
                        if (OrderDataLists[0].car_mgt_status >= 15)
                        {
                            flag = false;
                            errCode = "ERR209";
                        }else if (OrderDataLists[0].car_mgt_status < 11)
                        {
                            flag = false;
                            errCode = "ERR210";
                        }
                        else
                        {
                            Amount = OrderDataLists[0].final_price;
                        }
                    }
                    #endregion
                    #region 檢查車機狀態
                    if (flag)
                    {
                        flag = new CarCommonFunc().CheckReturnCar(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                    }
                    #endregion
                }
                #endregion
            }
            #endregion
            #region 取個人資料
            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletInfo);
                SPInput_GetWalletInfo SPInput = new SPInput_GetWalletInfo()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_GetWalletInfo SPOutput = new SPOutput_GetWalletInfo();
                SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo> sqlHelp = new SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);

                #region 台新錢包

                if (flag)
                {
                    SPInput_DonePayRent PayInput = new SPInput_DonePayRent()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        OrderNo = tmpOrder,
                        Token = Access_Token,
                        transaction_no = ""
                    };
                    if (Amount > 0)
                    {
                        DateTime NowTime = DateTime.Now;
                        string guid = Guid.NewGuid().ToString().Replace("-", "");
                        int nowCount = 1;
                        WebAPI_PayTransaction wallet = new WebAPI_PayTransaction()
                        {
                            AccountId = SPOutput.WalletAccountID,
                            ApiVersion = "0.1.01",
                            GUID = guid,
                            MerchantId = MerchantId,
                            POSId = "",
                            SourceFrom = "9",
                            StoreId = "",
                            StoreName = "",
                            StoreTransId = string.Format("{0}F{1}", tmpOrder, NowTime.ToString("yyMMddHHmmss")),
                            Amount = Amount,
                            BarCode = "",
                            StoreTransDate = NowTime.ToString("yyyyMMddHHmmss")

                        };
                        var body = JsonConvert.SerializeObject(wallet);
                        TaishinWallet WalletAPI = new TaishinWallet();
                        string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                        string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                        WebAPIOutput_PayTransaction output = null;
                        flag = WalletAPI.DoPayTransaction(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                        
                        if (flag)
                        {
                            if (apiInput.PayType == 0)
                            {
                                if (OrderDataLists[0].ProjType == 4)
                                {
                                    bool Motorflag = new CarCommonFunc().DoCloseRent(tmpOrder, IDNO, LogID, Access_Token, ref errCode);
                                    if (Motorflag == false)
                                    {
                                        //寫入車機錯誤
                                    }
                                }
                                #region 這邊要再加上更新訂單狀態
                                SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                                PayInput.transaction_no = wallet.StoreTransId;
                                SPOutput_Base PayOutput = new SPOutput_Base();
                                SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                                flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                                baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        if (apiInput.PayType == 0) //直接更新租約狀態
                        {
                            #region 這邊要再加上更新訂單狀態
                            PayInput.transaction_no = "Free";
                            SPName = new ObjType().GetSPName(ObjType.SPType.DonePayRentBill);
                            SPOutput_Base PayOutput = new SPOutput_Base();
                            SQLHelper<SPInput_DonePayRent, SPOutput_Base> SQLPayHelp = new SQLHelper<SPInput_DonePayRent, SPOutput_Base>(connetStr);
                            flag = SQLPayHelp.ExecuteSPNonQuery(SPName, PayInput, ref PayOutput, ref lstError);
                            baseVerify.checkSQLResult(ref flag, ref PayOutput, ref lstError, ref errCode);
                            #endregion
                        }
                    }

                   


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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
