using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Configuration;
using System.Web;
using Domain.Common;
using WebAPI.Models.BaseFunc;
using WebCommon;
using WebAPI.Models.Param.Output.PartOfParam;
using Domain.SP.Input.Booking;
using Domain.SP.Output;
using Domain.SP.Output.Booking;
using System.Data;
using Newtonsoft.Json;
using System.Text;
using WebAPI.Models.Param.Output;
using System.Net.Http.Headers;

namespace WebAPI.Controllers
{
    public class MemberUnbindController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> YA_MemberUnbind(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"];
            var objOutput = new Dictionary<string, object>(); //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MemberUnbindController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            string Contentjson = "";
            string IDNO = "";

            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                //寫入 API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
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
            #endregion

            #region 透過api問錢包是否有餘額
            if (flag && isGuest == false)
            {
                HttpClient client = new HttpClient();
                var Data = new
                {
                    //啥都不用給，是丟Access_Token來判斷的             
                };
                var jsonData = JsonConvert.SerializeObject(Data);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContext.Request.Headers["Authorization"].Remove(0, 7));//第一個參數原本給
                HttpContent contentPost = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = new HttpResponseMessage();

                response = client.PostAsync("http://localhost:2061/api/CreditAndWalletQuery", contentPost).Result;
                var result = JsonConvert.DeserializeObject<OAPI_CreditAndWalletQuery>(response.Content.ReadAsStringAsync().Result);

                if (result.TotalAmount > 0)
                {
                    flag = false;
                    errCode = "ERR989";
                }
            }
            

            #endregion

            #region TB
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }

            //開始做取消預約
            if (flag)
            {
                SPInput_BookingCancel spInput = new SPInput_BookingCancel()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    Token = Access_Token
                };
                string SPName = "usp_MemberUnbind";
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_BookingCancel, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_BookingCancel, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, spInput, ref spOut, ref lstError);
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
