using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Station;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    /// 同站以據點取出車型
    /// </summary>
    public class GetCarTypeController : ApiController
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
            string funName = "GetCarTypeController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetCarType apiInput = null;
            OAPI_GetCarType GetCarTypeAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;
            
            Int16 QueryMode = 0; //查詢模式，0:未帶入起迄日;1:代入起迄日
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate=DateTime.Now;
            string Contentjson = "";
            bool isGuest = true;
            
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetCarType>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                if (flag)
                {
                    if (string.IsNullOrWhiteSpace(apiInput.StationID) || string.IsNullOrEmpty(apiInput.StationID))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    if(string.IsNullOrWhiteSpace(apiInput.SDate)==false && string.IsNullOrWhiteSpace(apiInput.EDate)==false)
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
                                    else
                                    {
                                        QueryMode = 1;
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

            #endregion
            //#region 不支援訪客
            //if (flag)
            //{
            //    if (isGuest)
            //    {
            //        flag = false;
            //        errCode = "ERR150";
            //    }
            //}
            //#endregion
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
            }
            if (flag)
            {

                _repository = new StationAndCarRepository(connetStr);
                List<CarTypeData> iRentStations = new List<CarTypeData>();
                if (QueryMode == 0)
                {
                    iRentStations = _repository.GetStationCarType(apiInput.StationID);
                }
                else
                {
                    List<ProjectAndCarTypeData> lstData = new List<ProjectAndCarTypeData>();
                    List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
                    lstData = _repository.GetStationCarType(apiInput.StationID, SDate, EDate);
                    if (lstData != null)
                    {
                        int len = lstData.Count;
                        if (len > 0)
                        {
                            for(int i = 0; i < len; i++)
                            {
                                CarTypeData obj = new CarTypeData()
                                {
                                    CarBrend = lstData[i].CarBrend,
                                    CarType = lstData[i].CarType,
                                    CarTypeName = lstData[i].CarBrend + lstData[i].CarTypeName ,
                                    CarTypePic = lstData[i].CarTypePic,
                                    Operator = lstData[i].Operator,
                                    OperatorScore = lstData[i].OperatorScore,
                                    Seat = lstData[i].Seat,
                                    Price = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price, lstData[i].PRICE_H, lstHoliday))
                                };
                                iRentStations.Add(obj);
                            }
                        }
                    }
                }

                if (iRentStations != null)
                {
                    GetCarTypeAPI = new OAPI_GetCarType()
                    {
                        GetCarTypeObj = iRentStations.OrderBy(x => x.Price).ToList()
                    };
                  

                }
                else
                {
                    GetCarTypeAPI = new OAPI_GetCarType()
                    {
                        GetCarTypeObj = iRentStations
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, GetCarTypeAPI, token);
            return objOutput;
            #endregion
        }
    }
}
