using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.TB;
using Reposotory.Implement;
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
    /// <summary>
    /// 路邊租還
    /// </summary>
    public class AnyRentController : ApiController
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
            string funName = "AnyRentController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_AnyRent apiInput = null;
            OAPI_AnyRent OAnyRentAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_AnyRent>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    flag = apiInput.ShowALL.HasValue;
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                    if (flag)
                    {
                        if (apiInput.ShowALL.Value == 0)
                        {
                            if (!apiInput.Latitude.HasValue || !apiInput.Longitude.HasValue || !apiInput.Radius.HasValue)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                        }
                    }
                }
            }
            #endregion

            #region TB
            //Token判斷
            //20201109 ADD BY ADAM REASON.TOKEN判斷修改
            //if (flag && isGuest == false)
            if(flag && Access_Token_string.Split(' ').Length >= 2)
            {
                //string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenOnlyToken);
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    //Token = Access_Token
                    Token = Access_Token_string.Split(' ')[1].ToString()
                };
                //SPOutput_Base spOut = new SPOutput_Base();
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                //SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base>(connetStr);
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                //baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                //訪客機制BYPASS
                if (spOut.ErrorCode=="ERR101")
                {
                    flag = true;
                    spOut.ErrorCode = "";
                    spOut.Error = 0;
                    errCode = "000000";
                }
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);
                List<AnyRentObj> AllCars = new List<AnyRentObj>();
                if (apiInput.ShowALL == 1)
                {
                    //AllCars = _repository.GetAllAnyRent();
                    AllCars = _repository.GetAllAnyRent(IDNO);
                }
                else
                {
                    //AllCars = _repository.GetAllAnyRent(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                    AllCars = _repository.GetAllAnyRent(IDNO, apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                }

                OAnyRentAPI = new OAPI_AnyRent()
                {
                    AnyRentObj = AllCars
                };
            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, OAnyRentAPI, token);
            return objOutput;
            #endregion
        }
    }
}