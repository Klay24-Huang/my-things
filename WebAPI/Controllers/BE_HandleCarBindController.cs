using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using Domain.TB.BackEnd;
using Domain.WebAPI.Input.FET;
using Domain.WebAPI.output.FET;
using OtherService;
using Reposotory.Implement;
using Reposotory.Implement.BackEnd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 車機綁定
    /// </summary>
    public class BE_HandleCarBindController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】車機綁定
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> handleCarBind(Dictionary<string, object> value)
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
            string funName = "BE_HandleCarBindController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleCarBind apiInput = null;
            OAPI_BE_HandleCarBind apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";

            CarCardCommonRepository carCardCommonRepository = new CarCardCommonRepository(connetStr);
            FETDeviceMaintainAPI deviceMaintainAzure = new FETDeviceMaintainAPI(FETDeviceMaintainAPI.HASwitch.Azure);
            List<BE_CarBindImportData> lstCarBindImportData = new List<BE_CarBindImportData>();
            List<BE_CarBindImportDataResult> lstCarBindImportDataResult = new List<BE_CarBindImportDataResult>();
            string enableFETCloudDeviceMaintain = ConfigurationManager.AppSettings.Get("EnableFETCloudDeviceMaintain") ?? "0";
            FETDeviceMaintainAPI deviceMaintainFETCloud = (enableFETCloudDeviceMaintain == "1") ? new FETDeviceMaintainAPI(FETDeviceMaintainAPI.HASwitch.FETCloud) : null;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleCarBind>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID };
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);


            }
            #endregion

            #region TB

            if (flag)
            {
                lstCarBindImportData = apiInput.CarBindImportData;
                if (lstCarBindImportData.Count > 0)
                {
                    //取得CAT平台Token
                    var loginAzureCAT = deviceMaintainAzure.DoLogin();
                    var loginFETCloudCAT = new WebAPIOutput_ResultDTO<WebAPIOutput_Login>()
                    {
                        Result = true
                    };
                    if (enableFETCloudDeviceMaintain == "1")
                    {
                        loginFETCloudCAT = deviceMaintainFETCloud.DoLogin();
                    }

                    if (!loginAzureCAT.Result || !loginFETCloudCAT.Result)
                    {
                        flag = false;
                        errMsg = "取得CAT API Token失敗。";
                        errMsg += (!loginAzureCAT.Result) ? "Azure Error=" + loginAzureCAT.Message : "";
                        errMsg += (!loginFETCloudCAT.Result) ? "FET Cloud Error=" + loginFETCloudCAT.Message : "";
                    }
                    else
                    {
                        //查詢CarInfo資料
                        var lstCarInfoForMachineData = carCardCommonRepository.GetCarInfoForMachineData("");
                        foreach (var importData in lstCarBindImportData)
                        {
                            bool importFlag = true;
                            string importMessage = "";
                            if (string.IsNullOrEmpty(importData.CarNo) ||
                                string.IsNullOrEmpty(importData.CID))
                            {
                                importFlag = false;
                                importMessage = "[車號]、[車機編號]不可為空白";
                            }
                            else
                            {
                                var lstCarInfoData = (from data in lstCarInfoForMachineData
                                                      where data.CarNo.Trim() == importData.CarNo
                                                      select data).ToList();

                                if (lstCarInfoData.Count() == 0)
                                {
                                    importFlag = false;
                                    importMessage = "[車號]查無資料";
                                }
                                else
                                {
                                    BE_CarInfoForMachineData carInfoData = lstCarInfoData[0];
                                    if (carInfoData.HasIButton == 1 && importData.IBUTTON == "")
                                    {
                                        importFlag = false;
                                        importMessage = "[iButton]不可為空白";
                                    }
                                    if (importData.CID.Length != 4 && importData.CID.Length != 5)
                                    {
                                        importFlag = false;
                                        importMessage = "遠傳車機[CID]應為4碼，興聯車機[CID]應為5碼";
                                    }
                                    if (importData.CID.Length == 5)
                                    {
                                        //興聯車機直接更新資料
                                        if (importFlag)
                                        {
                                            string SPName = new ObjType().GetSPName(ObjType.SPType.BE_ImportCarBindData);
                                            //更新資料
                                            SPInput_BE_ImportCarBindData data = new SPInput_BE_ImportCarBindData()
                                            {
                                                CID = importData.CID,
                                                CarNo = carInfoData.CarNo,
                                                iButtonKey = importData.IBUTTON,
                                                MobileNum = importData.MobileNum,
                                                SIMCardNo = importData.SIMCardNo,
                                                UserID = apiInput.UserID,
                                                LogID = 0
                                            };

                                            SPOutput_Base SPOutput = new SPOutput_Base();
                                            flag = new SQLHelper<SPInput_BE_ImportCarBindData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                            baseVerify.checkSQLResult(ref importFlag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                            if (!importFlag)
                                            {
                                                importMessage = string.Format("更新車機綁定資訊:{0}", baseVerify.GetErrorMsg(errCode));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //遠傳車機
                                        //更新CAT資料，產生deviceId與deviceToken
                                        if (importFlag)
                                        {
                                            if (carInfoData.deviceId == "" || carInfoData.deviceToken == "")
                                            {
                                                var accessToken = string.Format("{0}-{1}", importData.CarNo, Guid.NewGuid().ToString().ToUpper().Substring(0, 20 - importData.CarNo.Length - 1));
                                                WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken> addDeviceTokenAzure = deviceMaintainAzure.AddDeviceToken(importData.CarNo,
                                                    carInfoData.IsMotor == 0 ? FETDeviceMaintainAPI.CATCarType.Car : FETDeviceMaintainAPI.CATCarType.Motor, accessToken);
                                                //需同步更新FET Cloud的資訊
                                                WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken> addDeviceTokenFETCloud = new WebAPIOutput_ResultDTO<WebAPIOutput_AddDeviceToken>()
                                                { 
                                                    Result = true
                                                };
                                                if (enableFETCloudDeviceMaintain == "1")
                                                {
                                                    addDeviceTokenFETCloud = deviceMaintainFETCloud.AddDeviceToken(importData.CarNo,
                                                    carInfoData.IsMotor == 0 ? FETDeviceMaintainAPI.CATCarType.Car : FETDeviceMaintainAPI.CATCarType.Motor, accessToken);
                                                }

                                                if (addDeviceTokenAzure.Result && addDeviceTokenFETCloud.Result)
                                                {
                                                    carInfoData.deviceId = addDeviceTokenAzure.Data.deviceId;
                                                    carInfoData.deviceToken = addDeviceTokenAzure.Data.deviceToken;

                                                    //更新deviceId與deviceToken
                                                    string SPName = new ObjType().GetSPName(ObjType.SPType.BE_UpdCATDeviceToken);
                                                    //更新資料
                                                    SPInput_BE_UpdCATDeviceToken data = new SPInput_BE_UpdCATDeviceToken()
                                                    {
                                                        CarNo = carInfoData.CarNo,
                                                        deviceId = addDeviceTokenAzure.Data.deviceId,
                                                        deviceToken = addDeviceTokenAzure.Data.deviceToken,
                                                        UserID = apiInput.UserID,
                                                        LogID = 0
                                                    };

                                                    SPOutput_Base SPOutput = new SPOutput_Base();
                                                    flag = new SQLHelper<SPInput_BE_UpdCATDeviceToken, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                                    baseVerify.checkSQLResult(ref importFlag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                                    if (!importFlag)
                                                    {
                                                        importMessage = string.Format("更新deviceToken:{0}", baseVerify.GetErrorMsg(errCode));
                                                    }
                                                }
                                                else
                                                {
                                                    importFlag = false;
                                                    importMessage = string.Format("CAT:{0}{1}", 
                                                        (!addDeviceTokenAzure.Result) ? "Azure:" + addDeviceTokenAzure.Message + ";" : "", 
                                                        (!addDeviceTokenFETCloud.Result) ? "FETCloud:" + addDeviceTokenFETCloud.Message + ";" : "");
                                                }
                                            }
                                        }
                                        //更新GCP資料
                                        if (importFlag)
                                        {
                                            List<WebAPIInput_GCPUpMapping> lstGCPUpMapping = new List<WebAPIInput_GCPUpMapping>();
                                            lstGCPUpMapping.Add(new WebAPIInput_GCPUpMapping
                                            {
                                                deviceCID = importData.CID,
                                                deviceName = carInfoData.CarNo,
                                                deviceToken = carInfoData.deviceToken,
                                                deviceType = carInfoData.IsMotor == 0 ? FETDeviceMaintainAPI.GCPCarType.Vehicle.ToString() : FETDeviceMaintainAPI.GCPCarType.Motorcycle.ToString()
                                            });
                                            WebAPIOutput_ResultDTO<List<WebAPIOutput_GCPUpMapping>> GCPUpMapping = deviceMaintainAzure.GCPUpMapping(lstGCPUpMapping);
                                            if (!GCPUpMapping.Result || GCPUpMapping.Data.Count == 0 || GCPUpMapping.Data[0].upResult == "NotOkay")
                                            {
                                                importFlag = false;
                                                importMessage = "GCP資料錯誤";
                                            }
                                            else
                                            {
                                                string SPName = new ObjType().GetSPName(ObjType.SPType.BE_ImportCarBindData);
                                                //更新資料
                                                SPInput_BE_ImportCarBindData data = new SPInput_BE_ImportCarBindData()
                                                {
                                                    CID = importData.CID,
                                                    CarNo = carInfoData.CarNo,
                                                    iButtonKey = importData.IBUTTON,
                                                    MobileNum = importData.MobileNum,
                                                    SIMCardNo = importData.SIMCardNo,
                                                    UserID = apiInput.UserID,
                                                    LogID = 0
                                                };

                                                SPOutput_Base SPOutput = new SPOutput_Base();
                                                flag = new SQLHelper<SPInput_BE_ImportCarBindData, SPOutput_Base>(connetStr).ExecuteSPNonQuery(SPName, data, ref SPOutput, ref lstError);
                                                baseVerify.checkSQLResult(ref importFlag, SPOutput.Error, SPOutput.ErrorCode, ref lstError, ref errCode);
                                                if (!importFlag)
                                                {
                                                    importMessage = string.Format("更新車機綁定資訊:{0}", baseVerify.GetErrorMsg(errCode));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            lstCarBindImportDataResult.Add(new BE_CarBindImportDataResult
                            {
                                CID = importData.CID,
                                CarNo = importData.CarNo,
                                Result = importFlag,
                                Message = importMessage
                            });
                        }
                        apiOutput = new OAPI_BE_HandleCarBind
                        {
                            CarBindImportDataResult = lstCarBindImportDataResult
                        };
                    }
                }
                #endregion

            }

            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}