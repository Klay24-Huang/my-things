﻿using Domain.Common;
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
    /// 接收車機資料
    /// </summary>
    public class ReceiveCarDataController : ApiController
    {
        private string connetStr = ConfigurationManager.ConnectionStrings["IRent"].ConnectionString;
        [HttpPost]
        public Dictionary<string, object> doVerifyEMail(Dictionary<string, object> value)
        {
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
            
            Int16 DataType = 0; //0:汽車;1:機車
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion
            #region 防呆

            flag = baseVerify.baseCheck(value,  ref errCode, funName);
            if (flag)
            {
                if (value["para"].ToString().IndexOf("Vehicle") > -1) //汽車
                {
                    VehicleInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_CarData>(Contentjson);
                }
                else
                {
                    MotorDataInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MotorData>(Contentjson);
                }
                
                //寫入API Log
                string ClientIP = baseVerify.GetClientIp(Request);
                flag = baseVerify.InsAPLog(value["para"].ToString(), ClientIP, funName, ref errCode, ref LogID);

              
            }
            #endregion
            #region TB
            if (flag)
            {
               
            }
            #endregion
            #region 寫入錯誤Log
            if (false == flag && false == isWriteError)
            {
                baseVerify.InsErrorLog(funName, errCode, ErrType, LogID, 0, 0, "");
            }
            #endregion
            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, CheckAccountAPI, token);
            return objOutput;
            #endregion
        }
    }
}
