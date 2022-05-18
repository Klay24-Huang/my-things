using Domain.Common;
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
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;
using WebAPI.Utils;
using Domain.Flow.Hotai;
using Domain.TB.Hotai;
using NLog;
using System.Security.Cryptography;
using WebAPI.Models.Param.Output.PartOfParam;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 查詢綁卡及錢包
    /// </summary>
    public class GetGameItemController : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private string ApiVer = ConfigurationManager.AppSettings["ApiVerOther"].ToString();
        private string EnKey = ConfigurationManager.AppSettings["GAMEAES128KEY"].ToString();
        private string EnSalt = ConfigurationManager.AppSettings["GAMEAES128IV"].ToString();
        [HttpPost]
        public Dictionary<string, object> DoGetGameItem(Dictionary<string, object> value)
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
            string funName = "GetGameItemController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            OAPI_GetGameItem apiOutput = null;
            GetGameItemP itemP = null;
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

            if (isGuest)
            {
                flag = false;
                errCode = "ERR101";
            }
            
            #endregion

            #region TB
            #region Token判斷
            //Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion
            #region 取GameToken
            if (flag)
            {
                DataSet ds = null;

                object[][] parms1 = {
                    new object[] {
                        IDNO,
                        LogID
                    }};

                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";
                string SPName = "usp_GetGameToken_Q01";

                ds = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, false, ref returnMessage, ref messageLevel, ref messageType);

                if (ds.Tables.Count == 0)
                {
                    flag = false;
                    errCode = "ERR999";
                    errMsg = returnMessage;
                }

                if (ds.Tables.Count > 0)
                {
                    itemP = new GetGameItemP
                    {
                        GameToken = ds.Tables[0].Rows[0]["GameToken"].ToString(),
                        GameID = (int)ds.Tables[0].Rows[0]["GameID"]
                    };
                    apiOutput = new OAPI_GetGameItem()
                    {
                        GameSrc = ds.Tables[0].Rows[0]["GameUrl"].ToString(), // 路徑在TB_Game
                        P = AESEncrypt.EncryptAES128(JsonConvert.SerializeObject(itemP), EnKey, EnSalt, CipherMode.ECB, PaddingMode.PKCS7)
                };

                }
                ds.Dispose();
            }
            #endregion

            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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