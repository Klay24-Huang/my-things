using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Station;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 設定常用站點
    /// </summary>
    public class SetFavoriteStationController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> doGetNormalRent(Dictionary<string, object> value)
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
            string funName = "SetFavoriteStationController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_SetFavoriteStation apiInput = null;
            OAPI_Base SetFavoriteStationAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_SetFavoriteStation>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (flag)
                {
                    if(string.IsNullOrWhiteSpace(apiInput.StationID) || string.IsNullOrEmpty(apiInput.StationID))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
                if (flag)
                {
                    if(apiInput.Mode<0 || apiInput.Mode > 1)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
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
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag,spOut.Error,spOut.ErrorCode, ref lstError, ref errCode);
        
                if (flag)
                {
                  string SetFavoriteName = new ObjType().GetSPName(ObjType.SPType.SetFavoriteStation);
                  SPInput_InsFavoriteStation spInput = new SPInput_InsFavoriteStation()
                  {
                    IDNO = spOut.IDNO,
                    LogID = LogID,
                    Mode = apiInput.Mode,
                    StationID = apiInput.StationID
                  };
                    SPOutput_Base spBaseOut = new SPOutput_Base();
                    SQLHelper<SPInput_InsFavoriteStation, SPOutput_Base> sqlInsHelp = new SQLHelper<SPInput_InsFavoriteStation, SPOutput_Base>(connetStr);
                    flag = sqlInsHelp.ExecuteSPNonQuery(SetFavoriteName, spInput, ref spBaseOut, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref spBaseOut, ref lstError, ref errCode);


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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, SetFavoriteStationAPI, token);
            return objOutput;
            #endregion
        }
    }
}
