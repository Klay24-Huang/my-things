using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Common;
using Domain.TB;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using Domain.SP.Input.Bill;
using Domain.SP.Output.Bill;
using WebCommon;
using System.Data;

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
            //string[] headers=httpContext.Request.Headers.AllKeys;
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
            StationAndCarRepository _repository;
            ProjectRepository projectRepository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
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
            //Token判斷
            //20201109 ADD BY ADAM REASON.TOKEN判斷修改
            //if (flag && isGuest == false)
            if (flag && Access_Token_string.Split(' ').Length >= 2)
            {
                /*
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenOnlyToken);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
                */

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

            if (flag)
            {
                projectRepository = new ProjectRepository(connetStr);
                BillCommon billCommon = new BillCommon();
                float MilUnit = billCommon.GetMilageBase(apiInput.ProjID, apiInput.CarType, SDate, EDate, LogID);

                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
                if (QueryMode == 0 || (QueryMode == 1 && ProjType == 3))
                {
                    //ProjectPriceBase priceBase = projectRepository.GetProjectPriceBase(apiInput.ProjID, apiInput.CarType, ProjType);
                    //20201110 ADD BY ADAM REASON.改為sp處理
                    SPInput_GetProjectPriceBase spInput = new SPInput_GetProjectPriceBase()
                    {
                        ProjID = apiInput.ProjID,
                        CarNo = apiInput.CarNo,
                        CarType = apiInput.CarType,
                        ProjType = ProjType,
                        IDNO = IDNO,
                        LogID = LogID
                    };
                    string SPName = new ObjType().GetSPName(ObjType.SPType.GetProjectPriceBase);
                    SPOutput_Base spOutBase = new SPOutput_Base();
                    SQLHelper<SPInput_GetProjectPriceBase, SPOutput_Base> sqlHelpQuery = new SQLHelper<SPInput_GetProjectPriceBase, SPOutput_Base>(connetStr);
                    DataSet ds = new DataSet();
                    flag = sqlHelpQuery.ExeuteSP(SPName, spInput, ref spOutBase, ref priceBase, ref ds, ref lstError);
                    baseVerify.checkSQLResult(ref flag, ref spOutBase, ref lstError, ref errCode);

                    if (flag)
                    {
                        if (priceBase.Count > 0)
                        {
                            int InsurancePer10Hours = priceBase[0].InsurancePerHours * 10;

                            outputApi = new OAPI_GetEstimate()
                            {
                                CarRentBill = billCommon.CarRentCompute(SDate, EDate, priceBase[0].PRICE, priceBase[0].PRICE_H, 10, lstHoliday),
                                InsuranceBill = (apiInput.Insurance == 1) ? billCommon.CarRentCompute(SDate, EDate, InsurancePer10Hours, InsurancePer10Hours, 10, lstHoliday) : 0,
                                InsurancePerHour = priceBase[0].InsurancePerHours,
                                MileagePerKM = (MilUnit < 0) ? Mildef : Math.Round(MilUnit,2),  //20201205 ADD BY ADAM REASON.小數點四捨五入
                                MileageBill = billCommon.CarMilageCompute(SDate, EDate, MilUnit, Mildef, 20, lstHoliday)
                            };
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