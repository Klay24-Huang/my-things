using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.output.Taishin.Wallet;
using Newtonsoft.Json;
using OtherService;
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
        public void doBE_WalletWithdraw(Dictionary<string, object> value)
        {
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
            IAPI_BE_GetOrderModifyInfo apiInput = null;
            OAPI_BE_GetOrderModifyInfo apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

        }

        private (bool flag, SPInput_WalletPay paymentInfo) WithdrawWalletFlow(long OrderNo, int Amount, string IDNO, string TradeType, string funName, long LogID, string Access_Token, ref string errCode)
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
                Mode = GetWalletHistoryMode(TradeType)
            };

        }
    }
}