using Domain.Common;
using Domain.SP.Input.Car;
using Domain.SP.Input.Common;
using Domain.SP.Output;
using Domain.SP.Output.Car;
using Domain.SP.Output.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 取得汽車狀態
    /// </summary>
    public class GetCarStatusController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetCarStatus(Dictionary<string, object> value)
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
            string funName = "GetCarStatusController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetCarStatus apiInput = new IAPI_GetCarStatus();
            OAPI_GetCarStatus outputAPI = new OAPI_GetCarStatus();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            string Contentjson = "";
            bool isGuest = true;
            #endregion

            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            
            if (flag)
            {
                apiInput = JsonConvert.DeserializeObject<IAPI_GetCarStatus>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                if (flag)
                {
                    if (string.IsNullOrEmpty(apiInput.CarNo))
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
            }

            if (isGuest)
            {
                flag = false;
                errCode = "ERR150";
            }
            #endregion

            #region TB
            //Token判斷
            if (flag && isGuest == false)
            {
                string CheckTokenName = new ObjType().GetSPName(ObjType.SPType.CheckTokenReturnID);
                SPInput_CheckTokenOnlyToken spCheckTokenInput = new SPInput_CheckTokenOnlyToken()
                {
                    LogID = LogID,
                    Token = Access_Token
                };
                SPOutput_CheckTokenReturnID spOut = new SPOutput_CheckTokenReturnID();
                SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID> sqlHelp = new SQLHelper<SPInput_CheckTokenOnlyToken, SPOutput_CheckTokenReturnID>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(CheckTokenName, spCheckTokenInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    IDNO = spOut.IDNO;
                }
            }

            flag = true;

            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.GetCarStatus);
                SPInput_CarStatus spCarStatusInput = new SPInput_CarStatus()
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID,
                    CarNo = apiInput.CarNo
                };
                SPOutput_Base SPOutputBase = new SPOutput_Base();
                SQLHelper<SPInput_CarStatus, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_CarStatus, SPOutput_Base>(connetStr);
                List<SPOutput_CarStatus> ListOut = new List<SPOutput_CarStatus>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spCarStatusInput, ref SPOutputBase, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    if (ListOut != null)
                    {
                        if (ListOut.Count > 0)
                        {
                            outputAPI = new OAPI_GetCarStatus
                            {
                                CarNo = ListOut[0].CarNo,
                                PowerOnStatus = ListOut[0].CentralLockStatus,
                                CentralLockStatus = ListOut[0].CentralLockStatus,
                                DoorStatus = ListOut[0].DoorStatus,
                                LockStatus = ListOut[0].LockStatus,
                                IndoorLightStatus = ListOut[0].IndoorLightStatus,
                                SecurityStatus = ListOut[0].SecurityStatus,
                            };
                        }
                    }
                }
            }
            #endregion

            #region 寫入錯誤Log
            if (flag == false && isWriteError == false)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputAPI, token);
            return objOutput;
            #endregion
        }
    
    }
}
