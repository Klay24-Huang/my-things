using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Project;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Project;
using Domain.TB;
using Domain.WebAPI.output.rootAPI;
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
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得專案及資費(未完成)
    /// </summary>
    public class GetProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        private CommonFunc baseVerify { get; set; }
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
            List<GetProjectObj> lstTmpData = new List<GetProjectObj>();
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate = DateTime.Now;
            int QueryMode = 0;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_GetProject>(Contentjson);
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
                        QueryMode = (apiInput.Mode.Value > 0) ? 1 : 0;
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
                                            //flag = false;
                                            //errCode = "ERR154";
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
            //20201103 ADD BY ADAM REASON.TOKEN判斷修改
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
                if (spOut.ErrorCode == "ERR101")
                {
                    flag = true;
                }
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);
                List<iRentStationData> iRentStations = new List<iRentStationData>();
                List<StationAndProjectAndCarTypeData> lstData = new List<StationAndProjectAndCarTypeData>();
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
                
                SPInput_GetStationCarTypeOfMutiStation spInput = new SPInput_GetStationCarTypeOfMutiStation()
                {
                    StationIDs = apiInput.StationID,
                    SD = SDate,
                    ED = EDate,
                    CarType = string.IsNullOrWhiteSpace(apiInput.CarType) ? "" : apiInput.CarType.Replace(" ", ""),
                    IDNO = IDNO,
                    LogID = LogID
                };

                if (apiInput.Mode == 1)
                {
                    iRentStations = _repository.GetAlliRentStation(apiInput.Latitude.Value, apiInput.Longitude.Value, apiInput.Radius.Value);
                    if (iRentStations != null && iRentStations.Count > 0)
                    {
                        List<string> StationIDs = iRentStations.Select(x => x.StationID).ToList();
                        spInput.StationIDs = String.Join(",", StationIDs);
                    }
                }

                var spList = GetStationCarTypeOfMutiStation(spInput, ref flag, ref lstError, ref errCode);
                if (spList != null && spList.Count > 0)
                {
                    lstData = (from a in spList
                               select new StationAndProjectAndCarTypeData
                               {
                                   ADDR = a.ADDR,
                                   CarBrend = a.CarBrend,
                                   CarOfArea = a.CarOfArea,
                                   CarType = a.CarType,
                                   CarTypeName = a.CarTypeName,
                                   CarTypePic = a.CarTypePic,
                                   Content = a.Content,
                                   Insurance = a.Insurance,
                                   InsurancePerHours = a.InsurancePerHours,
                                   IsRent = a.IsRent,
                                   Latitude = a.Latitude,
                                   Longitude = a.Longitude,
                                   Operator = a.Operator,
                                   OperatorScore = a.OperatorScore,
                                   PayMode = a.PayMode,
                                   Price = a.PriceBill, //租金改抓sp
                                   Price_W = a.Price,   //20201111 ADD BY ADAM REASON.原本Price改為預估金額，多增加Price_W當作平日價
                                   PRICE_H = a.PRICE_H, //目前用不到
                                   PRODESC = a.PRODESC,
                                   PROJID = a.PROJID,
                                   PRONAME = a.PRONAME,
                                   Seat = a.Seat,
                                   StationID = a.StationID,
                                   StationName = a.StationName                                   
                               }).ToList();
                }

                if (flag)
                {
                    if (lstData != null)
                    {
                        int DataLen = lstData.Count;

                        if (DataLen > 0)
                        {
                            lstTmpData.Add(new GetProjectObj()
                            {
                                ADDR = lstData[0].ADDR,
                                Content = lstData[0].Content,
                                Latitude = lstData[0].Latitude,
                                Longitude = lstData[0].Longitude,
                                StationID = lstData[0].StationID,
                                StationName = lstData[0].StationName,
                                IsRent = lstData[0].IsRent,     //20201027 ADD BY ADAM REASON.抓第一筆判斷是否可租
                                ProjectObj = new List<ProjectObj>()
                            });
                            lstTmpData[0].ProjectObj.Add(new ProjectObj()
                            {
                                StationID = lstData[0].StationID,
                                CarBrend = lstData[0].CarBrend,
                                CarType = lstData[0].CarType,
                                CarTypeName = lstData[0].CarTypeName,
                                CarTypePic = lstData[0].CarTypePic,
                                Insurance = lstData[0].Insurance,
                                InsurancePerHour = lstData[0].InsurancePerHours,
                                IsMinimum = 1,
                                Operator = lstData[0].Operator,
                                OperatorScore = lstData[0].OperatorScore,
                                ProjID = lstData[0].PROJID,
                                ProjName = lstData[0].PRONAME,
                                ProDesc = lstData[0].PRODESC,
                                Seat = lstData[0].Seat,
                                //Bill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[0].Price, lstData[0].PRICE_H, lstHoliday)),
                                //Price = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[0].Price, lstData[0].PRICE_H, lstHoliday)),
                                Price = lstData[0].Price,
                                WorkdayPerHour = lstData[0].PayMode == 0 ? lstData[0].Price_W / 10 : lstData[0].Price_W,
                                HolidayPerHour = lstData[0].PayMode == 0 ? lstData[0].PRICE_H / 10 : lstData[0].PRICE_H,
                                CarOfArea = lstData[0].CarOfArea,
                                Content = "",
                                IsRent = lstData[0].IsRent      //20201024 ADD BY ADAM REASON.增加是否可租
                            });
                            //lstTmpData[0].Minimum = lstTmpData[0].ProjectObj[0].Bill;
                            lstTmpData[0].Minimum = lstTmpData[0].ProjectObj[0].Price;

                            for (int i = 1; i < DataLen; i++)
                            {
                                int index = lstTmpData.FindIndex(delegate (GetProjectObj station)
                                  {
                                      return station.StationID == lstData[i].StationID;
                                  });
                                if (index < 0)
                                {
                                    //int tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price, lstData[i].PRICE_H, lstHoliday));
                                    int tmpBill = lstData[i].Price;
                                    int isMin = 0;
                                    ProjectObj tmpObj = new ProjectObj()
                                    {
                                        StationID = lstData[i].StationID,
                                        CarBrend = lstData[i].CarBrend,
                                        CarType = lstData[i].CarType,
                                        CarTypeName = lstData[i].CarTypeName,
                                        CarTypePic = lstData[i].CarTypePic,
                                        Insurance = lstData[i].Insurance,
                                        InsurancePerHour = lstData[i].InsurancePerHours,
                                        IsMinimum = isMin,
                                        Operator = lstData[i].Operator,
                                        OperatorScore = lstData[i].OperatorScore,
                                        ProjID = lstData[i].PROJID,
                                        ProjName = lstData[i].PRONAME,
                                        ProDesc = lstData[i].PRODESC,
                                        Seat = lstData[i].Seat,
                                        //Bill = tmpBill,
                                        Price = tmpBill,
                                        WorkdayPerHour = lstData[i].PayMode == 0 ? lstData[i].Price_W / 10 : lstData[i].Price_W,
                                        HolidayPerHour = lstData[i].PayMode == 0 ? lstData[i].PRICE_H / 10 : lstData[i].PRICE_H,
                                        CarOfArea = lstData[i].CarOfArea,
                                        Content = "",
                                        IsRent = lstData[i].IsRent      //20201024 ADD BY ADAM REASON.增加是否可租
                                    };
                                    GetProjectObj tmpGetProjectObj = new GetProjectObj()
                                    {
                                        ADDR = lstData[i].ADDR,
                                        Content = lstData[i].Content,
                                        Latitude = lstData[i].Latitude,
                                        Longitude = lstData[i].Longitude,
                                        StationID = lstData[i].StationID,
                                        StationName = lstData[i].StationName,
                                        IsRent = lstData[i].IsRent,
                                        ProjectObj = new List<ProjectObj>(),
                                        Minimum = tmpBill
                                    };
                                    tmpGetProjectObj.ProjectObj.Add(tmpObj);
                                    lstTmpData.Add(tmpGetProjectObj);
                                }
                                else
                                {
                                    //int tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price, lstData[i].PRICE_H, lstHoliday));
                                    int tmpBill = lstData[i].Price;
                                    int isMin = 0;
                                    if (tmpBill < lstTmpData[index].Minimum)
                                    {
                                        isMin = 1;
                                        lstTmpData[index].Minimum = tmpBill;
                                    }
                                    lstTmpData[index].ProjectObj.Add(new ProjectObj()
                                    {
                                        StationID = lstData[i].StationID,
                                        CarBrend = lstData[i].CarBrend,
                                        CarType = lstData[i].CarType,
                                        CarTypeName = lstData[i].CarTypeName,
                                        CarTypePic = lstData[i].CarTypePic,
                                        Insurance = lstData[i].Insurance,
                                        InsurancePerHour = lstData[i].InsurancePerHours,
                                        IsMinimum = isMin,
                                        Operator = lstData[i].Operator,
                                        OperatorScore = lstData[i].OperatorScore,
                                        ProjID = lstData[i].PROJID,
                                        ProjName = lstData[i].PRONAME,
                                        ProDesc = lstData[i].PRODESC,
                                        Seat = lstData[i].Seat,
                                        //Bill = tmpBill,
                                        Price = tmpBill,
                                        WorkdayPerHour = lstData[i].PayMode == 0 ? lstData[i].Price_W / 10 : lstData[i].Price_W,
                                        HolidayPerHour = lstData[i].PayMode == 0 ? lstData[i].PRICE_H / 10 : lstData[i].PRICE_H,
                                        CarOfArea = lstData[i].CarOfArea,
                                        Content = "",
                                        IsRent = lstData[i].IsRent      //20201024 ADD BY ADAM REASON.增加是否可租
                                    });
                                }
                            }
                        }
                    }
                }

                if (lstTmpData != null && lstTmpData.Count > 0)
                {
                    lstTmpData.ForEach(x => x.StationPic = x.StationPic ?? new string[0]);
                }

                outputApi = new OAPI_GetProject()
                {
                    GetProjectObj = lstTmpData
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        /// <summary>
        /// GetStationCarTypeOfMutiStation
        /// </summary>
        /// <param name="spInput">spInput</param>
        /// <param name="flag">flag</param>
        /// <param name="lstError">lstError</param>
        /// <param name="errCode">errCode</param>
        /// <returns></returns>
        private List<SPOutput_GetStationCarTypeOfMutiStation> GetStationCarTypeOfMutiStation(SPInput_GetStationCarTypeOfMutiStation spInput, ref bool flag, ref List<ErrorInfo> lstError, ref string errCode)
        {
            List<SPOutput_GetStationCarTypeOfMutiStation> re = new List<SPOutput_GetStationCarTypeOfMutiStation>();
            string SPName = new ObjType().GetSPName(ObjType.SPType.GetStationCarTypeOfMutiStation);
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_GetStationCarTypeOfMutiStation, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetStationCarTypeOfMutiStation, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref re, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return re;
        }
    }
}