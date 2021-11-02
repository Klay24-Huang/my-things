using Domain.Common;
using Domain.SP.Input.Wallet;
using Domain.SP.Output.Wallet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GiftTransferCheckController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGiftTransferCheck(Dictionary<string, object> value)
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
            string funName = "GiftTransferCheckController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GiftTransferCheck apiInput = null;
            OAPI_GiftTransferCheck outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GiftTransferCheck>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion

            #region TB
            //Token判斷
            if (flag)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }

            if (flag)
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetMemberInfo);
                SPInput_GetMemberName SPInput = new SPInput_GetMemberName()
                {
                    LoginIDNO = IDNO,
                    DonateIDNO = apiInput.IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOutput_GetMemberName SPOutput = new SPOutput_GetMemberName();
                SQLHelper<SPInput_GetMemberName, SPOutput_GetMemberName> sqlHelp = new SQLHelper<SPInput_GetMemberName, SPOutput_GetMemberName>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                if (string.IsNullOrEmpty(SPOutput.Name))
                {
                    flag = false;
                    errCode = "ERR201";
                }

                if (flag)
                {
                    outputApi = new OAPI_GiftTransferCheck
                    {
                        Name = SPOutput.Name,
                        PhoneNo = SPOutput.PhoneNo,
                        Amount = apiInput.Amount
                    };
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}