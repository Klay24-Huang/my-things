using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.OtherService.Taishin;
using Domain.SP.Input.Wallet;
using Domain.SP.Output;
using Domain.SP.Output.Wallet;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.Input.Taishin.GenerateCheckSum;
using Domain.WebAPI.Input.Taishin.Wallet;
using Domain.WebAPI.Input.Taishin.Wallet.Param;
using Domain.WebAPI.output.Taishin;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Service;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 錢包儲值-虛擬條碼
    /// </summary>
    public class WalletStoreVisualAccountController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string businessCode = ConfigurationManager.AppSettings["TaishinWalletVirtualAccountBusinessCod"].ToString(); //虛擬帳號企業代碼

        [HttpPost]
        public Dictionary<string, object> DoWalletStoreVisualAccount(Dictionary<string, object> value)
        {
            #region 初始宣告
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            var walletService = new WalletService();

            HttpContext httpContext = HttpContext.Current;
            var objOutput = new Dictionary<string, object>();    //輸出
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletStoreVisualAccountController";
            string PRGID = "221"; //APIId

            Int64 LogID = 0;
            Int16 ErrType = 0;

            IAPI_WalletStoreBase apiInput = null;
            var apiOutput = new OAPI_WalletStoreVisualAccount();

            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            DateTime payDeadLine = new DateTime();
            string virtualAccount = "";

            #endregion
            trace.traceAdd("apiIn", value);

            try
            {
                #region 防呆
                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

                flag = true;
                if (flag)
                {
                    //寫入API Log
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletStoreBase>(Contentjson);
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //必填檢查
                    if (apiInput.StoreMoney <= 0)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else if (apiInput.StoreMoney > 0 && apiInput.StoreMoney < 100)
                    {
                        flag = false;
                        errCode = "ERR284";
                    }


                    //不開放訪客
                    if (isGuest)
                    {
                        flag = false;
                        errCode = "ERR101";
                    }
                    trace.FlowList.Add("防呆");
                }
                #endregion

                #region Token判斷
                if (flag && isGuest == false)
                {
                    flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);

                }
                #endregion

                #region 儲值金額限制檢核
                if (flag)
                {
                    flag = walletService.CheckStoreAmtLimit(apiInput.StoreMoney,IDNO, LogID, Access_Token, ref flag, ref errCode);
                }
                #endregion

                #region TB
                if (flag)
                {
                    flag= CreateWalletStoreVisualAccount(apiInput,ref virtualAccount,ref payDeadLine, IDNO, LogID, Access_Token, ref flag, ref errCode);
                    trace.traceAdd("CreateVisualAccount", new { apiInput, virtualAccount,payDeadLine,errCode });
                    trace.FlowList.Add("產虛擬帳號");
                }

                if (flag)
                {
                    apiOutput = new OAPI_WalletStoreVisualAccount()
                    {
                        StoreMoney = apiInput.StoreMoney,
                        PayDeadline = string.Format("{0:yyyy/MM/dd 23:59}", payDeadLine),
                        VirtualAccount = $"(812){virtualAccount}"
                    };
                }
                #endregion

                apiOutput.StroeResult = flag ? 1 : 0;

                trace.traceAdd("TraceFinal", new { apiOutput, errCode, errMsg });
                carRepo.AddTraceLog(Convert.ToInt32(PRGID), funName, trace, flag);
            }
            catch (Exception ex)
            {
                flag = false;
                apiOutput.StroeResult = 0;
                trace.BaseMsg = ex.Message;
            }

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

        private bool CreateWalletStoreVisualAccount(IAPI_WalletStoreBase apiInput, ref string virtualAccount,ref DateTime payDeadLine,string IDNO, long LogID,string Access_Token,ref bool flag, ref string errCode) 
        {
            var wsp = new WalletSp();
            payDeadLine = DateTime.Now.AddDays(3); //+3天 繳費截止日
            virtualAccount = CreateVirtualAccountNum(payDeadLine, apiInput.StoreMoney);
            if (!string.IsNullOrWhiteSpace(virtualAccount))
            {
                #region 寫入台新虛擬帳號產生紀錄檔
                SPInput_InsWalletStoreVisualAccountLog spInput_Wallet = new SPInput_InsWalletStoreVisualAccountLog()
                {
                    TrnActNo = virtualAccount,
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID,
                    Amt = apiInput.StoreMoney.ToString(),
                    DueDate = payDeadLine
                };

                flag = wsp.sp_WalletStoreVisualAccount(spInput_Wallet, ref errCode);
               
                //若虛擬帳號重複，則重新產生
                while (flag == false && errCode== "ERR935")
                {
                   return CreateWalletStoreVisualAccount(apiInput, ref virtualAccount, ref payDeadLine, IDNO, LogID, Access_Token, ref flag, ref errCode);
                }
                #endregion
            }
            else
            {
                errCode = "ERR937";                   
            }
            return flag;
        }

        /// <summary>
        /// 產生虛擬帳號
        /// </summary>
        /// <param name="payDeadline"></param>
        /// <param name="storeMoney"></param>
        /// <returns></returns>
        private string CreateVirtualAccountNum(DateTime payDeadline, int storeMoney)
        {
            int value;
            Random rnd = new Random();
            string randomNum = rnd.Next(1, 99999).ToString().PadLeft(6, '0');
            string payDeadlineStr = $"{(payDeadline.Year - 1911).ToString().Substring(2, 1)}{payDeadline.DayOfYear.ToString()}";
            string strAccount = $"{businessCode}{randomNum}{payDeadlineStr}";          
            int[] accWeights = { 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3, 1, 3 };  // 權重 長度15
            int accSum = 0;
            for (int i = 0; i < accWeights.Length; i++)
            {
                value = (int)Char.GetNumericValue(strAccount[i]);
                accSum += (value * accWeights[i]);
            }

            int[] amtWeights = { 5, 4, 3, 2, 3, 4, 1 };  //權重 長度7
            int[] amtIntArray = GenerateAmtIntArr(storeMoney, 7);
            int amtSum = 0;
            for (int i = 0; i < amtWeights.Length; i++)
            {
                value = amtIntArray[i];
                amtSum += (value * amtWeights[i]);
            }

            int checkSum = GetCheckSum(accSum + amtSum);

            return $"{strAccount}{checkSum}";
        }

        /// <summary>
        /// 取得檢核碼
        /// </summary>
        /// <param name="sum"></param>
        /// <returns></returns>
        private int GetCheckSum(int sum)
        {
            int d = sum % 10;  // 個位數
            return (d == 0) ? 0 : 10 - d;
        }

        private int[] GenerateAmtIntArr(int amount, int totalWidth)
        {
            string strAmount = amount.ToString().PadLeft(totalWidth, '0');
            int[] intArray = new int[strAmount.Length];
            for (int i = 0; i < strAmount.Length; i++)
            {
                intArray[i] = (int)Char.GetNumericValue(strAmount[i]);
            }
            return intArray;
        }
    }
}
