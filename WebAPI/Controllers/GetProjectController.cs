using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.PolygonList;
using Domain.SP.Input.Project;
using Domain.SP.Input.Subscription;
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
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using WebAPI.Utils;
using Domain.SP.Output.Subscription;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得專案及資費(未完成)
    /// </summary>
    public class GetProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        private CommonFunc baseVerify { get; set; }
        [HttpPost]
        public Dictionary<string, object> DoGetProject(Dictionary<string, object> value)
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
            string funName = "GetProjectController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetProject();
            var outputApi = new OAPI_GetProject();
            List<GetProjectObj> lstTmpData = new List<GetProjectObj>();
            Token token = null;
            baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate = DateTime.Now;
            int QueryMode = 0;
            string IDNO = "";
            var bill = new BillCommon();
            var SeatGroups = new List<GetProject_SeatGroup>();
            var LstCarTypes = new List<string>();
            var LstSeats = new List<string>();
            var LstStationIDs = new List<string>();
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租

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
                    if (flag)
                    {
                        #region CarType處理

                        if (apiInput.CarType != null && apiInput.CarType != "")
                        {
                            //StrCarTypes = apiInput.CarType;
                            LstCarTypes.Add(apiInput.CarType.ToUpper());
                        }

                        if (apiInput.CarTypes != null && apiInput.CarTypes.Count() > 0)
                        {
                            //StrCarTypes = String.Join(",", apiInput.CarTypes);
                            LstCarTypes.AddRange(apiInput.CarTypes.Select(x=>x.ToUpper()).ToList());
                        }

                        if (LstCarTypes != null && LstCarTypes.Count() > 0)
                            LstCarTypes = LstCarTypes.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

                        #endregion

                        //Seat處理
                        if (apiInput.Seats != null && apiInput.Seats.Count() > 0)
                            LstSeats = apiInput.Seats.Select(x => x.ToString()).ToList();

                        if (!string.IsNullOrWhiteSpace(apiInput.StationID))
                            LstStationIDs.Add(apiInput.StationID); 
                    }                     
                }

                if (apiInput.Longitude == null)
                {
                    apiInput.Longitude = 0;
                }

                if (apiInput.Latitude == null)
                {
                    apiInput.Latitude = 0;
                }

            }
            #endregion

            #region TB
            //Token判斷
            //20201103 ADD BY ADAM REASON.TOKEN判斷修改
            if (flag && Access_Token_string.Split(' ').Length >= 2)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token_string.Split(' ')[1].ToString()
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

            //取得汽車使用中訂閱制月租
            if (flag)
            {
                if (!string.IsNullOrWhiteSpace(IDNO))
                {
                    var sp_in = new SPInput_GetNowSubs()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        SD = SDate,
                        ED = EDate,
                        IsMoto = 0
                    };
                    var sp_list = new MonSubsSp().sp_GetNowSubs(sp_in, ref errCode);
                    if (sp_list != null && sp_list.Count() > 0)
                        InUseMonth = sp_list;
                }
            }

            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);
                List<iRentStationData> iRentStations = new List<iRentStationData>();
                List<StationAndProjectAndCarTypeData> lstData = new List<StationAndProjectAndCarTypeData>();
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));

                if (apiInput.Mode == 1)
                {
                    var spIn = new SpInput_GetAlliRentStation()
                    {
                        LogID = LogID,
                        lat = apiInput.Latitude ?? 0,
                        lng = apiInput.Longitude ?? 0,
                        radius = apiInput.Radius ?? 0,
                        CarTypes = String.Join(",", LstCarTypes),
                        Seats = String.Join(",", LstSeats),
                        SD = SDate,
                        ED = EDate
                    };
                    var sp_re = staCom.sp_GetAlliRentStation(spIn, ref errCode);
                    if (sp_re != null && sp_re.Count() > 0)
                    {
                        iRentStations = (from a in sp_re
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
                        LstStationIDs = iRentStations.Select(x => x.StationID).ToList();
                    }
                }

                SPInput_GetStationCarTypeOfMutiStation spInput = new SPInput_GetStationCarTypeOfMutiStation()
                {
                    StationIDs = String.Join(",",LstStationIDs),
                    SD = SDate,
                    ED = EDate,
                    CarTypes = String.Join(",",LstCarTypes),
                    IDNO = IDNO,
                    Insurance = apiInput.Insurance,     //20201112 ADD BY ADAM REASON.增加是否使用安心服務
                    Mode = (apiInput.Latitude.Value > 0 && apiInput.Longitude.Value > 0) ? 1 : 0,       //20210416 ADD BY ADAM REASON.增加模式判斷，0沒有送定位點1則有送
                    LogID = LogID
                };

                var spList = GetStationCarTypeOfMutiStation(spInput, ref flag, ref lstError, ref errCode);
                if (spList != null && spList.Count > 0)
                {
                    lstData = (from a in spList
                               select new StationAndProjectAndCarTypeData
                               {
                                   ADDR = string.Format("{0}{1}{2}", a.CityName, a.AreaName, a.ADDR),
                                   CarBrend = a.CarBrend,
                                   CarOfArea = a.CarOfArea,
                                   CarType = a.CarType,
                                   CarTypeName = a.CarTypeName,
                                   CarTypePic = a.CarTypePic,
                                   Content = a.Content,
                                   ContentForAPP = a.ContentForAPP,
                                   CityName = a.CityName,
                                   AreaName = a.AreaName,
                                   Insurance = a.Insurance,
                                   InsurancePerHours = a.InsurancePerHours,
                                   IsRent = a.IsRent,
                                   Latitude = a.Latitude,
                                   Longitude = a.Longitude,
                                   Operator = a.Operator,
                                   OperatorScore = a.OperatorScore,
                                   PayMode = a.PayMode,
                                   // 預估金額 = 租金 + 里程費 + 安心服務費
                                   //Price = a.PriceBill, //租金改抓sp
                                   //20210115;因應春節專案，預估金額改用特殊算法
                                   Price = GetPriceBill(a, IDNO, LogID, lstHoliday, SDate, EDate, funName, 0) +
                                            bill.CarMilageCompute(SDate, EDate, a.MilageBase, Mildef, 20, new List<Holiday>()) +
                                            ((apiInput.Insurance == 1) ? bill.CarRentCompute(SDate, EDate, a.InsurancePerHours * 10, a.InsurancePerHours * 10, 10, lstHoliday) : 0),
                                   Price_W = a.Price,   //20201111 ADD BY ADAM REASON.原本Price改為預估金額，多增加Price_W當作平日價
                                   PRICE_H = a.PRICE_H, //目前用不到
                                   PRODESC = a.PRODESC,
                                   PROJID = a.PROJID,
                                   PRONAME = a.PRONAME,
                                   Seat = a.Seat,
                                   StationID = a.StationID,
                                   StationName = a.StationName,
                                   StationPicJson = a.StationPicJson,
                                   IsFavStation = a.IsFavStation //常用據點
                               }).ToList();

                    #region 過濾查詢結果

                    if (lstData != null && lstData.Count() > 0)
                        lstData.ForEach(x => { if ((string.IsNullOrWhiteSpace(x.IsRent) ? "" : x.IsRent.ToLower()) == "n") { x.IsShowCard = 0; } });

                    if (apiInput.Seats != null && apiInput.Seats.Count()>0)
                    {
                        lstData.ForEach(x => { if (!apiInput.Seats.Contains(x.Seat)) { x.IsRent = "N"; x.IsShowCard = 0; } });
                    }

                    if(LstCarTypes != null && LstCarTypes.Count() > 0)
                    {
                        lstData.ForEach(x => { if (!LstCarTypes.Any(y => y == x.CarType)) { x.IsRent = "N"; x.IsShowCard = 0;}});
                    }

                    //if (apiInput.PriceMin > 0 && apiInput.PriceMax > 0)
                    //    lstData.ForEach(x => { if (x.Price < apiInput.PriceMin || x.Price > apiInput.PriceMax) { x.IsRent = "N"; x.IsShowCard = 0; } });

                    #endregion
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
                                ContentForAPP = lstData[0].ContentForAPP,
                                CityName = lstData[0].CityName,
                                AreaName = lstData[0].AreaName,
                                Latitude = lstData[0].Latitude,
                                Longitude = lstData[0].Longitude,
                                StationID = lstData[0].StationID,
                                StationName = lstData[0].StationName,
                                IsRent = lstData[0].IsRent,     //20201027 ADD BY ADAM REASON.抓第一筆判斷是否可租
                                IsFavStation = lstData[0].IsFavStation,
                                IsShowCard = lstData[0].IsShowCard,
                                ProjectObj = new List<ProjectObj>(),
                                StationInfoObj = new List<StationInfoObj>()
                            });

                            List<StationInfoObj> tmpStationInfoObj = JsonConvert.DeserializeObject<List<StationInfoObj>>(lstData[0].StationPicJson);
                            foreach (var StationInfo in tmpStationInfoObj)
                            {
                                StationInfo.StationPic = string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["StorageBaseURL"], ConfigurationManager.AppSettings["stationContainer"], StationInfo.StationPic);
                            }
                            lstTmpData[0].StationInfoObj = tmpStationInfoObj;

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
                                IsRent = lstData[0].IsRent,      //20201024 ADD BY ADAM REASON.增加是否可租
                                IsFavStation = lstData[0].IsFavStation,
                                IsShowCard = lstData[0].IsShowCard
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
                                        IsRent = lstData[i].IsRent,      //20201024 ADD BY ADAM REASON.增加是否可租
                                        IsFavStation = lstData[i].IsFavStation,
                                        IsShowCard = lstData[i].IsShowCard
                                    };

                                    List<StationInfoObj> tmpStation = JsonConvert.DeserializeObject<List<StationInfoObj>>(lstData[i].StationPicJson);
                                    foreach (var StationInfo in tmpStation)
                                    {
                                        StationInfo.StationPic = string.Format("{0}{1}/{2}", ConfigurationManager.AppSettings["StorageBaseURL"], ConfigurationManager.AppSettings["stationContainer"], StationInfo.StationPic);
                                    }

                                    GetProjectObj tmpGetProjectObj = new GetProjectObj()
                                    {
                                        ADDR = lstData[i].ADDR,
                                        Content = lstData[i].Content,
                                        ContentForAPP = lstData[i].ContentForAPP,
                                        CityName = lstData[i].CityName,
                                        AreaName = lstData[i].AreaName,
                                        Latitude = lstData[i].Latitude,
                                        Longitude = lstData[i].Longitude,
                                        StationID = lstData[i].StationID,
                                        StationName = lstData[i].StationName,
                                        IsRent = lstData[i].IsRent,
                                        IsFavStation = lstData[i].IsFavStation,
                                        IsShowCard = lstData[i].IsShowCard,
                                        ProjectObj = new List<ProjectObj>(),
                                        StationInfoObj = new List<StationInfoObj>(),
                                        Minimum = tmpBill
                                    };

                                    tmpGetProjectObj.ProjectObj.Add(tmpObj);
                                    tmpGetProjectObj.StationInfoObj = tmpStation;
                                    lstTmpData.Add(tmpGetProjectObj);
                                }
                                else
                                {
                                    //int tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price, lstData[i].PRICE_H, lstHoliday));
                                    int tmpBill = lstData[i].Price;
                                    int isMin = 0;
                                    if (tmpBill <= lstTmpData[index].Minimum)
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
                                        IsRent = lstData[i].IsRent,      //20201024 ADD BY ADAM REASON.增加是否可租
                                        IsFavStation = lstData[i].IsFavStation,
                                        IsShowCard = lstData[i].IsShowCard
                                    });
                                }
                            }
                        }
                    }
                }

                #region 車款過濾

                if (flag)
                {
                    if (lstTmpData != null && lstTmpData.Count() > 0 && LstCarTypes != null && LstCarTypes.Count() > 0)
                    {
                        lstTmpData.ForEach(x => {
                            var ProjIsRents = x.ProjectObj.Where(y => LstCarTypes.Any(z => z == y.CarType) && y.IsRent == "Y").ToList();
                            x.ProjectObj = x.ProjectObj.Where(y => LstCarTypes.Any(z => z == y.CarType)).ToList();
                            if (ProjIsRents == null || ProjIsRents.Count() == 0)
                                x.IsRent = "N";
                            else
                                x.IsRent = "Y";
                        });
                    }
                }

                #endregion

                outputApi = new OAPI_GetProject()
                {
                    GetProjectObj = lstTmpData
                };

                #region 車款,金額下拉,是否有可租

                bool HaveRentY = false;
                if(lstData != null && lstData.Count() > 0)
                   HaveRentY = lstData.Where(y => (string.IsNullOrWhiteSpace(y.IsRent) ? "": y.IsRent.ToLower()) == "y").Count() > 0;

                if (lstData != null && lstData.Count() > 0 && HaveRentY)
                {
                    //outputApi.PriceMax = lstData.Where(y=>y.IsRent.ToLower() == "y").Select(x => x.Price).Max();
                    //outputApi.PriceMin = lstData.Where(y=>y.IsRent.ToLower() == "y").Select(x => x.Price).Min();
                    
                    List<int> SeatsList = lstData.Where(z => z.IsRent.ToLower() == "y" && z.IsShowCard == 1).GroupBy(x => x.Seat).Select(y => y.FirstOrDefault().Seat).ToList();
                    if (SeatsList != null && SeatsList.Count() > 0)
                    {
                        foreach (int se in SeatsList)
                        {
                            var item = new GetProject_SeatGroup();
                            item.Seat = Convert.ToInt16(se);

                            List<GetProject_CarInfo> CarInfos =
                                (from a in lstData
                                 where a.IsRent.ToLower() == "y" && a.Seat == se
                                 group new { a.Seat, a.CarType, a.CarTypeName, a.CarTypePic }
                                 by new { a.Seat, a.CarType, a.CarTypeName, a.CarTypePic } into g
                                 select new GetProject_CarInfo
                                 {
                                     Seat = item.Seat,
                                     CarType = g.Key.CarType,
                                     CarTypePic = g.Key.CarTypePic,
                                     CarTypeName = g.Key.CarTypeName
                                 }).ToList();
                            if (CarInfos != null && CarInfos.Count() > 0)
                                item.CarInfos = CarInfos;
                            SeatGroups.Add(item);
                        }
                    }

                    if (lstData.Where(x => (string.IsNullOrWhiteSpace(x.IsRent) ? "": x.IsRent.ToLower()) == "y" && x.IsShowCard == 1).ToList().Count() > 0)
                        outputApi.HasRentCard = true;
                    else
                        outputApi.HasRentCard = false;
                }

                #endregion

                #region 產出月租&Project虛擬卡片 

                if(outputApi.GetProjectObj != null && outputApi.GetProjectObj.Count() > 0)
                {
                    var VisProObjs = new List<GetProjectObj>();
                    var ProObjs = outputApi.GetProjectObj;                    
                    if (InUseMonth != null && InUseMonth.Count() > 0 && ProObjs != null && ProObjs.Count()>0)
                    {
                        ProObjs.ForEach(x => {
                           if(x.ProjectObj != null && x.ProjectObj.Count() > 0)
                           {
                                var newGetProjObj = objUti.Clone(x);
                                newGetProjObj.ProjectObj = new List<ProjectObj>();
                                x.ProjectObj.ForEach(y =>
                                {
                                    y.IsMinimum = 0;    //20210620 ADD BY ADAM REASON.先恢復為0
                                    newGetProjObj.ProjectObj.Add(y);
                                    InUseMonth.ForEach(z =>
                                    {
                                        ProjectObj newItem = objUti.Clone(y);

                                        #region 月租卡片欄位給值
                                        newItem.ProjName += "_" + z.MonProjNM;
                                        newItem.CarWDHours = z.WorkDayHours;
                                        newItem.CarHDHours = z.HolidayHours;
                                        newItem.MotoTotalMins = z.MotoTotalMins;
                                        newItem.WorkdayPerHour = Convert.ToInt32(z.WorkDayRateForCar);
                                        newItem.HolidayPerHour = Convert.ToInt32(z.HoildayRateForCar);
                                        newItem.MonthStartDate = z.StartDate.ToString("yyyy/MM/dd");
                                        newItem.MonthEndDate = z.StartDate.AddDays(30 * z.MonProPeriod).ToString("yyyy/MM/dd");
                                        newItem.MonthlyRentId = z.MonthlyRentId;
                                        newItem.WDRateForCar = z.WorkDayRateForCar;
                                        newItem.HDRateForCar = z.HoildayRateForCar;
                                        newItem.WDRateForMoto = z.WorkDayRateForMoto;
                                        newItem.HDRateForMoto = z.HoildayRateForMoto;
                                        var fn_in = new SPOutput_GetStationCarTypeOfMutiStation()
                                        {
                                            PriceBill = y.Price, //給預設
                                            PROJID = y.ProjID,
                                            CarType = y.CarType,
                                            Price = y.WorkdayPerHour * 10,
                                            PRICE_H = y.HolidayPerHour * 10,
                                        };
                                        newItem.Price = GetPriceBill(fn_in, IDNO, LogID, lstHoliday, SDate, EDate, MonId: z.MonthlyRentId);
                                        #endregion

                                        newGetProjObj.ProjectObj.Add(newItem);
                                    });
                                });
                                if (newGetProjObj.ProjectObj != null && newGetProjObj.ProjectObj.Count() > 0)
                                {
                                    //20210620 ADD BY ADAM REASON.排序，抓最小的出來設定IsMinimun
                                    //newGetProjObj.ProjectObj = newGetProjObj.ProjectObj.OrderBy(a => a.ProjID).ThenBy(b=>b.CarType).ThenBy(c => c.MonthlyRentId).ToList();
                                    newGetProjObj.ProjectObj = newGetProjObj.ProjectObj.OrderBy(a => a.Price).ThenByDescending(c => c.MonthlyRentId).ToList();
                                    newGetProjObj.ProjectObj.First().IsMinimum = 1;
                                    VisProObjs.Add(newGetProjObj);
                                }
                            }
                        });

                        outputApi.GetProjectObj = VisProObjs;
                    }                  
                }

                #endregion

                outputApi.GetProjectObj = outputApi.GetProjectObj ?? new List<GetProjectObj>();

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

        private int GetPriceBill(SPOutput_GetStationCarTypeOfMutiStation spItem, string IDNO, long LogID, List<Holiday> lstHoliday, DateTime SD, DateTime ED, string funNM = "",Int64 MonId=0)
        {
            int re = 0;
            var bill = new BillCommon();
            var cr_com = new CarRentCommon();
            var cr_sp = new CarRentSp();
            int PriceBill = spItem.PriceBill;//先給sp值

            string errMsg = "";
            var spre = cr_sp.sp_GetEstimate(spItem.PROJID, spItem.CarType, LogID, ref errMsg);
            int projType = -1;
            if (spre != null)
                projType = spre.PROJTYPE;

            bool isSpr = cr_com.isSpring(SD, ED);
            List<int> carProjTypes = new List<int>() { 0, 3 };
            if (carProjTypes.Any(x => x == projType) && isSpr)
            {
                var bizIn = new IBIZ_SpringInit()
                {
                    ProjID = spItem.PROJID,
                    ProjType = projType,
                    CarType = spItem.CarType,
                    IDNO = IDNO,
                    LogID = LogID,
                    lstHoliday = lstHoliday,
                    SD = SD,
                    ED = ED,
                    ProDisPRICE = Convert.ToDouble(spItem.Price) / 10,
                    ProDisPRICE_H = Convert.ToDouble(spItem.PRICE_H) / 10
                };
                var xre = cr_com.GetSpringInit(bizIn, connetStr, funNM);
                if (xre != null)
                    re = xre.RentInPay;
            }
            else
            {
                var input = new ICF_GetCarRentPrice()
                {
                    MonId = MonId,
                    IDNO = IDNO,
                    SD = SD,
                    ED = ED,
                    priceN = spItem.Price/10,
                    priceH = spItem.PRICE_H/10,
                    daybaseMins = 60,
                    dayMaxHour = 10,
                    lstHoliday = lstHoliday,
                    Discount = 0,
                    FreeMins = 0
                };
                re =  Convert.ToInt32(new MonSubsCommon().GetCarRentPrice(input));
            }

            return re;
        }
    }
}