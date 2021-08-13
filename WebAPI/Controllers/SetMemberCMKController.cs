using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Member;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

/*
 * 20210812 ADD BY YEH 新增程式
 */

namespace WebAPI.Controllers
{
    /// <summary>
    /// 更新會員條款
    /// </summary>
    public class SetMemberCMKController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoSetMemberCMK(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"];    //Bearer
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "SetMemberCMKController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_SetMemberCMK apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = false;
            string IDNO = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_SetMemberCMK>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (string.IsNullOrEmpty(apiInput.CHKStatus))
                {
                    flag = false;
                    errCode = "ERR900";
                }
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

            #region TB
            // Token判斷
            if (flag)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            if (flag)
            {
                if (apiInput.CHKStatus == "Y")
                {
                    string spName = new ObjType().GetSPName(ObjType.SPType.SetMemberCMK);
                    SPInput_SetMemberCMK spInput = new SPInput_SetMemberCMK()
                    {
                        IDNO = IDNO,
                        Source = "I",
                        AgreeDate = DateTime.Now,
                        VerType = "Hims",
                        Version = "",   // 從DB抓
                        TEL = "Y",      // 預設Y
                        SMS = "Y",      // 預設Y
                        EMAIL = "Y",    // 預設Y
                        POST = "Y",     // 預設Y
                        APIName = funName,
                        LogID = LogID
                    };
                    SPOutput_Base spOut = new SPOutput_Base();
                    SQLHelper<SPInput_SetMemberCMK, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_SetMemberCMK, SPOutput_Base>(connetStr);
                    List<MemberCMKList> ListOut = new List<MemberCMKList>();
                    DataSet ds = new DataSet();
                    flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref ListOut, ref ds, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                    if (flag)
                    {
                        // 拋短租
                    }
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
