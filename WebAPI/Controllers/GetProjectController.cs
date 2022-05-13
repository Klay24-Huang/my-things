using Domain.Common;
using Domain.SP.Input.PolygonList;
using Domain.SP.Input.Project;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Project;
using Domain.SP.Output.Subscription;
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
using WebAPI.Models.Param.CusFun.Input;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得同站專案及資費
    /// </summary>
    public class GetProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
        //20211123 ADD BY ADAM
        public string ActStations = ConfigurationManager.AppSettings["ActStation"].ToString();
        public string ActStartDate = ConfigurationManager.AppSettings["ActStartDate"].ToString();

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
            var LstCarTypes = new List<string>();
            var LstSeats = new List<string>();
            var LstStationIDs = new List<string>();
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租
            var Score = 100;  // 20210910 UPD BY YEH REASON:會員積分，預設100
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
                            LstCarTypes.Add(apiInput.CarType.ToUpper());
                        }

                        if (apiInput.CarTypes != null && apiInput.CarTypes.Count() > 0)
                        {
                            LstCarTypes.AddRange(apiInput.CarTypes.Select(x => x.ToUpper()).ToList());
                        }

                        if (LstCarTypes != null && LstCarTypes.Count() > 0)
                            LstCarTypes = LstCarTypes.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();
                        #endregion

                        //Seat處理
                        //if (apiInput.Seats != null && apiInput.Seats.Count() > 0)
                        //    LstSeats = apiInput.Seats.Select(x => x.ToString()).ToList();

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
            #region Token判斷
            //Token判斷
            if (flag && isGuest == false)
            {
                flag = baseVerify.GetIDNOFromToken(Access_Token, LogID, ref IDNO, ref lstError, ref errCode);
                if (errCode == "ERR101")    //訪客機制BYPASS
                {
                    flag = true;
                    errCode = "000000";
                }
            }
            #endregion

            #region 取得汽車使用中訂閱制月租
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
            #endregion

            #region 取得會員積分
            // 20210910 UPD BY YEH REASON:取得會員積分
            if (flag && !string.IsNullOrEmpty(IDNO))    // IDNO有值才撈積分
            {
                string spName = "usp_GetMemberScore_Q1";

                object[][] parms1 = {
                        new object[] {
                            IDNO,
                            1,
                            10,
                            LogID
                        }
                    };

                DataSet ds1 = null;
                string returnMessage = "";
                string messageLevel = "";
                string messageType = "";

                ds1 = WebApiClient.SPExeBatchMultiArr2(ServerInfo.GetServerInfo(), spName, parms1, true, ref returnMessage, ref messageLevel, ref messageType);

                if (ds1.Tables.Count != 3)
                {
                    if (ds1.Tables.Count == 1)  // SP有回錯誤訊息以SP為主
                    {
                        baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[0].Rows[0]["Error"]), ds1.Tables[0].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);
                    }
                    else
                    {
                        flag = false;
                        errCode = "ERR999";
                        errMsg = returnMessage;
                    }
                }
                else
                {
                    baseVerify.checkSQLResult(ref flag, Convert.ToInt32(ds1.Tables[2].Rows[0]["Error"]), ds1.Tables[2].Rows[0]["ErrorCode"].ToString(), ref lstError, ref errCode);

                    if (flag)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                            Score = Convert.ToInt32(ds1.Tables[0].Rows[0]["SCORE"]);
                        else
                            Score = 0;
                    }
                }
            }
            #endregion

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
                    StationIDs = String.Join(",", LstStationIDs),
                    SD = SDate,
                    ED = EDate,
                    CarTypes = String.Join(",", LstCarTypes),
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
                                   CarHornFlg = a.CarHornFlg,   //2022 ADD BY HANNIE REASON.為了在ContentForAPP欄位文字的最前面補上而取值
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
                               }).OrderByDescending(x => x.IsRent).ThenBy(x => x.Price).ThenBy(x => x.CarType).ToList();    // 20210813 UPD BY YEH REASON:增加排序，排序:IsRent(可>不可)>Price(低>高)>CarType

                    #region 過濾查詢結果
                    if (lstData != null && lstData.Count() > 0)
                        lstData.ForEach(x => { if ((string.IsNullOrWhiteSpace(x.IsRent) ? "" : x.IsRent.ToLower()) == "n") { x.IsShowCard = 0; } });

                    //if (apiInput.Seats != null && apiInput.Seats.Count() > 0)
                    //{
                    //    lstData.ForEach(x => { if (!apiInput.Seats.Contains(x.Seat)) { x.IsRent = "N"; x.IsShowCard = 0; } });
                    //}

                    if (LstCarTypes != null && LstCarTypes.Count() > 0)
                    {
                        lstData.ForEach(x => { if (!LstCarTypes.Any(y => y == x.CarType)) { x.IsRent = "N"; x.IsShowCard = 0; } });
                    }
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
                                CarHornFlg = lstData[0].CarHornFlg,
                                ContentForAPP = lstData[0].CarHornFlg == "N" ? "※尋車僅會閃車燈\n" + lstData[0].ContentForAPP : lstData[0].ContentForAPP,    //2022 ADD BY HANNIE REASON.在欄位文字的最前面補上
                                //ContentForAPP = lstData[0].ContentForAPP,
                                CityName = lstData[0].CityName,
                                AreaName = lstData[0].AreaName,
                                Latitude = lstData[0].Latitude,
                                Longitude = lstData[0].Longitude,
                                StationID = lstData[0].StationID,
                                StationName = lstData[0].StationName,
                                IsRent = lstData[0].IsRent,     //20201027 ADD BY ADAM REASON.抓第一筆判斷是否可租
                                //IsRent = lstData[0].IsRent == "N" ? lstData[0].IsRent : ActStations.IndexOf(lstData[0].StationID) > 0 ? "A":"Y",
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
                                        //IsRent = lstData[i].IsRent == "N" ? lstData[i].IsRent : ActStations.IndexOf(lstData[i].StationID) > 0 ? "A" : "Y",
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
                    if (lstTmpData != null && lstTmpData.Count() > 0)
                    {
                        // 20210813 UPD BY YEH REASON:外層的IsRent過濾要分成車款有值沒值分別判斷
                        if (LstCarTypes != null && LstCarTypes.Count() > 0)
                        {   // INPUT車款有值用所選的車款過濾
                            lstTmpData.ForEach(x =>
                            {
                                var ProjIsRents = x.ProjectObj.Where(y => LstCarTypes.Any(z => z == y.CarType) && y.IsRent == "Y").ToList();
                                x.ProjectObj = x.ProjectObj.Where(y => LstCarTypes.Any(z => z == y.CarType)).ToList();
                                if (ProjIsRents == null || ProjIsRents.Count() == 0)
                                    x.IsRent = "N";
                                else
                                {
                                    //x.IsRent = "Y";
                                    //20211123 aADD BY ADAM REASON.增加魔鬼剋星活動判斷
                                    x.IsRent = (ActStations.IndexOf(x.StationID) > 0 && int.Parse(DateTime.Now.ToString("yyyyMMdd")) > int.Parse(ActStartDate)) ? "A" : "Y";
                                }
                            });
                        }
                        else
                        {   // INPUT車款沒值用據點所有車過濾
                            lstTmpData.ForEach(x =>
                            {
                                var IsRentList = x.ProjectObj.Where(y => y.IsRent == "Y").ToList();
                                if (IsRentList != null && IsRentList.Count() > 0)
                                {
                                    //x.IsRent = "Y";
                                    //20211123 aADD BY ADAM REASON.增加魔鬼剋星活動判斷
                                    x.IsRent = (ActStations.IndexOf(x.StationID) > 0 && int.Parse(DateTime.Now.ToString("yyyyMMdd")) > int.Parse(ActStartDate)) ? "A" : "Y";
                                }
                                else
                                    x.IsRent = "N";
                            });
                        }
                    }
                }
                #endregion

                outputApi = new OAPI_GetProject()
                {
                    GetProjectObj = lstTmpData
                };

                #region 車款,金額下拉,是否有可租
                // 20210910 UPD BY YEH REASON:與APP TEAM確認HasRentCard欄位無使用，因此這段就不用做了
                //bool HaveRentY = false;
                //if (lstData != null && lstData.Count() > 0)
                //    HaveRentY = lstData.Where(y => (string.IsNullOrWhiteSpace(y.IsRent) ? "" : y.IsRent.ToLower()) == "y").Count() > 0;

                //if (lstData != null && lstData.Count() > 0 && HaveRentY)
                //{
                //    if (lstData.Where(x => (string.IsNullOrWhiteSpace(x.IsRent) ? "" : x.IsRent.ToLower()) == "y" && x.IsShowCard == 1).ToList().Count() > 0)
                //        outputApi.HasRentCard = true;
                //    else
                //        outputApi.HasRentCard = false;
                //}
                #endregion

                #region 產出月租&Project虛擬卡片
                bool isSpring = new CarRentCommon().isSpring(SDate, EDate); //是否為春節時段

                if (flag && Score >= 60 && !isSpring)    // 20210910 UPD BY YEH REASON:積分>=60才可使用訂閱制
                {
                    if (outputApi.GetProjectObj != null && outputApi.GetProjectObj.Count() > 0)
                    {
                        var VisProObjs = new List<GetProjectObj>();
                        var ProObjs = outputApi.GetProjectObj;
                        int copyCount = 0;
                        if (InUseMonth != null && InUseMonth.Count() > 0 && ProObjs != null && ProObjs.Count() > 0)
                        {
                            ProObjs.ForEach(x =>
                            {
                                if (x.ProjectObj != null && x.ProjectObj.Count() > 0)
                                {
                                    var newGetProjObj = objUti.Clone(x);
                                    newGetProjObj.ProjectObj = new List<ProjectObj>();
                                    x.ProjectObj.ForEach(y =>
                                    {
                                        y.IsMinimum = 0;    //20210620 ADD BY ADAM REASON.先恢復為0
                                        newGetProjObj.ProjectObj.Add(y);

                                        InUseMonth.ForEach(z =>
                                        {
                                            //只複製一次
                                            if (copyCount > 0) return;
                                            ProjectObj newItem = objUti.Clone(y);

                                            #region 月租卡片欄位給值
                                            //newItem.ProjName += "_" + z.MonProjNM;
                                            //20210706 ADD BY ADAM REASON.改為月租方案名稱顯示
                                            newItem.ProjName = z.MonProjNM;
                                            newItem.CarWDHours = z.WorkDayHours == 0 ? -999 : z.WorkDayHours;
                                            newItem.CarHDHours = z.HolidayHours == 0 ? -999 : z.HolidayHours;
                                            newItem.MotoTotalMins = z.MotoTotalMins;
                                            newItem.WorkdayPerHour = Convert.ToInt32(z.WorkDayRateForCar);
                                            newItem.HolidayPerHour = Convert.ToInt32(z.HoildayRateForCar);
                                            newItem.MonthStartDate = z.StartDate.ToString("yyyy/MM/dd");
                                            newItem.MonthEndDate = z.StartDate.AddDays(30 * z.MonProPeriod).ToString("yyyy/MM/dd");
                                            newItem.MonthlyRentId = z.MonthlyRentId;
                                            newItem.WDRateForCar = z.WorkDayRateForCar;
                                            //newItem.HDRateForCar = z.HoildayRateForCar;
                                            //newItem.HDRateForCar = y.HDRateForCar;//月租假日優惠費率用一般假日優惠費率(前端顯示用)
                                            //20211025 ADD BY ADAM REASON.原本的修改並沒有處理到HDRateForCar，改為使用HolidayPerHour
                                            newItem.HDRateForCar = y.HolidayPerHour;//月租假日優惠費率用一般假日優惠費率(前端顯示用)

                                            newItem.WDRateForMoto = z.WorkDayRateForMoto;
                                            newItem.HDRateForMoto = z.HoildayRateForMoto;
                                            newItem.ProDesc = z.MonProDisc; //20210715 ADD BY ADAM REASON.補上說明欄位
                                            var fn_in = new SPOutput_GetStationCarTypeOfMutiStation()
                                            {
                                                PriceBill = y.Price, //給預設
                                                PROJID = y.ProjID,
                                                CarType = y.CarType,
                                                Price = y.WorkdayPerHour * 10,
                                                PRICE_H = y.HolidayPerHour * 10,
                                            };
                                            newItem.Price = GetPriceBill(fn_in, IDNO, LogID, lstHoliday, SDate, EDate, MonId: z.MonthlyRentId)
                                                        //20211025 ADD BY ADAM REASON.補上里程費跟安心服務計算
                                                        + bill.CarMilageCompute(SDate, EDate, spList.First(a => a.CarType == y.CarType).MilageBase, Mildef, 20, new List<Holiday>())
                                                        + ((apiInput.Insurance == 1) ? bill.CarRentCompute(SDate, EDate, y.InsurancePerHour * 10, y.InsurancePerHour * 10, 10, lstHoliday) : 0);
                                            #endregion

                                            newGetProjObj.ProjectObj.Add(newItem);

                                            copyCount++;
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
                }
                #endregion

                outputApi.GetProjectObj = outputApi.GetProjectObj ?? new List<GetProjectObj>();
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        #region 取得專案
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
            string SPName = "usp_GetStationCarTypeOfMutiStation_Q01";
            SPOutput_Base spOut = new SPOutput_Base();
            SQLHelper<SPInput_GetStationCarTypeOfMutiStation, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetStationCarTypeOfMutiStation, SPOutput_Base>(connetStr);
            DataSet ds = new DataSet();
            flag = sqlHelp.ExeuteSP(SPName, spInput, ref spOut, ref re, ref ds, ref lstError);
            baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
            return re;
        }
        #endregion

        #region 預估金額
        /// <summary>
        /// 預估金額
        /// </summary>
        /// <param name="spItem"></param>
        /// <param name="IDNO">帳號</param>
        /// <param name="LogID"></param>
        /// <param name="lstHoliday"></param>
        /// <param name="SD">起日</param>
        /// <param name="ED">迄日</param>
        /// <param name="funNM"></param>
        /// <param name="MonId"></param>
        /// <returns></returns>
        private int GetPriceBill(SPOutput_GetStationCarTypeOfMutiStation spItem, string IDNO, long LogID, List<Holiday> lstHoliday, DateTime SD, DateTime ED, string funNM = "", Int64 MonId = 0)
        {
            int re = 0;
            var bill = new BillCommon();
            var cr_com = new CarRentCommon();
            var cr_sp = new CarRentSp();
            int PriceBill = spItem.PriceBill;   //先給sp值

            int projType = spItem.PROJTYPE;

            bool isSpr = cr_com.isSpring(SD, ED);
            List<int> carProjTypes = new List<int>() { 0, 3 };
            if (carProjTypes.Any(x => x == projType) && isSpr)  // 專案類型是汽車 且 春節期間
            {
                var bizIn = new IBIZ_SpringInit()
                {
                    IDNO = IDNO,
                    ProjID = spItem.PROJID,
                    ProjType = projType,
                    CarType = spItem.CarType,
                    SD = SD,
                    ED = ED,
                    ProDisPRICE = Convert.ToDouble(spItem.Price) / 10,
                    ProDisPRICE_H = Convert.ToDouble(spItem.PRICE_H) / 10,
                    lstHoliday = lstHoliday,
                    LogID = LogID
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
                    priceN = spItem.Price / 10,
                    priceH = spItem.PRICE_H / 10,
                    daybaseMins = 60,
                    dayMaxHour = 10,
                    lstHoliday = lstHoliday,
                    Discount = 0,
                    FreeMins = 0,
                    ProjID = spItem.PROJID,
                    CarType = spItem.CarType
                };
                re = Convert.ToInt32(new MonSubsCommon().GetCarRentPrice(input));
            }

            return re;
        }
        #endregion
    }
}