using Domain.Common;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_GiveGiftMinsController : ApiController
    {
        [HttpPost]
        public Dictionary<string, object> DoBE_GiveGiftMins(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            //bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_GiveGiftMins";
            Int64 LogID = 0;
            //Int16 ErrType = 0;
            IAPI_BE_GiveGiftMins apiInput = null;
            //OAPI_BE_IrentPaymentDetail apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            //Int16 APPKind = 2;
            string Contentjson = "";

            //iRentService認證
            string EncryptStr = "";
            string sourceStr = ConfigurationManager.AppSettings["HLCkey"] + ConfigurationManager.AppSettings["userid"] + System.DateTime.Now.ToString("yyyyMMdd");
            ASCIIEncoding enc = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] shaHash = sha.ComputeHash(enc.GetBytes(sourceStr));
            EncryptStr = System.BitConverter.ToString(shaHash).Replace("-", string.Empty);

            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_GiveGiftMins>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.MEMRFNBR };
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
            }
            #endregion


            #region 這邊資料用api拋給sqyhi01vm


            HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
            WebAPIOutput_NPR388Save wsOutput = new WebAPIOutput_NPR388Save();
            WebAPIInput_NPR388Save spInput = new WebAPIInput_NPR388Save()
            {
                MEMRFNBR = apiInput.MEMRFNBR,
                GIFTMINS = apiInput.GIFTMINS,
                GIFTTYPE = apiInput.GIFTTYPE,
                COUPONNAME = apiInput.COUPONNAME,
                SDATE = apiInput.SDATE,
                EDATE = apiInput.EDATE,
                sig = EncryptStr,
                user_id = ConfigurationManager.AppSettings["userid"]
            };
            flag = hiEasyRentAPI.NPR388Save(spInput, ref wsOutput);

            //輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, wsOutput, token);
            return objOutput;

            #endregion

        }
    }
}