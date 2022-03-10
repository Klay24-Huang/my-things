using Domain.Common;
using Domain.MemberData;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 查詢會員資料
    /// </summary>
    public class GetMemberDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> doGetMemberData(Dictionary<string, object> value)
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
            string funName = "GetMemberDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
      
            OAPI_GetMemberData outputAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            MemberRepository _repository;
            string IDNO = "";
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest,false);
            if (flag)
            {
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog("not need input", ClientIP, funName, ref errCode, ref LogID);
            }
            #endregion
            #region 不支援訪客
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
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
            }
            if (flag)
            {
                //20201022 ADD BY ADAM REASON.改為SP呼叫
                string spName = "usp_GetMemberData";
                SPInput_GetMemberData spMemberDataInput = new SPInput_GetMemberData()
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOutput_Base SPOutputBase = new SPOutput_Base();
                SQLHelper<SPInput_GetMemberData, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMemberData, SPOutput_Base>(connetStr);
                List<RegisterData> lstOut = new List<RegisterData>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spMemberDataInput, ref SPOutputBase, ref lstOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    outputAPI = new OAPI_GetMemberData()
                    {
                        UserData = (lstOut == null) ? null : (lstOut.Count == 0) ? null : lstOut[0]
                    };
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputAPI, token);
            return objOutput;
            #endregion
        }
    }
}