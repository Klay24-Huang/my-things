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
    /// 取得專案及資費(機車)
    /// </summary>
    public class GetMotorRentProjectController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoGetMotorRentProject(Dictionary<string, object> value)
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
            string funName = "GetMotorRentProjectController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetMotorProject apiInput = null;
            OAPI_GetMotorProject outputApi = null;
            List<MotorProjectObj> lstTmpData = new List<MotorProjectObj>();
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

                List<ProjectAndCarTypeDataForMotor> lstData = new List<ProjectAndCarTypeDataForMotor>();
                lstData = _repository.GetProjectOfMotorRent(apiInput.CarNo, SDate, EDate);
                List<Holiday> lstHoliday = new CommonRepository(connetStr).GetHolidays(SDate.ToString("yyyyMMdd"), EDate.ToString("yyyyMMdd"));


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
                                CarTypeName = lstData[0].CarTypeName,
                                CarTypePic = lstData[0].CarTypePic,
                                Insurance = 1,
                                InsurancePerHour = 20,
                                IsMinimum = isMin,
                                Operator = lstData[0].Operator,
                                OperatorScore = lstData[0].OperatorScore,
                                ProjID = lstData[0].PROJID,
                                ProjName = lstData[0].PRONAME,
                                BaseMinutes=lstData[0].BaseMinutes,
                                BasePrice=lstData[0].BasePrice,
                                MaxPrice=lstData[0].MaxPrice,
                                PerMinutesPrice=lstData[0].PerMinutesPrice,
                                CarOfArea = lstData[0].CarOfArea
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
                                        CarTypeName = lstData[i].CarTypeName,
                                        CarTypePic = lstData[i].CarTypePic,
                                        Insurance = 1,
                                        InsurancePerHour = 20,
                                        IsMinimum = isMin,
                                        Operator = lstData[i].Operator,
                                        OperatorScore = lstData[i].OperatorScore,
                                        ProjID = lstData[i].PROJID,
                                        ProjName = lstData[i].PRONAME,
                                        BaseMinutes = lstData[i].BaseMinutes,
                                        BasePrice = lstData[i].BasePrice,
                                        MaxPrice = lstData[i].MaxPrice,
                                        PerMinutesPrice = lstData[i].PerMinutesPrice,
                                        CarOfArea = lstData[i].CarOfArea
                                    });
                                }
                            }
                        }



                    }
                }
                outputApi = new OAPI_GetMotorProject()
                {
                    GetMotorProjectObj = lstTmpData
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
