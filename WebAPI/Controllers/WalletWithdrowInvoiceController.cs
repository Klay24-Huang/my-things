using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebCommon;
using WebAPI.Service;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 車機綁定
    /// </summary>
    public class WalletWithdrowInvoiceController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> WalletWithdrowInvoice(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "WalletWithdrowInvoice";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_WalletWithdrowInvoice apiInput = null;
            OAPI_WalletWithdrowInvoice apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            //Int16 APPKind = 2;
            string Contentjson = "";

            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_WalletWithdrowInvoice>(Contentjson);
           
            var inData = apiInput;

            //寫入API Log
            string ClientIP = baseVerify.GetClientIp(Request);
            flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            string[] checkList = { inData.SEQNO+"", inData.INV_NO, inData.INV_DATE, inData.RNDCODE };
            string[] errList = { "ERR900" };
            //1.判斷必填
            flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
          
            #endregion

            #region TB
            if (flag)
            {
                var spIn = new SPInput_WalletWithdrowInvoice()
                {
                    SEQNO = inData.SEQNO,
                    INV_NO = inData.INV_NO,
                    INV_DATE = inData.INV_DATE,
                    RNDCODE = inData.RNDCODE,
                    LogID = LogID
                };

                var wsp = new WalletSp();
                var spOut = wsp.sp_WalletWithdrowInvoice(spIn,ref errCode);
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