using Domain.Common;
using Domain.SP.Input.OtherService.CENS;
using Domain.SP.Input.OtherService.FET;
using Domain.SP.Input.Register;
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
    /// 興聯車機_定時回報
    /// </summary>
    public class ResponseInfoController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> ResponseInfoInsert([FromBody] Dictionary<string, object> value)
        {
            #region 初值設定
            var objOutput = new Dictionary<string, object>();
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ResponseInfoController";
            string spName = "";
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            IAPI_CENS_ResponseInfo apiInput = null;
            OAPI_Base outAPI = null;
            Int16 ErrType = 0;
            Token token = null;
            string Contentjson = "";
            Int64 LogID = 0;
            #endregion
            #region 基本邏輯判斷
            //0.判斷有無參數傳入
            try
            {
                flag = baseVerify.baseCheck(value, ref errCode, funName);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CENS_ResponseInfo>(value["para"].ToString());
                    //寫入ap log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(value["para"].ToString(), ClientIP, funName, ref errCode, ref LogID);
                    Contentjson = value["para"].ToString();
                    //  string[] checkList = { apiInput.CheckKey, apiInput.CarNo };
                    //  string[] errList = { "003", "003" };
                    //必填判斷
                    //  flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName);

                }
                #endregion
                #region 寫入tb
                InCarStatus(apiInput,LogID);
                #endregion
            }
            catch (Exception ex)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }

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
        ///// <summary>
        ///// 寫入OBDLOG
        ///// </summary>
        ///// <param name="objResInfo"></param>
        ///// 

        //private void InsOBD(IAPI_CENS_ResponseInfo objResInfo)
        //{
        //    DateTime GPSTime = DateTime.Now;
        //    //if (objResInfo.GPSTime < GPSTime.AddHours(-8))
        //    //{
        //    //    GPSTime = objResInfo.GPSTime.AddHours(8);
        //    //}
        //    //else
        //    //{
        //    GPSTime = objResInfo.GPSTime;
        //    //}

        //    SPInput_InsOBDData spInput = new SPInput_InsOBDData();
        //    spInput.AccON = objResInfo.AccON; //(objResInfo.objInfo[i].AccON < 0) ? 2 : objResInfo.objInfo[i].AccON;
        //    spInput.SPEED = Convert.ToSingle(objResInfo.SPEED); //(objResInfo.objInfo[i].Speed < 0) ? 2 : Convert.ToSingle(objResInfo.objInfo[i].Speed);
        //    spInput.CentralLock = objResInfo.CentralLock;// (objResInfo.objInfo[i].CentralLock < 0) ? 2 : objResInfo.objInfo[i].CentralLock;
        //    spInput.CID = objResInfo.CID;// objResInfo.objInfo[i].carCarMachineNO.ToUpper();
        //    spInput.doorStatus = objResInfo.doorStatus;// objResInfo.objInfo[i].doorStatus;
        //    spInput.GPRSStatus = objResInfo.GPRSStatus;// objResInfo.objInfo[i].GPRSStatus;
        //    spInput.GPSStatus = objResInfo.GPSStatus;// objResInfo.objInfo[i].GPSStatus;
        //    spInput.Lat = Convert.ToDecimal(objResInfo.Lat);
        //    spInput.Lng = Convert.ToDecimal(objResInfo.Lng);
        //    spInput.lockStatus = objResInfo.lockStatus;
        //    spInput.Milage = Convert.ToSingle(objResInfo.Milage); //(objResInfo.objInfo[i].Milage < 0) ? -1 : Convert.ToSingle(objResInfo.objInfo[i].Milage);
        //    spInput.OBDStatus = objResInfo.OBDStatus;// (objResInfo.objInfo[i].OBDStatus < 0) ? 2 : objResInfo.objInfo[i].OBDStatus;
        //    spInput.PowON = objResInfo.PowON;// (objResInfo.objInfo[i].PowON < 0) ? 2 : objResInfo.objInfo[i].PowON;
        //    spInput.Volt = Convert.ToSingle(objResInfo.Volt);// (objResInfo.objInfo[i].Volt < 0) ? 2 : Convert.ToSingle(objResInfo.objInfo[i].Volt);
        //    spInput.indoorLight = objResInfo.indoorLight;// (objResInfo.objInfo[i].indoorLight < 0) ? 2 : objResInfo.objInfo[i].indoorLight;
        //    spInput.OrderStatus = objResInfo.OrderStatus;// (objResInfo.objInfo[i].OrderStatus < 0) ? 2 : objResInfo.objInfo[i].OrderStatus;
        //    spInput.SecurityStatus = objResInfo.SecurityStatus;// (objResInfo.objInfo[i].SecurityStatus < 0) ? 2 : objResInfo.objInfo[i].SecurityStatus;
        //    spInput.GPSTime = GPSTime;// (objResInfo.objInfo[i].GpsTime == null) ? DateTime.Now : objResInfo.objInfo[i].GpsTime;
        //    spInput.iButton = objResInfo.iButton;// (objResInfo.objInfo[i].iButton < 0) ? 0 : objResInfo.objInfo[i].iButton;
        //    spInput.iButtonKey = objResInfo.iButtonKey;// (string.IsNullOrEmpty(objResInfo.objInfo[i].iButtonKey) ? "" : objResInfo.objInfo[i].iButtonKey);

        //    SPOutput_InsOBDData spOut = new SPOutput_InsOBDData();
        //    string spName = "usp_InsOBDData_201805";
        //    SQLHelper<SPInput_InsOBDData, SPOutput_InsOBDData> sqlHelp = new SQLHelper<SPInput_InsOBDData, SPOutput_InsOBDData>(connetStr);
        //    List<ErrorInfo> lstError = new List<ErrorInfo>();
        //    bool flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
        //    string errCode = "";
        //    new CommFunc().checkSQLResult(ref flag, spOut.Error, spOut.ErrorCode, ref lstError, ref errCode);

        //    if (false == flag)
        //    {
        //        new CommFunc().InsErrorLog("ResponseInfo", errCode, 0, 0, "");
        //    }
        //    else
        //    {
        //        doCalEntry(spInput.CID, spInput.Lat, spInput.Lng, spInput.GPSTime, spInput.PowON);
        //        InsCarStatusO(spInput);

        //    }


        //}
        //private void InsCarStatusO(SPInput_InsOBDData tmpInput)
        //{
        //    SPInput_InsCarStatus spInput = new SPInput_InsCarStatus()
        //    {
        //        AccON = tmpInput.AccON,
        //        CentralLock = tmpInput.CentralLock,
        //        GPRSStatus = tmpInput.GPRSStatus,
        //        GPSStatus = tmpInput.GPSStatus,
        //        GPSTime = tmpInput.GPSTime,
        //        Lat = tmpInput.Lat,
        //        LightStatus = tmpInput.indoorLight,
        //        Lng = tmpInput.Lng,
        //        LockStatus = (tmpInput.lockStatus.IndexOf("0") > -1) ? 0 : 1,
        //        LowPowStatus = (tmpInput.Volt < 12) ? 1 : 0,
        //        MachineNo = tmpInput.CID,
        //        Milage = tmpInput.Milage,
        //        OBDStatus = tmpInput.OBDStatus,
        //        PowON = tmpInput.PowON,
        //        SecurityStatus = tmpInput.SecurityStatus,
        //        SPEED = tmpInput.SPEED,
        //        Volt = tmpInput.Volt,
        //        DoorStatus = (tmpInput.doorStatus.IndexOf("0") > -1) ? 1 : 0,
        //        iButton = tmpInput.iButton,
        //        iButtonKey = (string.IsNullOrEmpty(tmpInput.iButtonKey) ? "" : tmpInput.iButtonKey)
        //    };
        //    InCarStatus(spInput);
        //}
        /// <summary>
        /// 寫入CarStatus
        /// </summary>
        /// <param name="spInput"></param>
        private void InCarStatus(IAPI_CENS_ResponseInfo objResInfo,Int64 LogID)
        {
    
            SPInput_InsCarStatus spInput = new SPInput_InsCarStatus();
            spInput.AccON = objResInfo.AccON; //(objResInfo.objInfo[i].AccON < 0) ? 2 : objResInfo.objInfo[i].AccON;
            spInput.SPEED = Convert.ToSingle(objResInfo.SPEED); //(objResInfo.objInfo[i].Speed < 0) ? 2 : Convert.ToSingle(objResInfo.objInfo[i].Speed);
            spInput.CentralLock = objResInfo.CentralLock;// (objResInfo.objInfo[i].CentralLock < 0) ? 2 : objResInfo.objInfo[i].CentralLock;
            spInput.MachineNo = objResInfo.CID;// objResInfo.objInfo[i].carCarMachineNO.ToUpper();
            spInput.DoorStatus = objResInfo.doorStatus;// objResInfo.objInfo[i].doorStatus;
            spInput.GPRSStatus = objResInfo.GPRSStatus;// objResInfo.objInfo[i].GPRSStatus;
            spInput.GPSStatus = objResInfo.GPSStatus;// objResInfo.objInfo[i].GPSStatus;
            spInput.Lat = Convert.ToDecimal(objResInfo.Lat);
            spInput.Lng = Convert.ToDecimal(objResInfo.Lng);
            spInput.LockStatus = objResInfo.lockStatus;
            spInput.Milage = Convert.ToSingle(objResInfo.Milage); //(objResInfo.objInfo[i].Milage < 0) ? -1 : Convert.ToSingle(objResInfo.objInfo[i].Milage);
            spInput.OBDStatus = objResInfo.OBDStatus;// (objResInfo.objInfo[i].OBDStatus < 0) ? 2 : objResInfo.objInfo[i].OBDStatus;
            spInput.PowON = objResInfo.PowON;// (objResInfo.objInfo[i].PowON < 0) ? 2 : objResInfo.objInfo[i].PowON;
            spInput.Volt = Convert.ToSingle(objResInfo.Volt);// (objResInfo.objInfo[i].Volt < 0) ? 2 : Convert.ToSingle(objResInfo.objInfo[i].Volt);
            spInput.LightStatus = objResInfo.indoorLight;// (objResInfo.objInfo[i].indoorLight < 0) ? 2 : objResInfo.objInfo[i].indoorLight;
            spInput.OrderStatus = objResInfo.OrderStatus;// (objResInfo.objInfo[i].OrderStatus < 0) ? 2 : objResInfo.objInfo[i].OrderStatus;
            spInput.SecurityStatus = objResInfo.SecurityStatus == 1 ? 0 : 1;// (objResInfo.objInfo[i].SecurityStatus < 0) ? 2 : objResInfo.objInfo[i].SecurityStatus;
            spInput.GPSTime = objResInfo.GPSTime;// (objResInfo.objInfo[i].GpsTime == null) ? DateTime.Now : objResInfo.objInfo[i].GpsTime;
            spInput.iButton = objResInfo.iButton;// (objResInfo.objInfo[i].iButton < 0) ? 0 : objResInfo.objInfo[i].iButton;
            spInput.iButtonKey = objResInfo.iButtonKey;// (string.IsNullOrEmpty(objResInfo.objInfo[i].iButtonKey) ? "" : objResInfo.objInfo[i].iButtonKey);
            spInput.fwver = objResInfo.fwver ?? "";
            spInput.CSQ = objResInfo.CSQ ?? ""; // 
            spInput.retrycnt = objResInfo.retrycnt ?? ""; //
            spInput.gaslvl = Convert.ToSingle(objResInfo.gaslvl); //

            string errCode = "";
            SPOutput_Base spOut = new SPOutput_Base();
            string spName = "usp_InsCarStatus_V20220427";

            SQLHelper<SPInput_InsCarStatus, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsCarStatus, SPOutput_Base>(connetStr);
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
            new CommonFunc().checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);
            if (false == flag)
            {
                //  Console.WriteLine("錯誤代碼{0}、錯誤訊息{1}", lstError[0].ErrorCode, lstError[0].ErrorMsg);
              //  new CommonFunc().InsErrorLog("InsCarStatus", errCode, 0, 0, "");
                new CommonFunc().InsErrorLog(spName, errCode, 0, LogID, 0, 0, "");
            }
        }
        /// <summary>
        /// 進出場域
        /// </summary>
        /// <param name="CID"></param>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <param name="GPSTime"></param>
        /// <param name="PowOn"></param>
        //private void doCalEntry(string CID, decimal lat, decimal lng, DateTime GPSTime, int PowOn)
        //{
        //    StationRepository _repository = new StationRepository(connetStr);
        //    List<CalEntryBlock> lstBlock = null; //_repository.GetCalEntryData();
        //    List<PCID> lstCID = _repository.getCID(CID);
        //    if (lstCID != null)
        //    {
        //        if (lstCID.Count > 0)
        //        {
        //            lstBlock = _repository.GetCalEntryDataByAB();
        //        }
        //    }
        //    else
        //    {
        //        lstBlock = _repository.GetCalEntryData();
        //    }
        //    //lstBlock = _repository.GetCalParkData(ProjID);
        //    int Len = 0;
        //    if (lstBlock != null)
        //    {
        //        Len = lstBlock.Count;
        //        if (Len > 0)
        //        {
        //            SPInput_InsEntryLog spInput = new SPInput_InsEntryLog()
        //            {
        //                StationID = "NO",
        //                Mode = 1,
        //                MachineNO = CID,
        //                PowOn = PowOn,
        //                MKTime = (GPSTime.ToString("yyyy-MM-dd HH:mm:ss") == "1970-01-01 00:00:00") ? DateTime.Now : GPSTime
        //            };
        //            WebCommon.Polygon checkPoint = new WebCommon.Polygon()
        //            {
        //                px = Convert.ToSingle(lat),
        //                py = Convert.ToSingle(lng)
        //            };

        //            for (int j = 0; j < Len; j++)
        //            {
        //                bool controlFlag = false;
        //                int Pnum = lstBlock[j].Polygon.Count;
        //                for (int k = 0; k < Pnum; k++)
        //                {
        //                    PolygonModel pm = new PolygonModel();

        //                    bool flag = pm.isInPolygonNew(ref lstBlock[j].Polygon[k].polygon, checkPoint);
        //                    if (flag)
        //                    {
        //                        spInput.StationID = lstBlock[j].StationID;
        //                        spInput.Mode = 0; //進站
        //                        controlFlag = true;
        //                        break;
        //                    }
        //                    if (flag)
        //                    {
        //                        controlFlag = true;
        //                        break;
        //                    }
        //                }
        //            }
        //            SPOutput_Base spOut = new SPOutput_Base();
        //            string spName = "usp_InsEntryLog_201701";
        //            List<ErrorInfo> lstError = new List<ErrorInfo>();
        //            SQLHelper<SPInput_InsEntryLog, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsEntryLog, SPOutput_Base>(connetStr);
        //            bool wflag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
        //            string errCode = "";
        //            new CommFunc().checkSQLResult(ref wflag, ref spOut, ref lstError, ref errCode);
        //            if (false == wflag)
        //            {
            
        //                new CommFunc().InsErrorLog("InsEntry", errCode, 0, 0, "");
        //            }
        //        }
        //    }

        //}
    }
}
