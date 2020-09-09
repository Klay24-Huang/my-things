using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output;
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
    /// 取得專案及資費(未完成)
    /// </summary>
    public class GetProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoGetProject(Dictionary<string, object> value)
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
            string funName = "GetProjectController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetProject apiInput = null;
            OAPI_GetProject outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate = DateTime.Now;
            int QueryMode = 0;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetProject>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    flag = apiInput.Mode.HasValue;
                    if (false == flag)
                    {
                        errCode = "ERR900";
                    }
                    else
                    {
                        
                        QueryMode = (apiInput.Mode.Value>0)?1:0;
                    }
                    if (flag)
                    {
                        if (QueryMode == 1)
                        {
                            if (!apiInput.Latitude.HasValue || !apiInput.Longitude.HasValue || !apiInput.Radius.HasValue)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                           
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(apiInput.StationID))
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                        }
                    }
                    //判斷日期
                    if (flag)
                    {
                        if (string.IsNullOrWhiteSpace(apiInput.SDate) == false && string.IsNullOrWhiteSpace(apiInput.EDate) == false)
                        {
                            flag = DateTime.TryParse(apiInput.SDate, out SDate);
                            if (flag)
                            {
                                flag = DateTime.TryParse(apiInput.EDate, out EDate);
                                if (flag)
                                {
                                    if (SDate >= EDate)
                                    {
                                        flag = false;
                                        errCode = "ERR153";
                                    }
                                    else
                                    {
                                        if (DateTime.Now > SDate)
                                        {
                                            flag = false;
                                            errCode = "ERR154";
                                        }
                                       
                                    }

                                }
                                else
                                {
                                    errCode = "ERR152";
                                }
                            }
                            else
                            {
                                errCode = "ERR151";
                            }
                        }
                    }
                }
            }
            #endregion

            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenOnlyToken);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {

                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            }
            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);
                List<iRentStationData> iRentStations = new List<iRentStationData>();
                if (apiInput.Mode == 1)
                {
                    iRentStations = _repository.GetAlliRentStation(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                }
                else
                {
                   // AllCars = _repository.GetAllAnyRent(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                }

                //OAnyRentAPI = new OAPI_AnyRent()
                //{
                //    AnyRentObj = AllCars
                //};

            }
            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
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
