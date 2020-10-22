﻿using Domain.Common;
using Domain.TB;
using Domain.TB.BackEnd;
using Reposotory.Implement;
using Reposotory.Implement.BackEnd;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.BillFunc;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.BackEnd.Output;
using WebCommon;
namespace WebAPI.Controllers
{
    /// <summary>
    /// 【後台】查詢假日
    /// </summary>
    public class BE_GetHoildayController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        /// <summary>
        /// 【後台】查詢假日
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public Dictionary<string, object> DoBE_GetHoilday(Dictionary<string, object> value)
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
            string funName = "BE_GetHoildayController";
            Int64 LogID = 0;
            Int16 ErrType = 0;

            IAPI_BE_GetHoilday apiInput = null;
            OAPI_BE_GetHoilday apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            string IDNO = "";
            bool isGuest = true;
            Int16 APPKind = 2;
            string Contentjson = "";
            int MonStart = 1;
            int MonEnd = 3;
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);

            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_GetHoilday>(Contentjson);
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                string[] checkList = {  apiInput.UserID };
                string[] errList = { "ERR900" };
                //1.判斷必填
                flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
               
                if (flag)
                {
                    if(apiInput.QuerySeason<1 || apiInput.QuerySeason > 4)
                    {
                        flag = false;
                        errCode = "ERR900";
                    }
                }

            }
            #endregion

            #region TB

            if (flag)
            {
                MonStart = ((apiInput.QuerySeason - 1) * 3) + 1;
                MonEnd = (apiInput.QuerySeason * 3) + 1;
                DateTime SD = new DateTime(apiInput.QueryYear, MonStart, 1);
              
                if (MonEnd > 12)
                {
                    apiInput.QueryYear += 1;
                    MonEnd = 1;
                }
               
                DateTime ED = new DateTime(apiInput.QueryYear, MonEnd, 1).AddSeconds(-1);

                List<Holiday> holidays =  new CommonRepository(connetStr).GetHolidays(SD.ToString("yyyyMMdd"), ED.ToString("yyyyMMdd"));
                apiOutput = new OAPI_BE_GetHoilday()
                {
                    holidays = holidays
                };


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
