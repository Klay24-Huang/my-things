using Domain.Common;
using Domain.SP.Input.Battle;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 更新交換站點資訊
    /// </summary>
    public class EDI_STATIONController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoEDI_STATION([FromBody] Dictionary<string, object> value)
        {
            #region 初值設定
            var objOutput = new Dictionary<string, object>();
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "EDI_STATIONController";
            string spName = "";
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            IAPI_EDI_STATION apiInput = null;
            OAPI_Base outAPI = null;
            Token token = null;
            Int64 LogID = 0;
            Int16 ErrType = 0;
            string Contentjson = "";
            string SPName = "";
            #endregion
            #region 防呆
            string ClientIP = baseVerify.GetClientIp(Request);

            flag = baseVerify.baseCheck(value, ref errCode, funName);
            if (flag)
            {
                //寫入API Log

                flag = baseVerify.InsAPLog(value["para"].ToString(), ClientIP, funName, ref errCode, ref LogID);
                Contentjson = value["para"].ToString();
            }

            if (flag)
            {
                string[] checkList = { apiInput.CheckKey, apiInput.Addr, apiInput.EmptyCnt.ToString(), apiInput.Station, apiInput.lon.ToString(), apiInput.lat.ToString(), apiInput.TotalCnt.ToString(), apiInput.FullCnt.ToString(), apiInput.UpdateTime.ToString() };
                string[] errList = { "003", "003", "003", "003", "003", "003", "003", "003", "003" };
                //必填判斷
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName,LogID);
                if (flag)
                {
                    if (apiInput.UpdateTime.Year < DateTime.Now.Year)
                    {
                        flag = false;
                        errCode = "998";
                    }
                }
            }


            #endregion
            #region 查詢金鑰
            if (flag)
            {
                try
                {


                    spName = new ObjType().GetSPName(ObjType.SPType.UPD_SW_DATA);
                    SPInput_EDI_STATION spInput = new SPInput_EDI_STATION()
                    {
                        CheckKey = apiInput.CheckKey,
                        Addr = apiInput.Addr,
                        EmptyCnt = apiInput.EmptyCnt,
                        FullCnt = apiInput.FullCnt,
                        lat = apiInput.lat,
                        lon = apiInput.lon,
                        Station = apiInput.Station,
                        TotalCnt = apiInput.TotalCnt,
                        UpdateTime = apiInput.UpdateTime
                    };
                    SPOutput_Base spOut = new SPOutput_Base();
                    SQLHelper<SPInput_EDI_STATION, SPOutput_Base> sqlHelper = new SQLHelper<SPInput_EDI_STATION, SPOutput_Base>(connetStr);
                    flag = sqlHelper.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                    baseVerify.checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

                }
                catch (Exception ex)
                {
                    flag = false;
                    errMsg = ex.InnerException.Message;
                    baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                    isWriteError = true;
                    errCode = "999";
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outAPI, token);
            return objOutput;
            #endregion
        }
    }
}
