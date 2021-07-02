using Domain.Common;
using Domain.SP.Input.Mochi;
using Domain.SP.Output;
using Domain.SP.Output.Mochi;
using Domain.TB.Mochi;
using Domain.WebAPI.output.Mochi;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得車麻吉Token
    /// </summary>
    public class GetMachiParkTokenController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> GetMachiParkToken(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMachiParkTokenController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetMachiParkToken apiInput = null;
            NullOutput outputApi = new NullOutput();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            ParkingRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            
            string MochiToken = ""; //車麻吉token
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMachiParkToken>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrWhiteSpace(apiInput.sig) || string.IsNullOrWhiteSpace(apiInput.user_id))
                {
                    flag = false;
                    errCode = "ERR900";
                }
                else if (apiInput.sig != GetVerifySigCode(apiInput.user_id))
                {
                    flag = false;
                    errCode = "ERR900";
                }

            }
            #endregion
            #region TB
            if (flag)
            {
                MochiParkAPI webAPI = new MochiParkAPI();
                DateTime NowDate = DateTime.Now;
                SPInput_GetMachiToken spIGetToken = new SPInput_GetMachiToken()
                {
                    NowTime = DateTime.Now.ToString("yyyyMMddHHmmss"),
                    LogID = LogID
                };
                SPOutput_GetMachiToken spOGetToken = new SPOutput_GetMachiToken();
                SQLHelper<SPInput_GetMachiToken, SPOutput_GetMachiToken> sqlGetHelp = new SQLHelper<SPInput_GetMachiToken, SPOutput_GetMachiToken>(connetStr);
                string spName = new ObjType().GetSPName(ObjType.SPType.GetMochiToken);

                flag = sqlGetHelp.ExecuteSPNonQuery(spName, spIGetToken, ref spOGetToken, ref lstError);
                if (spOGetToken.Token == "")
                {
                    WebAPIOutput_MochiLogin wsOutLogin = new WebAPIOutput_MochiLogin();
                    flag = webAPI.DoLogin(ref wsOutLogin);
                    if (flag && wsOutLogin.data.access_token != "")
                    {
                        long second = wsOutLogin.data.expires_in;
                        DateTime TokenEnd = NowDate.AddSeconds(second);

                        SPInput_MaintainMachiToken spMaintain = new SPInput_MaintainMachiToken()
                        {
                            Token = wsOutLogin.data.access_token,
                            StartDate = NowDate,
                            EndDate = TokenEnd,
                            LogID = LogID
                        };
                        SPOutput_Base spMainOut = new SPOutput_Base();
                        SQLHelper<SPInput_MaintainMachiToken, SPOutput_Base> sqlMainHelp = new SQLHelper<SPInput_MaintainMachiToken, SPOutput_Base>(connetStr);
                        spName = new ObjType().GetSPName(ObjType.SPType.MaintainMachiToken);
                        flag = sqlMainHelp.ExecuteSPNonQuery(spName, spMaintain, ref spMainOut, ref lstError);
                        if (flag)
                        {
                            MochiToken = wsOutLogin.data.access_token;
                        }
                    }
                }
                else
                {
                    MochiToken = spOGetToken.Token;
                }
            }
            #endregion
            #region 寫入錯誤Log
            //if (flag == false && isWriteError == false)
            //{
            //    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            //}
            #endregion
            #region 輸出
            //baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            objOutput.Add("Result", flag);
            objOutput.Add("Message", (!flag) ? "查詢Token失敗;ErrCode=" + errCode : "");
            objOutput.Add("Data", new object[] { 
                new {
                    token = MochiToken,
                    EndDate = ""
                }
            });
            return objOutput;
            #endregion
        }

        private string GetVerifySigCode(string user_id)
        {
            string apikey = "xTzTbNHsox+KD6rirQQgog==";
            string source = apikey + user_id + DateTime.Now.ToString("yyyyMMdd");
            ASCIIEncoding enc = new ASCIIEncoding();
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] shaHash = sha.ComputeHash(enc.GetBytes(source));
            StringBuilder sig = new StringBuilder();
            foreach (byte b in shaHash)
            {

                sig.Append(b.ToString("X2"));

            }
            return sig.ToString();
        }
    }
}