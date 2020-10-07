using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包轉贈
    /// </summary>
    public class WalletTransferStoredValueController : ApiController
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
            string funName = "WalletTransferStoredValueController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletTransferStoredValue apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            Int64 tmpOrder = 0;
            #endregion
            #region 防呆

            //flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                //寫入API Log
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletTransferStoredValue>(Contentjson);
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (apiInput.Amount <= 0 )
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
                    if (string.IsNullOrWhiteSpace(apiInput.TransID))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else
                    {
                        flag = baseVerify.checkIDNO(apiInput.TransID);
                        if (flag==false) {
                            errCode = "ERR103";
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

                SPName = new ObjType().GetSPName(ObjType.SPType.GetWalletInfoByTrans);
                SPInput_GetWalletInfo SPTransInput = new SPInput_GetWalletInfo()
                {
                    IDNO = apiInput.TransID,
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_GetWalletInfo SPTransOutput = new SPOutput_GetWalletInfo();
                sqlHelp = new SQLHelper<SPInput_GetWalletInfo, SPOutput_GetWalletInfo>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPTransInput, ref SPTransOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPTransOutput.Error, SPTransOutput.ErrorCode, ref lstError, ref errCode);
                if (SPTransOutput.Name == "" || SPTransOutput.PhoneNo=="" || SPTransOutput.Email=="")
                {
                    flag = false;
                    errCode = "ERR201";
                }


                #region 台新錢包

                if (flag)
                {
                    DateTime NowTime = DateTime.Now;
                    string guid = Guid.NewGuid().ToString().Replace("-", "");
                    int nowCount = 1;
                    WebAPI_TransferStoredValueCreateAccount wallet = new WebAPI_TransferStoredValueCreateAccount()
                    {
                        AccountId = SPOutput.WalletAccountID,
                        ApiVersion = "0.1.01",
                        GUID = guid,
                        MerchantId = MerchantId,
                        POSId = "",
                        SourceFrom = "2",
                        StoreId = "",
                        StoreName = "",
                        StoreTransId = string.Format("{0}S{1}", IDNO.Substring(0,9), NowTime.ToString("MMddHHmmss")),
                        Amount = apiInput.Amount,
                        BarCode = "",
                        StoreTransDate = NowTime.ToString("yyyyMMddHHmmss"),
                        Email = SPOutput.Email,
                        Name = SPOutput.Name,
                        ID = IDNO,
                        PhoneNo = SPOutput.PhoneNo,
                        TransMemo = string.Format("由{0}轉贈", SPOutput.Name),
                        AccountData = new List<Domain.WebAPI.Input.Taishin.Wallet.Param.AccountData>()



                    };
                    Domain.WebAPI.Input.Taishin.Wallet.Param.AccountData obj = new Domain.WebAPI.Input.Taishin.Wallet.Param.AccountData()
                    {
                        TransferAccountId = string.Format("{0}Wallet{1}", apiInput.TransID, nowCount.ToString().PadLeft(4, '0')),
                        TransferEmail = SPTransOutput.Email,
                        TransferID = apiInput.TransID,
                        TransferName = SPTransOutput.Name,
                        TransferPhoneNo = SPTransOutput.PhoneNo
                    };
                    wallet.AccountData.Add(obj);
                    var body = JsonConvert.SerializeObject(wallet);
                    TaishinWallet WalletAPI = new TaishinWallet();
                    string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                    string SignCode = WalletAPI.GenerateSignCode(wallet.MerchantId, utcTimeStamp, body, APIKey);
                    WebAPIOutput_TransferStoreValueCreateAccount output = null;
                    flag = WalletAPI.DoTransferStoreValueCreateAccount(wallet, MerchantId, utcTimeStamp, SignCode, ref errCode, ref output);
                    if (flag==false)
                    {
                        errCode = "ERR";
                        errMsg = output.Message;
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
