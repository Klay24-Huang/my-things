using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.BE.Output;
using Domain.SP.Input.Common;
using Domain.SP.Input.Rent;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.OrderList;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.ComboFunc;
using WebCommon;
using Domain.WebAPI.output.Mochi;
using WebAPI.Utils;
using System.Linq;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 重新計算租金明細(純租金，不折抵)
    /// </summary>
    public class RePayDetailController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        private List<object> traceDts { get; set; } = new List<object>();
        private List<SPInput_CalFinalPrice> jsonDts { get; set; } 
        private List<RePayDetailErrVM> errList { get; set; }

        [HttpPost]
        public Dictionary<string, object> DoRePayDetail(Dictionary<string, object> value)
        {
            #region mark-test
            //test:測試用
            //string orders = "7051420,7051526,7051944,7052135,7052152,7052236,7052401,7052440,7052614,7052616,7052622,7052689,7052691,7052792,7052819,7052835,7052869,7052888,7052895,7052902,7052992,7053142,7053238,7053251,7053299,7053527,7053573,7053618,7053681,7053816,7054079,7054178,7054330,7054406,7054414,7054470,7054551,7054875,7054924,7055243,7055255,7055379,7055509,7055680,7055726,7055749,7055907,7056033,7056046,7056124,7056266,7056386,7056453,7056665,7056760,7056950,7057046,7057566,7057688,7057756,7058017,7058023,7058172,7058198,7058245,7058266,7058365,7058380,7058403,7058496,7058642,7058708,7058791,7059075,7059107,7059139,7059268,7059945,7060379,7060397,7061502,7061539,7061684,7062298,7064045,7080181,7085444,7101988,7109662,7123572,7130186,7142738,7149865,7154358,7161595,7169386,7214397,7224589,7227237,7236718,7241930,7258385,7260135,7264496,7268150,7271848,7273027,7273843,7293135,7320618,7330000,7332737,7341093,7346515,7350375,7350987,7357932,7361530,7365101,7365331,7367757,7367804,7385098,7388946,7391136,7395159,7395618,7397913,7398376,7402079,7413069,7417711,7419364,7428046,7428404,7433687,7434931,7437027,7438542,7439580,7439606,7439857,7439935,7440483,7449048,7449479,7453672,7455553,7459065,7461797,7464439,7465626,7465856,7465977,7466751,7471524,7483462,7493849,7532103";
            //var vv = FixSpring(orders);
            //int vvv = 1;
            #endregion

            #region mark-多筆租金計算
//            string vipJsonSour = @"
//[
//	{
//		'orderNo': 7052691,
//		'idNo': 'C220861844',
//		'strSd': '2021-02-13 09:00:00.000',
//		'strEd': '2021/2/18 9:00 AM',
//        'insPrice':40
//	},
//	{
//		'orderNo': 7052792,
//		'idNo': 'T122771818',
//		'strSd': '2021-02-13 09:50:00.000',
//		'strEd': '2021/2/18 9:50 AM',
//        'insPrice':30
//	},
//	{
//		'orderNo': 7052902,
//		'idNo': 'F123872199',
//		'strSd': '2021-02-11 17:30:00.000',
//		'strEd': '2021/2/16 5:30 PM',
//        'insPrice':30
//	},
//	{
//		'orderNo': 7055509,
//		'idNo': 'S121033452',
//		'strSd': '2021-02-12 09:10:00.000',
//		'strEd': '2021/2/17 9:10 AM',
//        'insPrice':40
//	},
//	{
//		'orderNo': 7057046,
//		'idNo': 'F127828839',
//		'strSd': '2021-02-13 08:30:00.000',
//		'strEd': '2021/2/18 8:30 AM',
//        'insPrice':50
//	},
//	{
//		'orderNo': 7058172,
//		'idNo': 'F227037732',
//		'strSd': '2021-02-15 10:00:00.000',
//		'strEd': '2021/2/20 10:00 AM',
//        'insPrice':40
//	},
//	{
//		'orderNo': 7058403,
//		'idNo': 'F129034915',
//		'strSd': '2021-02-13 08:00:00.000',
//		'strEd': '2021/2/18 8:00 AM',
//        'insPrice':50
//	},
//	{
//		'orderNo': 7059139,
//		'idNo': 'K222820688',
//		'strSd': '2021-02-12 07:30:00.000',
//		'strEd': '2021/2/17 7:30 AM',
//        'insPrice':50
//	},
//	{
//		'orderNo': 7059268,
//		'idNo': 'H220713713',
//		'strSd': '2021-02-12 09:00:00.000',
//		'strEd': '2021/2/17 9:00 AM',
//        'insPrice':50
//	},
//	{
//		'orderNo': 7064045,
//		'idNo': 'A126766693',
//		'strSd': '2021-02-16 12:30:00.000',
//		'strEd': '2021/2/21 12:30 PM',
//        'insPrice':30
//	},
//	{
//		'orderNo': 7169386,
//		'idNo': 'R124797032',
//		'strSd': '2021-02-08 08:30:00.000',
//		'strEd': '2021/2/13 8:30 AM',
//        'insPrice':50
//	}
//]
//            ";
//            vipJsonSour = vipJsonSour.Replace("'", "\"");
//            var vips = new List<IBIZ_ListRentCompute>();
//            vips = JsonConvert.DeserializeObject<List<IBIZ_ListRentCompute>>(vipJsonSour);
//            var list = vips.OrderBy(x => x.orderNo).ToList();
//            string orderNos = string.Join(",", list.Select(x => x.orderNo.ToString()).ToList());
//            var vipre = ListRentCompute(vips);
//            if (vipre != null && vipre.Count() > 0)
//            {
//                var carRepo = new CarRentRepo(connetStr);
//                var ins = (from a in vipre
//                           select new
//                           {
//                               init_price = a.caRent,
//                               InsPrice = a.InsPrice,
//                               OrderNo = a.orderNo
//                           }).ToList();

//                ins.ForEach(x =>
//                {
//                    carRepo.UpdOrderMainByOrderNo(x.OrderNo, x.init_price, x.InsPrice);
//                });
//            }
//            int vipp = 1;
            #endregion

            #region 參數宣告
            jsonDts = new List<SPInput_CalFinalPrice>();
            errList = new List<RePayDetailErrVM>();
            string errCode = "";
            bool isGuest = true;
            bool flag = true;
            var baseVerify = new CommonFunc();
            HttpContext httpContext = HttpContext.Current;
            string funName = "RePayDetailController";
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            string Contentjson = "";
            var inApi = new IAPI_RePayDetailAll();
            var outApi = new OAPI_RePayDetailAll();
            var objOutput = new Dictionary<string, object>();    //輸出
            string errMsg = "ok";
            #endregion

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (!string.IsNullOrWhiteSpace(Contentjson))
                inApi = JsonConvert.DeserializeObject<IAPI_RePayDetailAll>(Contentjson);

            var dts = GetRePayDetails(inApi.SD, inApi.ED, ref errCode, inApi.OrderNos);
           
            if (dts != null && dts.Count() > 0)
            {
                int disc = 0;
                int motoDisc = 0;
                if (dts.Count() == 1)
                {
                    disc = inApi.Discount;
                    motoDisc = inApi.MotorDiscount;
                }

                foreach(var item in dts)
                {
                    var input = new IAPI_RePayDetail()
                    {
                        OrderNo = item.OrderNo,
                        IDNO = item.IDNO,
                        IsSave = inApi.IsSave,
                        RePayMode = inApi.RePayMode,
                        Discount = disc,
                        MotorDiscount =motoDisc,
                        jsonOut = inApi.jsonOut
                    };
                    try
                    {
                        xDoRePayDetail(input);
                    }
                    catch(Exception ex)
                    {
                        var err = objUti.TTMap<RePayDetailVM, RePayDetailErrVM>(item);
                        err.errMsg = ex.Message;
                        errList.Add(err);
                    }
                }
            }

            if (errList != null && errList.Count() > 0)
            {
                errMsg = "含有錯誤資料";
                outApi.MsgData = errList;
                outApi.Result = false;                
            }

            if (inApi.jsonOut == 1 && jsonDts != null && jsonDts.Count() > 0)
            {
                objOutput.Add(nameof(jsonDts), jsonDts);
                return objOutput;
            }
            else if(inApi.jsonOut == 2 && traceDts != null && traceDts.Count() > 0)
            {
                objOutput.Add(nameof(traceDts), traceDts);
                return objOutput;
            }
            else
            {
                objOutput.Add(nameof(outApi), outApi);
                return objOutput;
            }
        }

        /// <summary>
        /// 重算春節預估價格
        /// </summary>
        /// <param name="OrderNos"></param>
        /// <param name="isUpDate"></param>
        /// <returns></returns>
        private string FixSpring(string OrderNos,bool isUpDate = false)
        {
            string re = "";
            var objRe = new List<OrderQueryFullData>(); 
            if (!string.IsNullOrWhiteSpace(OrderNos))
            {
                var bill = new BillCommon();
                var repo = new CarRentRepo(connetStr);
                var lstHoliday = new List<Holiday>();

                List<OrderQueryFullData> sour = repo.GetOrders(OrderNos);
                var months = new List<MonthlyRentData>() {
                   new MonthlyRentData()
                   {
                       MonthlyRentId = 1,
                       StartDate = Convert.ToDateTime("2021-02-09 00:00:00"),
                       EndDate = Convert.ToDateTime("2021-02-17 00:00:00"),
                       WorkDayRateForCar = 220,
                       HoildayRateForCar = 220,
                       Mode = 0
                   }
                };

                var CarProjTypes = new List<int>() { 0, 3};
                if (sour != null && sour.Count() > 0)
                {
                    if (sour.Any(x => !CarProjTypes.Contains(x.ProjType)))
                        throw new Exception("只能汽車");

                    if(sour.Any(x=>x.OrderNo == 0))
                        throw new Exception("單號錯誤");

                    foreach (var s in sour)
                    {
                        DateTime sd = Convert.ToDateTime(s.start_time);
                        DateTime ed = Convert.ToDateTime(s.stop_time);
                        var xre = bill.CarRentInCompute(sd, ed, 99, 168, 60, 10, lstHoliday, months, 0);
                        if(xre != null)
                        {
                            s.init_price = xre.RentInPay;
                            objRe.Add(s);

                            if (isUpDate)
                                repo.SetInitPriceByOrderNo(s);
                        }
                    }

                    re = JsonConvert.SerializeObject(objRe);
                }
            }
            return re;
        }

        private Dictionary<string, object> xDoRePayDetail(IAPI_RePayDetail apiInput)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var cr_sp = new CarRentSp();
            var trace = new TraceCom();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "xDoRePayDetail";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            //IAPI_RePayDetail apiInput = new IAPI_RePayDetail();
            //輸出沿用
            OAPI_GetPayDetail outputApi = new OAPI_GetPayDetail();
            Int64 tmpOrder = -1;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<OrderQueryFullData> OrderDataLists = null;
            int ProjType = 0;
            string Contentjson = "";
            bool isGuest = true;
            int TotalPoint = 0; //總點數
            int MotorPoint = 0; //機車點數
            int CarPoint = 0;   //汽車點數
            string IDNO = "";
            int Discount = 0; //要折抵的點數
            List<Holiday> lstHoliday = null; //假日列表
            DateTime SD = new DateTime();
            DateTime ED = new DateTime();
            DateTime FED = new DateTime();
            DateTime? FineDate = null;
            bool hasFine = false; //是否逾時
            DateTime NowTime = DateTime.Now;
            int TotalRentMinutes = 0; //總租車時數
            int TotalFineRentMinutes = 0; //總逾時時數
            int TotalFineInsuranceMinutes = 0;  //安心服務逾時計算(一天上限超過6小時以10小時計)
            int days = 0; int hours = 0; int mins = 0; //以分計費總時數
            int FineDays = 0; int FineHours = 0; int FineMins = 0; //以分計費總時數
            int PDays = 0; int PHours = 0; int PMins = 0; //將點數換算成天、時、分
            int ActualRedeemableTimePoint = 0; //實際可抵折點數
            int CarRentPrice = 0; //車輛租金
            int MonthlyPoint = 0;   //月租折抵點數        20201128 ADD BY ADAM 
            int MonthlyPrice = 0;   //月租折抵換算金額      20201128 ADD BY ADAM 
            int TransferPrice = 0;      //轉乘優惠折抵金額  20201201 ADD BY ADAM
            MonthlyRentRepository monthlyRentRepository = new MonthlyRentRepository(connetStr);
            BillCommon billCommon = new BillCommon();
            List<MonthlyRentData> monthlyRentDatas = new List<MonthlyRentData>(); //月租列表
            bool UseMonthMode = false;  //false:無月租;true:有月租
            int InsurancePerHours = 0;  //安心服務每小時價
            int etagPrice = 0;      //ETAG費用 20201202 ADD BY ADAM
            CarRentInfo carInfo = new CarRentInfo();//車資料
            int ParkingPrice = 0;       //車麻吉停車費    20201209 ADD BY ADAM

            double nor_car_wDisc = 0;//只有一般時段時平日折扣
            double nor_car_hDisc = 0;//只有一般時段時價日折扣
            int nor_car_PayDisc = 0;//只有一般時段時總折扣
            int nor_car_PayDiscPrice = 0;//只有一般時段時總折扣金額

            int gift_point = 0;//使用時數(汽車)
            int gift_motor_point = 0;//使用時數(機車)
            int motoBaseMins = 6;//機車基本分鐘數
            int motoMaxMins = 200;//機車單日最大分鐘數 
            int carBaseMins = 60;//汽車基本分鐘數

            var neverHasFine = new List<int>() { 3, 4 };//路邊,機車不會逾時
            bool isSpring = false;//是否為春節時段
            var visMons = new List<MonthlyRentData>();//虛擬月租
            DateTime sprSD = Convert.ToDateTime(SiteUV.strSpringSd);
            DateTime sprED = Convert.ToDateTime(SiteUV.strSpringEd);
            int UseOrderPrice = 0;//使用訂金(4捨5入)
            #endregion

            #region trace
            if (Int32.TryParse(apiInput.OrderNo, out int ordNo))
                trace.OrderNo = ordNo;
            trace.traceAdd(nameof(apiInput), apiInput);
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(null, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            flag = true;
            errCode = "000000";
            Contentjson = JsonConvert.SerializeObject(apiInput);

            if (flag)
            {
                //apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_RePayDetail>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                var input = new IBIZ_InCheck()
                {
                    OrderNo = apiInput.OrderNo,
                    Discount = apiInput.Discount,
                    MotorDiscount = apiInput.MotorDiscount,
                    isGuest = isGuest,
                };
                var inck_re = cr_com.InCheck(input);
                if (inck_re != null)
                {
                    flag = inck_re.flag;
                    errCode = inck_re.errCode;
                    Discount = inck_re.Discount;
                    tmpOrder = inck_re.longOrderNo;
                }
            }

            #endregion
            #region 取出基本資料           
            if (flag)
            {               
                IDNO = apiInput.IDNO == null ? "" : apiInput.IDNO;

                #region 取出訂單資訊
                if (flag)
                {
                    SPInput_BE_GetOrderStatusByOrderNo spInput = new SPInput_BE_GetOrderStatusByOrderNo()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        LogID = LogID,
                        UserID = "99998"
                    };
                    string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
                    SPOutput_Base spOutBase = new SPOutput_Base();
                    SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_BE_GetOrderStatusByOrderNo, SPOutput_Base>(connetStr);
                    OrderDataLists = new List<OrderQueryFullData>();
                    DataSet ds = new DataSet();
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        OrderDataLists = objUti.ConvertToList<OrderQueryFullData>(ds.Tables[0]);
                    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                    //判斷訂單狀態
                    if (flag)
                    {
                        if (OrderDataLists.Count == 0)
                        {
                            flag = false;
                            errCode = "ERR203";
                        }
                    }
                }

                #endregion

                if (OrderDataLists != null && OrderDataLists.Count() > 0)
                {
                    var OrderData = OrderDataLists[0];
                    motoBaseMins = OrderData.BaseMinutes > 0 ? OrderData.BaseMinutes : motoBaseMins;
                    ProjType = OrderData.ProjType;
                    UseOrderPrice = OrderData.UseOrderPrice;
                    trace.FlowList.Add("取出基本資料");
                    trace.traceAdd(nameof(OrderData), OrderData);
                }

                if (ProjType != 4)
                    Discount = apiInput.Discount;
            }
            #endregion

            #region 第二階段判斷及計價
            if (flag)
            {
                //取得專案狀態
                if (flag)
                {
                    ProjType = OrderDataLists[0].ProjType;
                    SD = Convert.ToDateTime(OrderDataLists[0].final_start_time);
                    SD = SD.AddSeconds(SD.Second * -1); //去秒數

                    if (!string.IsNullOrWhiteSpace(OrderDataLists[0].fine_Time) && Convert.ToDateTime(OrderDataLists[0].fine_Time) > Convert.ToDateTime("1911-01-01 00:00:00"))
                    {
                        FineDate = Convert.ToDateTime(OrderDataLists[0].fine_Time);
                        FineDate = FineDate.Value.AddSeconds(ED.Second * -1); //去秒數
                    }

                    //機車路邊不計算預計還車時間
                    if (neverHasFine.Contains(OrderDataLists[0].ProjType))
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                        FineDate = null;
                    }
                    else
                    {
                        ED = Convert.ToDateTime(OrderDataLists[0].stop_time);
                        ED = ED.AddSeconds(ED.Second * -1); //去秒數
                    }

                    FED = Convert.ToDateTime(OrderDataLists[0].final_stop_time);
                    FED = FED.AddSeconds(FED.Second * -1);  //去秒數
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));

                    if (FineDate != null)
                    {
                        if (FED > FineDate)
                        {
                            hasFine = true;
                            ED = FineDate.Value;
                        }
                    }
                    else
                    {
                        if (FED.Subtract(ED).Ticks > 0)
                            hasFine = true;
                    }

                    #region trace

                    var timeComp = new
                    {
                        SD = SD,
                        ED = ED,
                        FED = FED,
                        FineDate = FineDate,
                        hasFine = hasFine
                    };
                    trace.FlowList.Add("時間計算");
                    trace.traceAdd(nameof(timeComp), timeComp);

                    #endregion

                    #region 春節不能使用折扣點數
                    //dev: 春節不能使用折扣點數 
                    var vsd = new DateTime();
                    var ved = new DateTime();
                    if (neverHasFine.Contains(ProjType))
                    {
                        isSpring = cr_com.isSpring(SD, ED);
                        vsd = SD;
                        ved = ED;
                    }
                    else
                    {
                        isSpring = cr_com.isSpring(SD, FED);
                        vsd = SD;
                        ved = FED;
                    }
                    if (isSpring)
                    {
                        Discount = 0;
                        apiInput.Discount = 0;
                        apiInput.MotorDiscount = 0;
                    }
                    #endregion

                    #region 計算非逾時及逾時時間

                    if (ProjType == 4)
                    {
                        //春前
                        if (ED <= sprSD)
                        {
                            var xre = billCommon.GetMotoRangeMins(SD, ED, 6, 200, lstHoliday);
                            TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                            var xDays = ED.Subtract(SD).TotalDays;
                            if (xDays > 1)
                                TotalRentMinutes -= 1;
                        }
                        //前跨春
                        else if (SD < sprSD && ED > sprSD)
                        {
                            var bef = billCommon.GetMotoRangeMins(SD, sprSD, 6, 200, lstHoliday);
                            TotalFineRentMinutes += Convert.ToInt32(Math.Floor(bef.Item1 + bef.Item2));
                            var xDays = sprSD.Subtract(SD).TotalDays;
                            if (xDays > 1)
                                TotalFineRentMinutes -= 1;
                            var af = billCommon.GetMotoRangeMins(sprSD, ED, 6, 600, lstHoliday);
                            TotalFineRentMinutes += Convert.ToInt32(Math.Floor(bef.Item1 + bef.Item2));
                        }
                        else
                        {
                            var xre = billCommon.GetMotoRangeMins(SD, ED, 6, 600, lstHoliday);
                            TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                        }
                    }
                    else
                    {
                        if (hasFine)
                        {
                            var xre = billCommon.GetCarRangeMins(SD, ED, 60, 600, lstHoliday);
                            if (xre != null)
                                TotalRentMinutes += Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                            var ov_re = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                            if (ov_re != null)
                                TotalRentMinutes += Convert.ToInt32(Math.Floor(ov_re.Item1 + ov_re.Item2));
                        }
                        else
                        {
                            var xre = billCommon.GetCarRangeMins(SD, FED, 60, 600, lstHoliday);
                            if (xre != null)
                                TotalRentMinutes = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                        }
                    }
                    TotalRentMinutes = TotalRentMinutes > 0 ? TotalRentMinutes : 0;

                    #endregion

                    #region 取得虛擬月租
                    //dev:取得虛擬月租

                    if (OrderDataLists != null && OrderDataLists.Count() > 0)
                    {
                        var item = OrderDataLists[0];
                        //春節專案才產生虛擬月租
                        if (isSpring)
                        {
                            var ibiz_vMon = new IBIZ_SpringInit()
                            {
                                SD = vsd,
                                ED = ved,
                                //ProjID = item.ProjID,
                                ProjType = item.ProjType,
                                CarType = item.CarTypeGroupCode,
                                ProDisPRICE = item.PRICE,
                                ProDisPRICE_H = item.PRICE_H
                            };
                            var vmonRe = cr_com.GetVisualMonth(ibiz_vMon);
                            if (vmonRe != null)
                            {
                                if (vmonRe.VisMons != null && vmonRe.VisMons.Count() > 0)
                                {
                                    visMons = vmonRe.VisMons;
                                    OrderDataLists[0].PRICE = Convert.ToInt32(Math.Floor(vmonRe.PRICE));
                                    OrderDataLists[0].PRICE_H = Convert.ToInt32(Math.Floor(vmonRe.PRICE_H));
                                    trace.traceAdd(nameof(vmonRe), vmonRe);
                                    trace.FlowList.Add("新增虛擬月租");
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region 不執行Api呼叫 與短租查時數 - 春節不執行
                if (false && flag && !isSpring)
                {
                    var inp = new IBIZ_NPR270Query()
                    {
                        IDNO = IDNO
                    };
                    var re270 = cr_com.NPR270Query(inp);
                    if (re270 != null)
                    {
                        trace.traceAdd(nameof(re270), re270);
                        flag = re270.flag;
                        MotorPoint = re270.MotorPoint;
                        CarPoint = re270.CarPoint;
                    }
                    trace.FlowList.Add("與短租查時數");

                    //判斷輸入的點數有沒有超過總點數
                    if (ProjType == 4)
                    {
                        if (Discount > 0 && Discount < OrderDataLists[0].BaseMinutes)   // 折抵點數 < 基本分鐘數
                        {
                            //flag = false;
                            //errCode = "ERR205";
                        }
                        else
                        {
                            if (Discount > (MotorPoint + CarPoint)) // 折抵點數 > (機車點數 + 汽車點數)
                            {

                                flag = false;
                                errCode = "ERR207";
                            }
                        }

                        if (TotalRentMinutes <= 6 && Discount == 6)
                        {

                        }
                        else if (Discount > (TotalRentMinutes + TotalFineRentMinutes))   // 折抵時數 > (總租車時數 + 總逾時時數)
                        {
                            flag = false;
                            errCode = "ERR303";
                        }
                        trace.FlowList.Add("機車一般點數檢查");
                    }
                    else
                    {
                        if (Discount > 0 && Discount % 30 > 0)
                        {
                            flag = false;
                            errCode = "ERR206";
                        }
                        else
                        {
                            if (Discount > CarPoint)
                            {
                                flag = false;
                                errCode = "ERR207";
                            }
                        }
                        trace.FlowList.Add("汽車一般點數檢查");
                    }
                }
                #endregion

                #region 汽車計費資訊
                int car_payAllMins = 0; //全部計費租用分鐘
                int car_payInMins = 0;//未超時計費分鐘
                int car_payOutMins = 0;//超時分鐘-顯示用
                int car_inPrice = 0;//未超時費用
                int car_outPrice = 0;//超時費用
                int car_n_price = OrderDataLists[0].PRICE;
                int car_h_price = OrderDataLists[0].PRICE_H;

                if (flag)
                {
                    if (ProjType != 4)
                    {
                        if (hasFine)
                        {
                            var ord = OrderDataLists[0];
                            var xre = billCommon.GetCarOutComputeMins(ED, FED, 0, 360, lstHoliday);
                            if (xre != null)
                                car_payOutMins = Convert.ToInt32(Math.Floor(xre.Item1 + xre.Item2));
                            car_outPrice = billCommon.CarRentCompute(ED, FED, ord.WeekdayPrice, ord.HoildayPrice, 6, lstHoliday, true, 0);
                            car_payAllMins += car_payOutMins;

                            var car_re = billCommon.CarRentInCompute(SD, ED, car_n_price, car_h_price, 60, 10, lstHoliday, new List<MonthlyRentData>(), Discount);
                            if (car_re != null)
                            {
                                trace.traceAdd(nameof(car_re), car_re);
                                car_payAllMins += car_re.RentInMins;
                                car_payInMins = car_re.RentInMins;
                                car_inPrice = car_re.RentInPay;
                                nor_car_PayDisc = car_re.useDisc;
                            }
                        }
                        else
                        {
                            var car_re = billCommon.CarRentInCompute(SD, FED, car_n_price, car_h_price, 60, 10, lstHoliday, new List<MonthlyRentData>(), Discount);
                            if (car_re != null)
                            {
                                trace.traceAdd(nameof(car_re), car_re);

                                car_payAllMins += car_re.RentInMins;
                                car_payInMins = car_re.RentInMins;
                                car_inPrice = car_re.RentInPay;
                                nor_car_PayDisc = car_re.useDisc;
                            }
                        }

                        trace.FlowList.Add("汽車計費資訊(非月租)");
                    }
                }

                #endregion

                #region mark-查資料庫最後資料 
                //查資料庫最後資料
                //if (flag)
                //{
                //    SPInput_GetGIFTMINS spInput = new SPInput_GetGIFTMINS()
                //    {
                //        MEMIDNO = IDNO
                //    };
                //    string SPName = new ObjType().GetSPName(ObjType.SPType.BE_GetOrderStatusByOrderNo);
                //    SPOutput_GetGIFTMINS spOutBase = new SPOutput_GetGIFTMINS();
                //    SQLHelper<SPInput_GetGIFTMINS, SPOutput_GetGIFTMINS> sqlHelpQuery = new SQLHelper<SPInput_GetGIFTMINS, SPOutput_GetGIFTMINS>(connetStr);
                //    //OrderDataLists = new List<OrderQueryFullData>();
                //    DataSet ds = new DataSet();
                //    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref ds, ref lstError);
                //    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);
                //    //判斷訂單狀態
                //    if (flag)
                //    {
                //        if (OrderDataLists.Count == 0)
                //        {
                //            flag = false;
                //            errCode = "ERR203";
                //        }
                //    }
                //}
                #endregion

                #region 建空模及塞入要輸出的值
                if (flag)
                {
                    outputApi.CanUseDiscount = 1;   //先暫時寫死，之後改專案設定，由專案設定引入
                    outputApi.CanUseMonthRent = 1;  //先暫時寫死，之後改專案設定，由專案設定引入
                    outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase();
                    outputApi.DiscountAlertMsg = "";
                    outputApi.IsMonthRent = 0;  //先暫時寫死，之後改專案設定，由專案設定引入，第二包才會引入月租專案
                    outputApi.IsMotor = (ProjType == 4) ? 1 : 0;    //是否為機車
                    outputApi.MonthRent = new Models.Param.Output.PartOfParam.MonthRentBase();  //月租資訊
                    outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase();  //機車資訊
                    outputApi.PayMode = (ProjType == 4) ? 1 : 0;    //目前只有機車才會有以分計費模式
                    outputApi.ProType = ProjType;
                    outputApi.Rent = new Models.Param.Output.PartOfParam.RentBase() //訂單基本資訊
                    {
                        BookingEndDate = ED.ToString("yyyy-MM-dd HH:mm:ss"),
                        BookingStartDate = SD.ToString("yyyy-MM-dd HH:mm:ss"),
                        CarNo = OrderDataLists[0].CarNo,
                        RedeemingTimeCarInterval = CarPoint.ToString(),
                        RedeemingTimeMotorInterval = MotorPoint.ToString(),
                        RedeemingTimeInterval = (ProjType == 4) ? (CarPoint + MotorPoint).ToString() : CarPoint.ToString(),
                        RentalDate = FED.ToString("yyyy-MM-dd HH:mm:ss"),
                        RentalTimeInterval = (TotalRentMinutes + TotalFineRentMinutes).ToString(),
                    };

                    if (ProjType == 4)
                    {
                        TotalPoint = (CarPoint + MotorPoint);
                        outputApi.MotorRent = new Models.Param.Output.PartOfParam.MotorRentBase()
                        {
                            BaseMinutePrice = OrderDataLists[0].BaseMinutesPrice,
                            BaseMinutes = OrderDataLists[0].BaseMinutes,
                            MinuteOfPrice = OrderDataLists[0].MinuteOfPrice
                        };
                    }
                    else
                    {
                        TotalPoint = CarPoint;
                        outputApi.CarRent = new Models.Param.Output.PartOfParam.CarRentBase()
                        {
                            HoildayOfHourPrice = OrderDataLists[0].PRICE_H,
                            HourOfOneDay = 10,
                            WorkdayOfHourPrice = OrderDataLists[0].PRICE,
                            WorkdayPrice = OrderDataLists[0].PRICE * 10,
                            MilUnit = OrderDataLists[0].MilageUnit,
                            HoildayPrice = OrderDataLists[0].PRICE_H * 10
                        };
                    }
                    //20201201 ADD BY ADAM REASON.轉乘優惠
                    TransferPrice = OrderDataLists[0].init_TransDiscount;
                    trace.FlowList.Add("建空模");
                }
                if (false && flag && OrderDataLists[0].ProjType != 4) //20201224 add by adam 問題未確定前先關掉車麻吉
                {
                    var input = new IBIZ_CarMagi()
                    {
                        LogID = LogID,
                        CarNo = OrderDataLists[0].CarNo,
                        SD = SD,
                        ED = FED.AddDays(1)
                    };
                    var magi_Re = cr_com.CarMagi(input);
                    if (magi_Re != null)
                    {
                        trace.traceAdd(nameof(magi_Re), magi_Re);
                        flag = magi_Re.flag;
                        outputApi.Rent.ParkingFee = magi_Re.ParkingFee;
                    }
                    trace.FlowList.Add("車麻吉");
                }

                #endregion

                #region 月租
                //note: 月租GetPayDetail
                if (flag)
                {
                    var item = OrderDataLists[0];
                    item = cr_com.dbValeFix(item);
                    var motoDayMaxMinns = 200;
                    var input = new IBIZ_MonthRent()
                    {
                        IDNO = IDNO,
                        LogID = LogID,
                        intOrderNO = tmpOrder,
                        ProjType = item.ProjType,
                        MotoDayMaxMins = motoDayMaxMinns,
                        MinuteOfPrice = item.MinuteOfPrice,
                        hasFine = hasFine,
                        SD = SD,
                        ED = ED,
                        FED = FED,
                        MotoBaseMins = motoBaseMins,
                        lstHoliday = lstHoliday,
                        Discount = Discount,
                        PRICE = item.PRICE,
                        PRICE_H = item.PRICE_H,
                        carBaseMins = 60
                    };

                    if (visMons != null && visMons.Count() > 0)
                        input.VisMons = visMons;

                    var mon_re = cr_com.MonthRentNoSave(input);
                    if (mon_re != null)
                    {
                        trace.traceAdd(nameof(mon_re), mon_re);
                        flag = mon_re.flag;
                        UseMonthMode = mon_re.UseMonthMode;
                        outputApi.IsMonthRent = mon_re.IsMonthRent;
                        if (UseMonthMode)
                        {
                            carInfo = mon_re.carInfo;
                            Discount = mon_re.useDisc;
                            monthlyRentDatas = mon_re.monthlyRentDatas;

                            if (ProjType == 4)
                                outputApi.Rent.CarRental = mon_re.CarRental;//機車用
                            else
                                CarRentPrice += mon_re.CarRental;//汽車用
                        }
                    }

                    trace.FlowList.Add("月租");
                }
                #endregion

                #region 開始計價
                if (flag)
                {
                    trace.FlowList.Add("開始計價");
                    lstHoliday = new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), FED.ToString("yyyyMMdd"));
                    if (ProjType == 4)
                    {
                        if (UseMonthMode)   //true:有月租;false:無月租
                        {
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas[0].HoildayRateForMoto;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas[0].WorkDayRateForMoto;
                        }
                        else
                        {
                            var item = OrderDataLists[0];

                            //春前
                            if (ED <= sprSD)
                            {
                                var xre = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, 6, 200, lstHoliday, new List<MonthlyRentData>(), Discount, 199, 300);
                                if (xre != null)
                                {
                                    carInfo = xre;
                                    outputApi.Rent.CarRental = xre.RentInPay;
                                    Discount = xre.useDisc;
                                }
                            }
                            //前跨春
                            else if (SD < sprSD && ED > sprSD)
                            {
                                carInfo = new CarRentInfo();
                                double bef_PriW = 0;
                                double bef_PriH = 0;
                                var bef_re = cr_sp.sp_GetEstimate("P686", item.CarTypeGroupCode, LogID, ref errMsg);
                                if (bef_re != null)
                                {
                                    bef_PriW = bef_re.PRICE;
                                    bef_PriH = bef_re.PRICE_H;
                                }
                                else
                                    throw new Exception("moto平日無對應價格");

                                var bef = billCommon.MotoRentMonthComp(SD, sprSD, bef_PriW, bef_PriH, 6, 200, lstHoliday, new List<MonthlyRentData>(), Discount, 199, 300);
                                if (bef != null)
                                {
                                    outputApi.Rent.CarRental += bef.RentInPay;
                                    carInfo.useDisc += bef.useDisc;
                                    carInfo.RentInPay += bef.RentInPay;
                                    carInfo.useMonthDisc += bef.useMonthDisc;
                                    carInfo.RentInMins += bef.RentInMins;
                                    carInfo.AfterDiscRentInMins += bef.AfterDiscRentInMins;
                                    carInfo.useMonthDiscW += bef.useMonthDiscW;
                                    carInfo.useMonthDiscH += bef.useMonthDiscH;
                                    Discount -= bef.useDisc;
                                    Discount = Discount > 0 ? Discount : 0;
                                }
                                var af = billCommon.MotoRentMonthComp(sprSD, ED, item.MinuteOfPrice, item.MinuteOfPrice, 6, 600, lstHoliday, new List<MonthlyRentData>(), Discount, 600, 901);
                                if (af != null)
                                {
                                    outputApi.Rent.CarRental += af.RentInPay;
                                    carInfo.useDisc += af.useDisc;
                                    carInfo.RentInPay += af.RentInPay;
                                    carInfo.useMonthDisc += af.useMonthDisc;
                                    carInfo.RentInMins += af.RentInMins;
                                    carInfo.AfterDiscRentInMins += af.AfterDiscRentInMins;
                                    carInfo.useMonthDiscW += af.useMonthDiscW;
                                    carInfo.useMonthDiscH += af.useMonthDiscH;
                                    Discount -= af.useDisc;
                                    Discount = Discount > 0 ? Discount : 0;
                                }
                            }
                            //春後
                            else if (SD >= sprSD)
                            {
                                var xre = billCommon.MotoRentMonthComp(SD, ED, item.MinuteOfPrice, item.MinuteOfPrice, 6, 600, lstHoliday, new List<MonthlyRentData>(), Discount, 600, 901);
                                if (xre != null)
                                {
                                    carInfo = xre;
                                    outputApi.Rent.CarRental = xre.RentInPay;
                                    carInfo.useDisc = xre.useDisc;
                                }
                            }

                            if (carInfo != null)
                                outputApi.Rent.CarRental = carInfo.RentInPay;

                            trace.FlowList.Add("機車非月租租金計算");
                        }

                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                    }
                    else
                    {
                        if (UseMonthMode)
                        {
                            outputApi.Rent.CarRental = CarRentPrice;
                            outputApi.MonthRent.HoildayRate = monthlyRentDatas == null ? 0 : monthlyRentDatas[0].HoildayRateForCar;
                            outputApi.MonthRent.WorkdayRate = monthlyRentDatas == null ? 0 : monthlyRentDatas[0].WorkDayRateForCar;
                        }
                        else
                        {
                            CarRentPrice = car_inPrice;//未逾時租用費用
                            if (hasFine)
                                outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                            trace.FlowList.Add("汽車非月租金額給值");
                        }

                        if (Discount > 0)
                        {
                            double n_price = Convert.ToDouble(OrderDataLists[0].PRICE);
                            double h_price = Convert.ToDouble(OrderDataLists[0].PRICE_H);

                            if (UseMonthMode)
                            {

                            }
                            else
                            {
                                //非月租折扣
                                //CarRentPrice -= nor_car_PayDiscPrice;
                                CarRentPrice = CarRentPrice > 0 ? CarRentPrice : 0;
                                trace.FlowList.Add("汽車非月租折扣扣除");
                            }
                        }
                        //安心服務
                        InsurancePerHours = OrderDataLists[0].Insurance == 1 ? Convert.ToInt32(OrderDataLists[0].InsurancePerHours) : 0;
                        if (InsurancePerHours > 0)
                        {
                            outputApi.Rent.InsurancePurePrice = Convert.ToInt32(Math.Floor(((car_payInMins / 30.0) * InsurancePerHours / 2)));

                            //逾時安心服務計算
                            if (TotalFineRentMinutes > 0)
                            {
                                outputApi.Rent.InsuranceExtPrice = Convert.ToInt32(Math.Floor(((car_payOutMins / 30.0) * InsurancePerHours / 2)));
                            }
                        }
                        trace.FlowList.Add("安心服務");

                        outputApi.Rent.CarRental = CarRentPrice;
                        outputApi.Rent.RentBasicPrice = OrderDataLists[0].BaseMinutesPrice;
                        outputApi.CarRent.MilUnit = (OrderDataLists[0].MilageUnit <= 0) ? Mildef : OrderDataLists[0].MilageUnit;
                        //里程費計算修改，遇到取不到里程數的先以0元為主
                        //outputApi.Rent.MileageRent = OrderDataLists[0].end_mile == 0 ? 0 : Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                        // 20201218 因應車機回應異常，因此判斷起始里程/結束里程有一個是0或里程數>1000公里，均先列為異常，不計算里程費，待系統穩定後再將這段判斷移除
                        // 20210119 里程數>1000公里的判斷移除
                        if (OrderDataLists[0].start_mile == 0 || OrderDataLists[0].end_mile == 0 || ((OrderDataLists[0].end_mile - OrderDataLists[0].start_mile) < 0))
                        {
                            outputApi.Rent.MileageRent = 0;
                        }
                        else
                        {
                            outputApi.Rent.MileageRent = Convert.ToInt32(OrderDataLists[0].MilageUnit * (OrderDataLists[0].end_mile - OrderDataLists[0].start_mile));
                        }
                        trace.FlowList.Add("里程費計算");
                    }

                    outputApi.Rent.ActualRedeemableTimeInterval = ActualRedeemableTimePoint.ToString();
                    outputApi.Rent.RemainRentalTimeInterval = (TotalRentMinutes > 0 ? TotalRentMinutes : 0).ToString();
                    outputApi.Rent.TransferPrice = (OrderDataLists[0].init_TransDiscount > 0) ? OrderDataLists[0].init_TransDiscount : 0;
                    //20201202 ADD BY ADAM REASON.ETAG費用
                    outputApi.Rent.ETAGRental = etagPrice;

                    var xTotalRental = outputApi.Rent.CarRental + outputApi.Rent.ParkingFee + outputApi.Rent.MileageRent + outputApi.Rent.OvertimeRental + outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice - outputApi.Rent.TransferPrice + outputApi.Rent.ETAGRental;
                    xTotalRental -= UseOrderPrice;//預繳定金扣抵
                    xTotalRental = xTotalRental < 0 ? 0 : xTotalRental;
                    outputApi.Rent.TotalRental = xTotalRental;
                    trace.FlowList.Add("總價計算");

                    #region 修正輸出欄位
                    //note: 修正輸出欄位PayDetail
                    outputApi.UseOrderPrice = UseOrderPrice;
                    if (ProjType == 4)
                    {
                        outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                        outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                        outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

                        //2020-12-29 所有點數改成皆可折抵
                        //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                        outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

                        outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數

                        var cDisc = apiInput.Discount;
                        var mDisc = apiInput.MotorDiscount;
                        if (carInfo.useDisc > 0)
                        {
                            int lastDisc = carInfo.useDisc;
                            var useMdisc = mDisc > carInfo.useDisc ? carInfo.useDisc : mDisc;
                            lastDisc -= useMdisc;
                            gift_motor_point = useMdisc;
                            if (lastDisc > 0)
                            {
                                var useCdisc = cDisc > lastDisc ? lastDisc : cDisc;
                                lastDisc -= useCdisc;
                                gift_point = useCdisc;
                            }
                        }

                    }
                    else
                    {
                        if (UseMonthMode)
                        {
                            outputApi.Rent.UseMonthlyTimeInterval = carInfo.useMonthDisc.ToString();
                            outputApi.Rent.UseNorTimeInterval = carInfo.useDisc.ToString();
                            outputApi.Rent.RentalTimeInterval = (carInfo.RentInMins).ToString();//租用時數(未逾時)

                            //2020 - 12 - 29 所有點數改成皆可折抵
                            //outputApi.Rent.ActualRedeemableTimeInterval = carInfo.DiscRentInMins.ToString();//可折抵租用時數
                            outputApi.Rent.ActualRedeemableTimeInterval = carInfo.AfterDiscRentInMins.ToString();

                            outputApi.Rent.RemainRentalTimeInterval = carInfo.AfterDiscRentInMins.ToString();//未逾時折扣後的租用時數
                            if (carInfo != null && carInfo.useDisc > 0)
                                gift_point = carInfo.useDisc;
                        }
                        else
                        {
                            outputApi.Rent.UseNorTimeInterval = Discount.ToString();
                            outputApi.Rent.RentalTimeInterval = car_payInMins.ToString(); //租用時數(未逾時)
                            outputApi.Rent.ActualRedeemableTimeInterval = Convert.ToInt32(car_payInMins).ToString();//可折抵租用時數
                            outputApi.Rent.RemainRentalTimeInterval = (car_payInMins - Discount).ToString();//未逾時折抵後的租用時數
                            gift_point = nor_car_PayDisc;
                        }

                        gift_motor_point = 0;
                        outputApi.Rent.OvertimeRental = car_outPrice;//逾時費用
                    }
                    trace.FlowList.Add("修正輸出欄位");
                    #endregion

                    #region trace
                    trace.FlowList.Add("修正輸出欄位");
                    trace.traceAdd(nameof(outputApi), outputApi);
                    var tempSpIn = new
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        final_price = outputApi.Rent.TotalRental,
                        pure_price = outputApi.Rent.CarRental,
                        mileage_price = outputApi.Rent.MileageRent,
                        Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                        fine_price = outputApi.Rent.OvertimeRental,
                        gift_point = apiInput.Discount,
                        gift_motor_point = apiInput.MotorDiscount,
                        monthly_workday = carInfo.useMonthDiscW,
                        monthly_holiday = carInfo.useMonthDiscH,
                        Etag = outputApi.Rent.ETAGRental,
                        parkingFee = outputApi.Rent.ParkingFee,
                        TransDiscount = outputApi.Rent.TransferPrice,
                        LogID = LogID
                    };
                    trace.traceAdd(nameof(tempSpIn), tempSpIn);
                    #endregion

                    if (apiInput.IsSave == 1)
                    {
                        string SPName = new ObjType().GetSPName(ObjType.SPType.CalFinalPrice_Re);
                        SPInput_CalFinalPrice SPInput = new SPInput_CalFinalPrice()
                        {
                            IDNO = IDNO,
                            OrderNo = tmpOrder,
                            final_price = outputApi.Rent.TotalRental,
                            pure_price = outputApi.Rent.CarRental,
                            mileage_price = outputApi.Rent.MileageRent,
                            Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                            fine_price = outputApi.Rent.OvertimeRental,
                            gift_point = apiInput.Discount,
                            gift_motor_point = apiInput.MotorDiscount,
                            monthly_workday = carInfo.useMonthDiscW,
                            monthly_holiday = carInfo.useMonthDiscH,
                            Etag = outputApi.Rent.ETAGRental,
                            parkingFee = outputApi.Rent.ParkingFee,
                            TransDiscount = outputApi.Rent.TransferPrice,
                            LogID = LogID
                        };
                        trace.traceAdd(nameof(SPInput), SPInput);
                        trace.FlowList.Add("sp存檔");
                        SPOutput_Base SPOutput = new SPOutput_Base();
                        SQLHelper<SPInput_CalFinalPrice, SPOutput_Base> SQLBookingStartHelp = new SQLHelper<SPInput_CalFinalPrice, SPOutput_Base>(connetStr);
                        flag = SQLBookingStartHelp.ExecuteSPNonQuery(SPName, SPInput, ref SPOutput, ref lstError);
                        baseVerify.checkSQLResult(ref flag, ref SPOutput, ref lstError, ref errCode);
                    }
                }
                #endregion

            }
            #endregion

            #region trace

            //輸出json顯示
            if (apiInput.jsonOut == 1)
            {
                if (jsonDts != null)
                {
                    var jItem = new SPInput_CalFinalPrice()
                    {
                        IDNO = IDNO,
                        OrderNo = tmpOrder,
                        final_price = outputApi.Rent.TotalRental,
                        pure_price = outputApi.Rent.CarRental,
                        mileage_price = outputApi.Rent.MileageRent,
                        Insurance_price = outputApi.Rent.InsurancePurePrice + outputApi.Rent.InsuranceExtPrice,
                        fine_price = outputApi.Rent.OvertimeRental,
                        gift_point = apiInput.Discount,
                        gift_motor_point = apiInput.MotorDiscount,
                        Etag = outputApi.Rent.ETAGRental,
                        parkingFee = outputApi.Rent.ParkingFee,
                        TransDiscount = outputApi.Rent.TransferPrice,
                        LogID = LogID
                    };
                    jsonDts.Add(jItem);
                }
            }
            else if (apiInput.jsonOut == 2)
            {
                trace.objs = trace.getObjs();
                traceDts.Add(trace);
            }

            #endregion

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                var errItem = new RePayDetailErrVM()
                {
                    OrderNo = apiInput.OrderNo,
                    IDNO = apiInput.IDNO,
                    errMsg = errCode
                };
                errList.Add(errItem);
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }

        private List<RePayDetailVM> GetRePayDetails(DateTime SD, DateTime ED, ref string errMsg, string OrderNos="")
        {
            List<RePayDetailVM> saveDetail = new List<RePayDetailVM>();

            string SPName = new ObjType().GetSPName(ObjType.SPType.GetRePayList);

            object[] param = new object[3];
            param[0] = SD;
            param[1] = ED;
            param[2] = OrderNos;

            DataSet ds1 = null;
            string returnMessage = "";
            string messageLevel = "";
            string messageType = "";

            ds1 = WebApiClient.SPRetB(ServerInfo.GetServerInfo(), SPName, param, ref returnMessage, ref messageLevel, ref messageType);

            if (string.IsNullOrWhiteSpace(returnMessage) && ds1 != null && ds1.Tables.Count >= 0)
            {
                if (ds1.Tables.Count > 0)
                {
                    saveDetail = objUti.ConvertToList<RePayDetailVM>(ds1.Tables[0]);
                }
            }
            else
                errMsg = returnMessage;

            return saveDetail;
        }

        private List<IBIZ_ListRentCompute> ListRentCompute(List<IBIZ_ListRentCompute> sour)
        {
            var re = new List<IBIZ_ListRentCompute>();
            var bill = new BillCommon();
            if(sour != null && sour.Count()>0)
            {
                foreach(var s in sour)
                {
                    var sd = Convert.ToDateTime(s.strSd);
                    var ed = Convert.ToDateTime(s.strEd);
                    var lstHoliday = new CommonRepository(connetStr).GetHolidays(sd.ToString("yyyyMMdd"), ed.ToString("yyyyMMdd"));

                    var xre = bill.CarRentCompute(sd, ed, 990, 1680, 10, lstHoliday);
                    var item = objUti.Clone(s);
                    item.caRent = xre;
                    var tre = bill.GetCarRangeMins(sd, ed, 60, 600, new List<Holiday>());                    
                    item.payMins = tre.Item1;
                    item.InsPrice = (item.payMins/60) * s.InsPrice;
                    re.Add(item);
                }
            }
            return re;
        } 
    }

    public class IBIZ_ListRentCompute
    {
        public int orderNo { get; set; }
        public string idNo { get; set; }
        public string strSd { get; set; }
        public string strEd { get; set; }
        public double caRent { get; set; }
        public double payMins { get; set; }
        public double InsPrice { get; set; }
    }

    public class IAPI_RePayDetailAll
    {
        public DateTime SD { get; set; }
        public DateTime ED { get; set; }
        public string OrderNos { get; set; } = "";
        public int IsSave { get; set; } = 0;
        public int RePayMode { get; set; } = 0;
        /// <summary>
        /// 是否輸出json
        /// </summary>
        public int jsonOut { get; set; } = 0;

        //====以下單筆驗證才能使用
        /// <summary>
        /// 汽車時數
        /// </summary>
        public int Discount { set; get; } = 0;
        /// <summary>
        /// 機車時數
        /// </summary>
        public int MotorDiscount { set; get; } = 0;
    }

    public class OAPI_RePayDetailAll
    {
        public bool Result { get; set; } = true;
        public List<RePayDetailErrVM> MsgData { get; set; }
        public List<SPInput_CalFinalPrice> jsonOut { get; set; }
    }

    public class RePayDetailVM
    {
        public string OrderNo { get; set; }
        public string IDNO { get; set; }
        public DateTime final_start_time { get; set; }
        public DateTime final_stop_time { get; set; }
        public int final_price { get; set; }
        public int gift_point { get; set; }
        public int gift_motor_point { get; set; }
        public string RetCode { get; set; }
    }

    public class RePayDetailErrVM: RePayDetailVM
    {
        public string errMsg { get; set; }
    }

}