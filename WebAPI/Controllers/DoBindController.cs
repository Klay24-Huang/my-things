using Domain.Common;
using Domain.WebAPI.Input.Taishin;
using Domain.WebAPI.output.Taishin;
using OtherService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebAPI.Models.BaseFunc;
using WebAPI.Models.Param.Input;
using WebAPI.Models.Param.Output;
using WebCommon;

namespace WebAPI.Controllers
{
    /// <summary>
    /// 綁卡
    /// </summary>
    public class DoBindController : ApiController
    {
        private string TaishinAPPOS = ConfigurationManager.AppSettings["TaishinAPPOS"].ToString();
        [HttpPost]
        public Dictionary<string, object> doBindCreditCard(Dictionary<string, object> value)
        {
            #region 初始宣告
            var objOutput = new Dictionary<string, object>();    //輸出
            bool flag = true;
            bool isWriteError = false;
            string errMsg = "Success"; //預設成功
            string errCode = "000000"; //預設成功
            string funName = "DoBindController";
            Int64 LogID = 0;
            Int16 ErrType = 0;
            IAPI_CheckAccount apiInput = null;
            OAPI_Login CheckAccountAPI = null;
            Token token = null;
            CommonFunc baseVerify = new CommonFunc();
            List<ErrorInfo> lstError = new List<ErrorInfo>();
            Int16 APPKind = 2;
            string Contentjson = "";
            #endregion
            TaishinCreditCardBindAPI WebAPI = new TaishinCreditCardBindAPI();
            WebAPIInput_Base wsInput = new WebAPIInput_Base()
            {
                ApiVer = "1.0.1",
                ApposId = TaishinAPPOS,
                RequestParams = new RequestParamsData()
                {
                    FailUrl = "",
                    SuccessUrl = "",
                    ResultUrl = HttpUtility.UrlEncode("http://irentv2-testapp-api.azurewebsites.net/api/bindResult"),
                    MemberId = "C121119150",
                    OrderNo = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                     PaymentType="04"
                 //   PaymentType = "04"
                   
                },
                Random= baseVerify.getRand(0, 9999999).PadLeft(16, '0'),
                 TimeStamp= DateTimeOffset.Now.ToUnixTimeSeconds().ToString()

        };
            WebAPIOutput_Base wsOutput = new WebAPIOutput_Base();
            flag = WebAPI.DoBind(wsInput,  ref errCode, ref wsOutput);
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
