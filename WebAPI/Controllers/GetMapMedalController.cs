using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Member;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Member;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class GetMapMedalController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetMapMedal()
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"];    //Bearer 
            var objOutput = new Dictionary<string, object>();   //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success";  //預設成功
            string errCode = "000000";  //預設成功
            string funName = "GetMapMedalController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            OAPI_GetMapMedal outputApi = new OAPI_GetMapMedal();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
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
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
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
                string spName = new ObjType().GetSPName(ObjType.SPType.GetMapMedal);
                SPInput_GetMapMedal spInput = new SPInput_GetMapMedal()
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetMapMedal, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMapMedal, SPOutput_Base>(connetStr);
                List<MapMedalList> MedalList = new List<MapMedalList>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spInput, ref spOut, ref MedalList, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    outputApi = new OAPI_GetMapMedal
                    {
                        MedalList = MedalList
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