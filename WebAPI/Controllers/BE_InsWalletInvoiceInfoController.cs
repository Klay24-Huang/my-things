using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_InsWalletInvoiceInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】儲存和雲錢包發票資訊
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        ///
        [HttpPost]
        public Dictionary<string, object> doBE_InsWalletInvoiceInfo(Dictionary<string, object> value)
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
            string funName = "BE_InsWalletInvoiceInfoController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_InsWalletInvoiceInfo apiInput = null;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_InsWalletInvoiceInfo>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.IDNO, apiInput.cashAmount };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
            }
            #endregion

            #region TB
            if (flag)
            {

                string spName = "usp_BE_WalletWithdraw_I01";
                SPInput_BE_InsWalletInvoiceInfo spInput = new SPInput_BE_InsWalletInvoiceInfo()
                {
                    UserID = apiInput.UserID,
                    IDNO = apiInput.IDNO,
                    Amount = int.Parse(apiInput.cashAmount),
                    Tax = apiInput.tax,
                    HandleFee = apiInput.handleFee,
                    InvoiceType = int.Parse(apiInput.invoiceMode),
                    CustID = apiInput.CustID,
                    Carrier = apiInput.carrier,
                    NPOBAN = apiInput.NPOBAN,
                    RVACNT = apiInput.RVACNT,
                    RVBANK = apiInput.RVBANk,
                    RV_NAME = apiInput.RV_NAME,
                    LogID = LogID,
                    PRGName = funName
                };
                SPOutput_BE_InsWalletInvoiceInfo spOut = new SPOutput_BE_InsWalletInvoiceInfo();
                SQLHelper<SPInput_BE_InsWalletInvoiceInfo, SPOutput_BE_InsWalletInvoiceInfo> sqlHelp = new SQLHelper<SPInput_BE_InsWalletInvoiceInfo, SPOutput_BE_InsWalletInvoiceInfo>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

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