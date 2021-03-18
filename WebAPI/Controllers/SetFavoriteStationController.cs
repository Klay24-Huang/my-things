using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Utils;
using Domain.SP.Output;
using System.CodeDom;
using Domain.SP.Input.Arrears;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Station;

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
            string IDNO = "";
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
                    if(apiInput == null || apiInput.FavoriteStations == null || apiInput.FavoriteStations.Count() == 0)
                    {
                        flag = false;
                        errCode = "ERR250";
                    }

                    if (flag)
                    {
                        var modes = new List<Int16>() { 0, 1 };
                        var ckSour = apiInput.FavoriteStations;
                        if (ckSour.Any(x => string.IsNullOrWhiteSpace(x.StationID)||!modes.Any(y=>y == x.Mode)))
                        {
                            flag = false;
                            errCode = "ERR251";
                        }
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
                    IDNO = spOut.IDNO;
                }

                if (flag)
                {
                    var spIn = new SPInput_InsFavoriteStation() { IDNO = IDNO, LogID = LogID };
                    FavoriteStation[] apiList = apiInput.FavoriteStations.ToArray();
                    var sp_re = sp_SetFavoriteStation(apiList, spIn, ref errMsg);
                    if (sp_re != null)
                    {
                        flag = sp_re.Error == 0;
                        if(sp_re.Error != 0 && sp_re.ErrorCode != "0000")
                        {
                            errCode = sp_re.ErrorCode;
                            errMsg = sp_re.ErrorMsg;
                        }
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR252";
                    }
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

        private SPOutput_Base sp_SetFavoriteStation(FavoriteStation[] apiList, SPInput_InsFavoriteStation spInput, ref string errMsg)
        {
            var re = new SPOutput_Base();

            try
            {
                string SPName = new ObjType().GetSPName(ObjType.SPType.SetFavoriteStation);
                int apiLen = apiList.Length;
                object[] objparms = new object[apiLen == 0 ? 1 : apiLen];
                if (apiLen > 0)
                {
                    for (int i = 0; i < apiLen; i++)
                    {
                        objparms[i] = new
                        {
                            StationID = apiList[i].StationID,
                            Mode = apiList[i].Mode
                        };
                    }
                }
                else
                {
                    objparms[0] = new
                    {
                        StationID = "",
                        Mode = 0     
                    };
                }

                object[][] parms1 = {
                    new object[] {
                        spInput.IDNO,
                        spInput.LogID
                    },
                    objparms
                };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), SPName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                   re = objUti.GetFirstRow<SPOutput_Base>(ds1.Tables[0]);
                else
                    errMsg = returnMessage;
            }
            catch (Exception ex)
            {
                errMsg = ex.ToString();
                throw ex;
            }

            return re;
        }

    }
}
