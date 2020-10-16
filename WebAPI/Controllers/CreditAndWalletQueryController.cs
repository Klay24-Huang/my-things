﻿using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// 查詢綁卡及錢包
    /// </summary>
    public class CreditAndWalletQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        private string ApiVer = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private string APIToken = ConfigurationManager.AppSettings["TaishinWalletAPIToken"].ToString();
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoCreditAndWalletQuery(Dictionary<string, object> value)
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
            string funName = "CreditAndWalletQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CreditAndWalletQuery apiInput = null;
            OAPI_CreditAndWalletQuery apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest, false);

            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Access_Token, ClientIP, funName, ref errCode, ref LogID);
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
            #region 送台新查詢
            if (flag)
            {
                TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
                PartOfGetCreditCardList wsInput = new PartOfGetCreditCardList()
                {
                    ApiVer = ApiVer,
                    ApposId = TaishinAPPOS,
                    RequestParams = new GetCreditCardListRequestParamasData()
                    {
                        MemberId = IDNO,
                    },
                    Random = baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                    TimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString(),
                    TransNo = string.Format("{0}_{1}", IDNO, DateTime.Now.ToString("yyyyMMddhhmmss"))

                };
                WebAPIOutput_GetCreditCardList wsOutput = new WebAPIOutput_GetCreditCardList();
                flag = WebAPI.DoGetCreditCardList(wsInput, ref errCode, ref wsOutput);
                if (flag)
                {
                    int Len = wsOutput.ResponseParams.ResultData.Count;
                    apiOutput = new OAPI_CreditAndWalletQuery()
                    {
                        HasBind = (Len == 0) ? 0 : 1,
                        BindListObj = new List<Models.Param.Output.PartOfParam.CreditCardBindList>()
                    };
                    for (int i = 0; i < Len; i++)
                    {
                        Models.Param.Output.PartOfParam.CreditCardBindList obj = new Models.Param.Output.PartOfParam.CreditCardBindList()
                        {
                            AvailableAmount = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].AvailableAmount),
                            BankNo = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].BankNo),
                            CardName = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardName),
                            CardNumber = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardNumber),

                            CardToken = baseVerify.BaseCheckString(wsOutput.ResponseParams.ResultData[i].CardToken)

                        };
                        apiOutput.BindListObj.Add(obj);
                    }
                }


            }
            #region 台新錢包
            if (flag)
            {
                #region 取個人資料
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
                #endregion
                TaishinWallet WalletAPI = new TaishinWallet();
                DateTime NowTime = DateTime.UtcNow;
                string guid = Guid.NewGuid().ToString().Replace("-", "");
                int nowCount = 1;
                WebAPI_GetAccountStatus walletStatus = new WebAPI_GetAccountStatus()
                {
                    AccountId = SPOutput.WalletAccountID,
                    ApiVersion = "0.1.01",
                    GUID = guid,
                    MerchantId = MerchantId,
                    POSId = "",
                    SourceFrom = "9",
                    StoreId = "",
                    StoreName = ""


                };
                var body = JsonConvert.SerializeObject(walletStatus);
                WebAPIOutput_GetAccountStatus statusOutput = null;
                string utcTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string SignCode = WalletAPI.GenerateSignCode(MerchantId, utcTimeStamp, body, APIKey);

                flag = WalletAPI.DoGetAccountStatus(walletStatus, MerchantId, utcTimeStamp, SignCode, ref errCode, ref statusOutput);
                if (flag)
                {
                    if (statusOutput.ReturnCode == "0000")
                    {
                        apiOutput.TotalAmount = statusOutput.Result.Amount;
                    }
                    if (statusOutput.Result.Status == "2")
                    {
                        apiOutput.HasWallet = 1;
                    }
                }

            }
            #endregion

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