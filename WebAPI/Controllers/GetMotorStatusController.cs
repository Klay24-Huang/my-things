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
    public class GetMotorStatusController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        [HttpPost]
        public Dictionary<string, object> DoGetMotorStatus(Dictionary<string, object> value)
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
            string funName = "GetMotorStatusController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_GetMotorStatus apiInput = new IAPI_GetMotorStatus();
            OAPI_GetMotorStatus outputAPI = new OAPI_GetMotorStatus();
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
                apiInput = JsonConvert.DeserializeObject<IAPI_GetMotorStatus>(Contentjson);
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

            if (flag)
            {
                string spName = new ObjType().GetSPName(ObjType.SPType.GetMotorStatus);
                SPInput_MotorStatus spCarStatusInput = new SPInput_MotorStatus()
                {
                    IDNO = IDNO,
                    Token = Access_Token,
                    LogID = LogID,
                    CarNo = apiInput.CarNo
                };
                SPOutput_Base SPOutputBase = new SPOutput_Base();
                SQLHelper<SPInput_MotorStatus, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_MotorStatus, SPOutput_Base>(connetStr);
                List<SPOutput_MotorStatus> ListOut = new List<SPOutput_MotorStatus>();
                DataSet ds = new DataSet();
                flag = sqlHelp.ExeuteSP(spName, spCarStatusInput, ref SPOutputBase, ref ListOut, ref ds, ref lstError);
                baseVerify.checkSQLResult(ref flag, SPOutputBase.Error, SPOutputBase.ErrorCode, ref lstError, ref errCode);
                if (flag)
                {
                    if (ListOut != null)
                    {
                        if (ListOut.Count > 0)
                        {
                            outputAPI = new OAPI_GetMotorStatus
                            {
                                CarNo = ListOut[0].CarNo,
                                CID = ListOut[0].CID,
                                ACCStatus = ListOut[0].ACCStatus,
                                Latitude = ListOut[0].Latitude,
                                Longitude = ListOut[0].Longitude,
                                Millage = ListOut[0].Millage,
                                deviceRDistance = ListOut[0].deviceRDistance == "NA" || ListOut[0].deviceRDistance == "" ? 0 : Convert.ToSingle(ListOut[0].deviceRDistance),
                                device2TBA = ListOut[0].device2TBA,
                                //device3TBA = ListOut[0].device3TBA,
                                //20210522 ADD BY ADAM REASON.如果可以讀到儀表板電量就以rsoc為主
                                device3TBA = ListOut[0].deviceRSOC == "NA" || ListOut[0].deviceRSOC == "" ? ListOut[0].device3TBA : Convert.ToInt32(ListOut[0].deviceRSOC),
                                deviceMBA = ListOut[0].deviceMBA,
                                deviceRBA = ListOut[0].deviceRBA,
                                deviceLBA = ListOut[0].deviceLBA,
                                extDeviceStatus1 = ListOut[0].extDeviceStatus1,
                                deviceBat_Cover = ListOut[0].deviceBat_Cover
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