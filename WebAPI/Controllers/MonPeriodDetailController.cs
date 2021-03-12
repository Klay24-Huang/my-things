using Domain.Common;
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

namespace WebAPI.Controllers
{
    public class MonPeriodDetailController : ApiController
    {
        [HttpPost()]
        public Dictionary<string, object> DoMonPeriodDetail([FromBody] Dictionary<string, object> value)
        {
            #region 初始宣告
            var cr_com = new CarRentCommon();
            var trace = new TraceCom();
            HttpContext httpContext = HttpContext.Current;
            //string[] headers=httpContext.Request.Headers.AllKeys;
            string Access_Token = "";
            string Access_Token_string = (httpContext.Request.Headers["Authorization"] == null) ? "" : httpContext.Request.Headers["Authorization"]; //Bearer 
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "MonPeriodDetailController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_MonPeriodDetail apiInput = null;
            OAPI_MonPeriodDetail outputApi = null;
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
                #region 防呆

                flag = baseVerify.baseCheck(value, ref Contentjson, ref errCode, funName, Access_Token_string, ref Access_Token, ref isGuest);
                if (flag)
                {
                    apiInput = Newtonsoft.Json.JsonConvert.DeserializeObject<IAPI_MonPeriodDetail>(Contentjson);
                    //寫入API Log
                    string ClientIP = baseVerify.GetClientIp(Request);
                    flag = baseVerify.InsAPLog(Contentjson, ClientIP, funName, ref errCode, ref LogID);

                    //if (string.IsNullOrWhiteSpace(apiInput.IDNO))
                    //{
                    //    flag = false;
                    //    errCode = "ERR900";
                    //}

                    //if (flag)
                    //{
                    //    //2.判斷格式
                    //    flag = baseVerify.checkIDNO(apiInput.IDNO);
                    //    if (false == flag)
                    //    {
                    //        errCode = "ERR103";
                    //    }
                    //}
                }

                #endregion

                #region TB


                #endregion
            }
            catch (Exception ex)
            {

            }

            #region 輸出
            baseVerify.GenerateOutput(ref objOutput, flag, errCode, errMsg, outputApi, token);
            return objOutput;
            #endregion        
        }

    }
}
