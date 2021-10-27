using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Input.Project;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.SP.Output.Subscription;
using Domain.TB;
using Domain.WebAPI.output.rootAPI;
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
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得專案及資費(路邊)
    /// </summary>
    public class GetAnyRentProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());

        [HttpPost]
        public Dictionary<string, object> DoGetAnyRentProject(Dictionary<string, object> value)
        {
            #region 初始宣告
            var bill = new BillCommon();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetAnyRentProjectController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetAnyRentProject apiInput = null;
            OAPI_GetAnyRentProject outputApi = null;
            List<ProjectObj> lstTmpData = new List<ProjectObj>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            string IDNO = "";
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetAnyRentProject>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
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
            //20201109 ADD BY ADAM REASON.TOKEN判斷修改
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
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));

                // 20210616 UPD BY YEH REASON:因應會員積分<60只能用定價專案，取專案改到SP處理
                string SPName = new ObjType().GetSPName(ObjType.SPType.GetAnyRentProject);
                SPInput_GetAnyRentProject SPInput = new SPInput_GetAnyRentProject
                {
                    IDNO = IDNO,
                    CarNo = apiInput.CarNo,
                    SD = SDate,
                    ED = EDate,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                List<ProjectAndCarTypeData> lstData = new List<ProjectAndCarTypeData>();
                SQLHelper<SPInput_GetAnyRentProject, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetAnyRentProject, SPOutput_Base>(connetStr);
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(SPName, SPInput, ref spOut, ref lstData, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                if (flag)
                {
                    if (lstData != null)
                    {
                        int DataLen = lstData.Count;
                        if (DataLen > 0)
                        {
                            //int tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[0].Price, lstData[0].PRICE_H, lstHoliday));
                            int isMin = 1;

                            int tmpBill = GetPriceBill(lstData[0], IDNO, LogID, lstHoliday, SDate, EDate, 0) +
                                     bill.CarMilageCompute(SDate, EDate, lstData[0].MilageBase, Mildef, 20, new List<Holiday>());

                     lstTmpData.Add(new ProjectObj()
                            {
                                StationID = lstData[0].StationID,
                                CarBrend = lstData[0].CarBrend,
                                CarType = lstData[0].CarType,
                                CarTypeName = lstData[0].CarBrend + ' ' + lstData[0].CarTypeName,
                                CarTypePic = lstData[0].CarTypePic,
                                Insurance = lstData[0].Insurance,
                                InsurancePerHour = lstData[0].InsurancePerHours,
                                IsMinimum = isMin,
                                Operator = lstData[0].Operator,
                                OperatorScore = lstData[0].OperatorScore,
                                ProjID = lstData[0].PROJID,
                                ProjName = lstData[0].PRONAME,
                                ProDesc = lstData[0].PRODESC,
                                Seat = lstData[0].Seat,
                                //Bill = tmpBill,
                                Price = tmpBill,
                                WorkdayPerHour = lstData[0].PayMode == 0 ? lstData[0].Price / 10 : lstData[0].Price,
                                HolidayPerHour = lstData[0].PayMode == 0 ? lstData[0].PRICE_H / 10 : lstData[0].PRICE_H,
                                CarOfArea = lstData[0].CarOfArea,
                                Content = lstData[0].Content
                            });

                            if (DataLen > 1)
                            {
                                for (int i = 1; i < DataLen; i++)
                                {
                                    //tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price, lstData[i].PRICE_H, lstHoliday));

                                    tmpBill = GetPriceBill(lstData[i], IDNO, LogID, lstHoliday, SDate, EDate, 0) +
                                             bill.CarMilageCompute(SDate, EDate, lstData[i].MilageBase, Mildef, 20, new List<Holiday>());

                                    isMin = 0;
                                    int index = lstTmpData.FindIndex(delegate (ProjectObj proj)
                                    {
                                        return proj.Price > tmpBill && proj.IsMinimum == 1;
                                    });
                                    if (index > -1)
                                    {
                                        lstTmpData[index].IsMinimum = 0;
                                        isMin = 1;
                                    }
                                    lstTmpData.Add(new ProjectObj()
                                    {
                                        StationID = lstData[i].StationID,
                                        CarBrend = lstData[i].CarBrend,
                                        CarType = lstData[i].CarType,
                                        CarTypeName = lstData[i].CarBrend + ' ' + lstData[i].CarTypeName,
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
                                        Price = tmpBill,
                                        WorkdayPerHour = lstData[i].PayMode == 0 ? lstData[i].Price / 10 : lstData[i].Price,
                                        HolidayPerHour = lstData[i].PayMode == 0 ? lstData[i].PRICE_H / 10 : lstData[i].PRICE_H,
                                        CarOfArea = lstData[i].CarOfArea,
                                        Content = lstData[i].Content
                                    });
                                }
                            }
                        }
                    }
                }

                //for春節專案使用，將原專案每小時金額改為春節價格，並將春節專案移除
                var Temp = lstTmpData.Where(x => x.ProjID == "R139").FirstOrDefault();
                if (Temp != null)
                {
                    foreach (var tmp in lstTmpData)
                    {
                        if (tmp.ProjID != "R139")
                        {
                            tmp.WorkdayPerHour = Temp.WorkdayPerHour;
                            tmp.HolidayPerHour = Temp.HolidayPerHour;
                        }
                    }

                    lstTmpData.Remove(Temp);
                }

                outputApi = new OAPI_GetAnyRentProject()
                {
                    GetAnyRentProjectObj = lstTmpData
                };

                #region 產出月租&Project虛擬卡片 

                if (outputApi.GetAnyRentProjectObj != null && outputApi.GetAnyRentProjectObj.Count() > 0)
                {
                    var VisProObjs = new List<ProjectObj>();
                    var ProObjs = outputApi.GetAnyRentProjectObj;
                    if (InUseMonth != null && InUseMonth.Count() > 0 && ProObjs != null && ProObjs.Count() > 0)
                    {
                        ProObjs.ForEach(x => {
                            x.IsMinimum = 0;    //20210620 ADD BY ADAM REASON.先恢復為0
                            VisProObjs.Add(x);                           
                            InUseMonth.ForEach(z =>
                            {
                                ProjectObj newItem = objUti.Clone(x);

                                #region 月租卡片欄位給值
                                //newItem.ProjName += "_" + z.MonProjNM;
                                //20210706 ADD BY ADAM REASON.改為月租方案名稱顯示
                                newItem.ProjName = z.MonProjNM;
                                newItem.CarWDHours = z.WorkDayHours == 0 ? -999 : z.WorkDayHours;
                                newItem.CarHDHours = z.HolidayHours == 0 ? -999 : z.HolidayHours;
                                newItem.MotoTotalMins = z.MotoTotalMins;
                                newItem.WorkdayPerHour = Convert.ToInt32(z.WorkDayRateForCar);
                                newItem.HolidayPerHour = Convert.ToInt32(z.HoildayRateForCar);
                                
                                //newItem.MonthStartDate = z.StartDate.ToString("yyyy/MM/dd");
                                //newItem.MonthEndDate = z.StartDate.AddDays(30 * z.MonProPeriod).ToString("yyyy/MM/dd");
                                //20210611 ADD BY ADAM REASON.調整日期輸出格式
                                newItem.MonthStartDate = z.StartDate.ToString("yyyy/MM/dd HH:mm");
                                DateTime EndDate = z.StartDate.AddDays(30 * z.MonProPeriod);
                                newItem.MonthEndDate = EndDate.ToString("HHmm") == "0000" ? EndDate.AddMinutes(-1).ToString("yyyy/MM/dd HH:mm") : EndDate.ToString("yyyy/MM/dd HH:mm");

                                newItem.MonthlyRentId = z.MonthlyRentId;
                                newItem.WDRateForCar = z.WorkDayRateForCar;

                                //newItem.HDRateForCar = z.HoildayRateForCar;
                                //newItem.HDRateForCar = x.HDRateForCar;//月租假日優惠費率用一般假日優惠費率(前端顯示用)
                                //20211025 ADD BY ADAM REASON.原本的修改並沒有處理到HDRateForCar，改為使用HolidayPerHour
                                newItem.HDRateForCar = x.HolidayPerHour;//月租假日優惠費率用一般假日優惠費率(前端顯示用)

                                newItem.WDRateForMoto = z.WorkDayRateForMoto;
                                newItem.HDRateForMoto = z.HoildayRateForMoto;
                                //20210715 ADD BY ADAM REASON.補上月租說明
                                newItem.ProDesc = z.MonProDisc;
                                var fn_in = new ProjectAndCarTypeData()
                                {
                                    Price = x.WorkdayPerHour * 10,
                                    PRICE_H = x.HolidayPerHour * 10,
                                    PROJID = x.ProjID,
                                    CarType = x.CarType
                                };
                                newItem.Price = GetPriceBill(fn_in, IDNO, LogID, lstHoliday, SDate, EDate, MonId: z.MonthlyRentId);
                                #endregion

                                VisProObjs.Add(newItem);
                            });
                        });

                        //20210620 ADD BY ADAM REASON.排序，抓最小的出來設定IsMinimun
                        VisProObjs.OrderBy(p => p.Price).ThenByDescending(p => p.MonthlyRentId).First().IsMinimum = 1;
                        VisProObjs = VisProObjs.OrderBy(p => p.Price).ThenByDescending(p => p.MonthlyRentId).ToList();
                        outputApi.GetAnyRentProjectObj = VisProObjs;
                    }
                }

                #endregion
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

        private int GetPriceBill(ProjectAndCarTypeData spItem, string IDNO, long LogID, List<Holiday> lstHoliday, DateTime SD, DateTime ED, Int64 MonId = 0)
        {
            int re = 0;

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

            return re;
        }

    }
}