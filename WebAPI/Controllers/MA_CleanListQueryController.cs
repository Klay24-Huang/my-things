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
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 【整備人員】查詢列表
    /// </summary>
    public class MA_CleanListQueryController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> DoMA_CleanListQuery(Dictionary<string, object> value)
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
            string funName = "MA_CleanListQueryController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MA_CleanListQuery apiInput = null;
            List<OAPI_MA_CleanListQuery> outputApi = null;
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            List<GetBookingMainForMaintain> lstOrder = new List<GetBookingMainForMaintain>();
            int totalNum = 0;
            ContactRepository _repository = new ContactRepository(connetStr);
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            DateTime SD = DateTime.Now;
            DateTime ED = DateTime.Now;
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
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MA_CleanListQuery>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

            }
            if (flag)
            {
                lstOrder = _repository.GetAllCleanDataHasStation(apiInput.IDNO);
                int len = lstOrder.Count;
                if (lstOrder.Count > 0)
                {
                    outputApi = new List<OAPI_MA_CleanListQuery>();
                    for (int i = 0; i < len; i++)
                    {
                        if (lstOrder[i].OrderStatus == 0 && lstOrder[i].cancel_status == 0)
                        {
                            OAPI_MA_CleanListQuery tmp = new OAPI_MA_CleanListQuery()
                            {
                                Anydispatch = lstOrder[i].Anydispatch,
                                assigned_car_id = lstOrder[i].assigned_car_id,
                                CanCancel = 1,
                                cancel_status = lstOrder[i].cancel_status,
                                CanPick = 0,
                                citizen_id = lstOrder[i].citizen_id,
                                dispatch = lstOrder[i].dispatch,
                                insideClean = lstOrder[i].insideClean,
                                IsCar = lstOrder[i].IsCar,
                                isOverTime = 0,
                                MachineID = lstOrder[i].MachineID,
                                MachineNo = lstOrder[i].MachineNo,
                                OrderNum = lstOrder[i].OrderNum,
                                OrderStatus = lstOrder[i].OrderStatus,
                                outsideClean = lstOrder[i].outsideClean,
                                rescue = lstOrder[i].rescue,
                                start_time = lstOrder[i].start_time,
                                stop_time = lstOrder[i].stop_time,
                                UserID = lstOrder[i].UserID,
                                Location = lstOrder[i].Location.Replace(" ", "").Replace("\t", ""),
                                Site_ID = lstOrder[i].Site_ID
                            };
                            DateTime nowDateTime = DateTime.Now;
                            if (nowDateTime >= tmp.start_time.AddMinutes(-15) && nowDateTime <= tmp.start_time.AddMinutes(15))
                            {
                                tmp.CanPick = 1;
                            }
                            else if (nowDateTime > tmp.start_time.AddMinutes(15))
                            {
                                tmp.isOverTime = 1;
                            }
                            outputApi.Add(tmp);

                        }
                        else
                        {
                            if (lstOrder[i].OrderStatus >= 1 && DateTime.Now.Subtract(lstOrder[i].stop_time).TotalDays < 8)
                            {
                                outputApi.Add(new OAPI_MA_CleanListQuery()
                                {
                                    Anydispatch = lstOrder[i].Anydispatch,
                                    assigned_car_id = lstOrder[i].assigned_car_id,
                                    CanCancel = 0,
                                    cancel_status = lstOrder[i].cancel_status,
                                    CanPick = 0,
                                    citizen_id = lstOrder[i].citizen_id,
                                    dispatch = lstOrder[i].dispatch,
                                    insideClean = lstOrder[i].insideClean,
                                    IsCar = lstOrder[i].IsCar,
                                    isOverTime = 0,
                                    MachineID = lstOrder[i].MachineID,
                                    MachineNo = lstOrder[i].MachineNo,
                                    OrderNum = lstOrder[i].OrderNum,
                                    OrderStatus = lstOrder[i].OrderStatus,
                                    outsideClean = lstOrder[i].outsideClean,
                                    rescue = lstOrder[i].rescue,
                                    start_time = lstOrder[i].start_time,
                                    stop_time = lstOrder[i].stop_time,
                                    UserID = lstOrder[i].UserID,
                                    Location = lstOrder[i].Location.Replace(" ", "").Replace("\t", ""),
                                    Site_ID = lstOrder[i].Site_ID
                                });
                            }

                        }
                    }
                    var queryOrder = from e in outputApi
                                     orderby e.OrderNum descending
                                     select e;
                    outputApi = queryOrder.ToList<OAPI_MA_CleanListQuery>();
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
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion
        }
    }
}
