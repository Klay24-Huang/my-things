using Domain.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetPayInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetPayInfo(Dictionary<string, object> value)
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
            string funName = "GetPayInfoController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CreditAndWalletQuery apiInput = null;
            OAPI_CreditAndWalletQuery apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

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
                    errCode = "ERR150";
                }
            }
            #endregion
            #region TB
            //Token判斷
            if (flag)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
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

        private (bool status, string errorCode, Int64 orderNo) OrderNoFormatVerify(string OrderNo)
        {
            (bool status, string errorCode, Int64 orderNo) result = (true, "", 0);
            result.errorCode = "000000";
            if (string.IsNullOrWhiteSpace(OrderNo))
            {
                result.status = false;
                result.errorCode = "ERR900";
            }
            else
            {
                if (OrderNo.IndexOf("H") < 0)
                {
                    result.status = false;
                    result.errorCode = "ERR902";
                }
                if (result.status)
                {
                    if (Int64.TryParse(OrderNo.Replace("H", ""), out Int64 tmpOrder))
                    {
                        if (tmpOrder <= 0)
                        {
                            result.status = false;
                            result.errorCode = "ERR902";
                        }
                        else
                        {
                            result.orderNo = tmpOrder;
                        }
                    }
                }
            }
            return result;
        }
    }
}