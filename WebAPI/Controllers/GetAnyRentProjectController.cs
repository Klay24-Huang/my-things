using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.TB;
using Domain.WebAPI.output.rootAPI;
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
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得專案及資費(路邊)
    /// </summary>
    public class GetAnyRentProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoGetAnyRentProject(Dictionary<string, object> value)
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
            string funName = "GetAnyRentProjectController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetAnyRentProject apiInput = null;
            OAPI_GetAnyRentProject outputApi = null;
            List<ProjectObj> lstTmpData = new List<ProjectObj>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            StationAndCarRepository _repository;
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            DateTime SDate = DateTime.Now;
            DateTime EDate = DateTime.Now.AddHours(1);
            
            int QueryMode = 0;
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
                                            flag = false;
                                            errCode = "ERR154";
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
            if (flag && isGuest == false)
            {
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
            }
            if (flag)
            {
                _repository = new StationAndCarRepository(connetStr);
            
                List<ProjectAndCarTypeData> lstData = new List<ProjectAndCarTypeData>();
                lstData = _repository.GetProjectOfAnyRent(apiInput.CarNo, SDate, EDate);
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));
           

                if (flag)
                {
                    
                    if (lstData != null)
                    {
                        int DataLen = lstData.Count;
                        if (DataLen > 0)
                        {
                            int tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[0].Price, lstData[0].PRICE_H, lstHoliday));
                            int isMin = 1;

                            lstTmpData.Add(new ProjectObj()
                            {
                                CarBrend = lstData[0].CarBrend,
                                CarType = lstData[0].CarType,
                                CarTypeName = lstData[0].CarTypeName,
                                CarTypePic = lstData[0].CarTypePic,
                                Insurance = 1,
                                InsurancePerHour = 20,
                                IsMinimum = isMin,
                                Operator = lstData[0].Operator,
                                OperatorScore = lstData[0].OperatorScore,
                                ProjID = lstData[0].PROJID,
                                ProjName = lstData[0].PRONAME,
                                Seat = lstData[0].Seat,
                                Bill = tmpBill,
                                WorkdayPerHour = lstData[0].PayMode == 0 ? lstData[0].Price/10 : lstData[0].Price,
                                HolidayPerHour = lstData[0].PayMode == 0 ? lstData[0].PRICE_H/10 : lstData[0].PRICE_H,
                                CarOfArea = lstData[0].CarOfArea
                            });

                            if (DataLen > 1)
                            {
                                for (int i = 1; i < DataLen; i++)
                                {
                                    tmpBill = Convert.ToInt32(new BillCommon().CalSpread(SDate, EDate, lstData[i].Price, lstData[i].PRICE_H, lstHoliday));
                                    isMin = 0;
                                    int index = lstTmpData.FindIndex(delegate (ProjectObj proj)
                                    {
                                        return proj.Bill > tmpBill && proj.IsMinimum == 1;
                                    });
                                    if (index > -1)
                                    {
                                        lstTmpData[index].IsMinimum = 0;
                                        isMin = 1;
                                    }
                                    lstTmpData.Add(new ProjectObj()
                                    {
                                        CarBrend = lstData[i].CarBrend,
                                        CarType = lstData[i].CarType,
                                        CarTypeName = lstData[i].CarTypeName,
                                        CarTypePic = lstData[i].CarTypePic,
                                        Insurance = 1,
                                        InsurancePerHour = 20,
                                        IsMinimum = isMin,
                                        Operator = lstData[i].Operator,
                                        OperatorScore = lstData[i].OperatorScore,
                                        ProjID = lstData[i].PROJID,
                                        ProjName = lstData[i].PRONAME,
                                        Seat = lstData[i].Seat,
                                        Bill = tmpBill,
                                        WorkdayPerHour = lstData[i].PayMode == 0 ? lstData[i].Price / 10 : lstData[i].Price,
                                        HolidayPerHour = lstData[i].PayMode == 0 ? lstData[i].PRICE_H / 10 : lstData[i].PRICE_H,
                                        CarOfArea = lstData[i].CarOfArea
                                    });
                                }
                            }
                        }
                    }
                }
                outputApi = new OAPI_GetAnyRentProject()
                {
                    GetAnyRentProjectObj = lstTmpData
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
    }
}
