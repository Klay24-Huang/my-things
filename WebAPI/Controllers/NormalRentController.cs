using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.PolygonList;
using Domain.SP.Output;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 同站租還
    /// </summary>
    public class NormalRentController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doGetNormalRent(Dictionary<string, object> value)
        {
            #region 初始宣告
            var staCom = new StationCommon();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "NormalRentController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_NormalRent apiInput = null;
            OAPI_NormalRent ONormalRentAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            string Contentjson = "";
            bool isGuest = true;
            string StrCarTypeList = "";
            string StrSeatList = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_NormalRent>(Contentjson);
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

                    if (flag)
                    {
                        if(apiInput != null)
                        {
                            if(apiInput.CarTypes != null && apiInput.CarTypes.Count() > 0)
                            {
                                StrCarTypeList = String.Join(",", apiInput.CarTypes);
                            }

                            if (apiInput.Seats != null && apiInput.Seats.Count() > 0)
                            {
                                StrCarTypeList = String.Join(",", apiInput.Seats.Select(x=>x.ToString()).ToArray());
                            }
                        }
                    }

                    if (flag)
                    {
                        if(apiInput.ShowALL.HasValue &&  apiInput.ShowALL.Value == 1)
                        {
                            apiInput.Latitude = 0;
                            apiInput.Longitude = 0;
                            apiInput.Radius = 0;
                        }
                    }
                
                }
            }
            #endregion

            #region TB
            //Token判斷
            //if (flag && isGuest == false)
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
                #region mark-old
                //_repository = new StationAndCarRepository(connetStr);
                //if (apiInput.ShowALL == 1)
                //{
                //    iRentStations = _repository.GetAlliRentStation(apiInput.CarTypes, apiInput.Seats);
                //}
                //else
                //{
                //    iRentStations = _repository.GetAlliRentStation(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value, apiInput.CarTypes, apiInput.Seats);
                //}
                #endregion

                List<iRentStationData> iRentStations = new List<iRentStationData>();
                var spIn = new SpInput_GetAlliRentStation()
                {
                    LogID = LogID,
                    lat = apiInput.Latitude?? 0,
                    lng = apiInput.Longitude?? 0,
                    radius = apiInput.Radius?? 0,
                    CarTypes = StrCarTypeList,
                    Seats = StrSeatList,
                    SD = apiInput.SD,
                    ED = apiInput.ED
                };
                var spList = staCom.sp_GetAlliRentStation(spIn, ref errCode);
                if (spList != null && spList.Count() > 0)
                {
                    iRentStations = (from a in spList
                                     select new iRentStationData
                                     {
                                         StationID = a.StationID,
                                         StationName = a.StationName,
                                         Tel = a.Tel,
                                         ADDR = a.ADDR,
                                         Latitude = (decimal)a.Latitude,
                                         Longitude = (decimal)a.Longitude,
                                         Content = a.Content,
                                         IsRent = a.IsRent
                                     }).ToList();
                }

                ONormalRentAPI = new OAPI_NormalRent()
                {
                    NormalRentObj = iRentStations
                };
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
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