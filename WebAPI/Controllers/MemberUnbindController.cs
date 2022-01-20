using Domain.Common;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Domain.SP.Output.Hotai;
using Domain.WebAPI.Input.Hotai.Member;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 會員解綁
    /// </summary>
    public class MemberUnbindController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoMemberUnbind(Dictionary<string, object> value)
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
            flag = baseVerify.baseCheck(ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("No Input", ClientIP, funName, ref errCode, ref LogID);
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
            #region Token
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            #endregion

            #region 判斷是否可以解綁
            if (flag)
            {
                string spName = "usp_MemberUnBindCheck";
                SPInput_MemberUnBindCheck spInput = new SPInput_MemberUnBindCheck
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_MemberUnBindCheck, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_MemberUnBindCheck, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #region 檢查欠費
            if (flag)
            {
                int TAMT = 0;
                Models.ComboFunc.ContactComm contract = new Models.ComboFunc.ContactComm();
                flag = contract.CheckNPR330(IDNO, LogID, ref TAMT);
                if (TAMT > 0)
                {
                    flag = false;
                    errCode = "ERR988";
                }
            }
            #endregion
            #endregion

            #region 解綁
            if (flag)
            {
                string spName = "usp_MemberUnbind";
                SPInput_MemberUnBind spInput = new SPInput_MemberUnBind()
                {
                    IDNO = IDNO,
                    LogID = LogID,
                    APIName = funName,
                    Token = Access_Token
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_MemberUnBind, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_MemberUnBind, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            }
            #endregion

            #region 和泰會員解綁
            if (flag)
            {
                string spName = "usp_HotaiMemberUnBind_U01";
                SPInput_MemberUnBind spInput = new SPInput_MemberUnBind()
                {
                    IDNO = IDNO,
                    APIName = funName
                };
                SPOutput_MemberUnBind spOut = new SPOutput_MemberUnBind();
                SQLHelper<SPInput_MemberUnBind, SPOutput_MemberUnBind> sqlHelp = new SQLHelper<SPInput_MemberUnBind, SPOutput_MemberUnBind>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    if (spOut != null)
                    {
                        var SignErrCode = "";
                        HotaiMemberAPI api = new HotaiMemberAPI();
                        WebAPIInput_SignOut signOut = new WebAPIInput_SignOut()
                        {
                            refresh_token = spOut.RefreshToken
                        };
                        var SignFlag = api.DoSignOut(spOut.AccessToken, signOut, ref SignErrCode);
                    }
                }
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