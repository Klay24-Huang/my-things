﻿using Domain.Common;
using Domain.SP.Input.Common;
using Domain.SP.Output.Common;
using Domain.TB;
using Domain.WebAPI.output.HiEasyRentAPI;
using OtherService;
using Reposotory.Implement;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebAPI.Models.Param.Output.PartOfParam;
using WebCommon;
using System.Data;
using WebAPI.Utils;
using Domain.SP.Output;
using System.CodeDom;
using Domain.SP.Input.Arrears;
using WebAPI.Models.BillFunc;
using Domain.SP.Input.Rent;
using Newtonsoft.Json;

namespace WebAPI.Controllers
{
    public class GetMonthListController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoGetMonthList([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var ms_com = new MonSubsCommon();
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            var carRepo = new CarRentRepo();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "GetMonthListController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            var apiInput = new IAPI_GetMonthList();
            var outputApi = new OAPI_GetMonthList();
            outputApi.MonCards = new List<MonCardParam>();
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();

            Int16 APPKind = 2;
            string Contentjson = "";
            bool isGuest = true;
            string IDNO = "";

            #endregion

            try
            {
                trace.traceAdd("apiIn", value);

                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_GetMonthList>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    if(apiInput.MonType == 0)
                    {
                        apiInput.MonType = 2; //暫時只有訂閱制月租
                        //flag = false;
                        //errCode = "ERR255";
                    }

                    trace.traceAdd("apiInCk", errCode);
                }

                #endregion

                #region TB

                var spIn = new SPInput_GetMonthList()
                {
                    LogID = LogID,
                    IsMoto = apiInput.IsMoto,
                    MonType = apiInput.MonType
                };
                trace.traceAdd("spIn", spIn);
                //取出月租列表
                var sp_mList = ms_com.sp_GetMonthList(spIn, ref errMsg);
                trace.traceAdd("sp_mList", sp_mList);
                if(sp_mList != null && sp_mList.Count() > 0)
                {
                    var cards = (from a in sp_mList
                                 select new MonCardParam
                                 {
                                     MonProjID = a.MonProjID,
                                     MonProjNM = a.MonProjNM,
                                     MonProPeriod = a.MonProPeriod,
                                     PeriodPrice = a.PeriodPrice,
                                     IsMoto = a.IsMoto,
                                     CarWDHours = a.CarWDHours,
                                     CarHDHours = a.CarHDHours,
                                     CarTotalHours = a.CarTotalHours,
                                     MotoWDMins = a.MotoWDMins,
                                     MotoHDMins = a.MotoHDMins,
                                     MotoTotalMins = a.MotoTotalMins,
                                     SDATE = a.SDATE,
                                     EDATE = a.EDATE
                                 }).ToList();
                    outputApi.MonCards = cards;

                    trace.traceAdd("outputApi", outputApi);
                }

                #endregion
            }
            catch (Exception ex)
            {                
                trace.BaseMsg = ex.Message;
                carRepo.AddTraceLog(178,funName, eumTraceType.exception,trace);
            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }
    }
}
