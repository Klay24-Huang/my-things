using Domain.Common;
using Domain.TB.BackEnd;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 車輛行程
    /// </summary>
    public class BE_CarScheduleTimeLogController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 車輛行程
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> doGetStationList(Dictionary<string, object> value)
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
            string funName = "BE_CarScheduleTimeLogController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            DateTime SD = DateTime.Now;
            DateTime ED = DateTime.Now;
            IAPI_BE_CarScheduleTimeLog apiInput = null;
            List<OAPI_BE_CarScheduleTimeLog> apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_CarScheduleTimeLog>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = {  apiInput.SD, apiInput.ED,apiInput.UserID };
                string[] errList = { "ERR900", "ERR900", "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
                if (flag)
                {
                    if(apiInput.StationID=="" && apiInput.CarNo == "")
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }
                if (flag)
                {
                    if (false == DateTime.TryParseExact(apiInput.SD, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out SD))
                    {
                        flag = false;
                        errCode = "ERR241";
                    }
                    if (false == DateTime.TryParseExact(apiInput.ED, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out ED))
                    {
                        flag = false;
                        errCode = "ERR243";
                    }
                    if (flag)
                    {
                        if (SD > ED)
                        {
                            flag = false;
                            errCode = "ERR267";
                        }
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {

                StationAndCarRepository _repository = new StationAndCarRepository(connetStr);
                List<BE_CarScheduleTimeLog> lstCarSchedule = _repository.GetCarScheduleNew(apiInput.StationID.ToUpper(),apiInput.CarNo.ToUpper().Replace(" ",""), apiInput.SD, apiInput.ED);
                if (lstCarSchedule != null)
                {
                   apiOutput =new List<OAPI_BE_CarScheduleTimeLog>();
                    int Len = lstCarSchedule.Count;
                    if (Len > 0)
                    {
                        apiOutput.Add(new OAPI_BE_CarScheduleTimeLog()
                        {
                            CarNo = lstCarSchedule[0].CarNo,
                            lstOrder = new List<BE_OrderInfo>()
                              {
                                   new BE_OrderInfo()
                                   {
                                        booking_status=lstCarSchedule[0].booking_status,
                                         cancel_status=lstCarSchedule[0].cancel_status,
                                          car_mgt_status=lstCarSchedule[0].car_mgt_status,
                                           ED=lstCarSchedule[0].ED.ToString("yyyy-MM-dd HH:mm:ss"),
                                            FE=lstCarSchedule[0].FE.ToString("yyyy-MM-dd HH:mm:ss"),
                                             FS=lstCarSchedule[0].FS.ToString("yyyy-MM-dd HH:mm:ss"),
                                              IDNO=lstCarSchedule[0].IDNO,
                                               Mobile=lstCarSchedule[0].Mobile,
                                                OrderNum=string.Format("H{0}",lstCarSchedule[0].OrderNum.PadLeft(7,'0')),
                                                  SD=lstCarSchedule[0].SD.ToString("yyyy-MM-dd HH:mm:ss"),
                                                   UName=lstCarSchedule[0].UName
                                   }
                              }
                        });
                    }
                    for (int i = 1; i < Len; i++)
                    {
                        int orderIndex = apiOutput.FindIndex(delegate (OAPI_BE_CarScheduleTimeLog car)
                        {
                            return car.CarNo == lstCarSchedule[i].CarNo;
                        });
                        BE_OrderInfo tmpOrderInfo = new BE_OrderInfo()
                        {
                            booking_status = lstCarSchedule[i].booking_status,
                            cancel_status = lstCarSchedule[i].cancel_status,
                            Mobile = lstCarSchedule[i].Mobile,
                            car_mgt_status = lstCarSchedule[i].car_mgt_status,
                            ED = lstCarSchedule[i].ED.ToString("yyyy-MM-dd HH:mm:ss"),
                            FE = lstCarSchedule[i].FE.ToString("yyyy-MM-dd HH:mm:ss"),
                            FS = lstCarSchedule[i].FS.ToString("yyyy-MM-dd HH:mm:ss"),
                            IDNO = lstCarSchedule[i].IDNO,
                            OrderNum = (lstCarSchedule[i].OrderNum == "0") ? "0" : "H" + lstCarSchedule[i].OrderNum.PadLeft(7, '0'),
                            SD = lstCarSchedule[i].SD.ToString("yyyy-MM-dd HH:mm:ss"),
                            UName = lstCarSchedule[i].UName
                        };
                        if (orderIndex < 0)
                        {
                            OAPI_BE_CarScheduleTimeLog tmp = new OAPI_BE_CarScheduleTimeLog()
                            {
                                CarNo = lstCarSchedule[i].CarNo,
                                lstOrder = new List<BE_OrderInfo>()
                            };
                            tmp.lstOrder.Add(tmpOrderInfo);
                            apiOutput.Add(tmp);
                        }
                        else
                        {
                            apiOutput[orderIndex].lstOrder.Add(tmpOrderInfo);
                        }

                    }
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, apiOutput, token);
            return objOutput;
            #endregion
        }
    }
}
