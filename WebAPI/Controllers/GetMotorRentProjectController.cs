using Domain.Common;
using Domain.SP.Input.Project;
using Domain.SP.Input.Subscription;
using Domain.SP.Output;
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
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Utils;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得專案及資費(機車)
    /// </summary>
    public class GetMotorRentProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoGetMotorRentProject(Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMotorRentProjectController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetMotorProject apiInput = null;
            OAPI_GetMotorProject outputApi = null;
            List<MotorProjectObj> lstTmpData = new List<MotorProjectObj>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            string IDNO = "";
            var InUseMonth = new List<SPOut_GetNowSubs>();//使用中月租
            var Score = 100;  // 20210913 UPD BY YEH REASON:會員積分，預設100
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMotorProject>(Contentjson);
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
            #region Token
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

            #region 取得機車使用中訂閱制月租
            //取得機車使用中訂閱制月租
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
                        IsMoto = 1
                    };
                    var sp_list = new MonSubsSp().sp_GetNowSubs(sp_in, ref errCode);
                    if (sp_list != null && sp_list.Count() > 0)
                        InUseMonth = sp_list;
                }
            }
            #endregion

            #region 取得會員積分
            // 20210913 UPD BY YEH REASON:取得會員積分
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
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));

                // 20210617 UPD BY YEH REASON:因應會員積分<60只能用定價專案，取專案改到SP處理
                string SPName = "usp_GetMotorRentProject";
                SPInput_GetMotorRentProject SPInput = new SPInput_GetMotorRentProject
                {
                    IDNO = IDNO,
                    CarNo = apiInput.CarNo,
                    SD = SDate,
                    ED = EDate,
                    LogID = LogID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_GetMotorRentProject, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_GetMotorRentProject, SPOutput_Base>(connetStr);
                List<ProjectAndCarTypeDataForMotor> lstData = new List<ProjectAndCarTypeDataForMotor>();
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
                            int isMin = 1;
                            lstTmpData.Add(new MotorProjectObj()
                            {
                                CarBrend = lstData[0].CarBrend,
                                CarType = lstData[0].CarType,
                                CarTypeName = lstData[0].CarBrend + ' ' + lstData[0].CarTypeName,
                                CarTypePic = lstData[0].CarTypePic,
                                Insurance = 0,
                                InsurancePerHour = 0,
                                IsMinimum = isMin,
                                Operator = lstData[0].Operator,
                                OperatorScore = lstData[0].OperatorScore,
                                ProjID = lstData[0].PROJID,
                                ProjName = lstData[0].PRONAME,
                                ProDesc = lstData[0].PRODESC,
                                BaseMinutes = lstData[0].BaseMinutes,
                                BasePrice = lstData[0].BasePrice,
                                MaxPrice = lstData[0].MaxPrice,
                                PerMinutesPrice = lstData[0].PerMinutesPrice,
                                CarOfArea = lstData[0].CarOfArea,
                                Content = lstData[0].Content,
                                Power = Convert.ToInt32(lstData[0].Power),
                                RemainingMileage = Convert.ToInt32(lstData[0].RemainingMileage)
                            });
                            if (DataLen > 1)
                            {
                                for (int i = 1; i < DataLen; i++)
                                {
                                    isMin = 0;
                                    int index = lstTmpData.FindIndex(delegate (MotorProjectObj proj)
                                    {
                                        return proj.MaxPrice > lstData[i].MaxPrice && proj.IsMinimum == 1;
                                    });
                                    if (index > -1)
                                    {
                                        lstTmpData[index].IsMinimum = 0;
                                        isMin = 1;
                                    }
                                    lstTmpData.Add(new MotorProjectObj()
                                    {
                                        CarBrend = lstData[i].CarBrend,
                                        CarType = lstData[i].CarType,
                                        CarTypeName = lstData[i].CarBrend + ' ' + lstData[i].CarTypeName,
                                        CarTypePic = lstData[i].CarTypePic,
                                        Insurance = 0,
                                        InsurancePerHour = 0,
                                        IsMinimum = isMin,
                                        Operator = lstData[i].Operator,
                                        OperatorScore = lstData[i].OperatorScore,
                                        ProjID = lstData[i].PROJID,
                                        ProjName = lstData[i].PRONAME,
                                        ProDesc = lstData[i].PRODESC,
                                        BaseMinutes = lstData[i].BaseMinutes,
                                        BasePrice = lstData[i].BasePrice,
                                        MaxPrice = lstData[i].MaxPrice,
                                        PerMinutesPrice = lstData[i].PerMinutesPrice,
                                        CarOfArea = lstData[i].CarOfArea,
                                        Content = lstData[i].Content,
                                        Power = Convert.ToInt32(lstData[i].Power),
                                        RemainingMileage = Convert.ToInt32(lstData[i].RemainingMileage)
                                    });
                                }
                            }
                        }
                    }
                }

                #region for春節專案使用
                //for春節專案使用，將原專案每日上限改為春節上限，並將春節專案移除
                //var Temp = lstTmpData.Where(x => x.ProjID == "R140").FirstOrDefault();
                //if (Temp != null)
                //{
                //    foreach (var tmp in lstTmpData)
                //    {
                //        if (tmp.ProjID != "R140")
                //        {
                //            tmp.MaxPrice = Temp.MaxPrice;
                //        }
                //    }

                //    lstTmpData.Remove(Temp);
                //}
                #endregion

                outputApi = new OAPI_GetMotorProject()
                {
                    GetMotorProjectObj = lstTmpData
                };

                #region 產出月租&Project虛擬卡片
                bool isSpring = new CarRentCommon().isSpring(SDate, EDate); //是否為春節時段

                if (flag && Score >= 60 && !isSpring)    // 20210913 UPD BY YEH REASON:積分>=60才可使用訂閱制
                {
                    if (outputApi.GetMotorProjectObj != null && outputApi.GetMotorProjectObj.Count() > 0)
                    {
                        var VisProObjs = new List<MotorProjectObj>();
                        var ProObjs = outputApi.GetMotorProjectObj;
                        if (InUseMonth != null && InUseMonth.Count() > 0 && ProObjs != null && ProObjs.Count() > 0)
                        {
                            ProObjs.ForEach(x =>
                            {
                                x.IsMinimum = 0;    //20210620 ADD BY ADAM REASON.先恢復為0
                                VisProObjs.Add(x);
                                InUseMonth.ForEach(z =>
                                {
                                    MotorProjectObj newItem = objUti.Clone(x);

                                    #region 月租卡片欄位給值
                                    //newItem.ProjName += "_" + z.MonProjNM;
                                    //20210706 ADD BY ADAM REASON.改為月租方案名稱顯示
                                    newItem.ProjName = z.MonProjNM;
                                    newItem.MotoTotalMins = z.MotoTotalMins;

                                    //newItem.MonthStartDate = z.StartDate.ToString("yyyy/MM/dd");
                                    //newItem.MonthEndDate = z.StartDate.AddDays(30 * z.MonProPeriod).ToString("yyyy/MM/dd");
                                    //20210611 ADD BY ADAM REASON.調整日期輸出格式
                                    newItem.MonthStartDate = z.StartDate.ToString("yyyy/MM/dd HH:mm");
                                    DateTime EndDate = z.StartDate.AddDays(30 * z.MonProPeriod);
                                    newItem.MonthEndDate = EndDate.ToString("HHmm") == "0000" ? EndDate.AddMinutes(-1).ToString("yyyy/MM/dd HH:mm") : EndDate.ToString("yyyy/MM/dd HH:mm");

                                    newItem.MonthlyRentId = z.MonthlyRentId;
                                    newItem.WDRateForMoto = z.WorkDayRateForMoto;
                                    newItem.HDRateForMoto = z.HoildayRateForMoto;

                                    //同步欄位，為了後續比對大小用
                                    newItem.PerMinutesPrice = (float)z.HoildayRateForMoto;    //20210620 ADD BY ADAM REASON.目前機車不分平假日，先用假日去判斷
                                                                                              //20210715 ADD BY ADAM REASON.補上說明欄位
                                    newItem.ProDesc = z.MonProDisc;
                                    #endregion

                                    VisProObjs.Add(newItem);
                                });
                            });

                            //20210620 ADD BY ADAM REASON.排序，抓最小的出來設定IsMinimum
                            VisProObjs.OrderBy(p => p.PerMinutesPrice).ThenByDescending(p => p.MonthlyRentId).First().IsMinimum = 1;
                            VisProObjs = VisProObjs.OrderBy(p => p.PerMinutesPrice).ThenByDescending(p => p.MonthlyRentId).ToList();
                            outputApi.GetMotorProjectObj = VisProObjs;
                        }
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
    }
}