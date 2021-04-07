using Domain.Common;
using Domain.SP.Input.Car;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Car;
using Domain.SP.Output.Common;
using Domain.SP.Output.Rent;
using Domain.TB;
using Newtonsoft.Json;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 同站以據點取出車型
    /// </summary>
    public class GetCarTypeController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonFunc baseVerify { get; set; }

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
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;

            Int16 QueryMode = 0; //查詢模式，0:未帶入起迄日;1:代入起迄日
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate = DateTime.Now;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";
            string CarTypeList = "";
            string SeatList = "";
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
                    if (string.IsNullOrWhiteSpace(apiInput.SD) == false && string.IsNullOrWhiteSpace(apiInput.ED) == false)
                    {
                        flag = DateTime.TryParse(apiInput.SD, out SDate);
                        if (flag)
                        {
                            flag = DateTime.TryParse(apiInput.ED, out EDate);
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
                                        //flag = false;
                                        //errCode = "ERR154";
                                    }
                                    else
                                    {
                                        QueryMode = 1;
                                    }
                                }
                            }
                            else
                            {
                                flag = false;
                                errCode = "ERR152";
                            }
                        }
                        else
                        {
                            flag = false;
                            errCode = "ERR151";
                        }
                    }

                    if (apiInput.CarTypes != null && apiInput.CarTypes.Count() > 0)
                        CarTypeList = String.Join(",", apiInput.CarTypes);

                    if (apiInput.Seats != null && apiInput.Seats.Count() > 0)
                        SeatList = String.Join(",", apiInput.Seats.Select(x=>x.ToString()).ToList());
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

                //訪客機制BYPASS
                if (spOut.ErrorCode == "ERR101")
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
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
                _repository = new StationAndCarRepository(connetStr);
                List<CarTypeData> iRentStations = new List<CarTypeData>();
                List<OAPI_GetCarTypeParam> OAPI_Params = new List<OAPI_GetCarTypeParam>();

                var spInput = new SPInput_GetStationCarType()
                {
                    StationID = apiInput.StationID,
                    SD = SDate,
                    ED = EDate,
                    CarTypes = CarTypeList,
                    Seats = SeatList,
                    IDNO = IDNO,
                    LogID = LogID,
                    
                };

                var sp_re = GetStationCarType(spInput, ref flag, ref lstError, ref errCode);
                var spList = new List<SPOutput_GetStationCarType_Cards>();
                if (sp_re != null && sp_re.Cards != null && sp_re.Cards.Count() > 0)
                    spList = sp_re.Cards;

                if (QueryMode == 0)
                {
                    if (spList != null && spList.Count() > 0)
                    {
                        //spList.ForEach(x => { x.CarTypeName = x.CarBrend + " " + x.CarTypeName; });
                        OAPI_Params = (from a in spList
                                       select new OAPI_GetCarTypeParam
                                       {
                                           CarBrend = a.CarBrend,
                                           CarType = a.CarType,
                                           CarTypeName = a.CarTypeName,
                                           CarTypePic = a.CarTypePic,
                                           Operator = a.Operator,
                                           OperatorScore = a.OperatorScore,
                                           Seat = a.Seat,
                                           //Price = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, a.Price_N, a.Price_H, lstHoliday))
                                           Price = a.Price                                           
                                       }).ToList();
                    }
                }
                else
                {
                    var lstData = spList;
                    if (lstData != null)
                    {
                        int len = lstData.Count;
                        if (len > 0)
                        {
                            for (int i = 0; i < len; i++)
                            {
                                CarTypeData obj = new CarTypeData()
                                {
                                    CarBrend = lstData[i].CarBrend,
                                    CarType = lstData[i].CarType,
                                    //CarTypeName = lstData[i].CarBrend + " " + lstData[i].CarTypeName,
                                    CarTypeName = lstData[i].CarTypeName,
                                    CarTypePic = lstData[i].CarTypePic,
                                    Operator = lstData[i].Operator,
                                    OperatorScore = lstData[i].OperatorScore,
                                    Seat = lstData[i].Seat,
                                    //Price = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price_N, lstData[i].Price_H, lstHoliday))
                                    Price = lstData[i].Price
                                };
                                iRentStations.Add(obj);
                            }
                        }

                        OAPI_Params = JsonConvert.DeserializeObject<List<OAPI_GetCarTypeParam>>(JsonConvert.SerializeObject(iRentStations));
                    }
                }

                if (OAPI_Params != null && OAPI_Params.Count()>0)
                {
                    GetCarTypeAPI = new OAPI_GetCarType()
                    {                       
                        GetCarTypeObj = OAPI_Params.OrderBy(x => x.Price).ToList()
                    };
                }
                else
                {
                    GetCarTypeAPI = new OAPI_GetCarType()
                    {
                        IsFavStation = 0,
                        GetCarTypeObj = OAPI_Params
                    };
                }

                if (sp_re != null)
                    GetCarTypeAPI.IsFavStation = sp_re.IsFavStation;
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

        /// <summary>
        /// GetStationCarType
        /// </summary>
        /// <param name="spInput">spInput</param>
        /// <param name="flag">flag</param>
        /// <param name="lstError">lstError</param>
        /// <param name="errCode">errCode</param>
        /// <returns></returns>
        private SPOutput_GetStationCarType GetStationCarType(SPInput_GetStationCarType spInput, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            var re = new SPOutput_GetStationCarType();

            List<SPOutput_GetStationCarType_Cards>  sp_list = new List<SPOutput_GetStationCarType_Cards>();
            string SPName = new ObjType().GetSPName(ObjType.SPType.GetStationCarType);
            SPOutBase_GetStationCarType spOut = new SPOutBase_GetStationCarType();
            SQLHelper<SPInput_GetStationCarType, SPOutBase_GetStationCarType> sqlHelp = new SQLHelper<SPInput_GetStationCarType, SPOutBase_GetStationCarType>(connetStr);
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref sp_list, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            re.IsFavStation = spOut.IsFavStation;
            re.Cards = sp_list; //20210406 ADD BY ADAM REASON.少了這段會有BUG
            return re;
        }

    }
}