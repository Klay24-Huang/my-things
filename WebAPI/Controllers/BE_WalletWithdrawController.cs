using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_WalletWithdrawController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string APIKey = ConfigurationManager.AppSettings["TaishinWalletAPIKey"].ToString();
        private string MerchantId = ConfigurationManager.AppSettings["TaishiWalletMerchantId"].ToString();
        private CommonFunc baseVerify { get; set; }
        /// <summary>
        /// 【後台】和雲錢包餘額提領
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public Dictionary<string, object> doBE_WalletWithdraw(Dictionary<string, object> value)
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
            string funName = "BE_WalletWithdrawController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_WalletWithdraw apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            string Contentjson = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_WalletWithdraw>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.IDNO, apiInput.cashAmount };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                #endregion
            }

            #region TB
            if (flag)
            {
                flag = WithdrawWalletFlow(0, int.Parse(apiInput.cashAmount), apiInput.IDNO, "Withdraw", funName, LogID, Access_Token, ref errCode).flag;
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

        private (bool flag, BE_UserWalletInfo wallet) WithdrawWalletFlow(long OrderNo, int Amount, string IDNO, string TradeType, string funName, long LogID, string Access_Token, ref string errCode)
        {
           
            (bool flag, BE_UserWalletInfo wallet) result = (false, new BE_UserWalletInfo());

            WalletRepository repository = new WalletRepository(connetStr);
            BE_UserWalletInfo wallet = new BE_UserWalletInfo();
            wallet = (repository.GetUserWallet(IDNO).Count > 0) ? repository.GetUserWallet(IDNO)[0] : null;

            if (wallet == null)
            {
                //未申請錢包
                errCode = "ERR932";
                return result;
            }

            //提領金額
            int WithdrawAmount = 0;

            //錢包剩餘儲值金額>=提領金額
            if (wallet.StoreAmount >= Amount)
            {
                WithdrawAmount = Amount;
            }
            else //錢包剩餘儲值金額<提領金額
            {
                //餘額不足
                errCode = "ERR934";
                return result;
            }


            //扣款
            return DoWalletPay(WithdrawAmount, IDNO, OrderNo, TradeType, funName, LogID, Access_Token, ref errCode);


        }
        /// <summary>
        /// 錢包扣款
        /// </summary>
        /// <param name="Amount">扣款金額</param>
        /// <param name="IDNO">扣款帳號</param>
        /// <param name="OrderNo">扣款訂單編號</param>
        /// <returns></returns>
        private (bool flag, BE_UserWalletInfo wallet) DoWalletPay(int Amount, string IDNO, long OrderNo, string TradeType, string PRGName, long LogID, string Access_Token, ref string errCode)
        {
            (bool flag, BE_UserWalletInfo wallet) result = (false, new BE_UserWalletInfo());

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
            }
            else
            {
                errCode = "ERR933";//扣款失敗
            }
            return result;
        }


        private string GetWalletAccountId(string IDNO, int cnt)
        {
            return $"{IDNO}Wallet{cnt.ToString().PadLeft(4, '0')}";
        }

        private bool IsWalletPayAmountEnough(int payAmount, int baseAmount)
        {
            if (baseAmount == 0)
                return (payAmount > baseAmount);
            return payAmount >= baseAmount;
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
                Mode = 4,
                InputSource = 2
            };
        }
    }
}