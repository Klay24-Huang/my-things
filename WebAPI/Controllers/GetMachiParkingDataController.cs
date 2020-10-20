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
    /// 取得特約停車場（車麻吉，不得還車）
    /// </summary>
    public class GetMachiParkingDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doGetMachiParkingData(Dictionary<string, object> value)
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
            string funName = "GetMachiParkingDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetParkingData apiInput = null;
            OAPI_GetParkingData ONormalRentAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            ParkingRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetParkingData>(Contentjson);
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
                        flag = apiInput.Mode.HasValue;
                        if (false == flag)
                        {
                            errCode = "ERR900";
                        }
                        else
                        {
                            if (apiInput.Mode.Value < 0 || apiInput.Mode.Value > 2)
                            {
                                flag = false;
                                errCode = "ERR900";
                            }
                        }
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
            //Token判斷(非訪客才需要驗)
            //if (flag && false == isGuest)
            //{
            //    string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenOnlyToken);
            //    SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
            //    {
            //        LogID = LogID,
            //        Token = Access_Token
            //    };
            //    SPOutput_Base spOut = new SPOutput_Base();
            //    SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base>(connetStr);
            //    flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
            //    baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            //}

            if (flag)
            {
                _repository = new ParkingRepository(connetStr);
                List<ParkingData> parkings = new List<ParkingData>();

                if (apiInput.ShowALL == 1 && apiInput.Mode.Value == 2)    //全部
                {
                    parkings = _repository.GetAllParking();
                }
                else if (apiInput.ShowALL == 0 && apiInput.Mode.Value == 2) //方圓顯示全部
                {
                    parkings = _repository.GetAllParking(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                }
                else if (apiInput.ShowALL == 1 && apiInput.Mode.Value < 2)
                {
                    parkings = _repository.GetAllParkingByType(apiInput.Mode.Value);
                }
                else
                {
                    parkings = _repository.GetAllParkingByType(apiInput.Mode.Value, apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                }

                ONormalRentAPI = new OAPI_GetParkingData()
                {
                    ParkingObj = parkings
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, ONormalRentAPI, token);
            return objOutput;
            #endregion
        }
    }
}