using Domain.Common;
using Domain.SP.BE.Input;
using Domain.SP.Output;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_HandleStationController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        string stationContainer = (System.Configuration.ConfigurationManager.AppSettings["stationContainer"] == null) ? "" : System.Configuration.ConfigurationManager.AppSettings["stationContainer"].ToString();
        /// <summary>
        /// 【後台】修改調度停車場
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doBE_HandleStation(Dictionary<string, object> value)
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
            string funName = "BE_HandleStationController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_BE_HandleStation apiInput = null;
            NullOutput apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            DateTime SD = DateTime.Now, ED = DateTime.Now;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_HandleStation>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = { apiInput.UserID, apiInput.StationID, apiInput.StationName, apiInput.ManagerStationID, apiInput.FCode, apiInput.Addr,apiInput.show_description };
                string[] errList = { "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (apiInput.Latitude <= 0 || apiInput.Longitude <= 0)
                {
                    flag = false;
                    errCode = "ERR900";
                }
                if (flag)
                {
                    if (apiInput.SDate > apiInput.EDate)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                    else
                    {
                        ED = apiInput.EDate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                }

            }
            #endregion
            #region 處理azure
            if (flag)
            {
                if (apiInput.fileData1 != "")
                {
                    string FileName = string.Format("{0}_1_{1}.png", apiInput.StationID, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.fileData1, FileName,stationContainer);
                    if (flag)
                    {
                        apiInput.fileName1 = FileName;
                    }
                }
                if (apiInput.fileData2 != "")
                {
                    string FileName = string.Format("{0}_2_{1}.png", apiInput.StationID, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.fileData2, FileName, stationContainer);
                    if (flag)
                    {
                        apiInput.fileName2 = FileName;
                    }
                }
                if (apiInput.fileData3 != "")
                {
                    string FileName = string.Format("{0}_3_{1}.png", apiInput.StationID, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.fileData3, FileName, stationContainer);
                    if (flag)
                    {
                        apiInput.fileName3 = FileName;
                    }
                }
                if (apiInput.fileData4 != "")
                {
                    string FileName = string.Format("{0}_4_{1}.png", apiInput.StationID, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.fileData4, FileName, stationContainer);
                    if (flag)
                    {
                        apiInput.fileName4 = FileName;
                    }
                }
                if (apiInput.fileData5 != "")
                {
                    string FileName = string.Format("{0}_5_{1}.png", apiInput.StationID, DateTime.Today.ToString("yyyyMMddHHmmss"));
                    flag = new AzureStorageHandle().UploadFileToAzureStorage(apiInput.fileData5, FileName, stationContainer);
                    if (flag)
                    {
                        apiInput.fileName5 = FileName;
                    }
                }
            }
            #endregion
            #region TB

            if (flag)
            {

                string spName = new ObjType().GetSPName(ObjType.SPType.BE_HandleStation);
                SPInput_HandleStation spInput = new SPInput_HandleStation()
                {
                    LogID = LogID,
                    SDate = apiInput.SDate,
                    Addr = apiInput.Addr,
                    Area = apiInput.Area,
                    AreaID = apiInput.AreaID,
                    CityID = apiInput.CityID,
                    EDate = ED,
                    FCode = apiInput.FCode,
                    Latitude = apiInput.Latitude,
                    Longitude = apiInput.Longitude,
                    fileDescript1 = apiInput.fileDescript1,
                    fileDescript2 = apiInput.fileDescript2,
                    fileDescript3 = apiInput.fileDescript3,
                    fileDescript4 = apiInput.fileDescript4,
                    fileDescript5 = apiInput.fileDescript5,
                    fileName1 = apiInput.fileName1,
                    fileName2 = apiInput.fileName2,
                    fileName3 = apiInput.fileName3,
                    fileName4 = apiInput.fileName4,
                    fileName5 = apiInput.fileName5,
                    in_description = apiInput.in_description,
                    IsRequired = apiInput.IsRequired,
                    ManagerStationID = apiInput.ManagerStationID,
                    Mode = apiInput.Mode,
                    OnlineNum = apiInput.OnlineNum,
                    ParkingNum = apiInput.ParkingNum,
                    show_description = apiInput.show_description,
                    StationID = apiInput.StationID,
                    StationName = apiInput.StationName,
                    StationPick = apiInput.StationPick,
                    StationType = apiInput.StationType,
                    TEL = apiInput.TEL,
                    UniCode = apiInput.UniCode,
                    UserID = apiInput.UserID
                };
                SPOutput_Base spOut = new SPOutput_Base();
                SQLHelper<SPInput_HandleStation, SPOutput_Base> sqlHelp = new SQLHelper<SPInput_HandleStation, SPOutput_Base>(connetStr);
                flag = sqlHelp.ExecuteSPNonQuery(spName, spInput, ref spOut, ref lstError);
                baseVerify.checkSQLResult(ref flag, ref spOut, ref lstError, ref errCode);

            }
            #endregion

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
