using System;
using System.Collections.Generic;
using System.Web.Http;

using Domain.Common;
using Domain.WebAPI.Input.HiEasyRentAPI;
using Domain.WebAPI.output.HiEasyRentAPI;
using Newtonsoft.Json;
using NLog;
using OtherService;
using System.Configuration;
using System.Web;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Enum;
using WebAPI.Models.Param.BackEnd.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    public class BE_IrentPaymentDetailController : ApiController
    {
        [HttpPost]
        public Dictionary<string, object> DoBE_IrentPaymentDetail(Dictionary<string, object> value)
        {
            #region 初始宣告
            HttpContext httpContext = HttpContext.Current;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            //bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "BE_IrentPaymentDetail";
            //Int64 LogID = 0;
            //Int16 ErrType = 0;
            IAPI_BE_IrentPaymentDetail apiInput = null;
            //OAPI_BE_IrentPaymentDetail apiOutput = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            bool isGuest = true;
            //Int16 APPKind = 2;
            string Contentjson = "";
            #endregion

            #region 防呆
            flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
            if (flag)
            {
                apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_BE_IrentPaymentDetail>(Contentjson);
                ////寫入API Log
                //string ClientIP = baseVerify.GetClientIp(Request);
                //flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                //string[] checkList = { apiInput.UserID };
                //string[] errList = { "ERR900" };
                ////1.判斷必填
                //flag = baseVerify.CheckISNull(checkList, errList, ref errCode, funName, LogID);
            }
            #endregion


            #region 這邊資料用api拋給sqyhi01vm
            if (apiInput.MODE == 1)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_IrentPaymentDetail wsOutput = new WebAPIOutput_IrentPaymentDetail();
                WebAPIInput_IrentPaymentDetail spInput = new WebAPIInput_IrentPaymentDetail()
                {
                    MODE = apiInput.MODE,
                    SPSD = apiInput.SPSD,
                    SPED = apiInput.SPED,
                    SPSD2 = apiInput.SPSD2,
                    SPED2 = apiInput.SPED2,
                    SPSD3 = apiInput.SPSD3,
                    SPED3 = apiInput.SPED3,
                    MEMACCOUNT = apiInput.MEMACCOUNT
                };
                flag = hiEasyRentAPI.NPR390Query(spInput, ref wsOutput);

                //輸出
                baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, wsOutput, token);
                return objOutput;
            }
            else if (apiInput.MODE == 2)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_IrentPaymentDetailExplode wsOutput = new WebAPIOutput_IrentPaymentDetailExplode();
                WebAPIInput_IrentPaymentDetail spInput = new WebAPIInput_IrentPaymentDetail()
                {
                    MODE = apiInput.MODE,
                    SPSD = apiInput.SPSD,
                    SPED = apiInput.SPED,
                    SPSD2 = apiInput.SPSD2,
                    SPED2 = apiInput.SPED2,
                    SPSD3 = apiInput.SPSD3,
                    SPED3 = apiInput.SPED3,
                    MEMACCOUNT = apiInput.MEMACCOUNT
                };
                flag = hiEasyRentAPI.NPR390Query2(spInput, ref wsOutput);

                //輸出
                baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, wsOutput, token);
                return objOutput;
            }
            else if (apiInput.MODE == 3)
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_IrentPaymentHistory wsOutput = new WebAPIOutput_IrentPaymentHistory();
                WebAPIInput_IrentPaymentDetail spInput = new WebAPIInput_IrentPaymentDetail()
                {
                    MODE = apiInput.MODE,
                    SPSD = apiInput.SPSD,
                    SPED = apiInput.SPED,
                    SPSD2 = apiInput.SPSD2,
                    SPED2 = apiInput.SPED2,
                    SPSD3 = apiInput.SPSD3,
                    SPED3 = apiInput.SPED3,
                    MEMACCOUNT = apiInput.MEMACCOUNT
                };
                flag = hiEasyRentAPI.NPR390Query3(spInput, ref wsOutput);

                //輸出
                baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, wsOutput, token);
                return objOutput;           
            }
            else
            {
                HiEasyRentAPI hiEasyRentAPI = new HiEasyRentAPI();
                WebAPIOutput_IrentPaymentHistoryExplode wsOutput = new WebAPIOutput_IrentPaymentHistoryExplode();
                WebAPIInput_IrentPaymentDetail spInput = new WebAPIInput_IrentPaymentDetail()
                {
                    MODE = apiInput.MODE,
                    SPSD = apiInput.SPSD,
                    SPED = apiInput.SPED,
                    SPSD2 = apiInput.SPSD2,
                    SPED2 = apiInput.SPED2,
                    SPSD3 = apiInput.SPSD3,
                    SPED3 = apiInput.SPED3,
                    MEMACCOUNT = apiInput.MEMACCOUNT
                };
                flag = hiEasyRentAPI.NPR390Query4(spInput, ref wsOutput);

                //輸出
                baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, wsOutput, token);
                return objOutput;      
            }
            #endregion

        }
    }
}
