using Domain.Common;
using Domain.SP.Input.OtherService.Common;
using Domain.SP.Input.OtherService.FET;
using Domain.SP.Input.Register;
using Domain.SP.Output;
using Newtonsoft.Json;
using NLog;
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
    public class ReceiveCarData2Controller : ApiController
    {
        protected static Logger logger = LogManager.GetCurrentClassLogger();

        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;

        public Dictionary<string, object> doVerifyEMail(Dictionary<string, object> value)
        {
            //20201030 NLog紀錄傳入資料
            //logger.Trace(value["para"].ToString());

            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "ReceiveCarDataController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CarData VehicleInput = null;
            IAPI_MotorData MotorDataInput = null;
            IAPI_ReceiveCMDBase CMDBase = null;
            IAPI_CarMachineVerInfo CarMachineVerInfoInput = null;


            Int16 DataType = 0; //0:汽車;1:機車;2:下指令回傳
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            string SPName = "";
            #endregion
            #region 防呆
            string ClientIP = baseVerify.GetClientIp(Request);
            Contentjson = JsonConvert.SerializeObject(value);

            flag = baseVerify.baseCheck(value, ref errCode, funName);
            if (!flag && errCode == "ERR902")
            {
                flag = true;
                errCode = "000000";
            }
            if (flag)
            {
                if (Contentjson.IndexOf("values") > -1)
                {
                    //寫入API Log

                    flag = baseVerify.InsAPLog(value["values"].ToString(), ClientIP, funName, ref errCode, ref LogID);
                    Contentjson = value["values"].ToString();
                }
                else
                {
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);
                }
            }

            if (flag)
            {
                //if (value["para"].ToString().IndexOf("method") > -1)
                if (Contentjson.IndexOf("method") > -1)
                {
                    DataType = 2;//一般的命令
                    //if (value["para"].ToString().IndexOf("extDeviceData4") > -1) //取得萬用卡或顧客卡
                    //{
                    //    DataType = 3;
                    //}

                }
                else if (Contentjson.IndexOf("deviceBrandName") > -1 && Contentjson.IndexOf("deviceFW") > -1)
                {
                    DataType = 3; //韌體版本
                }
                else
                {
                    //if (value["para"].ToString().IndexOf("Motorcycle") > -1)
                    if (Contentjson.IndexOf("Motorcycle") > -1)
                    {
                        DataType = 1;
                    }
                }
            }


            #endregion
            #region TB
            if (flag)
            {
                try
                {
                    SPOutput_Base spout = new SPOutput_Base();
                    switch (DataType)
                    {
                        case 0://寫入汽車定時回報
                            VehicleInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CarData>(Contentjson);
                            if (string.IsNullOrWhiteSpace(VehicleInput.extDeviceData7) == false)//讀卡
                            {
                                SPName = new OtherService.Enum.ObjType().GetSPName(OtherService.Enum.ObjType.SPType.InsReadCard);
                                SPInput_InsReadCard SPInsReadCard = new SPInput_InsReadCard()
                                {
                                    CardNo = VehicleInput.extDeviceData7.Split(',')[1],
                                    CID = VehicleInput.deviceCID.ToUpper(),
                                    GPSTime = Convert.ToDateTime(VehicleInput.deviceGPSTime).AddHours(8),
                                    Status = VehicleInput.extDeviceData7.Split(',')[0],
                                    LogID = LogID
                                };
                                SQLHelper<SPInput_InsReadCard, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_InsReadCard, SPOutput_Base>(connetStr);
                                flag = sqlHelp.ExecuteSPNonQuery(SPName, SPInsReadCard, ref spout, ref lstError);
                                baseVerify.checkSQLResult(ref flag, spout.Error, spout.ErrorCode, ref lstError, ref errCode);
                            }
                            SPName = new OtherService.Enum.ObjType().GetSPName(OtherService.Enum.ObjType.SPType.HandleCarStatus);

                            SPInput_CarStatus SPRepsonseInput = new SPInput_CarStatus()
                            {
                                devcieCentralLockStatus = VehicleInput.devcieCentralLockStatus,
                                deviceACCStatus = VehicleInput.deviceACCStatus,
                                deviceCID = VehicleInput.deviceCID.ToUpper(),
                                deviceDoorStatus = VehicleInput.deviceDoorStatus,
                                deviceGPRSStatus = VehicleInput.deviceGPRSStatus,
                                deviceGPSStatus = VehicleInput.deviceGPSStatus,
                                deviceGPSTime = Convert.ToDateTime(VehicleInput.deviceGPSTime).AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),
                                deviceIndoorLightStatus = VehicleInput.deviceIndoorLightStatus,
                                deviceLatitude = VehicleInput.deviceLatitude,
                                deviceLockStatus = VehicleInput.deviceLockStatus,
                                deviceLongitude = VehicleInput.deviceLongitude,
                                deviceMillage = VehicleInput.deviceMillage,
                                deviceOBDstatus = VehicleInput.deviceOBDstatus,
                                devicePowerONStatus = VehicleInput.devicePowerONStatus,
                                deviceSecurityStatus = VehicleInput.deviceSecurityStatus,
                                deviceSpeed = Convert.ToInt32(VehicleInput.deviceSpeed),
                                deviceType = 0,
                                deviceVolt = VehicleInput.deviceVolt,
                                extDeviceData3 = VehicleInput.extDeviceData3,
                                extDeviceData4 = VehicleInput.extDeviceData4,
                                extDeviceData7 = VehicleInput.extDeviceData7,
                                extDeviceData2 = VehicleInput.extDeviceData2,
                                extDeviceStatus1 = VehicleInput.extDeviceStatus1,
                                extDeviceStatus2 = VehicleInput.extDeviceStatus2,
                                deviceName = VehicleInput.deviceName,
                                LogID = LogID
                            };
                            SQLHelper<SPInput_CarStatus, SPOutput_Base> SQLRepsonseHelp = new SQLHelper<SPInput_CarStatus, SPOutput_Base>(connetStr);
                            flag = SQLRepsonseHelp.ExecuteSPNonQuery(SPName, SPRepsonseInput, ref spout, ref lstError);
                            baseVerify.checkSQLResult(ref flag, spout.Error, spout.ErrorCode, ref lstError, ref errCode);
                            break;
                        case 1:
                            MotorDataInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MotorData>(Contentjson);
                            SPName = new OtherService.Enum.ObjType().GetSPName(OtherService.Enum.ObjType.SPType.HandleCarStatusByMotor);

                            var device2TBA = 0.0;
                            var device3TBA = 0.0;
                            var deviceLBA = 0.0;
                            var deviceMBA = 0.0;
                            var deviceRBA = 0.0;
                            MotorDataInput.deviceLBA = MotorDataInput.deviceLBA == "NA" ? "-999" : MotorDataInput.deviceLBA;
                            MotorDataInput.deviceMBA = MotorDataInput.deviceMBA == "NA" ? "-999" : MotorDataInput.deviceMBA;
                            MotorDataInput.deviceRBA = MotorDataInput.deviceRBA == "NA" ? "-999" : MotorDataInput.deviceRBA;
                            double.TryParse(MotorDataInput.device2TBA, out device2TBA);
                            double.TryParse(MotorDataInput.device3TBA, out device3TBA);
                            double.TryParse(MotorDataInput.deviceLBA, out deviceLBA);
                            double.TryParse(MotorDataInput.deviceMBA, out deviceMBA);
                            double.TryParse(MotorDataInput.deviceRBA, out deviceRBA);
                            SPInput_CarStatusByMotor SPMRepsonseInput = new SPInput_CarStatusByMotor()
                            {
                                device2TBA = device2TBA,
                                device3TBA = device3TBA,
                                deviceALT = MotorDataInput.deviceALT,
                                deviceBat_Cover = MotorDataInput.deviceBat_Cover,
                                deviceBLE_BroadCast = MotorDataInput.deviceBLE_BroadCast,
                                deviceBLE_Login = MotorDataInput.deviceBLE_Login,
                                deviceCourse = MotorDataInput.deviceCourse,
                                deviceCur = MotorDataInput.deviceCur,
                                deviceEMG_Break = MotorDataInput.deviceEMG_Break,
                                deviceErr = MotorDataInput.deviceErr,
                                deviceGx = MotorDataInput.deviceGx,
                                deviceGy = MotorDataInput.deviceGy,
                                deviceGz = MotorDataInput.deviceGz,
                                deviceHard_ACC = MotorDataInput.deviceHard_ACC,
                                deviceiSpeed = MotorDataInput.deviceiSpeed,
                                deviceiVOL = MotorDataInput.deviceiVOL,
                                deviceLBA = deviceLBA,
                                deviceLBAA = MotorDataInput.deviceLBAA,
                                deviceLBAT_Hi = MotorDataInput.deviceLBAT_Hi,
                                deviceLBAT_Lo = MotorDataInput.deviceLBAT_Lo,
                                deviceLowVoltage = MotorDataInput.deviceLowVoltage,
                                deviceMBA = deviceMBA,
                                deviceMBAA = MotorDataInput.deviceMBAA,
                                deviceMBAT_Hi = MotorDataInput.deviceMBAT_Hi,
                                deviceMBAT_Lo = MotorDataInput.deviceMBAT_Lo,
                                devicePut_Down = MotorDataInput.devicePut_Down,
                                devicePwr_Mode = MotorDataInput.devicePwr_Mode,
                                devicePwr_Relay = MotorDataInput.devicePwr_Relay,
                                deviceRBA = deviceRBA,
                                deviceRBAA = MotorDataInput.deviceRBAA,
                                deviceRBAT_Hi = MotorDataInput.deviceRBAT_Hi,
                                deviceRBAT_Lo = MotorDataInput.deviceRBAT_Lo,
                                deviceRDistance = MotorDataInput.deviceRDistance,
                                deviceReversing = MotorDataInput.deviceReversing,
                                deviceRPM = MotorDataInput.deviceRPM,
                                deviceRSOC = MotorDataInput.deviceRSOC,
                                deviceSharp_Turn = MotorDataInput.deviceSharp_Turn,
                                deviceStart_OK = MotorDataInput.deviceStart_OK,
                                deviceTMP = MotorDataInput.deviceTMP,
                                deviceTPS = MotorDataInput.deviceTPS,
                                extDeviceData5 = MotorDataInput.extDeviceData5,
                                extDeviceData6 = MotorDataInput.extDeviceData6,
                                deviceACCStatus = MotorDataInput.deviceACCStatus,
                                deviceCID = MotorDataInput.deviceCID.ToUpper(),

                                deviceGPRSStatus = MotorDataInput.deviceGPRSStatus,
                                deviceGPSStatus = MotorDataInput.deviceGPSStatus,
                                deviceGPSTime = Convert.ToDateTime(MotorDataInput.deviceGPSTime).AddHours(8).ToString("yyyy-MM-dd HH:mm:ss"),

                                deviceLatitude = MotorDataInput.deviceLatitude,

                                deviceLongitude = MotorDataInput.deviceLongitude,
                                deviceMillage = MotorDataInput.deviceMillage,

                                deviceSpeed = Convert.ToInt32(MotorDataInput.deviceSpeed),
                                deviceType = 1,
                                deviceVolt = MotorDataInput.deviceVolt,

                                extDeviceData2 = MotorDataInput.extDeviceData2,
                                extDeviceStatus1 = MotorDataInput.extDeviceStatus1,
                                deviceName = MotorDataInput.deviceName,

                                LogID = LogID
                            };
                            SQLHelper<SPInput_CarStatusByMotor, SPOutput_Base> SQLMRepsonseHelp = new SQLHelper<SPInput_CarStatusByMotor, SPOutput_Base>(connetStr);
                            flag = SQLMRepsonseHelp.ExecuteSPNonQuery(SPName, SPMRepsonseInput, ref spout, ref lstError);
                            baseVerify.checkSQLResult(ref flag, spout.Error, spout.ErrorCode, ref lstError, ref errCode);
                            break;

                        case 2:
                            SPName = new OtherService.Enum.ObjType().GetSPName(OtherService.Enum.ObjType.SPType.InsReceiveCMD);
                            CMDBase = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_ReceiveCMDBase>(Contentjson);
                            SPInput_InsReceiveCMD spInput = new SPInput_InsReceiveCMD()
                            {
                                CmdReply = CMDBase.CmdReply,
                                method = CMDBase.method,
                                requestId = CMDBase.requestId,
                                receiveRawData = Contentjson,
                                LogID = LogID
                            };

                            SQLHelper<SPInput_InsReceiveCMD, SPOutput_Base> SQLCancelHelper = new SQLHelper<SPInput_InsReceiveCMD, SPOutput_Base>(connetStr);
                            flag = SQLCancelHelper.ExecuteSPNonQuery(SPName, spInput, ref spout, ref lstError);
                            baseVerify.checkSQLResult(ref flag, spout.Error, spout.ErrorCode, ref lstError, ref errCode);
                            break;

                        case 3:
                            SPName = new OtherService.Enum.ObjType().GetSPName(OtherService.Enum.ObjType.SPType.UpdCarMachineVerInfo);
                            CarMachineVerInfoInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CarMachineVerInfo>(Contentjson);
                            SPInput_UpdCarMachineVerInfo spCMVerInfoInput = new SPInput_UpdCarMachineVerInfo()
                            {
                                deviceName = CarMachineVerInfoInput.deviceName,
                                deviceCID = CarMachineVerInfoInput.deviceCID.ToUpper(),
                                deviceFW = CarMachineVerInfoInput.deviceFW,
                                LogID = LogID
                            };

                            SQLHelper<SPInput_UpdCarMachineVerInfo, SPOutput_Base> SQLCMVerInfoHelper = new SQLHelper<SPInput_UpdCarMachineVerInfo, SPOutput_Base>(connetStr);
                            flag = SQLCMVerInfoHelper.ExecuteSPNonQuery(SPName, spCMVerInfoInput, ref spout, ref lstError);
                            baseVerify.checkSQLResult(ref flag, spout.Error, spout.ErrorCode, ref lstError, ref errCode);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    flag = false;
                    errCode = "ERR999";
                    errMsg = "";
                    //logger.Trace(value["para"].ToString());
                    logger.Error(ex.Message);
                }
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
                errMsg = errCode;

                //20201020 ADD BY JERRY 增加DB錯誤寫入LOG的處理
                if (lstError.ToArray<ErrorInfo>().Length > 0)
                {
                    for (int i = 0; i < lstError.ToArray<ErrorInfo>().Length; i++)
                    {
                        ErrorInfo errorInfo = lstError.ToArray<ErrorInfo>()[i];
                        baseVerify.InsErrorLog(funName, errorInfo.ErrorCode, ErrType, LogID, 0, 0, errorInfo.ErrorMsg);

                        //20201020 ADD BY JERRY 增加錯誤訊息回傳
                        errCode = errorInfo.ErrorCode;
                        errMsg = errorInfo.ErrorMsg;
                    }
                }
            }


            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, CheckAccountAPI, token);
            return objOutput;
            #endregion
        }
    }
}