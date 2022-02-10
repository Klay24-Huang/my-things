using Domain.Common;
using Domain.SP.MA.Input;
using Domain.SP.Output;
using Domain.TB.Maintain;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Maintain.Input;
using WebAPI.Models.Param.Maintain.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【整備人員】取得車輛資料_列表模式
    /// </summary>
    public class MA_GetCleanCarByListController : ApiController
    {
        //20211130 ADD BY ADAM REASON.效能因素改為由鏡像主機取得
        private string connetStr = ConfigurationManager.ConnectionStrings["IRentMirror"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoMA_GetCleanCarByList(Dictionary<string, object> value)
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
            string funName = "MA_GetCleanCarByListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MA_CleanCarByList apiInput = null;
            List<OAPI_MA_CleanCarByLatLng> outputApi = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            CarRepository _repository = new CarRepository(connetStr);
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            string ManageStation = "";
            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;

            string IDNO = "";
            #endregion
            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MA_CleanCarByList>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            }
            #endregion
            #region 取得使用者資料
            MemberCleanSetting setting = _repository.GetMemberCleanSetting(apiInput.Account);
            if (setting != null)
            {
                if (setting.StationGroup != "")
                {
                    ManageStation = setting.StationGroup;
                }
            }
            else
            {
                ManageStation = "";
            }


            #endregion
            #region 組合輸出資料
            try
            {
                outputApi = new List<OAPI_MA_CleanCarByLatLng>();
                DateTime NowDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                switch (apiInput.NowType)
                {
                    case 0:
                        List<CarCleanDataNew> lstNor = _repository.GetCleanCarData(0, ManageStation, 0,0,0);

                        OAPI_MA_CleanCarByLatLng nor = new OAPI_MA_CleanCarByLatLng
                        {
                            CarList = lstNor,
                            projType = 0,
                            total = lstNor.Count
                        };
                        for (int i = 0; i < nor.total; i++)
                        {
                            nor.CarList[i].NowStationName = nor.CarList[i].NowStationName.Replace("\t", "");
                            DateTime Rent = new DateTime(nor.CarList[i].LastRentTime.Year, nor.CarList[i].LastRentTime.Month, nor.CarList[i].LastRentTime.Day, 0, 0, 0);
                            nor.CarList[i].afterRentDays = (int)(NowDay.Subtract(Rent).TotalDays);
                            if (DateTime.Now.Subtract(nor.CarList[i].GPSTime).TotalHours > 1)
                            {
                                nor.CarList[i].isNoResponse = 1;
                            }
                            if (nor.CarList[i].Milage - nor.CarList[i].LastMaintenanceMilage >= 10000)
                            {
                                nor.CarList[i].isNeedMaintenance = 1;
                            }
                        }
                        outputApi.Add(nor);
                        break;
                    case 1:
                        List<CarCleanDataNew> lstAny = _repository.GetCleanCarData(1, ManageStation, 0, 0, 0);

                        OAPI_MA_CleanCarByLatLng any = new OAPI_MA_CleanCarByLatLng
                        {
                            CarList = lstAny,
                            projType = 3,
                            total = lstAny.Count
                        };
                        for (int i = 0; i < any.total; i++)
                        {
                            any.CarList[i].NowStationName = any.CarList[i].NowStationName.Replace("\t", "");
                            DateTime Rent = new DateTime(any.CarList[i].LastRentTime.Year, any.CarList[i].LastRentTime.Month, any.CarList[i].LastRentTime.Day, 0, 0, 0);
                            any.CarList[i].afterRentDays = (int)(NowDay.Subtract(Rent).TotalDays);
                            if (DateTime.Now.Subtract(any.CarList[i].GPSTime).TotalHours > 1)
                            {
                                any.CarList[i].isNoResponse = 1;
                            }
                            if (any.CarList[i].Milage - any.CarList[i].LastMaintenanceMilage >= 10000)
                            {
                                any.CarList[i].isNeedMaintenance = 1;
                            }
                        }
                        outputApi.Add(any);
                        break;
                    case 4:
                        List<CarCleanDataNew> lstMoto = _repository.GetCleanMotoData(ManageStation, 0, 0, 0);
                        OAPI_MA_CleanCarByLatLng moto = new OAPI_MA_CleanCarByLatLng
                        {
                            CarList = lstMoto,
                            projType = 4,
                            total = lstMoto.Count
                        };
                        for (int i = 0; i < moto.total; i++)
                        {
                            moto.CarList[i].NowStationName = moto.CarList[i].NowStationName.Replace("\t", "");
                            DateTime Rent = new DateTime(moto.CarList[i].LastRentTime.Year, moto.CarList[i].LastRentTime.Month, moto.CarList[i].LastRentTime.Day, 0, 0, 0);
                            moto.CarList[i].afterRentDays = (int)(NowDay.Subtract(Rent).TotalDays);
                            if (DateTime.Now.Subtract(moto.CarList[i].GPSTime.AddHours(8)).TotalHours > 1)
                            {
                                moto.CarList[i].isNoResponse = 1;
                            }

                            if (moto.CarList[i].Milage - moto.CarList[i].LastMaintenanceMilage >= 3000)
                            {
                                moto.CarList[i].isNeedMaintenance = 1;
                            }


                        }
                        outputApi.Add(moto);
                        break;
                    case 5:
                        List<CarCleanDataNew> lstNorT = _repository.GetCleanCarDataOfMaintenance(0, ManageStation, 0, 0, 0);
                        List<CarCleanDataNew> lstAnyT = _repository.GetCleanCarDataOfMaintenance(1, ManageStation, 0, 0, 0);
                        List<CarCleanDataNew> lstMotoT = _repository.GetCleanMotoDataOfMaintenance(ManageStation, 0, 0, 0);
                        OAPI_MA_CleanCarByLatLng norT = new OAPI_MA_CleanCarByLatLng
                        {
                            CarList = lstNorT,
                            projType = 0,
                            total = lstNorT.Count
                        };
                        for (int i = 0; i < norT.total; i++)
                        {
                            norT.CarList[i].NowStationName = norT.CarList[i].NowStationName.Replace("\t", "");
                            DateTime Rent = new DateTime(norT.CarList[i].LastRentTime.Year, norT.CarList[i].LastRentTime.Month, norT.CarList[i].LastRentTime.Day, 0, 0, 0);
                            norT.CarList[i].afterRentDays = (int)(NowDay.Subtract(Rent).TotalDays);
                            if (DateTime.Now.Subtract(norT.CarList[i].GPSTime).TotalHours > 1)
                            {
                                norT.CarList[i].isNoResponse = 1;
                            }
                            if (norT.CarList[i].Milage - norT.CarList[i].LastMaintenanceMilage >= 10000)
                            {
                                norT.CarList[i].isNeedMaintenance = 1;
                            }

                        }
                        OAPI_MA_CleanCarByLatLng anyT = new OAPI_MA_CleanCarByLatLng
                        {
                            CarList = lstAnyT,
                            projType = 3,
                            total = lstAnyT.Count
                        };
                        for (int i = 0; i < anyT.total; i++)
                        {
                            anyT.CarList[i].NowStationName = anyT.CarList[i].NowStationName.Replace("\t", "");
                            DateTime Rent = new DateTime(anyT.CarList[i].LastRentTime.Year, anyT.CarList[i].LastRentTime.Month, anyT.CarList[i].LastRentTime.Day, 0, 0, 0);
                            anyT.CarList[i].afterRentDays = (int)(NowDay.Subtract(Rent).TotalDays);
                            if (DateTime.Now.Subtract(anyT.CarList[i].GPSTime).TotalHours > 1)
                            {
                                anyT.CarList[i].isNoResponse = 1;
                            }
                            if (anyT.CarList[i].Milage - anyT.CarList[i].LastMaintenanceMilage >= 10000)
                            {
                                anyT.CarList[i].isNeedMaintenance = 1;
                            }
                            // anyT.CarList[i].isNeedMaintenance = 1;
                        }
                        OAPI_MA_CleanCarByLatLng motoT = new OAPI_MA_CleanCarByLatLng
                        {
                            CarList = lstMotoT,
                            projType = 4,
                            total = lstMotoT.Count
                        };
                        for (int i = 0; i < motoT.total; i++)
                        {
                            motoT.CarList[i].NowStationName = motoT.CarList[i].NowStationName.Replace("\t", "");
                            DateTime Rent = new DateTime(motoT.CarList[i].LastRentTime.Year, motoT.CarList[i].LastRentTime.Month, motoT.CarList[i].LastRentTime.Day, 0, 0, 0);
                            motoT.CarList[i].afterRentDays = (int)(NowDay.Subtract(Rent).TotalDays);
                            if (DateTime.Now.Subtract(motoT.CarList[i].GPSTime.AddHours(8)).TotalHours > 1)
                            {
                                motoT.CarList[i].isNoResponse = 1;
                            }
                            if (motoT.CarList[i].Milage - motoT.CarList[i].LastMaintenanceMilage >= 3000)
                            {
                                motoT.CarList[i].isNeedMaintenance = 1;
                            }
                            // motoT.CarList[i].isNeedMaintenance = 1;


                        }
                        outputApi.Add(norT);
                        outputApi.Add(anyT);
                        outputApi.Add(motoT);
                        break;
                }
            }
            catch (Exception ex)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, ex.Message);
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
