using Domain.Common;
using Domain.Flow.CarRentCompute;
using Domain.SP.Input.Bill;
using Domain.SP.Output;
using Domain.TB;
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
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 資費明細
    /// </summary>
    public class GetEstimateController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        public float Mildef = (ConfigurationManager.AppSettings["Mildef"] == null) ? 3 : Convert.ToSingle(ConfigurationManager.AppSettings["Mildef"].ToString());
        [HttpPost]
        public Dictionary<string, object> DoGetProject(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetEstimateController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetEstimate apiInput = null;
            OAPI_GetEstimate outputApi = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<ProjectPriceBase> priceBase = new List<ProjectPriceBase>();
            ProjectRepository projectRepository;
            string Contentjson = "";
            bool isGuest = true;
            var CarRepo = new CarRentRepo(connetStr);
            DateTime SDate = DateTime.Now.AddHours(-1);
            DateTime EDate = DateTime.Now;
            ProjectInfo obj;
            int QueryMode = 0;
            int PayMode = 0;
            int ProjType = 0;
            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetEstimate>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    string[] checkList = { apiInput.ProjID, apiInput.SDate, apiInput.EDate };
                    string[] errList = { "ERR900", "ERR900", "ERR900" };
                    //1.判斷必填
                    flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
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
                        if (flag)
                        {
                            projectRepository = new ProjectRepository(connetStr);
                            obj = projectRepository.GetProjectInfo(apiInput.ProjID);

                            if (obj != null)
                            {
                                PayMode = obj.PayMode;
                                ProjType = obj.PROJTYPE;
                            }

                            if (string.IsNullOrWhiteSpace(apiInput.CarType) && string.IsNullOrWhiteSpace(apiInput.CarNo))
                            {
                                errCode = "ERR900";
                                flag = false;
                            }
                            else if (string.IsNullOrWhiteSpace(apiInput.CarType) && !string.IsNullOrWhiteSpace(apiInput.CarNo))
                            {
                                QueryMode = 1;
                            }
                            else if (!string.IsNullOrWhiteSpace(apiInput.CarType) && string.IsNullOrWhiteSpace(apiInput.CarNo))
                            {
                                QueryMode = 0;
                            }
                            else
                            {
                                if (obj != null)
                                {
                                    if (ProjType == 0)
                                    {
                                        QueryMode = 0;
                                    }
                                    else
                                    {
                                        QueryMode = 1;
                                    }
                                }
                                else
                                {
                                    flag = false;
                                    errCode = "ERR155";
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region TB
            #region Token判斷
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

            if (flag)
            {
                projectRepository = new ProjectRepository(connetStr);
                BillCommon billCommon = new BillCommon();
                float MilUnit = billCommon.GetMilageBase(apiInput.ProjID, apiInput.CarType, SDate, EDate, LogID);

                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
                if (QueryMode == 0 || (QueryMode == 1 && ProjType == 3))
                {
                    //20201110 ADD BY ADAM REASON.改為sp處理
                    SPInput_GetProjectPriceBase spInput = new SPInput_GetProjectPriceBase()
                    {
                        ProjID = apiInput.ProjID,
                        CarNo = apiInput.CarNo,
                        CarType = apiInput.CarType,
                        ProjType = ProjType,
                        IDNO = IDNO,
                        MonId = apiInput.MonId,     //20211104 ADD BY ADAM REASON.訂閱制調整
                        LogID = LogID
                    };
                    //20211104 ADD BY ADAM REASON.訂閱制定金修改
                    string SPName = "usp_GetProjectPriceBase";
                    SPOutput_Base spOutBase = new SPOutput_Base();
                    SQLHelper<SPInput_GetProjectPriceBase, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetProjectPriceBase, SPOutput_Base>(connetStr);
                    DataSet ds = new DataSet();
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref priceBase, ref ds, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

                    if (flag)
                    {
                        if (priceBase.Count > 0)
                        {
                            #region 春節汽車
                            var cr_com = new CarRentCommon();
                            var pr = priceBase[0];
                            List<int> proTypes = new List<int>() { 0, 3 };
                            bool isSpring = cr_com.isSpring(SDate, EDate);
                            if (proTypes.Any(x => x == ProjType) && isSpring)
                            {
                                //有跨到春節就會回傳春節專案,只針對同站 
                                var bizIn = new IBIZ_SpringInit()
                                {
                                    IDNO = IDNO,
                                    ProjID = apiInput.ProjID,
                                    ProjType = ProjType,
                                    CarNo = apiInput.CarNo,
                                    CarType = apiInput.CarType,
                                    SD = SDate,
                                    ED = EDate,
                                    ProDisPRICE = Convert.ToDouble(pr.PRICE) / 10,
                                    ProDisPRICE_H = Convert.ToDouble(pr.PRICE_H) / 10,
                                    lstHoliday = lstHoliday,
                                    LogID = LogID
                                };

                                //if (ProjType == 3)
                                //{   //路邊projID一定是非春節(一般時段),春節期間仍然回傳非春節ProjID, 邏輯已確認過
                                //    //if (!string.IsNullOrWhiteSpace(apiInput.CarNo))
                                //    //    carCode = CarRepo.GetCarTypeGroupCode(apiInput.CarNo);
                                //    //else
                                //    //    throw new Exception("路邊CarNo為必填");

                                //    string carCode = CarRepo.GetCarTypeGroupCode(apiInput.CarNo);

                                //    if (!string.IsNullOrEmpty(carCode))
                                //    {
                                //        bizIn.CarType = carCode;
                                //        //bizIn.PRICE = Convert.ToDouble(pr.PRICE) / 10;
                                //        //bizIn.PRICE_H = Convert.ToDouble(pr.PRICE_H) / 10;
                                //        //bizIn.ProDisPRICE = 0;
                                //        //bizIn.ProDisPRICE_H = 0;
                                //    }
                                //    //else
                                //    //    throw new Exception("無對應CarTypeGroupCoder");
                                //}

                                var xre = cr_com.GetSpringInit(bizIn, connetStr, funName);
                                if (xre != null)
                                {
                                    double InsurBill = Convert.ToDouble(pr.InsurancePerHours) * (Convert.ToDouble(xre.RentInMins) / 60);
                                    outputApi = new OAPI_GetEstimate()
                                    {
                                        CarRentBill = xre.RentInPay,
                                        InsuranceBill = (apiInput.Insurance == 1) ? Convert.ToInt32(Math.Floor(InsurBill)) : 0,
                                        InsurancePerHour = priceBase[0].InsurancePerHours,
                                        MileagePerKM = (MilUnit <= 0) ? Mildef : Math.Round(MilUnit, 2),  //20201205 ADD BY ADAM REASON.小數點四捨五入
                                        MileageBill = billCommon.CarMilageCompute(SDate, EDate, MilUnit, Mildef, 20, lstHoliday)
                                    };
                                }
                            }
                            else
                            {
                                int InsurancePer10Hours = priceBase[0].InsurancePerHours * 10;
                                outputApi = new OAPI_GetEstimate()
                                {
                                    CarRentBill = billCommon.CarRentCompute(SDate, EDate, priceBase[0].PRICE, priceBase[0].PRICE_H, 10, lstHoliday),
                                    InsuranceBill = (apiInput.Insurance == 1) ? billCommon.CarRentCompute(SDate, EDate, InsurancePer10Hours, InsurancePer10Hours, 10, lstHoliday) : 0,
                                    InsurancePerHour = priceBase[0].InsurancePerHours,
                                    MileagePerKM = (MilUnit <= 0) ? Mildef : Math.Round(MilUnit, 2),  //20201205 ADD BY ADAM REASON.小數點四捨五入
                                    MileageBill = billCommon.CarMilageCompute(SDate, EDate, MilUnit, Mildef, 20, lstHoliday)
                                };
                            }
                            #endregion

                            outputApi.Bill = outputApi.CarRentBill + outputApi.InsuranceBill + outputApi.MileageBill;
                        }
                    }
                }
                else
                {
                    //先不開啟汽車以分計費
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}